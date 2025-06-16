using acordemus.Models;
using acordemus.Services;

namespace acordemus.Endpoints
{
    public static class ExcerptEndpoints
    {
        public static void MapExcerptEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/excerpts");
            group.MapGet("/", (IExcerptService svc, string expand="") => svc.GetAsync(expand)).RequireAuthorization();
            group.MapGet("/{excerptId}", (IExcerptService svc, string excerptId, string expand = "") => svc.GetByIdAsync(excerptId, expand)).RequireAuthorization();
            group.MapPost("/", (IExcerptService svc, Excerpt excerpt, HttpContext context) => svc.CreateAsync(excerpt, context)).RequireAuthorization();
            group.MapPut("/{excerptId}", (IExcerptService svc, string excerptId, Excerpt excerpt, HttpContext context) => svc.UpdateAsync(excerptId, excerpt, context)).RequireAuthorization();
            group.MapPatch("/{excerptId}/like", (IExcerptService svc, string excerptId, HttpContext context) => svc.RegisterLike(excerptId, context)).RequireAuthorization();
            group.MapPatch("/{excerptId}/dislike", (IExcerptService svc, string excerptId, HttpContext context) => svc.RegisterDislike(excerptId, context)).RequireAuthorization();
            group.MapDelete("/{excerptId}", (IExcerptService svc, string excerptId) => svc.DeleteAsync(excerptId)).RequireAuthorization();

        }
    }
}
