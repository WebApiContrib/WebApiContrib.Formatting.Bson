using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using NUnit.Framework;
using WebApiContrib.Formatting.Bson.Tests.Infrastructure;

namespace WebApiContrib.Formatting.Bson.Tests
{
    [TestFixture]
    public class BsonFormatterTests
    {
        private HttpClient _client;

        [TestFixtureSetUp]
        public void fixture_init()
        {
            var config = new HttpConfiguration();
            config.Formatters.Add(new BsonMediaTypeFormatter());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new {id = RouteParameter.Optional}
           );


            var server = new HttpServer(config);

            _client = new HttpClient(server);
            _client.BaseAddress = new Uri("http://www.test.com/");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/bson"));
        }

        [Test]
        public void serialize_single()
        {
            var response = _client.GetAsync("test/1").Result;
            var result = response.Content.ReadAsAsync<Item>(new HashSet<MediaTypeFormatter> {new BsonMediaTypeFormatter()}).Result;

            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("test", result.Name);
            Assert.AreEqual(3, result.Items.Count());
        }

        [Test]
        public void serialize_array()
        {
            var response = _client.GetAsync("test").Result;
            var result = response.Content.ReadAsAsync<IEnumerable<Item>>(new HashSet<MediaTypeFormatter> { new BsonMediaTypeFormatter() }).Result;

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.NotNull(result.Where(x => x.Id == 1));
            Assert.NotNull(result.Where(x => x.Id == 2));
            Assert.NotNull(result.Where(x => x.Id == 3));
        }

        [Test]
        public void deserialize_single()
        {
            var item = new Item {Id = 100, Name = "abc", Items = new List<Item> {new Item {Name = "x"}}};
            var response = _client.PostAsync("test", item, new BsonMediaTypeFormatter()).Result;

            var result = response.Content.ReadAsAsync<Item>(new HashSet<MediaTypeFormatter> { new BsonMediaTypeFormatter() }).Result;

            Assert.NotNull(result);
            Assert.AreEqual(item.Id, result.Id);
            Assert.AreEqual(item.Name, result.Name);
            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual("x", result.Items.FirstOrDefault().Name);
        }

        [Test]
        public void deserialize_array()
        {
            var items = new List<Item> {new Item {Id = 100, Name = "abc"}, new Item {Id = 200, Name = "def"}};
            var response = _client.PutAsync("test", items, new BsonMediaTypeFormatter()).Result;

            var result = response.Content.ReadAsAsync<IEnumerable<Item>>(new HashSet<MediaTypeFormatter> { new BsonMediaTypeFormatter() }).Result;

            Assert.NotNull(result);
            Assert.AreEqual(items.Count, result.Count());
            Assert.NotNull(result.Where(x => x.Id == 100));
            Assert.NotNull(result.Where(x => x.Id == 200));
        }
    }
}
