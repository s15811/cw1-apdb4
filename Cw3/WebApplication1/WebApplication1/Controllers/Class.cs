using Cw3.DTOs.Promotion;
using Cw3.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Controllers
{
    [Route("api/enrollments/promotions")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private IStudentsDbService _service;
        public PromotionController(IStudentsDbService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult PromotionStudents(PromoteStudents request)
        {
            return Ok(_service.PromoteStudents(request.Semester, request.Studies));
        }

    }
}
