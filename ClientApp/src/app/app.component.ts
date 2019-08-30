import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public user: object;
  constructor(private _http: HttpClient) { }

  ngOnInit() {
    this.getUser();
  }

  getUser(): void {
    this._http.get<object>('./Home/GetUser').subscribe(
      result => this.user = result,
      error => console.error(error)
    )
  }

}
