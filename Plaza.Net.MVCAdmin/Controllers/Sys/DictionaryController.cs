using Microsoft.AspNetCore.Mvc;
using Plaza.Net.IServices.Sys;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.Sys;
using System.Linq.Expressions;


namespace Plaza.Net.MVCAdmin.Controllers.Sys
{
    public class DictionaryController : Controller
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly IDictionaryItemService _dictionaryItemService;

        public DictionaryController(
            IDictionaryService dictionaryService,
            IDictionaryItemService dictionaryItemService
            )
        {
            _dictionaryService = dictionaryService;
            _dictionaryItemService = dictionaryItemService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Item(int id)
        {

            var items = _dictionaryItemService.GetOneByIdAsync(id);
            if (items == null)
            {
                return NotFound(); // 如果没有找到字典项，返回404
            }

            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> GetData(
            int pageIndex,
            int pageSize,
            int? status,
            string keyword = null!,
            string name = null!,
            string code = null!)
        {

            Expression<Func<DictionaryEntity, bool>> predicate = p =>
                 (string.IsNullOrWhiteSpace(keyword) ||
                  p.Name.Contains(keyword) ||
                  p.Code.Contains(keyword)) &&
                 (string.IsNullOrWhiteSpace(name) || p.Name.Contains(name)) &&
                 (string.IsNullOrWhiteSpace(code) || p.Code.Contains(code)) &&
                 (!status.HasValue || p.IsDeleted == (status.Value == 1 ? true : false));

            var Query = await _dictionaryService.GetPagedListByAsync(pageIndex, pageSize, predicate);
            var Total = await _dictionaryService.CountByAsync(predicate); // 确保统计过滤后的总数
            return Json(new
            {
                rows = Query,
                total = Total
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetItemData(int pageIndex,
            int pageSize,
            int? status,
            int dictionaryId,
            string keyword = null!,
            string name = null!,
            string lbl = null!)
        {
            Expression<Func<DictionaryItemEntity, bool>> predicate = p =>
                 p.DictionaryId == dictionaryId &&
                (string.IsNullOrWhiteSpace(keyword) ||
                 p.Dictionary.Name.Contains(keyword) ||
                 p.Label.Contains(keyword)) &&
                (string.IsNullOrWhiteSpace(name) || p.Dictionary.Name.Contains(name)) &&
                (string.IsNullOrWhiteSpace(lbl) || p.Label.Contains(lbl)) &&
                (!status.HasValue || p.IsDeleted == (status.Value == 1 ? true : false));
            var Query = await _dictionaryItemService.GetItemPagedListByAsync(dictionaryId, pageIndex, pageSize, predicate);
            var Total = await _dictionaryItemService.CountByAsync(predicate);
            return Json(new
            {
                rows = Query,
                total = Total
            });
        }
    }
}
