namespace MVC.ViewModels
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public int Capacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TrainerName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public int AvailableSlots { get; set; }

        public string DateDisplay => $"{StartDate:MMM dd, yyyy}";
        public string TimeRangeDisplay => $"{StartDate:hh:mm tt} - {EndDate:hh:mm tt}";
        public TimeSpan Duration => EndDate - StartDate;
        public string DurationDisplay => $"{(int)Duration.TotalHours} Hours {Duration.Minutes} Minutes";

        public string Status
        {
            get
            {
                if (StartDate > DateTime.Now)
                {
                    return "Upcoming";
                }

                if (StartDate <= DateTime.Now && EndDate >= DateTime.Now)
                {
                    return "Ongoing";
                }

                return "Completed";
            }
        }
    }
}
