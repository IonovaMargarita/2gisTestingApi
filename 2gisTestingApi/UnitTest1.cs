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
        [TestCase("Санкт-Петербург")]

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
                   
                    var regionName = item["name"].Value<string>();
                    Assert.AreEqual(1, count);
                    Assert.AreEqual(name.ToUpper(), regionName.ToUpper());
                    
                }
            }

        }

        [TestCase(" ")]
        [TestCase("")]
        [TestCase("абракадабра")]
        [TestCase("qwerty")]
        [TestCase("1234")]
        [TestCase("novosibirsk")]


        public async Task RegionFindIncorrectTest(string name)
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
                    Assert.AreEqual(0, count);
                    return;

                }

                Assert.Fail("500 Internal Server Error");
            }

        }

     



        [Test, Combinatorial]
              
        public async Task CountryCodeFindTest([Values("ru", "kz","ua", "cz","kg")] string code, [Values(5, 10, 15)] int pageSize)
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
            {
                var response = await client.GetAsync(string.Format("?country_code={0}&page_size={1}", code, pageSize));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(json);
                    var items = obj["items"];
                    var count = items.Count();
                    Assert.LessOrEqual(count , pageSize);

                    var result = true;
                    foreach(var item in items)
                    {
                        var country = item["country"];
                        var countryCode = country["code"].Value<String>();
                        if (countryCode!= code)
                        {
                            result = false;
                            break;
                        }
                    }


                    Assert.IsTrue(result);

                }
            }

        }

        [Test, Combinatorial]

        public async Task PageSizeTest([Values(5, 10, 15)] int pageSize)
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(_baseUri) })
            {
                var response = await client.GetAsync(string.Format("?page_size={0}", pageSize));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var obj = JObject.Parse(json);
                    var items = obj["items"];
                    var count = items.Count();
                    Assert.LessOrEqual(count, pageSize);

                    
                }
            }

        }



    }
}