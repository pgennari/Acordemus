using acordemus.Models;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface ICondoService
    {
        Task<List<Condo>> GetAsync(string expand);
        Task<Condo> GetByIdAsync(string id, string expand);
        Task<Condo> CreateAsync(Condo condo, HttpContext context);
        Task UpdateAsync(string id, Condo condo, HttpContext context);
        Task DeleteAsync(string id);
    }
    public class CondoService(IMongoDatabase database, IDocumentService documentService) : ICondoService
    {
        private readonly IMongoCollection<Condo> _condoCollection = database.GetCollection<Condo>("condos");

        public async Task<Condo> CreateAsync(Condo condo, HttpContext context)
        {
            condo.CreatedBy = context.User.FindFirstValue("sub");
            condo.CreatedAt = DateTime.Now;
            await _condoCollection.InsertOneAsync(condo);
            return condo;
        }

        public async Task DeleteAsync(string id) =>
            await _condoCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Condo>> GetAsync(string expand)
        {
            var condos = await _condoCollection.Find(_ => true)
                                  .ToListAsync();

            if (!string.IsNullOrEmpty(expand) && "documents|all".Contains(expand))
                foreach (var condo in condos)
                    documentService.GetAsync(expand).Result.FindAll(doc => doc.CondoId == condo.Id).ForEach(d=> condo.Documents.Add(d));

            return condos;
        }

        public async Task<Condo> GetByIdAsync(string id, string expand)
        {
            var condo = await _condoCollection.Find(x => x.Id == id)
                            .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(expand) && "documents|all".Contains(expand))
                condo.Documents = documentService.GetAsync(expand).Result.FindAll(doc => doc.CondoId == condo.Id).ToList();

            return condo;
        }

        public async Task UpdateAsync(string id, Condo updatedCondo, HttpContext context)
        {
            var actualCondo = await _condoCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

            actualCondo.Name = updatedCondo.Name;
            actualCondo.ShortName = updatedCondo.ShortName;
            actualCondo.UpdatedBy = context.User.FindFirstValue("sub");
            actualCondo.UpdatedAt = DateTime.Now;
            await _condoCollection.ReplaceOneAsync(x => x.Id == id, actualCondo);
        }
    }
}
