using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace AServer.Test.TestCases
{
    public class car
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
        public void GetTest()
        {
            var client = new HttpClient();
            var result = client.GetStringAsync("http://localhost:5000/api/cars").Result;

            Assert.IsNotNull(result);
            var cars = JsonConvert.DeserializeObject<List<car>>(result);
            Assert.IsNotNull(cars);

            var ae86 = cars.FirstOrDefault(c => c.name == "ae86");
            Assert.IsNotNull(ae86);
            Assert.AreEqual(ae86.name,"ae86");
            var c911 = cars.FirstOrDefault(c => c.name == "911");
            Assert.IsNotNull(c911);
            Assert.AreEqual(c911.name, "911");
        }

        [TestMethod]
        public void GetCarTest()
        {
            var client = new HttpClient();
            var result = client.GetStringAsync("http://localhost:5000/api/cars/911").Result;

            Assert.IsNotNull(result);
            var car = JsonConvert.DeserializeObject<car>(result);
            Assert.IsNotNull(car);
            Assert.AreEqual(car.name, "911");
        }

        [TestMethod]
        public void AddCarTest()
        {
            var car = new car()
            {
                id="1",
                name="ae86"
            };
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(car));
            var result = client.PostAsync("http://localhost:5000/api/cars", content).Result.Content.ReadAsStringAsync().Result;
            var userObj = JsonConvert.DeserializeObject<car>(result);

            Assert.IsNotNull(userObj);
            Assert.AreEqual(userObj.name, "ae86");
            Assert.AreEqual(userObj.id, "1");
        }

        [TestMethod]
        public void UpdateCarTest()
        {
            var car = new car()
            {
                id = "001",
                name = "ae86"
            };
            var client = new HttpClient();
            var content = new StringContent(JsonConvert.SerializeObject(car));
            var result = client.PutAsync("http://localhost:5000/api/cars/001", content).Result.Content.ReadAsStringAsync().Result;
            var userObj = JsonConvert.DeserializeObject<car>(result);

            Assert.IsNotNull(userObj);
            Assert.AreEqual(userObj.name, "ae86");
            Assert.AreEqual(userObj.id, "001");
        }

        [TestMethod]
        public void DeleteCarTest()
        {
            var client = new HttpClient();
            var result = client.DeleteAsync("http://localhost:5000/api/cars/001").Result.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(result, "ok");
        }
    }
}
