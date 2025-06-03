using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Services
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Assuming the user ID is stored in the connection's User property
            // You can customize this logic based on your authentication mechanism
            return connection.User?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value
                   ?? connection.User?.Identity?.Name
                   ?? connection.ConnectionId; // Fallback to ConnectionId if no user ID is found     
        }
    }
}
