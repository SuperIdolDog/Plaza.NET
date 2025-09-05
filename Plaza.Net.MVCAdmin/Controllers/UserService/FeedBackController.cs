using Microsoft.AspNetCore.Mvc;
using Plaza.Net.IServices.User;
using Plaza.Net.Model.Entities.User;
using System.Linq.Expressions;

namespace Plaza.Net.MVCAdmin.Controllers.Service
{
    public class FeedBackController : Controller
    {
        private readonly IUserFeedbackService _userFeedbackService;
        private readonly IUserService _userService;
        public FeedBackController(IUserFeedbackService userFeedbackService,
            IUserService userService)
        {
            _userFeedbackService = userFeedbackService;
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetUserFeedbacks(
           int pageIndex,
           int pageSize,
           bool? isReplied,
           string keyword = null,
           int? userId = null,
           int? repliedById = null)
        {
            try
            {
                // 组合查询条件
                Expression<Func<UserFeedbackEntity, bool>> predicate = feedback =>
                    (string.IsNullOrWhiteSpace(keyword) ||
                     feedback.Content.Contains(keyword) ||
                     feedback.User.UserName.Contains(keyword) ||
                     feedback.RepliedBy.UserName.Contains(keyword)) &&
                    (!userId.HasValue || feedback.UserId == userId) &&
                    (!repliedById.HasValue || feedback.RepliedById == repliedById) &&
                    (!isReplied.HasValue || (feedback.ReplyTime.HasValue == isReplied.Value));

                var query = await _userFeedbackService.GetPagedListByAsync(pageIndex, pageSize, predicate);
                var total = await _userFeedbackService.CountByAsync(predicate);

                return Json(new
                {
                    rows = query,
                    total = total
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserFeedbackEntity userFeedback)
        {
            try
            {
                if (userFeedback == null)
                {
                    return BadRequest("用户反馈数据不能为空");
                }

                userFeedback.UpdateTime = DateTime.Now;
                var result = await _userFeedbackService.UpdateAsync(userFeedback);

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
        public async Task<IActionResult> Add(UserFeedbackEntity userFeedback)
        {
            try
            {
                if (userFeedback == null)
                {
                    return BadRequest("用户反馈数据不能为空");
                }

                var result = await _userFeedbackService.CreateAsync(userFeedback);

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
        public async Task<IActionResult> Delete(int id, UserFeedbackEntity userFeedback)
        {
            try
            {
                if (id <= 0 && userFeedback == null)
                {
                    return BadRequest("无效的删除请求");
                }

                var target = userFeedback ?? await _userFeedbackService.GetOneByIdAsync(id);
                if (target == null)
                {
                    return NotFound("未找到要删除的用户反馈");
                }

                var result = await _userFeedbackService.DeleteAsync(target);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "服务器内部错误" });
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

                var result = await _userFeedbackService.DeleteRangeAsync(ids);

                if (result)
                {
                    return Json(new { success = true, message = "删除成功" });
                }
                else
                {
                    return Json(new { success = false, message = "删除失败" });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFeedbackDropdownData()
        {
            try
            {
                var users = await _userService.GetAllAsync();

                return Json(new
                {
                    users = users.Select(u => new { id = u.Id, name = u.UserName }),
                    admins = users.Where(u => u.UserRole.Name is "admin").Select(a => new { id = a.Id, name = a.UserName })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "服务器内部错误");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReplyFeedback(int id, string replyContent)
        {
            try
            {
                if (id <= 0 || string.IsNullOrWhiteSpace(replyContent))
                {
                    return BadRequest("无效的回复请求");
                }

                var feedback = await _userFeedbackService.GetOneByIdAsync(id);
                if (feedback == null)
                {
                    return NotFound("未找到要回复的用户反馈");
                }

                feedback.ReplyContent = replyContent;
                feedback.ReplyTime = DateTime.Now;
                feedback.RepliedById = 1; // 假设当前登录用户ID为1，实际应用中应从身份验证获取

                var result = await _userFeedbackService.UpdateAsync(feedback);

                if (result)
                {
                    return Json(new { success = true, message = "回复成功" });
                }
                else
                {
                    return Json(new { success = false, message = "回复失败" });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
    }
}
