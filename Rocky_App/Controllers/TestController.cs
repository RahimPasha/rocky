using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocky.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController:ControllerBase
    {
        [HttpGet]
        [Route("get/{name}")]
        public async Task<IActionResult> Get(string name)
        {
            return Ok($"Test {name}");
        }
    }
}
