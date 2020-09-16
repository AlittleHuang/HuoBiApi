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


        [HttpGet("/api/depth")]
        public object GetDepth(string symbol)
        {
            return Ok(_depthService.GetDepth(symbol));
        }
    }
}