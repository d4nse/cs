using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary1
{
    public enum PersonMaritalStatus
    {
        Merried,
        Single
    }

    [Serializable]
    public class Person
    {
        public string Name; // Изменил на public для доступа извне
        public string LastName; // Изменил на public для доступа извне
        public int Age; // Изменил на public для доступа извне
        public PersonMaritalStatus MaritsalStatus; // Изменил на public для доступа извне

        public Person(string Name, string Lastname, int Age)
        {
            this.Name = Name;
            this.LastName = Lastname;
            this.Age = Age;
            this.MaritsalStatus = PersonMaritalStatus.Single;
        }

        public void Print()
        {
            Console.WriteLine("Person:\nName: " + Name + "\nLastname: " + LastName + "\nAge: " + Age);
        }
    }

    public class Employee : Person
    {
        public string Position; // Изменил на public для доступа извне
        public decimal Salary; // Изменил на public для доступа извне

        public Employee(string Name, string Lastname, int Age, string Position, decimal Salary) :
            base(Name, Lastname, Age)
        {
            this.Position = Position;
            this.Salary = Salary;
        }
    }
}