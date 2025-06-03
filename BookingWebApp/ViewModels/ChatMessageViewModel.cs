using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class ChatMessageViewModel
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message {get; set; }
        public DateTime TimeSent { get; set; }

        public static ChatMessageViewModel ConvertToViewModel(Message message)
        {
            return new ChatMessageViewModel
            {
                SenderId = message.SenderId.ToString(),
                ReceiverId = message.ReceiverId.ToString(),
                Message = message.Content,
                TimeSent = message.TimeSent
            };
        }

        public static List<ChatMessageViewModel> ConvertToViewModel(List<Message> messages)
        {
            var chatMessages = new List<ChatMessageViewModel>();
            foreach (var message in messages)
            {
                chatMessages.Add(ConvertToViewModel(message));
            }
            return chatMessages;
        }

    }
    
}
