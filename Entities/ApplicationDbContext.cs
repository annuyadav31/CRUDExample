﻿namespace Entities
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");


            modelBuilder.Entity<Country>().HasData(new Country() { CountryId = Guid.Parse("D05F7FC7-D025-4CE2-B174-1A61417BFA27"), CountryName="India"},
                new Country() { CountryId = Guid.Parse("13F1AE27-3967-440D-8DD2-316B5F5F88BE"), CountryName = "UK" }
                );

            ////seed to countries


            //string countriesJson = System.IO.File.ReadAllText("countries.json");

            //List<Country>? countries =System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            //if (countries != null)
            //{
            //    foreach (Country country in countries)
            //    { modelBuilder.Entity<Country>().HasData(country); }
            //}

            ////seed to persons
            ////seed to countries
            //string personsJson = System.IO.File.ReadAllText("persons.json");

            //List<Person>? persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            //if (persons != null)
            //{
            //    foreach (Person person in persons)
            //    { modelBuilder.Entity<Person>().HasData(person); }
            //}

            //Table Relations
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne<Country>(c => c.country).WithMany(p => p.Persons).HasForeignKey(p=>p.CountryId);
            });

        }
    }
}
