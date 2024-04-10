import { Component, OnInit } from '@angular/core';
import { PassengerService } from './../api/services/passenger.service';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { Router, ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-register-passenger',
  templateUrl: './register-passenger.component.html',
  styleUrls: ['./register-passenger.component.css']
})
export class RegisterPassengerComponent implements OnInit {

  constructor(private passengerService: PassengerService,
    
    private authService: AuthService, 
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private fb: FormBuilder) { }


    requestedUrl?: string = undefined;

  form = this.fb.group({
    email: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(254)])],
    firstName: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.max(40)])],
    lastName: ['', Validators.compose([Validators.required, Validators.minLength(1), Validators.max(40)])],
    isFemale: [true, Validators.required]
  })

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(p => this.requestedUrl =
      p["requestedUrl"])
  }

  //metoda provere Pasosa
  checkPassenger(): void {
    const emailValue = this.form.get('email')?.value; // Dobavljanje vrednosti emaila iz forme

    if (emailValue !== null && emailValue !== undefined) { // Provera da li je email definisan
      const params = { email: emailValue }; // Kreiranje objekta sa definisanim emailom

      this.passengerService
        .findPassenger(params)
        .subscribe(
          () => this.login(), // Poziv metode login u uspešnom odgovoru
          error => {
            if (error.status !== 404) { // Obrada greške
              console.error(error);
            }
          }
        );
    } else {
      console.error("Email is null or undefined");
    }
  }



  register() { //ovo metoda je povezana u html sa dugmetom Login


    if (this.form.invalid)
      return;

    console.log("Form Values:", this.form.value)

    //ovde pozivamo metodu Login 1
    this.passengerService.registerPassenger({ body: this.form.value })
      .subscribe(this.login, console.error)
  }

  private login = () => {
    const emailValue = { email: this.form.get('email')?.value as string };
    this.authService.loginUser(emailValue)
    this.router.navigate([this.requestedUrl ?? '/search-flights']);
  }   


}



 


 

