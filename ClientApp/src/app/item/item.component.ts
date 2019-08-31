import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Params, Router } from "@angular/router";

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css']
})
export class ItemComponent implements OnInit {
  
  public ItemId: number;
  public Ammount: number;
  public item: object;
  public errors: object = [];
  public lastbid: number;
  public user: object;

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
    if(JSON.parse(sessionStorage.getItem('user')) == null)
    {
      return this._router.navigate(['login']);
    } else{
      this.user = JSON.parse(sessionStorage.getItem('user'));
    }
  }

  getItem(){
    this._http.get<object>(`./Home/GetItem/${this.ItemId}`).subscribe(
      res => {
        console.log(res);
        this.item = res;
        this.lastbid = res['Bids'].length-1;
      }, 
      err => console.error(err)
    );
  }
  
  Bid(){
    if(this.Ammount == null){
      this.errors['Ammount'] = "Please enter a value";
      return
    }
    const NewBid = {
      Ammount: this.Ammount,
      UserId: this.user['UserId'],
      ItemId: this.ItemId,
    }
    return this._http.post("./Home/BidItem", NewBid).subscribe(
      data => {
        this.errors = [];
        this.getItem();
      },
      err => {
        for(let key in err.error.value){
          this.errors[key] = err.error.value[key].errors[0].errorMessage;
        }
      }
    );
  }
  ngOnDestroy() {
    
  }
}
