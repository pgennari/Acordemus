using acordemus.Models;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface IDocumentService
    {
        Task<List<Document>> GetAsync(string expand);
        Task<Document> GetByIdAsync(string id, string expand);
        Task<Document> CreateAsync(Document Document, HttpContext context);
        Task UpdateAsync(string id, Document Document, HttpContext context);
        Task DeleteAsync(string id);
    }
    public class DocumentService(IMongoDatabase database, IExcerptService excerptService) : IDocumentService
    {
        private readonly IMongoCollection<Document> _documentsCollection = database.GetCollection<Document>("documents");

        public async Task<Document> CreateAsync(Document document, HttpContext context)
        {
            document.CreatedBy = context.User.FindFirstValue("sub");
            document.CreatedAt = DateTime.Now;
            await _documentsCollection.InsertOneAsync(document);
            return document;
        }

        public async Task DeleteAsync(string id) => await _documentsCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Document>> GetAsync(string expand)
        {
            var docs = await _documentsCollection.Find(_ => true)
                                  .ToListAsync();

            if (!string.IsNullOrEmpty(expand) && "excerpts|all".Contains(expand))
                foreach (var doc in docs)
                    excerptService.GetAsync(expand).Result.FindAll(exc => exc.DocumentId == doc.Id && exc.ParentId == null).ForEach(e=> doc.Excerpts.Add(e));

            return docs;
        }
        public async Task<Document> GetByIdAsync(string id, string expand)
        {
            var doc = await _documentsCollection.Find(filter: c => c.Id == id)
                            .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(expand) && "excerpts|all".Contains(expand))
                doc.Excerpts = excerptService.GetAsync(expand).Result.FindAll(exc => exc.DocumentId == doc.Id).ToList();

            return doc;
        }

        public async Task UpdateAsync(string id, Document documentUpdated, HttpContext context)
        {
            var actualDocument = await _documentsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            actualDocument.Name = documentUpdated.Name;
            actualDocument.Description = documentUpdated.Description;
            actualDocument.UpdatedBy = context.User.FindFirstValue("sub");
            actualDocument.UpdatedAt = DateTime.Now;
            await _documentsCollection.ReplaceOneAsync(x => x.Id == id, actualDocument);

        }
    }
}
