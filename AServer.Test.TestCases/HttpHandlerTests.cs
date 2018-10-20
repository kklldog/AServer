using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AServer.Test.TestCases
{
    [TestClass]

    public class HttpHandlerTests
    {
        [TestMethod]
        public void GetTest()
        {
            var client = new HttpClient();
            var result = client.GetStringAsync("http://localhost:5000/api/user").Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(result, "['kklldog','agile']");
        }
        [TestMethod]
        public void GetUserTest()
        {
            var client = new HttpClient();
            var result = client.GetStringAsync("http://localhost:5000/api/user/0001").Result;

            Assert.IsNotNull(result);
            Assert.AreEqual(result, "userId:0001");
        }
        [TestMethod]
        public void QueryTest()
        {
            var client = new HttpClient();
            var result = client.GetStringAsync("http://localhost:5000/api/query?name=xxx").Result;

            Assert.IsNotNull(result);
            var user = JsonConvert.DeserializeObject<dynamic>(result);
            Assert.IsNotNull(user);
            Assert.AreEqual((string)user.name,"xxx");
            Assert.AreEqual((string)user.id,"0001");
        }

        [TestMethod]
        public void AddUserTest()
        {
            var client = new HttpClient();
            var user = new
            {
                id="001",
                name="张三"
            };
            var content = new StringContent(JsonConvert.SerializeObject(user));
            var result = client.PostAsync("http://localhost:5000/api/user", content).Result.Content.ReadAsStringAsync().Result;
            var userObj = JsonConvert.DeserializeObject<dynamic>(result);

            Assert.IsNotNull(userObj);
            Assert.AreEqual((string)userObj.name, "张三");
            Assert.AreEqual((string)userObj.id, "001");
        }

        [TestMethod]
        public void DeleteUserTest()
        {
            var client = new HttpClient();
            var result = client.DeleteAsync("http://localhost:5000/api/user/0001").Result.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(result, "user 0001 be deleted .");
        }

        [TestMethod]
        public void ExceptionTest()
        {
            var client = new HttpClient();
            var result = client.GetAsync("http://localhost:5000/api/ex").Result;
            Assert.AreEqual(result.StatusCode ,HttpStatusCode.InternalServerError);
        }
    }
}
