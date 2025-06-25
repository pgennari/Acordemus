using acordemus.Models;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface IExcerptService
    {
        Task<List<Excerpt>> GetAsync(string expand);
        Task<Excerpt> GetByIdAsync(string excerptId, string expand);
        Task<Excerpt> CreateAsync(Excerpt excerpt, HttpContext context);
        Task UpdateAsync(string excerptId, Excerpt excerpt, HttpContext context);
        Task DeleteAsync(string excerptId);
        Task<IResult> RegisterLike(string excerptId, HttpContext context);
        Task<IResult> RegisterDislike(string excerptId, HttpContext context);
    }
    public class ExcerptService(IMongoDatabase database, ICommentService commentService) : IExcerptService
    {
        private readonly IMongoCollection<Excerpt> _excerptCollection = database.GetCollection<Excerpt>("excerpts");

        public async Task<Excerpt> CreateAsync(Excerpt excerpt, HttpContext context)
        {
            excerpt.CreatedBy = context.User.FindFirstValue("sub");
            excerpt.CreatedAt = DateTime.Now;
            await _excerptCollection.InsertOneAsync(excerpt);
            return excerpt;
        }

        public async Task DeleteAsync(string id) => await _excerptCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Excerpt>> GetAsync(string expand)
        {
            var excerpts = await _excerptCollection.Find(_ => true).ToListAsync();

            if (!string.IsNullOrEmpty(expand) && "excerpts|all".Contains(expand))
                foreach (var excerpt in excerpts)
                    GetChildExcerpts(excerpt.Id).Result.ForEach(excerpt.Excerpts.Add);

            if (!string.IsNullOrEmpty(expand) && "comments|all".Contains(expand))
                foreach (var excerpt in excerpts)
                    commentService.GetAsync(expand).Result.FindAll(comm => comm.ExcerptId == excerpt.Id).ForEach(excerpt.Comments.Add);

            return [.. excerpts.OrderBy(o => o.Order)];
        }

        public async Task<Excerpt> GetByIdAsync(string id, string expand)
        {
            var excerpt = await _excerptCollection.Find(x => x.Id == id).FirstOrDefaultAsync();


            if (!string.IsNullOrEmpty(expand) && "excerpts|all".Contains(expand))
                GetChildExcerpts(id).Result.ForEach(excerpt.Excerpts.Add);
            
            if (!string.IsNullOrEmpty(expand) && "comments|all".Contains(expand))
                commentService.GetAsync(expand).Result.FindAll(comm => comm.ExcerptId == excerpt.Id).ForEach(excerpt.Comments.Add);

            return excerpt;
        }

        private async Task<List<Excerpt>> GetChildExcerpts(string parentId)
        {
            var childs = await _excerptCollection.Find(x => x.ParentId == parentId).ToListAsync();

            foreach (var child in childs)
            {
                child.Excerpts = GetChildExcerpts(child.Id).Result;
            }
            return childs;
        }

        public async Task<IResult> RegisterLike(string excerptId, HttpContext context)
        {
            var actualExcerpt = await _excerptCollection.Find(x => x.Id == excerptId)
                                              .FirstOrDefaultAsync();
            actualExcerpt.Likes.Add(context.User.FindFirstValue("sub"));
            await _excerptCollection.ReplaceOneAsync(x => x.Id == excerptId, actualExcerpt);
            return Results.NoContent();
        }
        public async Task<IResult> RegisterDislike(string excerptId, HttpContext context)
        {
            var actualExcerpt = await _excerptCollection.Find(x => x.Id == excerptId)
                                              .FirstOrDefaultAsync();
            actualExcerpt.Dislikes.Add(context.User.FindFirstValue("sub"));
            await _excerptCollection.ReplaceOneAsync(x => x.Id == excerptId, actualExcerpt);
            return Results.NoContent();
        }

        public async Task UpdateAsync(string excerptId, Excerpt excerptUpdated, HttpContext context)
        {
            var actualExcerpt = await _excerptCollection.Find(x => x.Id == excerptId)
                                              .FirstOrDefaultAsync();
            if (actualExcerpt != null)
            {
                actualExcerpt.Title = excerptUpdated.Title ?? actualExcerpt.Title;
                actualExcerpt.Content = excerptUpdated.Content ?? actualExcerpt.Content;
                actualExcerpt.Type = excerptUpdated.Type ?? actualExcerpt.Type;
                actualExcerpt.Status = excerptUpdated.Status ?? actualExcerpt.Status;
                actualExcerpt.Order = excerptUpdated.Order ?? actualExcerpt.Order;
                actualExcerpt.UpdatedBy = context.User.FindFirstValue("sub");
                actualExcerpt.UpdatedAt = DateTime.Now;
                await _excerptCollection.ReplaceOneAsync(x => x.Id == excerptId, actualExcerpt);
            }
        }
    }
}
