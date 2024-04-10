using System.ComponentModel;

namespace Flights.Dtos
{
    public record FlightSearchParameters
    (
        [DefaultValue("12/25/2024 10:30:00 AM")]
        DateTime? FromDate,

        [DefaultValue("12/31/2024 11:30:00 AM")]
        DateTime? ToDate,

        [DefaultValue("Los Angeles")]
        string? From,

        [DefaultValue("Berlin")]
        string? Destination,

        [DefaultValue(1)]
        int? NumberOfPassengers
    );
    
}
