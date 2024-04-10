using System.ComponentModel.DataAnnotations;

namespace Flights.Domain.Entities
{
    public record Passenger
    (
        string Email, //primary.key
        string FirstName,
        string LastName,
        bool Gender
    );
}
