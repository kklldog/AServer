using Agile.AServer.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AServer.Test.TestCases
{
    [TestClass]
    public class PathUtilTests
    {
        [TestMethod]
        public void IsMatchPathTest()
        {
            var p1 = "";
            var p2 = "";
            var result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/";
            p2 = "/";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "a";
            p2 = "c";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsFalse(result);

            p1 = "/api";
            p2 = "/api";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/user";
            p2 = "/api/user";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/user/";
            p2 = "/api/user";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/user/";
            p2 = "/api/user/";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/USER/";
            p2 = "/api/user/";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/user/";
            p2 = "/api/USER/";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/user/000";
            p2 = "/api/user/";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsFalse(result);

            p1 = "/api/user/000";
            p2 = "/api/user/:id";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsTrue(result);

            p1 = "/api/user/000/1";
            p2 = "/api/user/:id";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsFalse(result);

            p1 = "/api/user/000";
            p2 = "/api/user/:id/1";
            result = PathUtil.IsMatch(p1, p2);
            Assert.IsFalse(result);
        }
    }
}
