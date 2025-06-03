using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace Interfaces.IRepositories
{
    public interface IChatMessageRepository
    {
        public void SaveMessage(int senderId, int receiverId, string message,DateTime TimeSent);
        public List<Message> GetAllMessages(int userId);
        //public bool DeleteMessage(int messageId);
        //public bool DoesMessageExist(int messageId);
    }
}
