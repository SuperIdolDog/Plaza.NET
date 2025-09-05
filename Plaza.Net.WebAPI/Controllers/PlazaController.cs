using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Plaza.Net.IServices.Basic;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.FromBody;
using Plaza.Net.Model.ViewModels.DTO;
using System.Linq.Expressions;

namespace Plaza.Net.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlazaController : ControllerBase
    {
        private readonly IPlazaService _plazaService;
        private readonly IFloorService _floorService;
        private readonly EFDbContext _dbContext;
        public PlazaController(IPlazaService plazaService, IFloorService floorService , EFDbContext dbContext)
        {
            _plazaService = plazaService;
            _floorService = floorService;
            _dbContext = dbContext;
        }
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyAsync(
            [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] string city,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20
            )
        {
            if (Math.Abs(lat) > 90 || Math.Abs(lng) > 180)
                return BadRequest(new { success = false, message = "坐标非法" });

            var list = await _plazaService.GetNearbyAsync(lat, lng, city, page, size);
            return Ok(new { success = true, data = list });
        }

        [HttpGet("city")]
        public async Task<IActionResult> GetCity([FromQuery] double lat, [FromQuery] double lng)
        {
            var key = "STIBZ-OTNRT-YQBX7-LBY4N-W3YXZ-O6FL3"; // 替换为你的腾讯地图Key
            var url = $"https://apis.map.qq.com/ws/geocoder/v1/?location={lat},{lng}&key={key}&get_poi=0";

            try
            {
                using var http = new HttpClient();
                var response = await http.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<dynamic>(json);

                if (result.status == 0 && result.result != null)
                {
                    string city = result.result.address_component.city;
                    if (string.IsNullOrEmpty(city) || city.EndsWith("市辖区"))
                    {
                        city = result.result.address_component.province;
                    }

                    return Ok(new
                    {
                        status = 0,
                        result = new
                        {
                            address_component = new { city }
                        }
                    });
                }

                return StatusCode(200, new
                {
                    status = (int)result.status,
                    message = result.message ?? "腾讯地图API错误",
                    result = new
                    {
                        address_component = new { city = "未知城市" }
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(200, new
                {
                    status = 500,
                    message = $"服务器错误: {ex.Message}",
                    result = new
                    {
                        address_component = new { city = "未知城市" }
                    }
                });
            }
        }

        [HttpGet("{plazaId}/floors")]
        public async Task<IActionResult> GetFloorsByPlaza(int plazaId)
        {
            // 查询条件：仅根据 plazaId
            var floors = await _dbContext.Floor
                .Include(f => f.FloorItem) // 加载关联的 DictionaryItem
                .Where(f => f.PlazaId == plazaId)
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
