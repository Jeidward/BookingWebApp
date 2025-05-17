using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class AmenitiesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImgIcon { get; set; }

        public static AmenitiesViewModel ConvertToViewModel(Amenities amenities)
        {
            return new AmenitiesViewModel
            {
                Id = amenities.Id,
                Name = amenities.Name,
                ImgIcon = amenities.ImgIcon
            };
        }

        public static List<AmenitiesViewModel> ConverToViewModel(List<Amenities> amenities)
        {
            List<AmenitiesViewModel> amenitiesViewModels = new List<AmenitiesViewModel>();
            foreach (var amenity in amenities)
            {
                amenitiesViewModels.Add(ConvertToViewModel(amenity));
            }

            return amenitiesViewModels;
        }

        public static Amenities ConvertToEntity(AmenitiesViewModel viewModel)
        {
            return new Amenities(viewModel.Id, viewModel.Name, viewModel.ImgIcon);
        }

        //public static List<Amenities> ConvertToEntity(List<AmenitiesViewModel> viewModels)
        //{
        //    List<Amenities> amenities = new List<Amenities>();
        //    foreach (var viewModel in viewModels)
        //    {
        //        amenities.Add(ConvertToEntity(viewModel));
        //    }
        //    return amenities;
        //}

        public static List<Amenities> ConvertToEntity(List<int> ids)
        {
            List<Amenities> amenities = new List<Amenities>();
            foreach (var id in ids)
            {
                amenities.Add(new Amenities(id, string.Empty, string.Empty));
            }
            return amenities;
        }
    }
}
