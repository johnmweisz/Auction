import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params, Router } from "@angular/router";

@Component({
  selector: 'app-auction',
  templateUrl: './auction.component.html',
  styleUrls: ['./auction.component.css']
})
export class AuctionComponent implements OnInit {
  public user: object;
  public items: object;

  constructor(
    private _http: HttpClient,
    private _router: Router
    ) { 
    this.getItems();
  }

  ngOnInit() {
    if(JSON.parse(sessionStorage.getItem('user')) == null)
    {
      return this._router.navigate(['login']);
    } else{
      this.user = JSON.parse(sessionStorage.getItem('user'));
    }
  }

  getItems(){
    this._http.get<object>('./Home/GetItems').subscribe(
      result => this.items = result, 
      error => console.error(error)
    )
  }

}
