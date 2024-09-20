using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using TestApp.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestApp.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class Providers : ControllerBase
    {
        [HttpGet("name/{id}")]
        public string Get(int id)
        {
            var provider = DBControllers.GenericController<Provider>.GetModel((db) => db.Provider.ToList()).SingleOrDefault((o) => o.Id == id);
            return provider == null ? "" : (provider.Name ?? "");
        }

        [HttpGet("")]
        public IEnumerable<string> Get()
        {
            var providers = DBControllers.GenericController<Provider>.GetModel((db) => db.Provider.ToList());
            List<string> result = new List<string>();

            foreach (var provider in providers)
                result.Add(provider.Id + ":" + provider.Name ?? "");

            return result;
        }
    }
}