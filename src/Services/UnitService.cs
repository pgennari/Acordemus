using acordemus.DTOs;
using acordemus.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface IUnitService
    {
        Task<List<Unit>> GenerateUnits(string condoId, GenerateUnits generateUnits, HttpContext context);
        Task<Unit> AssociateOwnerToUnit(string condoId, AssociateUnit associateUnit, HttpContext context);

        Task<Unit> DisassociateOwnerToUnit(string condoId, AssociateUnit associateUnit, HttpContext context);
        Task<IResult> DeleteUnit(string condoId, string unitId);
    }
    public class UnitService(IMongoDatabase database) : IUnitService
    {
        private readonly IMongoCollection<Condo> _condoCollection = database.GetCollection<Condo>("condos");

        public async Task<Unit> AssociateOwnerToUnit(string condoId, AssociateUnit associateUnit, HttpContext context)
        {
            var condo = await _condoCollection.Find(x => x.Id == condoId)
                                              .FirstOrDefaultAsync();
            if (condo == null)
                throw new Exception("Condo not found");

            var unitPersisted = condo.Units?.Find(u => u.Id == associateUnit.unitId);

            if (unitPersisted == null)
                throw new Exception("Unit not found");

            unitPersisted.Owners.Add(associateUnit.personId);
            unitPersisted.UpdatedBy = context.User.FindFirstValue("sub");
            unitPersisted.UpdatedAt = DateTime.Now;
            await _condoCollection.ReplaceOneAsync(x => x.Id == condoId, condo);
            return unitPersisted;

        }
        public async Task<Unit> DisassociateOwnerToUnit(string condoId, AssociateUnit associateUnit, HttpContext context)
        {
            var condo = await _condoCollection.Find(x => x.Id == condoId)
                                              .FirstOrDefaultAsync();
            if (condo == null)
                throw new Exception("Condo not found");

            var unitPersisted = condo.Units?.Find(u => u.Id == associateUnit.unitId);

            if (unitPersisted == null)
                throw new Exception("Unit not found");

            unitPersisted.Owners.Remove(associateUnit.personId);
            unitPersisted.UpdatedBy = context.User.FindFirstValue("sub");
            unitPersisted.UpdatedAt = DateTime.Now;
            await _condoCollection.ReplaceOneAsync(x => x.Id == condoId, condo);
            return unitPersisted;

        }

        public async Task<IResult> DeleteUnit(string condoId, string unitId)
        {
            var condo = await _condoCollection.Find(x => x.Id == condoId)
                                              .FirstOrDefaultAsync();
            if (condo == null)
                throw new Exception("Condo not found");

            var unit = condo.Units?.Find(u => u.Id == unitId);

            if (unit == null)
                throw new Exception("Unit not found");

            condo.Units?.Remove(unit);

            await _condoCollection.ReplaceOneAsync(x => x.Id == condoId, condo);

            return Results.NoContent();
        }

        public async Task<List<Unit>> GenerateUnits(string condoId, GenerateUnits generateUnits, HttpContext context)
        {
            var condo = await _condoCollection.Find(x => x.Id == condoId)
                                              .FirstOrDefaultAsync();
            if (condo == null)
                throw new Exception("Condo not found");

            for (int floor = 0; floor <= generateUnits.NumberOfFloors; floor++)
            {
                for (int unitNumber = 1; unitNumber <= generateUnits.NumberOfUnitsPerFloor; unitNumber++)
                {
                    var unit = new Unit
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        Block = generateUnits.Block,
                        Number = string.Concat(floor, unitNumber),
                        CreatedBy = context.User.FindFirstValue("sub"),
                        CreatedAt = DateTime.Now
                    };
                    condo.Units?.Add(unit);
                }
            }

            await _condoCollection.ReplaceOneAsync(x => x.Id == condoId, condo);
            return condo.Units;
        }
    }
}
