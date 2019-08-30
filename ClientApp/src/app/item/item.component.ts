import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {
  
  public ItemId: number;
  public item: object;

  constructor(public _http: HttpClient) { }

  ngOnInit() {
    this.getItem();
  }

  getItem(){
    this._http.get<object>('./Home/GetItem').subscribe(
      result => this.item = result, 
      error => console.error(error)
    )
  }
}
