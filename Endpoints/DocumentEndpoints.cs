using acordemus.Models;
using acordemus.Services;
using Microsoft.AspNetCore.Mvc;

namespace acordemus.Endpoints
{
    public static class DocumentEndpoints
    {
        public static void MapDocumentEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/documents");
            group.MapGet("/", (IDocumentService svc, [FromQuery] string expand = "") => svc.GetAsync(expand)).RequireAuthorization();
            group.MapGet("/{documentId}", (IDocumentService svc, string documentId, [FromQuery] string expand = "") => svc.GetByIdAsync( documentId, expand)).RequireAuthorization();
            group.MapPost("/", (IDocumentService svc, Document document, HttpContext context) => svc.CreateAsync(document, context)).RequireAuthorization();
            group.MapPut("/{documentId}", (IDocumentService svc, string documentId, Document document, HttpContext context) => svc.UpdateAsync(documentId, document, context)).RequireAuthorization();
            group.MapDelete("/{documentId}", (IDocumentService svc, string documentId) => svc.DeleteAsync(documentId)).RequireAuthorization();

        }
    }
}
