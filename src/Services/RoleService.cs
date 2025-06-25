using acordemus.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace acordemus.Services
{
	public interface IRoleService
	{
		Task<IResult> PersonHasRoleAsync(string peopleId, string condoId, string roleName);
		Task<IResult> AddRoleToPersonAsync(string peopleId, string condoId, string roleName);
	}
    public class RoleService : IRoleService
    {
        private readonly IMongoCollection<Person> _peopleCollection;

        public RoleService(IMongoDatabase database)
        {
            _peopleCollection = database.GetCollection<Person>("people");
        }

        public async Task<IResult> PersonHasRoleAsync(string peopleId, string condoId, string roleName)
        {
            var person = await _peopleCollection.Find(p => p.id == peopleId).FirstOrDefaultAsync();
            if (person == null)
                return Results.BadRequest("Person not found");

            // Fix: Check if the dictionary contains a key with the condoId
            var condo = person.Roles.Keys.FirstOrDefault(c => c.Id == condoId);
            if (condo == null)
                return Results.NotFound("Condo not found in person's roles");

            var role = person.Roles[condo];
            if (role.Name?.Equals(roleName, StringComparison.OrdinalIgnoreCase) == true)
                return Results.Ok(true);

            return Results.Ok(false);
        }

        public async Task<IResult> AddRoleToPersonAsync(string peopleId, string condoId, string roleName)
        {
            var person = await _peopleCollection.Find(p => p.id == peopleId).FirstOrDefaultAsync();
            if (person == null)
                return Results.BadRequest("Person not found");

            // Fix: Add a role to the dictionary if it doesn't already exist
            var condo = person.Roles.Keys.FirstOrDefault(); // Assuming a condo is already present
            if (condo == null)
                return Results.BadRequest("No condo found for the person");

            if (!person.Roles.TryGetValue(condo, out var role) || role.Name != roleName)
            {
                person.Roles[condo] = new Role { Name = roleName };
                await _peopleCollection.ReplaceOneAsync(p => p.id == peopleId, person);
            }

            return Results.Ok("Role added successfully");
        }
    }
}
