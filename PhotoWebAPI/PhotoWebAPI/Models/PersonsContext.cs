using Microsoft.EntityFrameworkCore;

namespace PhotoWebAPI.Models
{
    public class PersonsContext:DbContext
    {
        public DbSet<Person> Persons { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public PersonsContext(DbContextOptions<PersonsContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>().HasKey(p => new { p.IdPerson, p.Id }); //composite key
        }
    }
}
