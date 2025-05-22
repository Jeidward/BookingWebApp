using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail; // email sending
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Services
{
    public class EmailSenderService
    {
        private readonly string _mail;
        private readonly string _password;
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
             _configuration= configuration;
            _mail = "jeidwardruiz@gmail.com";
            _password = configuration["Email:Smtp:Password"]!;
        }
        public Task SendEmailForBooking(string receiver, Booking booking, User user)
        {

            var client = CreateSmtpClient();

            var subject = "Booking Confirmation";
            var message = $"Dear {user.Name},\n\nYour booking has been confirmed!\n\nBooking ID: {booking.Id}\nCheck-in Date: {booking.CheckInDate:MMMM dd, yyyy}\nCheck-out Date: {booking.CheckOutDate:MMMM dd, yyyy}\nTotal Payment: {booking.TotalPrice:C}\n\nThank you for choosing us!\n\nBest regards,\nThe Booking Team";
            return client.SendMailAsync(new MailMessage(
                from: _mail,
                to: receiver,
                subject,
                message));
        }
        public Task SendEmailForCheckout(string email, Booking booking,string name)
        {
            var client = CreateSmtpClient();

            var subject = "Checkout Reminder";
            var message =
                $"Hey {name},\nJust a reminder that today is your checkout day for {booking.Apartment.Name}. We hope you had a great stay! Please make sure to vacate the premises by the designated checkout time.\nSafe travels, and we hope to see you again soon!\nBest regards,\nThe Booking Team";

            return client.SendMailAsync(new MailMessage(
                from: _mail,
                to: email,
                subject,
                message));
        }


        private SmtpClient CreateSmtpClient()
        {
            return new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(_mail, _password),
                EnableSsl = true
            };
        }


    }
}