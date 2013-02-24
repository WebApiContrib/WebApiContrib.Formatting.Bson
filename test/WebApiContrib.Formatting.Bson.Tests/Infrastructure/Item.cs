using System.Collections.Generic;

namespace WebApiContrib.Formatting.Bson.Tests.Infrastructure
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Item> Items { get; set; } 
    }
}