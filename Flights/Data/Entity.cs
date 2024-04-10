using Flights.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Configuration;


namespace Flights.Data
{
    public class Entity : DbContext
    {
        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<Passenger> Passengers => Set<Passenger>();

        public Entity(DbContextOptions<Entity> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Konfiguracija entiteta Letenje
            modelBuilder.Entity<Passenger>()
                .HasKey(p => p.Email); // Postavljanje primarnog ključa Passenger

            modelBuilder.Entity<Flight>() //18 lekcija
                .Property(p => p.RemainingNumberOfSeats)
                .IsConcurrencyToken(); // Preostali broj sedista

            modelBuilder.Entity<Flight>().OwnsOne(f => f.Departure); //odlazak

            modelBuilder.Entity<Flight>().OwnsOne(f => f.Arrival); //dolazak

            modelBuilder.Entity<Flight>().OwnsMany(f => f.Bookings);

            //// Promeniti tip svojstva Bookings
            //modelBuilder.Entity<Flight>()
            //    .HasMany(f => f.Bookings)
            //    .WithOne()
            //    .HasForeignKey(b => b.FlightId); // Pretpostavljajući da Booking ima svojstvo FlightId kao strani ključ

            //modelBuilder.Entity<Booking>()
            //   .HasKey(b => b.FlightId);
        }
    }
}

