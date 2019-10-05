using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
//using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace SecureCacheCore22.Controllers
{
    [Route("v1")]
    public class CacheController : Controller
    {
        //private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cache;
        private readonly IDataProtector _protector;

        public CacheController(IDistributedCache memoryCache, IDataProtectionProvider protector)
        {
            _cache = memoryCache;
            _protector = protector.CreateProtector("SecureCache.v1");
        }

        [HttpGet("get/{id}")]
        public async Task<string> GetAsync(string id)
        {
            string data;
            try
            {
                var encodedData = await _cache.GetAsync(id);

                if (encodedData != null)
                {
                    data = Encoding.UTF8.GetString(encodedData);
                }
                else
                {
                    data = string.Empty;
                }
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
                data = string.Empty;
            }
            return data;
        }

        [HttpGet("set/{id}/{data}")]
        public string Set(string id, string data)
        {
            string result;
            try
            {
                _cache.SetStringAsync(id, data);
                result = "Ok";
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        [HttpGet("unprotect/{id}")]
        public async Task<string> Unprotect(string id)
        {
            string unprotectedPayload;
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
                    data = string.Empty;
                }

                unprotectedPayload = _protector.Unprotect(data);
            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
            catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            {
                unprotectedPayload = string.Empty;
            }
            return unprotectedPayload;
        }

        [HttpGet("protect/{id}/{data}")]
        public string Protect(string id, string data)
        {
            string result;
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

        [HttpDelete]
        public string Delete(string id)
        {
            _cache.Remove(id);
            return "Ok";
        }

        [HttpPost]
        public string Post(object item)
        {
            string id="1";
            string data = _protector.Protect(item.ToString());
            _cache.SetString(id, data);
            //string unprotectedPayload = _protector.Unprotect(data);
            return data;
        }
    }
}
