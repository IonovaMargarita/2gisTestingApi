using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace _2gisTestingApi
{
    public class Tests
    {
        private const string TotalRegions = "22";
        private string _baseUri = "https://regions-test.2gis.com/1.0/regions";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TotalTest()
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
            {
                var response = await client.GetAsync("");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    string total = result["total"].Value<string>();
                    Assert.AreEqual(TotalRegions, total);
                }
            }
        }


        [TestCase("Новосибирск")]
        [TestCase("НОВОСИБИРСК")]
        [TestCase("новосибирск")]
        [TestCase("Ош")]
        public async Task RegionFindTest(string name)
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
            {
                var response = await client.GetAsync(string.Format("?q={0}", name));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var items = result["items"];
                    var count = items.Count();
                    var item = items.First;
                    //var uppername = name.ToUpper();
                    var regionName = item["name"].Value<string>();
              

                    Assert.AreEqual(1, count);
                    Assert.AreEqual(name.ToUpper(), regionName.ToUpper());
                }
            }

        }

        [TestCase("qwerty")]
        [TestCase("1234")]
        [TestCase("novosibirsk")]
        public async Task RegionFindSubstTest(string name)
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
            {
                var response = await client.GetAsync(string.Format("?q={0}", name));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var items = result["items"];
                    var count = items.Count();
                    //var item = items.First;
                    // var regionName = item["name"].Value<string>();

                    Assert.AreEqual(0, count);
                    //  Assert.AreEqual("Новосибирск", regionName);

                }
            }

        }

       

        //[TestCase("ru")]
        //[TestCase("RU")]
        //[TestCase("novosibirsk")]
        //public async Task CountryCodeFindTest(string code)
        //{
        //    using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
        //    {
        //        var response = await client.GetAsync(string.Format("?countryCode={0}", code)

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = await response.Content.ReadAsStringAsync();
        //            var result = JObject.Parse(json);
        //            var items = result["country"];
        //            var count = items.Count();

        //            for (int i = 0; i< count; i++)
        //            { 
        //                var item = items.First;
        //                var countryCode= item["code"].Value<string>();
        //            }


        //            Assert.AreEqual(13, count);
        //            Assert.AreEqual(code, countryCode);

        //        }
        //    }

        //}

        [TestCase("5")]
        [TestCase("10")]
        [TestCase("15")]
        public async Task PageSizeTest(string pageSize)
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
            {
                var response = await client.GetAsync(string.Format("?page_size={0}", pageSize));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(json);
                    var items = result["items"];
                    var count = items.Count();
                    //var item = items.First;
                    // var regionName = item["name"].Value<string>();

                    if (pageSize=="5")
                    {
                        Assert.AreEqual(5, count);
                    }

                    if (pageSize=="10")
                    {
                        Assert.Equals(10, count);
                    }

                    if (pageSize == "15")
                    {
                        Assert.Equals(15, count);
                    }

                    //Assert.AreEqual(0, count);
                    //  Assert.AreEqual("Новосибирск", regionName);

                }
            }

        }



    }
}