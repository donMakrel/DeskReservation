using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication1.Controllers;
using Xunit;

namespace WebApplication1.Tests.Controllers
{
    public class LocationDeskControllerTests
    {
        private readonly IConfiguration _configuration;

        public LocationDeskControllerTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            _configuration = configurationBuilder.Build();
        }

        [Fact]
        public void GetLocations_ReturnsJsonResult_WithDataTable()
        {
            var controller = new LocationDeskController(_configuration);

            var result = controller.GetLocations();

            var jsonResult = Assert.IsType<JsonResult>(result);
            var dataTable = Assert.IsType<DataTable>(jsonResult.Value);
            Assert.NotEmpty(dataTable.Rows);
        }
    }
}
