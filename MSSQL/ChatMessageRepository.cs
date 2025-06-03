using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Interfaces.IRepositories
{
    public class ChatMessageRepository : Repository, IChatMessageRepository
    {
        public ChatMessageRepository(IConfiguration configuration) : base(configuration) { }
        public void SaveMessage(int senderId, int receiverId, string message,DateTime TimeSent)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"INSERT INTO ChatMessage
                                ([SenderId], [ReceiverId], [Message],[TimeSent],HasAlreadySend) VALUES (@SenderId, @ReceiverId, @Message,@TimeSent,1)";
            using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@SenderId", senderId);
            cmd.Parameters.AddWithValue("@ReceiverId", receiverId);
            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@TimeSent", TimeSent);
            cmd.ExecuteNonQuery();
        }


        public List<Message> GetAllMessages(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
            SELECT Message, TimeSent 
            FROM ChatMessage 
            WHERE (SenderId = @UserId AND ReceiverId = @HostId)
               OR (SenderId = @HostId AND ReceiverId = @UserId)
            ORDER BY TimeSent";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@HostId", 2); 

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var messages = new List<Message>();
                        while (reader.Read())
                        {
                            var message = new Message(
                                Convert.ToString(reader["Message"])!,
                                Convert.ToDateTime(reader["TimeSent"])
                            );
                            messages.Add(message);
                        }

                        return messages;
                    }
                }
            }
        }







    }


}
