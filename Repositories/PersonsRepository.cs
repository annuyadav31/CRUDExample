using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        //Readonly field
        private readonly ApplicationDbContext _db;

        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonByPersonId(Guid personID)
        {
            Person? person =await _db.Persons.FirstOrDefaultAsync(temp=>temp.PersonId == personID);
            if (person != null)
            {
                _db.Persons.Remove(person);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("country").Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personID)
        {
            return await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == personID);
        }

        public async Task<Person?> UpdatePerson(Person person)
        {
            Person? personDetails = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonId == person.PersonId);
            if(personDetails == null) { return person; }

            personDetails.PersonName = person.PersonName;
            personDetails.Email = person.Email;
            personDetails.Gender = person.Gender;
            personDetails.Address = person.Address;
            personDetails.ReceiveNewsLetters = person.ReceiveNewsLetters;
            personDetails.CountryId = person.CountryId;
            personDetails.DateOfBirth = person.DateOfBirth;

            int countUpdated=await _db.SaveChangesAsync();

            return personDetails;

        }
    }
}
