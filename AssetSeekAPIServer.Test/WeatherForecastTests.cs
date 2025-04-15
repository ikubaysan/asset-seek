using AssetSeekAPIServer.Test.Fixtures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;

namespace AssetSeekAPIServer.Test
{
    [TestClass]
    public class WeatherForecastTests
    {
        private static ApiTestFactory _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            _factory = new ApiTestFactory();
            _client = _factory.CreateClient();
        }

        [TestMethod]
        public async Task WeatherForecast_Endpoint_ReturnsForecasts()
        {
            var response = await _client.GetAsync("/WeatherForecast");
            response.EnsureSuccessStatusCode();

            var forecasts = await response.Content.ReadFromJsonAsync<List<WeatherForecast>>();
            Assert.IsNotNull(forecasts);
            Assert.AreEqual(5, forecasts.Count);
        }
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; set; }
        public string Summary { get; set; }
    }
}
