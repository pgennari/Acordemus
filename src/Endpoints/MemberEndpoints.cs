using acordemus.Models;
using acordemus.Services;
using Microsoft.AspNetCore.Mvc;

namespace acordemus.Endpoints
{
    public static class MemberEndpoints
    {
        public static void MapMemberEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/members");
            group.MapPost("/", (IMemberService svc, Member member, HttpContext context) => svc.CreateMamberAsync(member, context)).RequireAuthorization();
            group.MapPatch("/", (IMemberService svc, Member member, HttpContext context) => svc.UpdateMemberAsync(member, context)).RequireAuthorization(auth => auth.RequireRole("Admin"));
            group.MapGet("/roles", (IMemberService svc, [FromBody]Member member) => svc.GetPersonRoles(member)).RequireAuthorization();
        }

    }
}
