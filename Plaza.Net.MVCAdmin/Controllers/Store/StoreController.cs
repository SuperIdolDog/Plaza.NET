using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.ViewModels;

using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Store
{
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IStoreTypeService _storeTypeService;
        private readonly IPlazaService _plazaService;
        private readonly IFloorService _floorService;
        private readonly IUserService _userService;
        private readonly IImageUploadService _imageUploadService;
        private readonly EFDbContext _dbContext;

        public StoreController(
            IStoreService storeService,
            IStoreTypeService storeTypeService,
            IPlazaService plazaService,
            IFloorService floorService,
            IUserService userService,
            EFDbContext dbContext,
            IImageUploadService imageUploadService)
        {
            _storeService = storeService;
            _storeTypeService = storeTypeService;
            _plazaService = plazaService;
            _floorService = floorService;
            _userService = userService;
            _dbContext = dbContext;
            _imageUploadService = imageUploadService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file, string uploadType)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "请选择有效的图片文件" });

                var imagePath = await _imageUploadService.UploadImageAsync(file, uploadType);

                // 返回相对路径
                return Json(new { success = true, imageUrl = imagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public IActionResult DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return Json(new { success = false, message = "图片路径不能为空" });

                // 调用 DeleteImageAsync 方法删除图片
                var isDeleted = _imageUploadService.DeleteImageAsync(imageUrl);

                if (isDeleted)
                    return Json(new { success = true });
                else
                    return Json(new { success = false, message = "删除图片失败" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetFloorsByPlazaId(int plazaId)
        {
            var floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId);
            return Json(floors);
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? plazaId)
        {
            var plazas = await _plazaService.GetAllAsync();
            IEnumerable<FloorEntity> floors = new List<FloorEntity>();
            var storeTypes = await _storeTypeService.GetAllAsync();
            var users = await _userService.GetManyByAsync(u => u.UserRoleId == 3); // 假设店长角色ID为3
            if (plazaId.GetValueOrDefault() != 0)
            {
                floors = await _floorService.GetManyByAsync(f => f.PlazaId == plazaId!.Value);
            }

            var viewModel = new StoreIndexViewModel
            {
                StoreTypes = storeTypes,
                Plazas = plazas,
                Floors = floors,
                Users = users
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetStoreData(
            int pageIndex,
            int pageSize,
            int? status,
            int? storeTypeId,
            int? plazaId,
            int? floorId,
            int? userId,
            string keyword,
            string storeName
        )
        {
            Expression<Func<StoreEntity, bool>> predicate = p =>
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Name.Contains(keyword) ||
                 p.Description.Contains(keyword) ||
                 (p.Floor != null && p.Floor.Name.Contains(keyword)) ||
                 (p.Floor != null && p.Floor.Plaza != null && p.Floor.Plaza.Name.Contains(keyword)) ||
                 p.Contact.Contains(keyword)) &&
                (string.IsNullOrWhiteSpace(storeName) || (p.Name != null && p.Name.Contains(storeName))) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1)) &&
                (!storeTypeId.HasValue || p.StoreTypeId == storeTypeId.Value) &&
                (!plazaId.HasValue || (p.Floor != null && p.Floor.PlazaId == plazaId.Value)) &&
                (!floorId.HasValue || (p.Floor != null && p.FloorId == floorId.Value)) &&
                (!userId.HasValue || p.UserId == userId.Value);

            var query = await _storeService.GetPagedListByAsync(pageIndex, pageSize, predicate,
                include: p => p.Include(s => s.StoreType)
                              .Include(s => s.Floor)
                              .ThenInclude(f => f.Plaza)
                              .Include(s => s.User));

            var total = await _storeService.CountByAsync(predicate);

            var result = query.Select(p => new
            {
                p.Id,
                p.Name,
                p.ImageUrl,
                p.Location,
                p.BusinessHours,
                p.Contact,
                p.Description,
                p.StoreTypeId,
                storeTypeName = p.StoreType?.Name,
                p.FloorId,
                floorName = p.Floor?.Name,
                p.Floor?.PlazaId,
                plazaName = p.Floor?.Plaza?.Name,
                p.UserId,
                userName = p.User?.UserName,
                p.IsDeleted,
                p.CreateTime,
                p.UpdateTime,
            });

            return Json(new
            {
                rows = result,
                Total = total
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StoreEntity store)
        {
            try
            {
                if (store == null)
                {
                    return BadRequest("店铺数据不能为空");
                }
              
                store.UpdateTime = DateTime.Now;
                var result = await _storeService.UpdateAsync(store);

                if (result)
                {
                    return Json(new { success = true, message = "更新成功" });
                }
                else
                {
                    return Json(new { success = false, message = "更新失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(StoreEntity store)
        {
            try
            {
                var result = await _storeService.CreateAsync(store);

                if (result)
                {
                    return Json(new { success = true, message = "添加成功" });
                }
                else
                {
                    return Json(new { success = false, message = "添加失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, StoreEntity store)
        {
            var result = await _storeService.DeleteAsync(store);
            if (result)
            {
                return Json(new { success = true, message = "删除成功" });
            }
            else
            {
                return Json(new { success = false, message = "删除失败" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRange(int[] ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest("未提供要删除的ID");
                }

                var result = await _storeService.DeleteRangeAsync(ids);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
    }
}
