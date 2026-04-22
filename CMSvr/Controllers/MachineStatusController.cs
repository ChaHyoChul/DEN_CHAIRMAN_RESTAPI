using Microsoft.AspNetCore.Mvc;
using CMSvr.Infrastructure.Services;
using CMSvr.Domain.Dtos.MachineStatus;

namespace CMSvr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MachineStatusController : ControllerBase
    {
        private readonly MachineStatusService _machineStatusService;

        public MachineStatusController(MachineStatusService machineStatusService)
        {
            _machineStatusService = machineStatusService;
        }

        [HttpGet]
        public ActionResult<MachineStatusDto> GetStatus()
        {
            try
            {
                var status = _machineStatusService.GetMachineStatus();
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("tools")]
        public ActionResult<ToolListDto> GetToolStatus()
        {
            try
            {
                var toolStatus = _machineStatusService.GetToolStatus();
                return Ok(toolStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
