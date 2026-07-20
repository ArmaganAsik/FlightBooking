using FlightBooking.AIAgentServices.TravelAgentServices;
using FlightBooking.DTOs.AIAgentDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class AgentController : Controller
    {
        private readonly ITravelAgentService _travelAgentService;

        public AgentController(ITravelAgentService travelAgentService)
        {
            _travelAgentService = travelAgentService;
        }

        public async Task<IActionResult> AskAgent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AskAgent([FromBody] AgentPromptRequestDto agentPromptRequestDto)
        {
            AgentResponseDto result = await _travelAgentService.AskAgentAsync(agentPromptRequestDto.Prompt);
            return Json(result);
        }
    }
}
