using HuoBiApi.Models.Depth;
using Microsoft.AspNetCore.Mvc;

namespace HuoBiApi.Controllers
{
    [Controller]
    public class DepthController : ControllerBase
    {
        private readonly DepthService _depthService;

        public DepthController(DepthService depthService)
        {
            _depthService = depthService;
        }

        /// <summary>
        /// 深度
        /// </summary>
        /// <param name="symbol">币对</param>
        /// <returns></returns>
        [HttpGet("/api/depth")]
        public ActionResult<DepthTick> GetDepth(string symbol)
        {
            return Ok(_depthService.GetDepth(symbol));
        }
    }
}