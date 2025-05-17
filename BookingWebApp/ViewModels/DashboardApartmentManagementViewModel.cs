namespace BookingWebApp.ViewModels
{
    public class DashboardApartmentManagementViewModel
    {
        public List<ApartmentViewModel> ApartmentViewModels;

        public List<AddApartmentViewModel> EditApartmentViewModels;
        public int ApartmentCurrentlyOccupied { get; set; }
        public int ApartmentAvailable { get; set; }
        public int ApartmentTotal { get; set; }
        public List<AmenitiesViewModel>Amenities { get; set; }
    }

    
}
