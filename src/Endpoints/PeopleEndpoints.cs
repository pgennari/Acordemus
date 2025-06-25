using acordemus.Models;
using acordemus.Services;
using Microsoft.AspNetCore.Mvc;

namespace acordemus.Endpoints
{
    public static class PeopleEndpoints
    {
        public static void MapPeopleEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/people");
            group.MapGet("/", (IPersonService svc, [FromQuery] string expand = "") => svc.GetAsync(expand)).RequireAuthorization();
            group.MapGet("/{id}", (IPersonService svc, string id, [FromQuery] string expand = "") => svc.GetByIdAsync(id, expand)).RequireAuthorization();
            group.MapPost("/", (IPersonService svc, Person person, HttpContext context) => svc.CreateAsync(person, context));
            group.MapPatch("/{id}", (IPersonService svc, string id, Person person, HttpContext context) => svc.UpdateAsync(id, person, context)).RequireAuthorization();
            group.MapDelete("/{id}", (IPersonService svc, string id) => svc.DeleteAsync(id)).RequireAuthorization(auth => auth.RequireRole("Admin"));
        }
    }
}
