using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Message(string content, DateTime timeSent)
    {
        public int SenderId { get; } 
        public int ReceiverId { get; } 
        public string Content { get; } = content;
        public DateTime TimeSent { get; } = timeSent;
    }
}
