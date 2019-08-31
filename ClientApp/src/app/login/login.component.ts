import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  Email:string;
  Password:string;
  errors:object[] = [];

  constructor(
    private _http: HttpClient,
    private _router: Router ) {
      
  }

  ngOnInit() {

  }

  login(){
    const tryUser = {
      Email: this.Email,
      Password: this.Password,
    }
    return this._http.post('/LogReg/Login', tryUser).subscribe(
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
