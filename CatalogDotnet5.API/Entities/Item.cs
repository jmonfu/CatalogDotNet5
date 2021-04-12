using System;

namespace CatalogDotnet5.API.Entities
{
    // here we will use a record instead of a class for this immutable (instance Cannot be changed) property
    public class Item
    {
        // instead of the set, we use init..  After creating the property, you can no longer modify the propery.
        // this is like we used to do with private set;
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}