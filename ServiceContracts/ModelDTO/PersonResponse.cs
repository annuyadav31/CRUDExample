namespace ServiceContracts.ModelDTO
{
    /// <summary>
    /// Represents DTO model class that is used as a return type of most of the methods of person service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryId { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }
        public double? Age { get; set; }

        /// <summary>
        /// Compares the given object with the personResponse object
        /// </summary>
        /// <param name="obj">Response object to compare</param>
        /// <returns>True or False indicating whether all person details are matched with the specified paramter object</returns>
        public override bool Equals(object? obj)
        {
            if(obj == null)
            { return false; }

            if(obj.GetType() != typeof(PersonResponse)) { return false; }

            PersonResponse personResponse = (PersonResponse)obj;
            return PersonId == personResponse.PersonId &&
                PersonName == personResponse.PersonName &&
                Email == personResponse.Email &&
                DateOfBirth == personResponse.DateOfBirth &&
                Gender == personResponse.Gender &&
                CountryId == personResponse.CountryId &&
                Address == personResponse.Address &&
                ReceiveNewsLetters == personResponse.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"Person ID: {PersonId}, Person Name: {PersonName}, Email: {Email}, DateOfBirth:{DateOfBirth?.ToString("dd-mm-yyyy")}, Gender : {Gender}," +
                $"CountryId: {CountryId},Country:{Country}, Address:{Address},RecieveNewsLetter:{ReceiveNewsLetters}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {
            return new PersonUpdateRequest()
            {
                PersonId = PersonId,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                Address = Address,
                CountryId = CountryId,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }

    public static class PersonExtensions
    {
        /// <summary>
        /// An extension method to convert an object of Person class to PersonResponse class
        /// </summary>
        /// <param name="person">The person object to convert</param>
        /// <returns>Returns the converted personResponse class</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                PersonName = person.PersonName,
                Email = person.Email,
                Gender = person.Gender,
                Address = person.Address,
                DateOfBirth = person.DateOfBirth,
                CountryId = person.CountryId,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth).Value.TotalDays / 365.25) : null,
                Country = (person.country != null)?person.country.CountryName: null

            };
        }
    }
}
