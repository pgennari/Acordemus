using acordemus.Models;  
using MongoDB.Driver;
using System.Security.Claims;

namespace acordemus.Services
{
    public interface IPersonService
    {
        Task<List<Person>> GetAsync(string expand);
        Task<Person> GetByIdAsync(string id, string expand);
        Task<Person> CreateAsync(Person person, HttpContext context);
        Task UpdateAsync(string id, Person person, HttpContext context);
        Task DeleteAsync(string id);

    }
    public class PersonService(IMongoDatabase database,IRoleService roleService) : IPersonService
    {
        private readonly IMongoCollection<Person> _peopleCollection = database.GetCollection<Person>("people");
        public async Task<Person> CreateAsync(Person person, HttpContext context)
        {
            person.CreatedAt = DateTime.Now;
            await _peopleCollection.InsertOneAsync(person);
            return person;
        }

        public async Task DeleteAsync(string id)
        {
            await _peopleCollection.DeleteOneAsync(x => x.id == id);
        }

        public Task<List<Person>> GetAsync(string expand)
        {
            var people = _peopleCollection.Find(_ => true)
                                   .ToListAsync();

            //if (!string.IsNullOrEmpty(expand) && "roles|all".Contains(expand))
            //    foreach (var person in people)
            //        roleService.GetAsync(expand).Result.FindAll(doc => doc.CondoId == person.Id).ForEach(d => person.Documents.Add(d));

            return people;

        }

        public Task<Person> GetByIdAsync(string id, string expand)
        {
            var person = _peopleCollection.Find(x => x.id == id)
                             .FirstOrDefaultAsync();
            
            //if (!string.IsNullOrEmpty(expand) && "roles|all".Contains(expand))
            //    person.Roles = roleService.GetAsync(expand).Result.FindAll(doc => doc.CondoId == person.Id).ToList();
            
            return person;
        }

        public async Task UpdateAsync(string id, Person person, HttpContext context)
        {
            var actualPerson = await _peopleCollection.Find(x => x.id == id).FirstOrDefaultAsync();
            if (actualPerson == null)
                throw new Exception("Person not found");
            
            actualPerson.name = person.name ?? actualPerson.name;
            actualPerson.email = person.email ?? actualPerson.email;
            actualPerson.phoneNumber = person.phoneNumber ?? actualPerson.phoneNumber;
            actualPerson.socialName = person.socialName ?? actualPerson.socialName;
            actualPerson.cpf = person.cpf ?? actualPerson.cpf;
            actualPerson.UpdatedAt = DateTime.Now;
            actualPerson.UpdatedBy = context.User.FindFirstValue("sub");

            await _peopleCollection.ReplaceOneAsync(x => x.id == id, actualPerson);
        }
    }
}
