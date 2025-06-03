using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IRepositories;
using Models.Entities;

namespace Services
{
    public class ChatService
    {
        private readonly IChatMessageRepository _chatMessageRepository;

        public ChatService(IChatMessageRepository chatMessageRepository)
        {
            _chatMessageRepository = chatMessageRepository;
        }
        public void SaveMessage(int senderId, int receiverId, string message,DateTime  TimeSent)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty.", nameof(message));
            if (senderId <= 0)
                throw new ArgumentException("Sender ID must be greater than zero.", nameof(senderId));

            _chatMessageRepository.SaveMessage(senderId, receiverId, message,TimeSent);
        }

        public List<Message> GetAllMessages(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than zero.", nameof(userId));

            return _chatMessageRepository.GetAllMessages(userId);
        }
    }
}
