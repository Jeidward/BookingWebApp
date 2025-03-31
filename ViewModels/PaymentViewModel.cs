namespace BookingWebApp.ViewModels
{
    public class PaymentViewModel
    {

        public string CardNumber { get; set; }
        public int CardExpirationMonth { get; set; }
        public int CardExpirationYear { get; set; }
        public string CardCVC { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

       

    };
}
