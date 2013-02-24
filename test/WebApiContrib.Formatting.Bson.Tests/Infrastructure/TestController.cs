using System.Collections.Generic;
using System.Web.Http;

namespace WebApiContrib.Formatting.Bson.Tests.Infrastructure
{
    public class TestController : ApiController
    {
        private static List<Item> _items = new List<Item>
            {
                new Item {Id = 1, Name = "a"},
                new Item {Id = 2, Name = "b"},
                new Item {Id = 3, Name = "c"}
            };

        public Item Get(int id)
        {
            return new Item {Id = id, Name = "test", Items = _items};
        }

        public IEnumerable<Item> GetAll()
        {
            return _items;
        }

        public Item Post(Item item)
        {
            return item;
        }

        public IEnumerable<Item> Put(IEnumerable<Item> items)
        {
            return items;
        }
    }
}