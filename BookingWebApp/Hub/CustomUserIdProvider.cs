using Microsoft.AspNetCore.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {

        var userId = connection.User?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return "2"; // Default to host if no user ID is found
        }

        return userId;


    }
}