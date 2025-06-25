using acordemus.Data;
using acordemus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace acordemus.Endpoints
{
    public static class Users
    {
        public static async Task<IResult> Get([FromServices] AppDbContext dbContext)
        {
            var users = await dbContext.Users.ToListAsync();
            return Results.Ok(users);
        }
        public static async Task<IResult> Handler(
            HttpContext context,
            HttpResponse response,
            [FromServices] AppDbContext dbContext)
        {
            string body;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }
            Console.WriteLine($"Notification: {body}");

            var snsMessage = JsonSerializer.Deserialize<SnsSubscription>(body);

            if (snsMessage?.Type == "Notification")
            {

                // Deserialize the inner message
                Console.WriteLine($"Message: {snsMessage.Message}");
                var peopleEvent = JsonSerializer.Deserialize<PeopleEvent>(snsMessage.Message);
                
                User user = new User()
                {
                    peopleId = peopleEvent.PeopleId,
                    name = peopleEvent.Data.Name,
                    email = peopleEvent.Data.Email,
                    phoneNumber = peopleEvent.Data.PhoneNumber,
                    socialName = peopleEvent.Data.Name
                };

                // Create a database record
                var record = new NotificationRecord()
                {
                    MessageId = snsMessage.MessageId,
                    TopicArn = snsMessage.TopicArn,
                    Subject = snsMessage.Subject,
                    Message = snsMessage.Message,
                    Timestamp = snsMessage.Timestamp
                };

                // Save to the database
                dbContext.Users.Add(user);
                dbContext.NotificationRecords.Add(record);
                await dbContext.SaveChangesAsync();

                return Results.Created();
            }
            else if (snsMessage?.Type == "SubscriptionConfirmation")
            {
                // Handle subscription confirmation (e.g., call snsMessage.SubscribeURL)
                Console.WriteLine($"Subscription Confirmation: {snsMessage.SubscribeURL}");
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(snsMessage.SubscribeURL);
                var confirmation = await client.GetAsync(snsMessage.SubscribeURL);
                Console.WriteLine($"Response Confirmation: {confirmation.Content.ReadAsStringAsync().Result}");
                // You might want to make an HTTP request to the SubscribeURL here
                return Results.Ok("Subscription Confirmation received.");
            }
            else
            {
                Console.WriteLine($"Other message type: {snsMessage?.Type}");
                return Results.BadRequest("Invalid message type.");
            }
        }
    }
    public class SnsSubscription
    {
        public string Type { get; set; }
        public string MessageId { get; set; }
        public string Token { get; set; }
        public string TopicArn { get; set; }
        public string Message { get; set; }
        public string SubscribeURL { get; set; }
        public DateTime Timestamp { get; set; }
        public string Subject { get; set; }
    }

    public class PeopleEvent
    {
        [JsonPropertyName("isSnapshot")]
        public bool IsSnapshot { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("peopleId")]
        public string PeopleId { get; set; }

        [JsonPropertyName("data")]
        public PeopleData Data { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }
    }

    public class PeopleData
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }


}
