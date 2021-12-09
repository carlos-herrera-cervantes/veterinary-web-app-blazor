namespace Veterinary.Domain.Models
{
    public class Employee
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Birthdate { get; set; }

        public string Municipality { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }

        public string Colony { get; set; }

        public string Number { get; set; }

        public string[] Roles { get; set; }
    }
}