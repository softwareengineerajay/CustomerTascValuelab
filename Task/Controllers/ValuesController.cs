using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task.Models;

namespace Task.Controllers
{
    public class ValuesController : ApiController
    {
        Customer[] products = new Customer[]
        {
            new Customer { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
            new Customer { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
            new Customer { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
        };
        // Default folder    

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }
        [HttpPost]
        public IHttpActionResult GetDataFromFile(Customer customer)
        {

            return Ok();


        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
