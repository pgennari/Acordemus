using acordemus.Models;
using acordemus.Services;
using Microsoft.AspNetCore.Mvc;

namespace acordemus.Endpoints
{
    public static class CondoEndpoints
    {
        public static void MapCondoEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/condos");
            group.MapGet("/", (ICondoService svc, [FromQuery] string expand = "") => svc.GetAsync(expand)).RequireAuthorization();
            group.MapGet("/{id}", (ICondoService svc, string id, [FromQuery] string expand = "") => svc.GetByIdAsync(id, expand)).RequireAuthorization();
            group.MapPost("/", (ICondoService svc, Condo condo, HttpContext context) => svc.CreateAsync(condo, context)).RequireAuthorization(auth => auth.RequireRole("Admin"));
            group.MapPatch("/{id}", (ICondoService svc, string id, Condo condo, HttpContext context) => svc.UpdateAsync(id, condo, context)).RequireAuthorization(auth => auth.RequireRole("Admin"));
            group.MapDelete("/{id}", (ICondoService svc, string id) => svc.DeleteAsync(id)).RequireAuthorization(auth => auth.RequireRole("Admin"));
        }
    }
}
