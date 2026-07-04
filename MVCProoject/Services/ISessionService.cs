using MVC.ViewModels;

namespace MVC.Services
{
    public interface ISessionService
    {
        Task<List<SessionViewModel>> GetAllAsync();
        Task<Result<SessionViewModel>> GetDetailsAsync(int id);
        Task<CreateSessionViewModel> BuildCreateViewModelAsync(CreateSessionViewModel? selected = null);
        Task<UpdateSessionViewModel?> BuildUpdateViewModelAsync(int id);
        Task<UpdateSessionViewModel> PopulateUpdateLookupsAsync(UpdateSessionViewModel model);
        Task<Result> CreateAsync(CreateSessionViewModel model);
        Task<Result> UpdateAsync(UpdateSessionViewModel model);
        Task<Result> DeleteAsync(int id);
    }
}
