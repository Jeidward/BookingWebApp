using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class ChatPageViewModel
    {
        public List<ContactViewModel> Contacts { get; set; } = new List<ContactViewModel>();
        public List<ChatMessageViewModel> ChatMessage { get; set; } = new List<ChatMessageViewModel>();
        public int SelectedContactId { get; set; } // To track the selected contact

        public static ChatPageViewModel ConvertToViewModel(List<ContactViewModel> contacts, List<ChatMessageViewModel> chatMessages, int selectedContactId)
        {
            return new ChatPageViewModel
            {
                Contacts = contacts,
                ChatMessage = chatMessages,
                SelectedContactId = selectedContactId
            };
        }
    }
}
