using acordemus.DTOs;
using acordemus.Models;
using acordemus.Services;

namespace acordemus.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/users");
            group.MapGet("/", (IUserService svc) => svc.GetAsync()).RequireAuthorization();
            group.MapGet("/{id}", (IUserService svc, string id) => svc.GetByIdAsync(id)).RequireAuthorization();
            group.MapPost("/", (IUserService svc, User user, HttpContext context) => svc.CreateAsync(user, context)).RequireAuthorization();
        }
    }
}
