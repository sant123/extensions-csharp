using System;
using System.Collections.Generic;

namespace ExtensionsTest
{
    public class MainTest
    {
        public string Cnn => "Connection Timeout=0;Data Source=(localdb)\\ProjectsV12;Initial Catalog=MyBeautifulTests;Persist Security Info=True;User ID=sa;Password=SP5550123*;";

        protected static string DesktopPath => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        protected static string LegalFolderName => "abc";

        protected static string LegalFileName => "ghj.txt";

        protected static string IllegalFolderName => "<My Folder>|123";

        protected static string IllegalFileName => "<My file>|123.txt";

        protected static List<Person> GetPeople()
        {
            var people = new List<Person>
                {
                    new Person { Id = 8693, Name = "Santiago", LastName = "Aguilar", Age = 21 },
                    new Person { Id = 54980, Name = "Paola", LastName = "Moreno", Age = 20 },
                    new Person { Id = 54988, Name = "Javier", LastName = "Florian", Age = 21 },
                    new Person { Id = 52324, Name = "Gina", LastName = "Suarez", Age = 21 },
                    new Person { Id = 54970, Name = "Jessica", LastName = "Carrillo", Age = 22 },
                    new Person { Id = 8693, Name = "The Copy", LastName = "The Copy", Age = 21 }
                };

            return people;
        }

        protected static List<Product> GetProducts()
        {
            var people = new List<Product>
                {
                    new Product { Id = 2208, Name = "Tijeras", IdPerson = 8693 },
                    new Product { Id = 9654, Name = "Ventilador", IdPerson = 8693 },
                    new Product { Id = 74854, Name = "Monitor",  IdPerson = 54980 },
                    new Product { Id = 22141, Name = "Cuaderno", IdPerson = 54980 },
                    new Product { Id = 365288, Name = "Botella", IdPerson = 54970 },
                    new Product { Id = 11124, Name = "Taladro",  IdPerson = 54970 },
                    new Product { Id = 11124, Name = "Nevera",  IdPerson = 855521 }
                };

            return people;
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} - Name:{Name}";
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public int IdPerson { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id:{Id} - Name:{Name}";
        }
    }
}
