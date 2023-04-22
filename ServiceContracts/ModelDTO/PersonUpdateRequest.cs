namespace ServiceContracts.ModelDTO
{
    /// <summary>
    /// Reprsents the DTO class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage ="PersonId is required")]
        public Guid PersonId { get; set; }
        [Required(ErrorMessage = "PersonName is required")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter correct email pattern")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "CountryId is required")]
        public Guid? CountryId { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts the personUpdateRequest to person domain model class
        /// </summary>
        /// <returns>Returns the person model</returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonId = PersonId,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryId = CountryId,
                Address = Address,
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
