using Flights.ReadModels;
using Microsoft.AspNetCore.Mvc;
using Flights.Domain.Entities;
using Flights.Dtos;
using Flights.Domain.Errors;
using Flights.Data;
using System.Data.Entity.Infrastructure;

namespace Flights.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class FlightController : ControllerBase
    {
        private readonly ILogger<FlightController> _logger;

        private readonly Entity _entity;

        public FlightController(ILogger<FlightController> logger, Entity entity)
        {
            _logger = logger;
            _entity = entity; 
        }

        //ova metoda vrsi pretragu letova i vraca listu objekata tipa FlightRm
        //vraca enumeraciju (NIZ) objekata tipa 'FlightRm'
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(IEnumerable<FlightRm>), 200)]
        public IEnumerable<FlightRm> Search([FromQuery]FlightSearchParameters @params)
        {
            //Dakle, ova linija koda zapisuje informaciju u log da se vrši pretraga za Skyline
            //za odredište koje je specificirano u Destination.
            _logger.LogInformation("Searching for a skiline for: {Destination}", @params.Destination);
            //Pravi se upit prema bazi podataka (ili nekom drugom izvoru podataka) za listu letova,
            //koja će biti početna tačka za dodavanje dodatnih filtera na osnovu pretrage.
            IQueryable<Flight> flights = _entity.Flights;
            //'From' nedefinisan, prazan ili sastoji se samo od beline,
            //filter će biti primenjen tako da se samo letovi koji polaze sa mesta koje odgovara
            //parametru 'From' zadrže.
            if (!string.IsNullOrWhiteSpace(@params.From))
                flights = flights.Where(f => f.Departure.Place.Contains(@params.From));
            //'Destination' nedefinisan, prazan ili sastoji se samo od beline, filter će biti
            //primenjen tako da se samo letovi koji imaju odredište koje odgovara
            //parametru 'Destination' zadrže.
            if (!string.IsNullOrWhiteSpace(@params.Destination))
                flights = flights.Where(f => f.Arrival.Place.Contains(@params.Destination));
            //primenjuje se filter da se zadrže samo letovi koji polaze posle ili tačno u vreme
            //koje odgovara parametru 'FromDate'.
            if (@params.FromDate != null)
                flights = flights.Where(f => f.Departure.Time >= @params.FromDate.Value.Date);
            //Ovaj uslov proverava da li je parametar 'ToDate' definisan.
            //Ako jeste, to znači da postoji gornja granica vremenskog opsega za polazak leta
            //Filter uzima sve letove koji polaze posle ili tačno u vreme definisano parametrom 'ToDate'
            //'AddDays' - Ovo dodaje jedan dan na datum koji je definisan parametrom 'ToDate'
            //'AddTicks(-1)' - Ovo oduzima jedan tiks od datuma kako bi se osiguralo da filter
            //uključuje letove koji polaze tačno na zadati datum.Bez ovog koraka,
            //filter bi uključio letove koji polaze nakon ponoći sledećeg dana.
            if (@params.ToDate != null)
                flights = flights.Where(f => f.Departure.Time >= @params.ToDate.Value.AddDays(1).AddTicks(-1));

            if (@params.NumberOfPassengers != 0 && @params.NumberOfPassengers != null) //znači da korisnik zahteva određeni broj putnika za prevoz.
                flights = flights.Where(f => f.RemainingNumberOfSeats >= @params.NumberOfPassengers);
            //Zadržava samo one letove koji imaju dovoljan broj preostalih sedišta
            //za broj putnika koji je specificiran u parametru 'NumberOfPassengers'
            else
                flights = flights.Where(f => f.RemainingNumberOfSeats >= 1);
            //Ovaj deo filtrira letove tako da zadrži samo one koji imaju bar jedno
            //preostalo slobodno sedište, što znači da može biti bilo koji broj putnika
            //manji od maksimalnog kapaciteta aviona.

            //kreirali smo kolekciju 'flights' koja sadrzi informacije o letovima
            //metoda Where. Ovde se koristi LINQ metoda Where kako bi se filtrirali letovi prema odredištu.
            //Samo letovi koji imaju odredište jednako onome koje je dostavljeno putem @params.Destination
            //bit će uključeni u rezultirajuću listu. @params.Destination verovatno predstavlja odredište
            //koje korisnik ili sastav traži.
            //Metoda Select() se koristi za iteriranje kroz tu kolekciju i
            //stvaranje novih objekata tipa FlightRm na osnovu podataka svakog pojedinačnog leta.
            //new FlightRm(...): Za svaki let, stvara se novi objekat tipa FlightRm.
            var flightRmList = flights
                .Select(flight => new FlightRm(
                flight.Id,
                flight.Airline,
                flight.Price,
                new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time), //odlazak
                new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time), //dolazak
                flight.RemainingNumberOfSeats
                ));  

            return flightRmList; //vraca listu objekata, koja predstavlja rezultat pretrage letova. 
        }

        //metoda se koristi za pronalazenje leta na osnovu datog identifikatora ('id') i vraca 
        //odgovarajuci objekat tipa 'FlightRm' koji predstavlja detalje pronadjenog leta
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(typeof(FlightRm), 200)]
        //'Guid id' se koristi kao ulazni parametar, sadrzi 'FlightRm' objekat ili odgovarajuci
        //status kod HTTP-a (NotFound ili Ok)
        public ActionResult<FlightRm> Find(Guid id)
        {
            var flight = _entity.Flights.SingleOrDefault(f => f.Id == id); //ova linija trazi let u listi
                                                                   //'flights' (pretpostavljam da je to lista objekata tipa Flight)
                  
            if (flight == null) //proverava se da li je let pronadjen
                return NotFound();

            var readModel = new FlightRm( //ako je let pronadjen stvara se novi objekat 'FlightRm' koji se naziva 'readModel'
                flight.Id,
                flight.Airline,
                flight.Price,
                new TimePlaceRm(flight.Departure.Place.ToString(), flight.Departure.Time), //odlazak
                new TimePlaceRm(flight.Arrival.Place.ToString(), flight.Arrival.Time), //dolazak
                flight.RemainingNumberOfSeats
                );

            //ako je let uspešno pronađen i objekat 'FlightRm' je uspešno kreiran,
            //metoda vraća status koda 200 - 'OK' zajedno sa objektom 'FlightRm'
            //koji sadrži detalje pronađenog leta.
            return Ok(readModel);
        }

        //Ova metoda Book je HTTP POST endpoint koji se koristi za rezervaciju leta
        //na osnovu prosleđenih podataka o rezervaciji.
        //prima objekat "BookDto" kao ulazni parametar, koji sadrzi info o rez leta
        //ukljucujuci identifikator leta ('FlightId'), email putnika ('passengerEmail') i 'numberOfSeats'
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(200)]
        public IActionResult Book(BookDto dto)
        {   //BookDto kao ulazni parametar. Sadrzi info o rezervaciji leta, kao i 'FlightId'
            System.Diagnostics.Debug.WriteLine($"Booking a new flight {dto.FlightId}");

            var flight = _entity.Flights.SingleOrDefault(f => f.Id == dto.FlightId); 
            
            if (flight == null)
                return NotFound();
            //poziva se metoda 'MakeBooking' nad instancom leta (flight) koja je pronadjena na osnovu 'FlightId'
            var error = flight.MakeBooking(dto.PassengerEmail, dto.NumberOfSeats);

            if (error is OverbookError)
                return Conflict(new { message = "Not enough seats." });

            try 
            {
                _entity.SaveChanges();//cuvanje podataka u bazu podataka 
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(new { message = "Doslo je do greske prilikom rezervacije. Molimo vas pokusajte kasnije." });
            }

            return CreatedAtAction(nameof(Find), new { id = dto.FlightId });
 
        }
    }
}