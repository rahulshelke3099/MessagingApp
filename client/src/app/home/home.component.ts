import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  public registerMode:boolean=false;
  users:any;
  constructor(private http:HttpClient) { }

  ngOnInit(): void {
    
  }

  cancelRegisterMode(registerMode:boolean)
  {
  this.registerMode=registerMode;
  }
  
  registerToggle()
  {
    this.registerMode=!this.registerMode;
  }



}
