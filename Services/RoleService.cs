using acordemus.Data;
using acordemus.Models;
using Microsoft.AspNetCore.Mvc;

namespace acordemus.Services
{
    public class RoleService([FromServices] AppDbContext dbContext)
    {

        public async Task<bool> UserHasRoleAsync(string peopleId, string roleName)
        {
            var user = await dbContext.Users.FindAsync(peopleId);
            if (user == null)
                return false;
            return user.Roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }
        public async Task AddRoleToUserAsync(string peopleId, string roleName)
        {
            var user = await dbContext.Users.FindAsync(peopleId);
            if (user == null)
                throw new ArgumentException("User not found");
            if (!user.Roles.Any(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)))
            {
                user.Roles.Add(new Role { Name = roleName });
                await dbContext.SaveChangesAsync();
            }
        }

    }
}
