import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Student } from '../Student';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  constructor(private http: HttpClient) {}

  student: Student = new Student();

  ngOnInit() {}

  register() {
    this.http
      .post('https://localhost:7183/api/student/register', this.student)
      .subscribe(
        (data: any) => {
          alert(data);

          const token = data.token;
          const expiration = data.expiration;
          localStorage.setItem('token', token);
          localStorage.setItem('expiration', expiration);
        },
        (error: any) => {
          alert('Error: ' + error.message);
        }
      );
  }
}
