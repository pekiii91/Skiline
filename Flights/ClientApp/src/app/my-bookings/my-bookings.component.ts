import { Component, OnInit } from '@angular/core';
import { BookDto, BookingRm } from '../api/models';
import { BookingService } from './../api/services/booking.service';
import { AuthService } from './../auth/auth.service';

@Component({
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.css']
})
export class MyBookingsComponent implements OnInit {

  bookings!: BookingRm[];
  constructor(private bookingService: BookingService,
    private authService: AuthService) { }

  ngOnInit(): void { 
    this.bookingService.listBooking({ email: this.authService.currentUser?.email ?? '' })
      .subscribe(result => this.bookings = result, this.handleError);
  }

  private handleError(err: any) {
    console.log("Response Error. Status: ", err.status);
    console.log("Response Error. Status: ", err.status);
    console.log(err);
  }
  //sluzi za otkazivanje rezervacije
  //prima objekat rezervacije 'booking', tipa BookingRm. Ova metoda se poziva kada korisnik zeli da
  //otkaze svoju rezervaciju. 'BookDto' sadrzi info koje su potrebne za otkazivanje reze
  //Nakon sto se stvorio DTO objekat poziva se metoda 'bookingService'.Ova metoda salje HTTP zahtev
  //za otkazivanje rezervacije. DTO koje sadrzi informacije o rezervaciji salje se kao telo zahteva
  //
  cancel(booking: BookingRm)
  {
    const dto: BookDto = {
      flightId: booking.flightId,
      passengerEmail: booking.passengerEmail,
      numberOfSeats: booking.numberOfBookedSeats
      
    };

    this.bookingService.cancelBooking({ body: dto })
      .subscribe(_ =>
        this.bookings.filter(b => b != booking)
        ,this.handleError);
    //Ovde se koristi anonimna funkcija koja će biti pozvana kada se uspešno reši HTTP zahtjev.
    //Ova funkcija ažurira listu rezervacija (this.bookings) tako što filtrira rezervacije
    //i uklanja onu koja je otkazana 
    //Metoda 'handleError' se koristi za obradu greške. Ova metoda ispisuje grešku u konzolu
  }
}
