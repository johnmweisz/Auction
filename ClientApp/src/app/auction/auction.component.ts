import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrls: ['./auction.component.css']
})
export class AuctionComponent implements OnInit {
  public user: object;
  public items: object;

  constructor(private _http: HttpClient) { 
    this.getItems();
    this.getUser();
  }

  ngOnInit() {

  }

  getItems(){
    this._http.get<object>('./Home/GetItems').subscribe(
      result => this.items = result, 
      error => console.error(error)
    )
  }

  getUser(): void {
    this._http.get<object>('./Home/GetUser').subscribe(
      result => this.user = result,
      error => console.error(error)
    )
  }
}
