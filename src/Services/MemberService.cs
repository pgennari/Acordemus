using acordemus.Models;
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface IMemberService
    {
        Task<bool> PersonHasRoleAsync(string personId, string condoId, string roleName);
        Task<Member> CreateMamberAsync(Member member, HttpContext context);
        Task<bool> UpdateMemberAsync(Member member, HttpContext context);
        Task<List<Role>> GetPersonRoles(Member member);
    }
    public class MemberService : IMemberService
    {
        private readonly IMongoCollection<Member> _membershipCollection;

        public MemberService(IMongoDatabase database)
        {
            _membershipCollection = database.GetCollection<Member>("membership");
        }

        public async Task<bool> PersonHasRoleAsync(string personId, string condoId, string roleName)
        {
            var member = await _membershipCollection.Find(p => p.PersonId == personId && p.CondoId == condoId).FirstOrDefaultAsync() ?? throw new ArgumentException("Member not found");

            if (member.Roles.Exists(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
                return true;

            return false;
        }

        public async Task<Member> CreateMamberAsync(Member member, HttpContext context)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            member.CreatedBy = context.User.FindFirstValue("sub");
            member.CreatedAt = DateTime.Now;
            
            await _membershipCollection.InsertOneAsync(member);
            
            return member;
        }
        public async Task<bool> UpdateMemberAsync(Member member,  HttpContext context)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            var existingMember = await _membershipCollection.Find(m => m.Id == member.Id).FirstOrDefaultAsync();
            if (existingMember == null)
                throw new ArgumentException("Member not found");
            
            existingMember.Roles = member.Roles;
            existingMember.UpdatedBy = context.User.FindFirstValue("sub");
            existingMember.UpdatedAt = DateTime.Now;
            
            var result = await _membershipCollection.ReplaceOneAsync(m => m.Id == member.Id, existingMember);
            
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<List<Role>> GetPersonRoles(Member member)
        {
            member = await _membershipCollection.Find(p => p.PersonId == member.PersonId && p.CondoId == member.CondoId).FirstOrDefaultAsync() ?? throw new ArgumentException("Member not found");

            return member.Roles;
        }
    }
}
