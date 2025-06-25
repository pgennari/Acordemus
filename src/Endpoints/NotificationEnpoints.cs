using acordemus.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace acordemus.Endpoints
{
    public static class NotificationEnpoints
    {
        public static void MapNotificationEndpoints(this WebApplication app)
        {
            app.MapPost("/notification", async (ISubscriptionService subscriptionService, HttpRequest request) =>
            {
                using var reader = new StreamReader(request.Body);
                var bodyText = await reader.ReadToEndAsync();
                var subscription = JsonSerializer.Deserialize<SnsSubscription>(bodyText);
                return await subscriptionService.ReceiveNotification(subscription);
            })
            .Produces<IResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
