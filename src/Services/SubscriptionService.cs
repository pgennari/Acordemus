using acordemus.Models;
using MongoDB.Driver;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace acordemus.Services
{

    public interface ISubscriptionService
    {
        Task<IResult> ReceiveNotification(SnsSubscription subscription);
    }

    public class SubscriptionService(IMongoDatabase database) : ISubscriptionService
    {

        private readonly IMongoCollection<User> _userCollection = database.GetCollection<User>("users");

        public async Task<IResult> ReceiveNotification(SnsSubscription subscription)
        {
            if (subscription.Type == "SubscriptionConfirmation")
            {
                // Handle subscription confirmation
                // You can use the SubscribeURL to confirm the subscription
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(subscription.SubscribeURL);
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Subscription confirmed");
                        return Results.Ok("Subscription confirmed");
                    }
                }
            }
            else if (subscription.Type == "Notification")
            {
                // Handle notification
                // Process the message as needed
                var peopleEvent = JsonSerializer.Deserialize<PeopleEvent>(subscription.Message);

                User user = new()
                {
                    Id= peopleEvent.PeopleId,
                    Name = peopleEvent.Data.socialName ?? peopleEvent.Data.Name,
                    CreatedAt = DateTime.Now
                };

                _userCollection.InsertOne(user);

            }
            return Results.Ok("Notification processed");
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

        [JsonPropertyName("socialName")]
        public string socialName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
