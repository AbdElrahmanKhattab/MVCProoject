using Microsoft.EntityFrameworkCore;
using MVC.Models;
using MVC.Repositories;
using MVC.ViewModels;

namespace MVC.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SessionViewModel>> GetAllAsync()
        {
            var sessions = await _unitOfWork.Sessions.Query()
                .Include(x => x.Bookings)
                .OrderBy(x => x.StartDate)
                .ToListAsync();

            return sessions.Select(MapToViewModel).ToList();
        }

        public async Task<Result<SessionViewModel>> GetDetailsAsync(int id)
        {
            var session = await _unitOfWork.Sessions.Query()
                .Include(x => x.Bookings)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (session is null)
            {
                return Result<SessionViewModel>.Fail("Session does not exist.");
            }

            return Result<SessionViewModel>.Ok(MapToViewModel(session));
        }

        public async Task<CreateSessionViewModel> BuildCreateViewModelAsync(CreateSessionViewModel? selected = null)
        {
            return new CreateSessionViewModel
            {
                Description = selected?.Description ?? string.Empty,
                Capacity = selected?.Capacity ?? 1,
                StartDate = selected?.StartDate == default ? DateTime.Now.AddDays(1) : selected?.StartDate ?? DateTime.Now.AddDays(1),
                EndDate = selected?.EndDate == default ? DateTime.Now.AddDays(1).AddHours(1) : selected?.EndDate ?? DateTime.Now.AddDays(1).AddHours(1),
                TrainerId = selected?.TrainerId ?? 0,
                CategoryId = selected?.CategoryId ?? 0,
                Trainers = await GetTrainerOptionsAsync(),
                Categories = await GetCategoryOptionsAsync()
            };
        }

        public async Task<UpdateSessionViewModel?> BuildUpdateViewModelAsync(int id)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id);

            if (session is null)
            {
                return null;
            }

            var trainer = await _unitOfWork.Trainers.Query()
                .FirstOrDefaultAsync(x => x.Name == session.TrainerName);

            return await PopulateUpdateLookupsAsync(new UpdateSessionViewModel
            {
                Id = session.Id,
                CategoryName = session.CategoryName,
                Capacity = session.Capacity,
                Description = session.Description,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                TrainerId = trainer?.Id ?? 0
            });
        }

        public async Task<UpdateSessionViewModel> PopulateUpdateLookupsAsync(UpdateSessionViewModel model)
        {
            model.Trainers = await GetTrainerOptionsAsync();
            return model;
        }

        public async Task<Result> CreateAsync(CreateSessionViewModel model)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(model.CategoryId);
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(model.TrainerId);

            var validationResult = await ValidateSessionRulesAsync(
                model.StartDate,
                model.EndDate,
                model.Capacity,
                model.CategoryId,
                model.TrainerId,
                null,
                category,
                trainer,
                requireFutureStart: true);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            await _unitOfWork.Sessions.AddAsync(new GymSession
            {
                CategoryName = category!.CategoryName,
                TrainerName = trainer!.Name,
                Description = model.Description,
                Capacity = model.Capacity,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            });

            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(UpdateSessionViewModel model)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(model.Id);

            if (session is null)
            {
                return Result.Fail("Session does not exist.");
            }

            if (session.StartDate <= DateTime.Now)
            {
                return Result.Fail("Only upcoming sessions can be edited.");
            }

            var category = await _unitOfWork.Categories.Query()
                .FirstOrDefaultAsync(x => x.CategoryName == session.CategoryName);
            var trainer = await _unitOfWork.Trainers.GetByIdAsync(model.TrainerId);

            var validationResult = await ValidateSessionRulesAsync(
                model.StartDate,
                model.EndDate,
                session.Capacity,
                category?.Id ?? 0,
                model.TrainerId,
                session.Id,
                category,
                trainer,
                requireFutureStart: true);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            session.Description = model.Description;
            session.TrainerName = trainer!.Name;
            session.StartDate = model.StartDate;
            session.EndDate = model.EndDate;

            _unitOfWork.Sessions.Update(session);
            await _unitOfWork.SaveChangesAsync();

            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id);

            if (session is null)
            {
                return Result.Fail("Session does not exist.");
            }

            var now = DateTime.Now;
            if (session.StartDate <= now && session.EndDate > now)
            {
                return Result.Fail("Ongoing sessions cannot be deleted.");
            }

            _unitOfWork.Sessions.Remove(session);
            await _unitOfWork.SaveChangesAsync();
            return Result.Ok();
        }

        private async Task<Result> ValidateSessionRulesAsync(
            DateTime startDate,
            DateTime endDate,
            int capacity,
            int categoryId,
            int trainerId,
            int? currentSessionId,
            Category? category,
            Trainer? trainer,
            bool requireFutureStart)
        {
            if (category is null)
            {
                return Result.Fail("Selected category does not exist.");
            }

            if (trainer is null)
            {
                return Result.Fail("Selected trainer does not exist.");
            }

            if (capacity < 1 || capacity > 25)
            {
                return Result.Fail("Capacity must be between 1 and 25.");
            }

            if (startDate >= endDate)
            {
                return Result.Fail("End date must be after start date.");
            }

            if (requireFutureStart && startDate <= DateTime.Now)
            {
                return Result.Fail("Start date must be in the future.");
            }

            if (!string.Equals(trainer.Specialty, category.CategoryName, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Fail("Trainer specialty must match the selected category.");
            }

            var trainerIsBusy = await _unitOfWork.Sessions.Query()
                .AnyAsync(x =>
                    x.TrainerName == trainer.Name &&
                    x.Id != currentSessionId &&
                    x.StartDate < endDate &&
                    startDate < x.EndDate);

            if (trainerIsBusy)
            {
                return Result.Fail("The selected trainer already has another session in this time slot.");
            }

            return Result.Ok();
        }

        private async Task<List<TrainerSelectViewModel>> GetTrainerOptionsAsync()
        {
            return await _unitOfWork.Trainers.Query()
                .OrderBy(x => x.Name)
                .Select(x => new TrainerSelectViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToListAsync();
        }

        private async Task<List<CategorySelectViewModel>> GetCategoryOptionsAsync()
        {
            return await _unitOfWork.Categories.Query()
                .OrderBy(x => x.CategoryName)
                .Select(x => new CategorySelectViewModel
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName
                })
                .ToListAsync();
        }

        private static SessionViewModel MapToViewModel(GymSession session)
        {
            return new SessionViewModel
            {
                Id = session.Id,
                Description = session.Description,
                Capacity = session.Capacity,
                StartDate = session.StartDate,
                EndDate = session.EndDate,
                TrainerName = session.TrainerName,
                CategoryName = session.CategoryName,
                AvailableSlots = session.Capacity - session.Bookings.Count
            };
        }
    }
}
