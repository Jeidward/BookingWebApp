﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using BookingWebApp.ViewModel;
using BookingWebApp.ViewModels;

namespace BookingWebApp.ViewModels
{
    public class CreateBookingViewModel
    {
        public bool AddMyself { get; set; }
        public int NumberOfGuests { get; set; }

        [DataType(DataType.Date)]
        [IsOverlappingBookingExist]
        public DateTime CheckInDate { get; set; }
       
        [DateRange]
        public DateTime CheckOutDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckInTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckOutTime { get; set; }

        public int ApartmentId { get; set; }
        public ExtraServiceViewModel ExtraServiceViewModel { get; set; }
        public CreateBookingViewModel()
        {
            ExtraServiceViewModel = new ExtraServiceViewModel();
            CheckInDate = DateTime.Today;
            CheckOutDate = DateTime.Today.AddDays(1);

            CheckInTime = DateTime.Today.AddHours(15); 
            CheckOutTime = DateTime.Today.AddHours(11); 

     
            NumberOfGuests = 1;
        }
      
    }
}
