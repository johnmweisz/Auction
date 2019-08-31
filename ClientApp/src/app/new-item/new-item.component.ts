import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { ActivatedRoute, Params, Router } from "@angular/router";

@Component({
  selector: 'app-new-item',
  templateUrl: './new-item.component.html',
  styleUrls: ['./new-item.component.css']
})
export class NewItemComponent implements OnInit {
  public Name: string;
  public End: Date;
  public StartingBid: number;
  public Description: string;
  public user: object;
  public errors: object = [];

  constructor(
    private _http: HttpClient,   
     private _router: Router
     ) { 
  }

  ngOnInit() {
    if(JSON.parse(sessionStorage.getItem('user')) == null)
    {
      return this._router.navigate(['login']);
    } else{
      this.user = JSON.parse(sessionStorage.getItem('user'));
    }
  }

  createItem(){
    const NewItem = {
      Name: this.Name,
      End: this.End,
      StartingBid: this.StartingBid,
      Description: this.Description,
      UserId: this.user['UserId']
    }
    return this._http.post("./Home/NewItem", NewItem)
    .subscribe(
      data => {
        return this._router.navigate(['auction']);
      },
      err => {
        for(let key in err.error.value){
          this.errors[key] = err.error.value[key].errors[0].errorMessage;
        }
      }
    );
  }

}

interface Item {
  Name: string;
  End: Date;
  StartingBid: number;
  Description: string;
}
