namespace Entities
{
    /// <summary>
    /// Domain Model For Country
    /// </summary>
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; } 
        public string? CountryName { get; set; }
    }
}