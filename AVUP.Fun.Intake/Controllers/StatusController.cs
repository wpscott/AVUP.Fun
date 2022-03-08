using AcFunDanmu;
using AcFunDanmu.Models.Client;
using AVUP.Fun.Intake.Models;
using AVUP.Fun.Intake.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace AVUP.Fun.Intake.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : Controller
    {
        private readonly ILogger<StatusController> _logger;
        private readonly IConfiguration _configuration;

        public StatusController(IConfiguration configuration, ILogger<StatusController> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _logger = logger;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok();
        }

        [HttpGet("live")]
        public ActionResult<LiveData[]> Get()
        {
            return Json(IntakeWorker.Monitors.Values);
        }

        [HttpGet("gift")]
        public ActionResult GetGiftList()
        {
            return Json(Client.Gifts);
        }

        [HttpGet("gift/{uper:long}/{id:long}")]
        public async Task<ActionResult> GetGift(long uper, long id)
        {
            while (true)
            {
                var gift = _configuration.GetSection($"Gifts:{id}").Get<Dictionary<string, string>>().ToGiftInfo();
                if (gift != null)
                {
                    return Json(gift);
                }
                else if (Client.Gifts.TryGetValue(id, out gift))
                {
                    return Json(gift);
                }
                var client = new Client();
                await client.Initialize(uper, true);
            }
        }
    }

    public static class DictionaryExtension
    {
        public static GiftInfo ToGiftInfo(this IDictionary<string, string> source)
        {
            if (source == null) { return null; }

            var obj = new GiftInfo();
            var type = obj.GetType();
            foreach (var item in source)
            {
                var property = type.GetProperty(item.Key, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                switch (item.Key)
                {
                    case "pic":
                        property.SetValue(obj, new Uri(item.Value)); break;
                    case "value":
                        property.SetValue(obj, int.Parse(item.Value)); break;
                    case "name":
                        property.SetValue(obj, item.Value); break;
                }
            }
            return obj;
        }
    }
}
