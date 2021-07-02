using Dyw.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Ordering.Domain.Entities.OrderAggregate
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string ZipCode { get; private set; }

        public Address() { }
        public Address(string street, string city, string zipCode)
        {
            Street = street;
            City = city;
            ZipCode = zipCode;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Street;
            yield return City;
            yield return ZipCode;
        }
    }
}
