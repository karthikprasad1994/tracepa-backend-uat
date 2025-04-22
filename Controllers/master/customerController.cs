using Microsoft.AspNetCore.Mvc;
using TracePca.Interface;
using TracePca.Interface.Master;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracePca.Controllers.master
{
    [Route("api/[controller]")]
    [ApiController]
    public class customerController : ControllerBase
    {
        private readonly MasterInterface _MasterInterface;
        //private  IHttpContextAccessor _httpContextAccessor;
        public customerController(MasterInterface masterInterface) { 
            
            _MasterInterface = masterInterface;
        }





        // GET: api/<customerController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<customerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<customerController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<customerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<customerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
