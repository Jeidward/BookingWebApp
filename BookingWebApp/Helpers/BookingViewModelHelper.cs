﻿using BookingWebApp.ViewModels;
using Models.Entities;
using MSSQL;
using Services;

namespace BookingWebApp.Helpers
{
    public static class BookingViewModelHelper
    {
      

        //factory to produce object wihtout us needing to know of how it producing it, almost always static
        public static BookingViewModel ConvertToViewModel(Booking booking)
        {
            BookingViewModel viewModel = new BookingViewModel() {Id = booking.Id, ApartmentId = booking.Apartment.Id, CheckInDate = booking.CheckInDate, CheckOutDate = booking.CheckOutDate, TotalPrice = booking.TotalPrice, Status = booking.Status };

            viewModel.ApartmentViewModel = new ApartmentViewModel
            {
                Id = booking.Apartment.Id,
                Name = booking.Apartment.Name,
                ImageUrl = booking.Apartment.FirstImage,
                Description = booking.Apartment.Description,
                Adress = booking.Apartment.Adress,
                Gallery = booking.Apartment.Gallery,
                PricePerNight = booking.Apartment.PricePerNight,
                AvgRating = booking.Apartment.AvgRating,
                ReviewsCount = booking.Apartment.ReviewsCount
            };

            foreach (var guest in booking.GuestProfiles)
            {
             
                viewModel.GuestProfiles.Add(GuestProfileViewModel.ConvertToViewModel(guest));
            }

            return viewModel;
        }

        public static List<BookingViewModel> ConvertToViewModel(List<Booking> bookings)
        {
            var viewModels = new List<BookingViewModel>();          
            foreach (var booking in bookings)
            {
                viewModels.Add(ConvertToViewModel(booking)); 
            }
            return viewModels;
        }

        public static string CreateString(BookingViewModel bookingViewModel)
        {
            return $"{bookingViewModel.ApartmentId}${bookingViewModel.CheckInDate}${bookingViewModel.CheckOutDate}${bookingViewModel.TotalPrice}${bookingViewModel.ExtraServiceViewModels.Pool}${bookingViewModel.ExtraServiceViewModels.Laundry}${bookingViewModel.ExtraServiceViewModels.CarRental}${bookingViewModel.NumberOfGuests}";
        }

        public static string CreateString(Booking booking)
        {
            return $"{booking.Id}${booking.Apartment.Id}${booking.CheckInDate}${booking.CheckOutDate}${booking.TotalPrice}";
        }

        public static BookingViewModel ReadModelBooking(string bookingString)
        {
            string[] bookingInfo = bookingString.Split('$');
            BookingViewModel readBooking = new BookingViewModel()
            {
                Id = int.Parse(bookingInfo[0]),
                CheckInDate = DateTime.Parse(bookingInfo[2]),
                CheckOutDate = DateTime.Parse(bookingInfo[3]),
                TotalPrice = decimal.Parse(bookingInfo[4]),
                ApartmentId = int.Parse(bookingInfo[1])
            };
                
            return readBooking;
        }


        public static BookingViewModel ReadString(string bookingString)
        {
            string[] bookingInfo = bookingString.Split('$');    
            var extras = new ExtraServiceViewModel
            {
                Pool = bool.Parse(bookingInfo[4]),
                Laundry = bool.Parse(bookingInfo[5]),
                CarRental = bool.Parse(bookingInfo[6])
            };

            return new BookingViewModel
            {
                ApartmentId = int.Parse(bookingInfo[0]),
                CheckInDate = DateTime.Parse(bookingInfo[1]),
                CheckOutDate = DateTime.Parse(bookingInfo[2]),
                TotalPrice = decimal.Parse(bookingInfo[3]),
                ExtraServiceViewModels = extras,
                NumberOfGuests = int.Parse(bookingInfo[7])
            };
        }
    }
    
}
