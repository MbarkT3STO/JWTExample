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
          alert('Error: ' + error.message);
        }
      );
  }
}
