using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using StockManagementSystem.Core;
using StockManagementSystem.Core.Configuration;
using StockManagementSystem.Core.Infrastructure;
using Tests;

namespace Core.Tests
{
    [TestFixture]
    public class WebHelperTests
    {
        private DefaultHttpContext _httpContext;
        private Mock<IFileProviderHelper> _fileProvider;
        private IWebHelper _webHelper;

        [SetUp]
        public void SetUp()
        {
            var queryString = new QueryString("");
            queryString = queryString.Add("Key1", "Value1");
            queryString = queryString.Add("Key2", "Value2");

            _httpContext = new DefaultHttpContext();
            _httpContext.Request.QueryString = queryString;
            _httpContext.Request.Headers.Add(HeaderNames.Host, "www.Example.com");

            _fileProvider = new Mock<IFileProviderHelper>();

            _webHelper = new WebHelper(new HostingConfig(), _fileProvider.Object, new FakeHttpContextAccessor(_httpContext));
        }

        [Test]
        public void Can_get_appHost_without_ssl()
        {
            _webHelper.GetHost(false).ShouldEqual("http://www.Example.com/");
        }

        [Test]
        public void Can_get_appHost_with_ssl()
        {
            _webHelper.GetHost(true).ShouldEqual("https://www.Example.com/");
        }

        [Test]
        public void Can_get_appLocation_without_ssl()
        {
            _webHelper.GetLocation(false).ShouldEqual("http://www.Example.com/");
        }

        [Test]
        public void Can_get_appLocation_with_ssl()
        {
            _webHelper.GetLocation(true).ShouldEqual("https://www.Example.com/");
        }

        [Test]
        public void Can_get_appLocation_in_virtual_directory()
        {
            _httpContext.Request.PathBase = "/smspath";
            _webHelper.GetLocation(false).ShouldEqual("http://www.Example.com/smspath/");
        }

        [Test]
        public void Can_get_queryString()
        {
            _webHelper.QueryString<string>("Key1").ShouldEqual("Value1");
            _webHelper.QueryString<string>("Key2").ShouldEqual("Value2");
            _webHelper.QueryString<string>("Key3").ShouldEqual(null);
        }

        [Test]
        public void Can_remove_queryString()
        {
            //empty URL
            _webHelper.RemoveQueryString(null, null).ShouldEqual(string.Empty);
            
            //empty key
            _webHelper.RemoveQueryString("http://www.example.com/", null).ShouldEqual("http://www.example.com/");
            
            //non-existing param with fragment
            _webHelper.RemoveQueryString("http://www.example.com/#fragment", "param").ShouldEqual("http://www.example.com/#fragment");
            
            //first param (?)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1", "param1")
                .ShouldEqual("http://www.example.com/?param2=value1");

            //second param (&)
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1", "param2")
                .ShouldEqual("http://www.example.com/?param1=value1");

            //non-existing param
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1", "param3")
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value1");

            //with fragment
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param2=value1#fragment", "param1")
                .ShouldEqual("http://www.example.com/?param2=value1#fragment");

            //specific value
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param1=value2&param2=value1", "param1", "value1")
                .ShouldEqual("http://www.example.com/?param1=value2&param2=value1");

            //all values
            _webHelper.RemoveQueryString("http://www.example.com/?param1=value1&param1=value2&param2=value1", "param1")
                .ShouldEqual("http://www.example.com/?param2=value1");
        }

        [Test]
        public void Can_modify_queryString()
        {
            //empty URL
            _webHelper.ModifyQueryString(null, null).ShouldEqual(string.Empty);

            //empty key
            _webHelper.ModifyQueryString("http://www.example.com/", null).ShouldEqual("http://www.example.com/");

            //empty value
            _webHelper.ModifyQueryString("http://www.example.com/", "param").ShouldEqual("http://www.example.com/?param=");

            //first param (?)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param1", "value2")
                .ShouldEqual("http://www.example.com/?param1=value2&param2=value1");

            //second param (&)
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param2", "value2")
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value2");

            //non-existing param
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param3", "value1")
                .ShouldEqual("http://www.example.com/?param1=value1&param2=value1&param3=value1");

            //multiple values
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1", "param1", "value1", "value2", "value3")
                .ShouldEqual("http://www.example.com/?param1=value1,value2,value3&param2=value1");

            //with fragment
            _webHelper.ModifyQueryString("http://www.example.com/?param1=value1&param2=value1#fragment", "param1", "value2")
                .ShouldEqual("http://www.example.com/?param1=value2&param2=value1#fragment");
        }
    }

    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public FakeHttpContextAccessor(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; set; }
    }
}