using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flights.Dtos;
using Flights.ReadModels;
using Flights.Domain.Entities;
using System.Xml;
using Flights.Data;

namespace Flights.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly Entity _entity;

        public PassengerController(Entity entity)
        {
            _entity = entity;
        }

        //Login button
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Register(NewPassengerDto dto)
        {
            _entity.Passengers.Add(
                new Passenger(
                    dto.Email,
                    dto.FirstName,
                    dto.LastName,
                    dto.Gender));

            _entity.SaveChanges();//cuvanje u bazu podataka

            return CreatedAtAction(nameof(Find), new {email = dto.Email});
        }

        [HttpGet("{email}")]
        public ActionResult<PassengerRm> Find(string email)
        {
            var passenger = _entity.Passengers.FirstOrDefault(p => p.Email == email);

            if (passenger == null)
                return NotFound();

            var readmodel = new PassengerRm(
                passenger.Email,
                passenger.FirstName,
                passenger.LastName,
                passenger.Gender
                );

            return Ok(readmodel);
        }
    }
}
