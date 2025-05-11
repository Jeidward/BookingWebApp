namespace BookingWebApp.ViewModels
{
    public class DashboardApartmentManagementViewModel
    {
        public List<ApartmentViewModel> ApartmentViewModels;
        public ApartmentViewModel ApartmentViewModel;
        public int ApartmentCurrentlyOccupied { get; set; }
        public int ApartmentAvailable { get; set; }
        public int ApartmentTotal { get; set; }
    }

    
}
