using acordemus.Models;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface IInvitationService
    {
        Task<Invitation> CreateAsync(string condoId, string email, HttpContext context);
        Task<Invitation?> GetByTokenAsync(string token);
        Task MarkAsAcceptedAsync(string token);
    }

    public class InvitationService : IInvitationService
    {
        private readonly IMongoCollection<Invitation> _invitesCollection;

        public InvitationService(IMongoDatabase database)
        {
            _invitesCollection = database.GetCollection<Invitation>("invites");
        }

        public async Task<Invitation> CreateAsync(string condoId, string email, HttpContext context)
        {
            var invitation = new Invitation
            {
                CondoId = condoId,
                Email = email,
                CreatedBy = context?.User?.FindFirstValue("sub")
            };

            await _invitesCollection.InsertOneAsync(invitation);
            Console.WriteLine($"Invitation token for {email}: {invitation.Token}");
            return invitation;
        }

        public Task<Invitation?> GetByTokenAsync(string token)
        {
            return _invitesCollection.Find(i => i.Token == token).FirstOrDefaultAsync();
        }

        public async Task MarkAsAcceptedAsync(string token)
        {
            var update = Builders<Invitation>.Update.Set(i => i.AcceptedAt, DateTime.Now);
            await _invitesCollection.UpdateOneAsync(i => i.Token == token, update);
        }
    }
}
