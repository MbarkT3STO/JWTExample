export class Student {
  id: number;
  firstName: string;
  lastName: string;
  email: string;

  constructor(
    id: number = 0,
    firstName: string = '',
    lastName: string = '',
    email: string = ''
  ) {
    this.id = id;
    this.firstName = firstName;
    this.lastName = lastName;
    this.email = email;
  }
}
