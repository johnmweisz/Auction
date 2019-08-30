import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params, Router } from "@angular/router";

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {
  
  public ItemId: number;
  public item: object;

  constructor(
    public _http: HttpClient,
    private _route: ActivatedRoute,
    private _router: Router,
    ) { 
      this._route.params.subscribe( 
        params => {
          this.ItemId = params.ItemId;
          this.getItem()
        },
        error => {
          return console.error(error);
        },
      );
    }

  ngOnInit() {

  }

  getItem(){
    this._http.get<object>(`./Home/item/${this.ItemId}`).subscribe(
      result => this.item = result, 
      error => console.error(error)
    );
  }
}
