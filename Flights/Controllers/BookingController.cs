using Flights.Data;
using Flights.Domain.Entities;
using Flights.ReadModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flights.ReadModels;
using Flights.Dtos;
using Flights.Domain.Errors;

namespace Flights.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly Entity _entity;

        public BookingController(Entity entity)
        {
            _entity = entity;
        }

        //email predstavlja parametar koji ce biti prosledjen u metodu
        //var bookings: Ovde se deklariše promenljiva bookings, koja će sadržati rezultat LINQ upita.
        //_entity.Flights.ToArray(): Ovo predstavlja pristup kolekciji letova u kontekstu baze podataka,
        //koja je pretpostavka da postoji u _entity objektu.
        //Metoda ToArray() se koristi za materijalizaciju podataka i pretvaranje ih u niz
        [HttpGet("{email}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(IEnumerable<BookingRm>), 200)]

        public ActionResult<IEnumerable<BookingRm>> List(string email)
        {
            var bookings = _entity.Flights.ToArray()
                .SelectMany(f => f.Bookings         // Ova metoda SelectMany() se koristi da bi se
        // izvršio spajanje (join) ili flatiranje kolekcija.
        // U ovom slučaju, za svaki let (f) u nizu letova, pristupamo kolekciji rezervacija (Bookings) za taj let.
                .Where(b => b.PassengerEmail == email)  //metoda za filtriranje rezervacija.
        //Samo rezervacije za koje je email putnika (PassengerEmail) jednak datom emailu će biti zadržane.
                .Select(b=> new BookingRm //Ova linija vrši projekciju svake rezervacije (b)
  //na novi objekat tipa BookingRm.
  //Konstruktor BookingRm objekta se koristi za inicijalizaciju novog
  //objekta sa relevantnim podacima o letu i rezervaciji.
                (
                    f.Id,
                    f.Airline,
                    f.Price.ToString(),
                    new TimePlaceRm(f.Arrival.Place, f.Arrival.Time),
                    new TimePlaceRm(f.Departure.Place, f.Departure.Time),
                    b.NumberOfSeats,
                    email
                )));

            return Ok(bookings);
            //ovaj LINQ upit prvo pronalazi sve rezervacije za datog putnika na osnovu emaila,
            //a zatim mapira te rezervacije na novi tip objekta BookingRm.
            //Na kraju, rezultat upita se dodeljuje promenljivoj bookings koja
            //će se vratiti kao deo HTTP odgovora iz akcije kontrolera.
        }


        //Ova metoda prima DTO 'BookDto' koji sadrži informacije o rezervaciji koju treba otkazati.
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Cancel(BookDto dto)
        {
            // Pretvaranje FlightId iz Guid u string
            string flightIdAsString = dto.FlightId.ToString();
            //Metoda pokušava pronaći let na temelju ID-a leta koji je dostavljen u DTO.
            var flight = _entity.Flights.Find(flightIdAsString);
            //Metoda poziva metodu 'CancelBooking' na objektu flight,
            //koja pokušava otkazati rezervaciju za putnika s određenim emailom i brojem sjedala.
            //Rezultat se ubacuje u varijablu 'error', koja može biti null ako je rezervacija uspješno otkazana
            //ili sadrži grešku ako je došlo do problema pri otkazivanju.
            var error = flight?.CancelBooking(dto.PassengerEmail, dto.NumberOfSeats);

            //Ako error nema vrednost, to znači da je rezervacija uspješno otkazana.
            //Tada se promene spremaju u bazu podataka pomoću _entity.SaveChanges() i metoda vraća NoContent()
            //(statusni kod 204).
            if (error == null) {
                _entity.SaveChanges();
                return NoContent();
            }

            //Ako je error tipa NotFoundError, to znači da rezervacija nije pronađena ili nije mogla
            //biti otkazana.U tom slučaju metoda vraća statusni kod 404(Not Found).
            if (error is NotFoundError)
                return NotFound();
            

            throw new Exception($"Doslo je do greske je tipa: {error.GetType().Name}" +
                $" prilikom otkazivanja rezervacije koju je izvrsio {dto.PassengerEmail}");
     //Ova metoda pruža osnovnu implementaciju otkazivanja rezervacije za određeni let,
     //uz odgovarajuće upravljanje greškama i statusnim kodovima HTTP-a.
        }
    }
}
