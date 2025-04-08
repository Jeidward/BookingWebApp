namespace BookingWebApp.ViewModels
{
    public class CheckOutFormViewModel
    {  //[MustBeTrue(ErrorMessage = "Please leave your key in the mailbox.")]// will create a custom must be true.
        public bool KeyLeft { get; set; }
        //[Required(ErrorMessage = "Please check whether you have personal belongings left in the room.")]
        public bool PersonalBelongings { get; set; }
        //[Required(ErrorMessage = "Please check the room condition.")]
        public bool RoomCondition { get; set; }

        public bool SkipReview { get; set; }
    }
}
