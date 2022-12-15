using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ContainerApp.TodoApi.Controllers
{
        
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ValuesController(IConfiguration config)
        {
            _config = config;
        }
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            string myDbContext = _config["MyDbContext"].ToString();

            string[] res = new string[] { "value1", "value2",  myDbContext};
            return res;
        }
    }
}
