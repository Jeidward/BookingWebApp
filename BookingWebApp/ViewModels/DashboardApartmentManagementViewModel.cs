namespace BookingWebApp.ViewModels
{
    public class DashboardApartmentManagementViewModel
    {
        public PaginatedList<ApartmentViewModel> ApartmentViewModels;

        public List<AddApartmentViewModel> EditApartmentViewModels;
        public int ApartmentCurrentlyOccupied { get; set; }
        public int ApartmentAvailable { get; set; }
        public int ApartmentTotal { get; set; }
        public List<AmenitiesViewModel>Amenities { get; set; }


        public static DashboardApartmentManagementViewModel ConvertToViewModel(
            PaginatedList<ApartmentViewModel> apartments,
            int totalCount,
            int occupiedCount,
            int availableCount,
            List<AddApartmentViewModel> editApartments,
            List<AmenitiesViewModel> amenities)
        {
            return new DashboardApartmentManagementViewModel
            {
                ApartmentViewModels = apartments,
                ApartmentTotal = totalCount,
                ApartmentCurrentlyOccupied = occupiedCount,
                ApartmentAvailable = availableCount,
                EditApartmentViewModels = editApartments,
                Amenities = amenities
            };
        }

    }


}
