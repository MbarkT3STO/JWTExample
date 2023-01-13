import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginModel } from '../LoginModel';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  constructor(private http: HttpClient) {}

  login: LoginModel = new LoginModel();

  ngOnInit() {}

  loginSubmit() {
    this.http.post('https://localhost:7183/api/student', this.login).subscribe(
      (response: any) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('username', response.userName);
        localStorage.setItem('expiration', response.expiration);

        alert('Login successful');
      },
      (error) => {
        alert('Error: ' + error.message);
      }
    );
  }
}
