using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Agile.AServer;
using Newtonsoft.Json;

namespace AServer.Test.Server
{
    public class ApiController : HttpHandlerController
    {
        public class car
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        [HttpHandler("/api/cars", "GET")]
        public Task GetAllCars(Request req, Response resp)
        {
            List<car> cars = new List<car>();
            cars.Add(new car { name = "ae86" });
            cars.Add(new car { name = "911" });

            var json = JsonConvert.SerializeObject(cars);

            return resp.WriteJson(json);
        }

        [HttpHandler("/api/cars/:name", "GET")]
        public Task GetCar(Request req, Response resp)
        {
            var name = req.Params.name;

            List<car> cars = new List<car>();
            cars.Add(new car { id="001", name = "ae86" });
            cars.Add(new car { id="002",name = "911" });

            var car = cars.FirstOrDefault(c => c.name == name);
            if (car != null)
            {
                var json = JsonConvert.SerializeObject(car);
                return resp.WriteJson(json);
            }
            else
            {
                return resp.Write("NotFound", HttpStatusCode.NotFound, null);
            }
        }

        [HttpHandler("/api/cars","POST")]
        public async Task AddCar(Request req, Response resp)
        {
            var car = await req.Body<car>();
            //mock return id
            var json = JsonConvert.SerializeObject(car);
            await resp.WriteJson(json);
        }

        [HttpHandler("/api/cars/001", "PUT")]
        public async Task UpdateCar(Request req, Response resp)
        {
            var car = await req.Body<car>();
            //mock return id
            var json = JsonConvert.SerializeObject(car);
            await resp.WriteJson(json);
        }

        [HttpHandler("/api/cars/001","DELETE")]
        public async Task DeleteCar(Request req, Response resp)
        {
            //delete car
            //...

            await resp.WriteJson("ok");
        }
    }
}
