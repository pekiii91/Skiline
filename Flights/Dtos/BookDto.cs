using System.ComponentModel.DataAnnotations;

namespace Flights.Dtos
{
    public record BookDto
    (
        [Required]  // definise da je polje obavezno
        Guid FlightId,
         
        [Required]
        [EmailAddress]
        [StringLength(50, MinimumLength = 3)]
        string PassengerEmail,

        [Required]
        [Range(1,853)]  //definise raspon vrednosti za numericka svojstva
        byte NumberOfSeats
    );
}
