using Cerbos.Sdk.Builder;
using Cerbos.Sdk.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
[Authorize(Roles = "admin")]

        public IActionResult Privacy()
       
        {
          

            var client = CerbosClientBuilder.ForTarget("http://cerbos.mydomain.com:3593").WithPlaintext()
                .Build();

            var request = CheckResourcesRequest.NewInstance()
    .WithRequestId(RequestId.Generate())
    .WithIncludeMeta(true)
    .WithPrincipal(
        Principal.NewInstance(User.Identity.Name, "employee")
            .WithPolicyVersion("20210210")
            .WithAttribute("department", AttributeValue.StringValue("marketing"))
            .WithAttribute("geography", AttributeValue.StringValue("GB"))
    )
    .WithResourceEntries(
        ResourceEntry.NewInstance("leave_request", "XX125")
            .WithPolicyVersion("20210210")
            .WithAttribute("department", AttributeValue.StringValue("marketing"))
            .WithAttribute("geography", AttributeValue.StringValue("GB"))
            .WithAttribute("owner", AttributeValue.StringValue("john"))
            .WithActions("approve", "view:public")
    );
            var r = client.CheckResources(request);

            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize(Roles = "admin")]
        public IActionResult Admin()
        {
            return View();
        }
    }
}
