using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

class Program
{
    static void Main()
    {
        // Sample data
        List<Person> people = new List<Person>
        {
            new Person { Id = 1, Name = "John Doe", Age = 25 },
            new Person { Id = 2, Name = "Jane Smith", Age = 30 },
            new Person { Id = 3, Name = "Bob Johnson", Age = 28 }
        };

        // Convert the list of people to JSON
        string json = JsonConvert.SerializeObject(people, Formatting.Indented);

        // Save JSON to a file
        File.WriteAllText("output.json", json);

        Console.WriteLine("JSON data has been written to output.json");
    }
}

class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
