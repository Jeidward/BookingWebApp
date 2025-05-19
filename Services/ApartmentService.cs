using Microsoft.Extensions.Configuration;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IServices;
using Interfaces.IRepositories;

namespace Services
{
    public class ApartmentService
    {
        private readonly IApartmentRepository _apartmentRepository;
        private readonly IAmenitiesRepository _amenitiesRepository;
        private readonly IReviewService _reviewService;

        public ApartmentService(IApartmentRepository apartmentRepository, IReviewService reviewService, IAmenitiesRepository amenitiesRepository)
        {
            _apartmentRepository = apartmentRepository;
            _reviewService = reviewService;
            _amenitiesRepository = amenitiesRepository;
        }

        public ApartmentService(IApartmentRepository apartmentRepository)// for Unit test
        {
            _apartmentRepository = apartmentRepository;
        }

        public Apartment GetApartment(int id)
        {
            Apartment selectedApartment = _apartmentRepository.GetApartment(id);
            if (selectedApartment.Id == 0)
                return selectedApartment;

            var reviews = _reviewService.GetReviewsForApartment(id);
            if(reviews==null)
            {
                reviews = new List<Review>();
            }
            selectedApartment.SetReviews(reviews);
            selectedApartment.SetAvgRating(_reviewService.GetAverageRating(id));
            selectedApartment.SetReviewsCount(reviews.Count);
            var gallery = _apartmentRepository.GetGallery(id);
            var onlyThisGallery = gallery.Skip(1).ToList();
            selectedApartment.SetFirstImage(gallery.First());
            selectedApartment.SetGallery(onlyThisGallery);
            selectedApartment.SetAmenities(_amenitiesRepository.GetAmenities(id));
            return selectedApartment;
        }

        public List<Apartment> GetAllApartments()
        {
            var apartments = _apartmentRepository.GetApartments();
            foreach (var apartment in apartments)
            {
                var reviews = _reviewService.GetReviewsForApartment(apartment.Id);
                if (reviews == null)
                {
                    reviews = new List<Review>();
                }
                apartment.SetReviews(reviews);
                apartment.SetAvgRating(_reviewService.GetAverageRating(apartment.Id));
                apartment.SetReviewsCount(reviews.Count);
                apartment.SetFirstImage(_apartmentRepository.GetGallery(apartment.Id).First());
                apartment.SetGallery(_apartmentRepository.GetGallery(apartment.Id));
                apartment.SetAmenities(_amenitiesRepository.GetAmenities(apartment.Id));
            }
            return apartments;
        }

        public void DeleteApartment(int apartmentId)
        {
            if(apartmentId <= 0)
                throw new ArgumentException("Invalid apartment ID.");
            _apartmentRepository.Delete(apartmentId);
        }

        public void AddApartment(Apartment apartment)
        {
            this.ValidateApartmentObject(apartment);
            _apartmentRepository.CreateApartment(apartment);
            var lastApartment = _apartmentRepository.GetApartments().LastOrDefault(); // redundant, but it works
            if (lastApartment == null)
                throw new Exception("No apartments found in the database.");    // for stop complaining about null reference.

            foreach (var image in apartment.Gallery)
                _apartmentRepository.AddApartmentImages(lastApartment.Id, image);

            foreach (var amenities in apartment.Amenities)
                _amenitiesRepository.AddAmenities(lastApartment.Id,amenities.Id);
        }

        public void UpdateApartment(Apartment apartment)
        {
            if (apartment == null)
                throw new ArgumentNullException(nameof(apartment));
            _apartmentRepository.Update(apartment);
            _apartmentRepository.UpdateGallery(apartment.Id, apartment.Gallery);
            _amenitiesRepository.Delete(apartment.Id); // I first delete everything

            foreach (var amenity in apartment.Amenities)
                _amenitiesRepository.AddAmenities(apartment.Id, amenity.Id);
        }

        public List<Amenities> GetAmenitiesList() => _amenitiesRepository.GetAmenitiesList();

        public List<Amenities> GetAmenitiesForApartment(int id) => _amenitiesRepository.GetSelectedAmenities(id);


        private void ValidateApartmentObject(Apartment apartment) // this method is used to validate the apartment object before adding or updating it
        {
            if (apartment == null)throw new ArgumentNullException(nameof(apartment));
            if (string.IsNullOrEmpty(apartment.Name)) throw new ArgumentNullException(nameof(apartment.Name),"Argument cannot be null or empty");
            if (string.IsNullOrEmpty(apartment.Description)) throw new ArgumentNullException(nameof(apartment.Description),"Argument cannot be null or empty");
            if (string.IsNullOrEmpty(apartment.Adress)) throw new ArgumentNullException(nameof(apartment.Adress),"Argument cannot be null or empty");
            if (apartment.PricePerNight <= 0) throw new ArgumentException("apartment price cannot be less than or equal to 0");
            if (apartment.Bedrooms <= 0) throw new ArgumentException("apartment bedrooms cannot be less than or equal to 0");
            if (apartment.Bathrooms <= 0) throw new ArgumentException("apartment bathrooms cannot be less than or equal to 0");
            if (apartment.Gallery == null || apartment.Gallery.Count == 0) throw new ArgumentException("apartment gallery cannot be null or empty");
            if (apartment.Amenities == null || apartment.Amenities.Count == 0) throw new ArgumentException("apartment amenities cannot be null or empty");
        }


    }
}
