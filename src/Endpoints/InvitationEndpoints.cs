using acordemus.Services;
using Microsoft.AspNetCore.Mvc;

namespace acordemus.Endpoints
{
    public static class InvitationEndpoints
    {
        public static void MapInvitationEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/invites").RequireAuthorization();

            group.MapPost("/", (IInvitationService svc, [FromBody] InvitationRequest request, HttpContext context) =>
                svc.CreateAsync(request.CondoId, request.Email, context));
        }
    }

    public record InvitationRequest(string CondoId, string Email);
}
