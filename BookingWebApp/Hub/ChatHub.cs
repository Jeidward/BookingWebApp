using Microsoft.AspNetCore.SignalR;
using Services;

namespace BookingWebApp.Hub;

public class ChatHub(ChatService chatService, UserService userService, ILogger<ChatHub> logger) : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task SendMessage(string message, string? targetCustomerId = null)
    {

        message = null;
        if (string.IsNullOrEmpty(message))
        {
            logger.LogError("Message cannot be null or empty.");
            throw new ArgumentNullException(nameof(message), "should not be null");

        }

        var senderId = Context.User?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        if (string.IsNullOrEmpty(senderId)) //using Ilogger
        {
            var hostId = "2";
            senderId = hostId;
        }

        var timestamp = DateTime.Now;

        if (senderId == "2")
        {
            if (string.IsNullOrEmpty(targetCustomerId))
            {
                logger.LogError("Host tried to send a message but no customer ID provided.");
                Console.WriteLine("Host tried to send a message but no customer ID provided.");
                return;
            }

            await Clients.User(targetCustomerId).SendAsync("ReceiveMessage", "Host", message, timestamp.ToString("HH:mm"));
            await Clients.Caller.SendAsync("ReceiveMessage", "you", message, timestamp.ToString("HH:mm"));

        }
        else
        {
            var user = userService.GetUser(int.Parse(senderId));
            await Clients.User("2").SendAsync("ReceiveMessage", user.FirstName, message, timestamp.ToString("HH:mm")); // Send to host
            await Clients.User(senderId).SendAsync("ReceiveMessage", "you", message, timestamp.ToString("HH:mm")); // Send to the customer themselves
        }

        chatService.SaveMessage(int.Parse(senderId), int.Parse(targetCustomerId ?? "2"), message, timestamp);

    }

}