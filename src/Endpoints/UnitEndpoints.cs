using acordemus.DTOs;
using acordemus.Services;

namespace acordemus.Endpoints
{
    public static class UnitEndpoints
    {
        public static void MapUnitEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/condos/{condoId}/units");
            group.MapPost("/generate", (IUnitService svc, string condoId, GenerateUnits generateUnits, HttpContext context) => svc.GenerateUnits(condoId, generateUnits, context)).RequireAuthorization();
            group.MapPatch("/associate", (IUnitService svc, string condoId, AssociateUnit unit, HttpContext context) => svc.AssociateOwnerToUnit(condoId, unit, context)).RequireAuthorization();
            group.MapPatch("/disassociate", (IUnitService svc, string condoId, AssociateUnit unit, HttpContext context) => svc.DisassociateOwnerToUnit(condoId, unit, context)).RequireAuthorization();
            group.MapDelete("/{unitId}", (IUnitService svc, string condoId, string unitId) => svc.DeleteUnit(condoId, unitId)).RequireAuthorization();
        }
    }
}
