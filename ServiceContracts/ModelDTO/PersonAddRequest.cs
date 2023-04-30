namespace ServiceContracts.ModelDTO
{
    /// <summary>
    /// Acts as a DTO for inserting new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="PersonName is required")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage ="Email is required")]
        [EmailAddress(ErrorMessage ="Please enter correct email pattern")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage ="DateOfBirth is required")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage ="Gender is required")]
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage ="CountryId is required")]
        public Guid? CountryId { get; set; }

        [Required(ErrorMessage ="Address is required")]
        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
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
