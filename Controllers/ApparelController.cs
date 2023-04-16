using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_rpg.Dtos.Apparel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ApparelController : ControllerBase
    {
        private readonly IApparelService _apparelService;

        public ApparelController(IApparelService apparelService)
        {
            _apparelService = apparelService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddApparel(AddApparelDto newApparel)
        {
            return Ok(await _apparelService.AddApparel(newApparel));
        }
    }
}