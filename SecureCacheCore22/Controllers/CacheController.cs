using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;

namespace SecureCacheCore22.Controllers
{
    [Route("api/[controller]")]
    public class CacheController : Controller
    {
        int id = 1;
        private readonly IDataProtector _protector;
        private readonly IMemoryCache _cache;

        public CacheController(IMemoryCache memoryCache, IDataProtectionProvider protector)
        {
            _cache = memoryCache;
            _protector = protector.CreateProtector("Cache.v1");
        }

        [HttpGet]
        public string Get()
        {
            string data = string.Empty;

            try
            {
                data = _cache.Get(id).ToString();
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
            }

            return data;
        }

        [HttpDelete]
        public string Delete()
        {
            _cache.Remove(id);
            return "Ok";
        }

        [HttpGet("set/{data}")]
        public string Get(string data)
        {
            data = _protector.Protect(data);
            _cache.Set(id, data);

            return "Ok";
        }

        [HttpPost]
        public string Post(object item)
        {
            string data = _protector.Protect(item.ToString());
            _cache.Set(id, data);
            //string unprotectedPayload = _protector.Unprotect(data);

            return data;
        }
    }
}
