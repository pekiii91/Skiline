using Flights.Domain.Errors;
using System.ComponentModel.DataAnnotations;

namespace Flights.Domain.Entities
{
    public class Flight
    {
        [Key]
        public string Airline { get; set; }
        public Guid Id { get; set; }
        public string Price {  get; set; } 
        public TimePlace Departure { get; set; }
        public TimePlace Arrival { get; set; }
        public int RemainingNumberOfSeats { get; set; }

        //kopirali smo listu iz FlightController-a
        public IList<Booking> Bookings = new List<Booking>();

        //konstruktor bez parametara
        public Flight()
        {

        }

        //konstruktor sa parametrimna
        public Flight(
            Guid id,
            string airline,
            string price,
            TimePlace departure,
            TimePlace arrival,
            int remainingNumberOfSeats
        )
        {
            //dodati vrednosti "value"
            Id= id;
            Airline= airline;
            Price= price;
            Departure= departure;
            Arrival= arrival;
            RemainingNumberOfSeats = remainingNumberOfSeats;
        }
      
        //object nulleble
        // koristi za obavljanje rezervacija letova.
        // Ova metoda verovatno predstavlja internu implementaciju
        // funkcionalnosti za pravljenje rezervacija,
        // prima email putnika i broj sedista kao ulazne parametre
        // prvo se proverava da li na letu ima slobodnih mesta za rezervaciju
        // Ukoliko nema vraca se istanca klase 'OverbookError' koja signalizira da nema dovoljnih mesta
        // Ako ima dovoljno mesta, kreira se nova rezervacija leta i dodaje u kolekciju rez leta. Takodje
        // smanjuje se broj preostalih mesta na letu. 
        public object? MakeBooking(string passengerEmail, byte numberOfSeats)  
        {
            var flight = this;

            if (flight.RemainingNumberOfSeats < numberOfSeats)
            {
                return new OverbookError();
            }

            flight.Bookings.Add(
              new Booking(
                  passengerEmail,
                  numberOfSeats)
              );

            //promenili smo inmutibul u mutibul RemainingNumberOfSeats
            flight.RemainingNumberOfSeats -= numberOfSeats;
            return null;  //ako je uspesna, metoda vraca 'null'
        }
        public object? CancelBooking(string passengerEmail, byte numberOfSeats)
        {
            var booking = Bookings.FirstOrDefault(b => numberOfSeats == b.NumberOfSeats
            && passengerEmail.ToLower() == b.PassengerEmail.ToLower());

            if (booking == null)
                return new NotFoundError();

            Bookings.Remove(booking);
            RemainingNumberOfSeats += booking.NumberOfSeats;
            return null;
        }

      
    }
}
