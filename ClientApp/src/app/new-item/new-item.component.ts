import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Http, Response, Headers, RequestOptions } from '@angular/http';

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
  //public UserId: number;

  public errors: object = [];

  constructor(private _http: HttpClient) { }

  ngOnInit() {
  }

  createItem(){
    const NewItem = {
      Name: this.Name,
      End: this.End,
      StartingBid: this.StartingBid,
      Description: this.Description,
      //UserId: this.UserId,
    }
    return this._http.post("./Home/NewItem", NewItem)
    .subscribe(
      data => console.log(data),
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
