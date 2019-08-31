import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  FirstName: string;
  LastName:string;
  Email:string;
  Password:string;
  Confirm:string;
  errors:object[] = [];

  constructor(
    private _http: HttpClient,
    private _router: Router ) {
      
  }

  ngOnInit() {

  }

  register(){
    const newUser = {
      FirstName: this.FirstName,
      LastName: this.LastName,
      Email: this.Email,
      Password: this.Password,
      Confirm: this.Confirm
    }
    return this._http.post('/LogReg/Register', newUser).subscribe(
      res => {
        this.errors = [];
        sessionStorage.setItem('user', JSON.stringify(res));
        this._router.navigate(['auction']);
      },
      err => {
        for(let key in err.error.value){
          this.errors[key] = err.error.value[key].errors[0].errorMessage;
        }
      }
    );
  }
}
