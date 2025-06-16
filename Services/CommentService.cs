using acordemus.Models;
using MongoDB.Driver;
using System.Text.Json;
using System.Xml.Linq;

namespace acordemus.Services
{
    public interface ICommentService
    {
        Task<List<Comment>> GetAsync(string expand);
        Task<Comment> GetByIdAsync(string commentId, string expand);
        Task<Comment> CreateAsync(Comment comment, HttpContext context);
        Task DeleteAsync(string commentId);
    }
    public class CommentService(IMongoDatabase database) : ICommentService
    {
        private readonly IMongoCollection<Comment> _commentCollection = database.GetCollection<Comment>("comments");

        public async Task<Comment> CreateAsync(Comment Comment, HttpContext context)
        {
            Comment.CreatedAt = DateTime.Now;
            await _commentCollection.InsertOneAsync(Comment);
            return Comment;
        }

        public async Task DeleteAsync(string id) => await _commentCollection.DeleteOneAsync(x => x.Id == id);

        public async Task<List<Comment>> GetAsync(string expand)
        {
            var comments =  await _commentCollection.Find(_ => true).ToListAsync();
            if ("comments|all".Contains(expand))
                foreach (var comment in comments)
                    comment.Comments = GetChildComments(comment.Id).Result;
            return comments;
        }

        public async Task<Comment> GetByIdAsync(string id, string expand)
        {
            var comment = await _commentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if ("comments|all".Contains(expand))
                    comment.Comments = GetChildComments(comment.Id).Result;
            return comment;

        }

        private async Task<List<Comment>> GetChildComments(string parentId)
        {
            var childs = await _commentCollection.Find(x => x.ParentId == parentId).ToListAsync();

            foreach (var child in childs)
            {
                child.Comments = GetChildComments(child.Id).Result;
            }
            return childs;
        }

        public async Task<bool> RegisterLike(string CommentId, HttpContext context)
        {
            var actualComment = await _commentCollection.Find(x => x.Id == CommentId)
                                              .FirstOrDefaultAsync();
            actualComment.Likes.Add(JsonSerializer.Deserialize<User>(context.User.FindFirst("UserData")?.Value).Id);
            await _commentCollection.ReplaceOneAsync(x => x.Id == CommentId, actualComment);
            return true;
        }
        public async Task<bool> RegisterDislike(string condoId, string documentId, string CommentId, string commentId, HttpContext context)
        {
            var actualComment = await _commentCollection.Find(x => x.Id == CommentId)
                                              .FirstOrDefaultAsync();
            actualComment.Dislikes.Add(JsonSerializer.Deserialize<User>(context.User.FindFirst("UserData")?.Value).Id);
            await _commentCollection.ReplaceOneAsync(x => x.Id == CommentId, actualComment);
            return true;
        }

    }
}
