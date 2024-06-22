import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {  HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HttpClientModule, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'DatingApp';
  users:any;

  constructor(private http:HttpClient){

  }

  ngOnInit(){
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: resp=>this.users=resp,
      error: err=>console.log(err),
      complete: ()=>console.log('get request has completed')
    })
  }

}
