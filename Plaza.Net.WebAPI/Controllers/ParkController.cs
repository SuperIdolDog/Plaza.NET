using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Sys;

namespace Plaza.Net.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParkController : ControllerBase
    {
        private readonly EFDbContext _dbContext;
        public ParkController(EFDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("{plazaId}/floors")]
        public async Task<IActionResult> GetFloorsByPlaza(int plazaId)
        {
            var floors = await _dbContext.Floor
                .Include(f => f.FloorItem)
                .Where(f =>
                    f.PlazaId == plazaId &&
                    (f.Name != null && f.Name.Contains("停车") ||
                     f.FloorItem != null && f.FloorItem.Label != null && f.FloorItem.Label.Contains("停车")))
                .Select(f => new
                {
                    f.Id,
                    f.Name,
                    FloorItemName = f.FloorItem.Label ?? "未知楼层"
                })
                .ToListAsync();

            return Ok(floors);
        }


    }
}
