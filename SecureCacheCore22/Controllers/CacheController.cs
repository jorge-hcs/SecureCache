using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
//using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace SecureCacheCore22.Controllers
{
    [Route("api/[controller]")]
    public class CacheController : Controller
    {
        readonly string id = "1";
        private readonly IDataProtector _protector;

        //private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cache;

        public CacheController(IDistributedCache memoryCache, IDataProtectionProvider protector)
        {
            _cache = memoryCache;
            _protector = protector.CreateProtector("Cache.v1");
        }

        [HttpGet]
        public async Task<string> GetAsync()
        {
            string data = string.Empty;

            try
            {
                var encodedData = await _cache.GetAsync(id);

                if (encodedData != null)
                {
                    data = Encoding.UTF8.GetString(encodedData);
                }
                else
                {
                    data = "-";
                }
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
            }
            return data;
        }

        [HttpGet("secure/")]
        public async Task<string> GetSecureAsync()
        {
            string unprotectedPayload = string.Empty;
            try
            {
                var encodedData = await _cache.GetAsync(id);

                string data;

                if (encodedData != null)
                {
                    data = Encoding.UTF8.GetString(encodedData);
                }
                else
                {
                    data = "-";
                }
                unprotectedPayload = _protector.Unprotect(data);

            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
            }
            return unprotectedPayload;
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
            string result = string.Empty;
            try
            {
                data = _protector.Protect(data);
                _cache.SetStringAsync(id, data);
                result = "Ok";
            }
            catch(Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        [HttpPost]
        public string Post(object item)
        {
            string data = _protector.Protect(item.ToString());
            _cache.SetString(id, data);
            //string unprotectedPayload = _protector.Unprotect(data);
            return data;
        }
    }
}
