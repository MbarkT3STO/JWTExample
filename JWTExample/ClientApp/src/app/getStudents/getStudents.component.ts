import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Student } from '../Student';

@Component({
  selector: 'app-getStudents',
  templateUrl: './getStudents.component.html',
  styleUrls: ['./getStudents.component.css'],
})
export class GetStudentsComponent implements OnInit {
  constructor(private http: HttpClient) {}

  students: Student[] = [];

  ngOnInit() {
    this.http
      .get<Student[]>('https://localhost:7183/api/student/students', {
        headers: new HttpHeaders({
          Authorization: 'Bearer ' + localStorage.getItem('token'),
        }),
      })
      .subscribe(
        (data) => {
          this.students = data;
        },
        (error) => {
          if (error.status == 401) {
            this.getRefreshToken();

            this.http
              .get<Student[]>('https://localhost:7183/api/student/students', {
                headers: new HttpHeaders({
                  Authorization: 'Bearer ' + localStorage.getItem('token'),
                }),
              })
              .subscribe((data) => {
                this.students = data;
              });
          } else alert('Error: ' + error.message);
        }
      );
  }

  getRefreshToken() {
    let request = {
      userName: localStorage.getItem('username'),
      refreshToken: localStorage.getItem('refreshToken'),
    };

    this.http
      .post('https://localhost:7183/api/student/refreshToken', request)
      .subscribe(
        (response: any) => {
          localStorage.setItem('token', response.token);
          localStorage.setItem('expiration', response.expiration);
          localStorage.setItem('refreshToken', response.refreshToken);
          localStorage.setItem('username', response.userName);

          alert('Refresh successful');
        },
        (error) => {
          alert('Error: ' + error.message);
        }
      );
  }
}
