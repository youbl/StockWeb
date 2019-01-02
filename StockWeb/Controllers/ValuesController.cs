using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockWeb.Model;
using StockWeb.Services;

namespace StockWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<Tuple<IEnumerable<ReadyEnt>, IEnumerable<ListedEnt>>> Get()
        {
            var xls = Path.Combine(Environment.CurrentDirectory, @"docs/1.xlsx");
            var listedEnts = ExcelHelper.ReadListedExcel(xls);

            xls = Path.Combine(Environment.CurrentDirectory, @"docs/2.xls");
            var readyEnts = ExcelHelper.ReadReadyExcel(xls);

            return new Tuple<IEnumerable<ReadyEnt>, IEnumerable<ListedEnt>>(readyEnts, listedEnts);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var ret = await ListedEvt.SaveAll();
            return ret.ToString();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
