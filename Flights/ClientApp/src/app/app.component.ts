import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']



})
export class AppComponent {
  title = 'app';
}


//styleUrls: Ovaj atribut je deo metadata koji se koristi unutar definicije Angular komponente.
//On omogućuje komponenti da referencira izdvojene CSS datoteke ili stilove.

//['./app.component.css']: Ovde je specificirana relativna putanja do CSS datoteke koja sadrži
//stilove za komponentu.U ovom slučaju,./ app.component.css označava da se CSS stilovi
//za komponentu nalaze u datoteci nazvanoj app.component.css, koja se nalazi u
//istom direktoriju kao i komponenta.
