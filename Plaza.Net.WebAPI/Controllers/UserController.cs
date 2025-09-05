using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Plaza.Net.Auth;
using Plaza.Net.IServices.Basic;
using Plaza.Net.IServices.User;
using Plaza.Net.Model;
using Plaza.Net.Model.Entities.Basic;
using Plaza.Net.Model.Entities.User;
using Plaza.Net.Model.FromBody;
using Plaza.Net.Model.ViewModels.DTO;
using Plaza.Net.Services.User;
using Plaza.Net.Utility.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;

namespace Plaza.Net.WebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IOptionsSnapshot<JwtSetting> _jwt;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        private readonly IUserFeedbackService _feedbackService;
        private readonly EFDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IImageUploadService _imageUploadService;
        private readonly ILogger<UserController> _logger;
        private readonly IAddressService _addressService;


        public UserController(
            IOptionsSnapshot<JwtSetting> jwt,
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            IConfiguration cfg,
            IUserService userService,
            EFDbContext dbContext,
        IUserFeedbackService feedbackService,
            IImageUploadService imageUploadService,
            ILogger<UserController> logger,
            IAddressService addressService
            )
        {
            _jwt = jwt;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = cfg;
            _userService = userService;
            _feedbackService = feedbackService;
            _imageUploadService = imageUploadService;
             _dbContext=dbContext;
            _logger = logger;
            _addressService = addressService;
        }
        private async Task<JwtSecurityToken> GenerateJwtTokenAsync(UserEntity user)
        {
            // 获取用户角色
            var roles = await _userManager.GetRolesAsync(user);

            // 创建声明列表
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // 关键：必须包含 Sub 声明
        new Claim(JwtRegisteredClaimNames.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // 备用声明
    };

            // 添加角色声明
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 从配置中获取密钥和其他设置
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 创建令牌
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                claims: claims,
                expires: DateTime.Now.AddSeconds(Convert.ToDouble(_configuration["JwtConfig:ExpireSeconds"])),
                signingCredentials: creds
            );

            return token;
        }
        /// <summary>
        /// 账号密码登录
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] FMAccountPwdLogin dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Account) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { success = false, message = "参数不完整" });

            var user = dto.Account.Contains("@")
       ? await _userManager.FindByEmailAsync(dto.Account.Trim())
       : await _userManager.FindByNameAsync(dto.Account.Trim());

            if (user == null)
                return Ok(new { success = false, message = "账号或密码错误" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password.Trim(), lockoutOnFailure: true);
            if (!result.Succeeded)
                return Ok(new { success = false, message = "账号或密码错误" });

            var token = await GenerateJwtTokenAsync(user);
            return Ok(new
            {
                success = true,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expires = token.ValidTo,
                userInfo = new
                {
                    user.Id,
                    user.UserName,
                    user.Email
                }
            });
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {

                // 1. 从 JWT 获取用户ID
                var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

                // 备用方案：如果 Sub 声明不存在，尝试其他声明
                if (string.IsNullOrEmpty(userId))
                {
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

              

                // 2. 验证用户ID是否存在
                if (string.IsNullOrWhiteSpace(userId))
                {
                   
                    return Unauthorized(new { success = false, message = "无法获取用户身份信息" });
                }

                // 3. 查询用户信息
                var user = await _userService.GetOneByIdAsync(int.Parse(userId));
                if (user == null)
                {
                   
                    return NotFound(new { success = false, message = "用户不存在" });
                }

                // 4. 构建并返回用户信息
                var dto = new FMUserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = ImagePathHelper.ConvertToFullUrl(user.AvatarUrl ?? string.Empty),
                    FullName = user.FullName,
                    IDNumber = user.IDNumber, // 注意：敏感信息，考虑是否必要
                    RegisterDate = user.RegisterDate
                };

              
                return Ok(new { success = true, data = dto });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { success = false, message = "服务器内部错误" });
            }
        }
        [HttpPost("feedback")]
        public async Task<IActionResult> Feedback([FromBody] FeedbackDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
                return Ok(new { success = false, message = "反馈内容不能为空" });

            // 2. 构建实体
            var entity = new UserFeedbackEntity
            {
                Content = dto.Content,
                Imageurl = dto.ImageUrls ?? string.Empty,
                Contact = dto.Contact,
                UserId = dto.UserId
            };
            var result = await _feedbackService.CreateAsync(entity);
            if (result)
            {
                return Ok(new { success = true, message = "反馈提交成功" });
            }
            else
            {
                return Ok(new { success = false, message = "反馈提交失败" });
            }
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            return Ok(new { success = true, message = "退出登录成功" });
        }
        [HttpGet("addressinfo")]
        public async Task<IActionResult> AddressInfo(int userid)
        {
            var addresses = await _addressService.GetManyByAsync(f => f.UserId == userid);
            // 2. 插入新地址
            
            return Ok(addresses.Select(p => new
            {
                p.Id,
                p.Name,
                p.Phone,
                FullAddress = $"{p.Province}{p.City}{p.County}{p.Detail}",
                p.Province,
                p.City,
                p.County,
                p.Detail,
                p.Label,
                p.IsDefault
            }));
        }
        [HttpGet("defaultaddressinfo")]
        public async Task<IActionResult> DefaultAddressInfo(int userid)
        {
            var addr = await _addressService.GetManyByAsync(a =>
                 a.UserId == userid && a.IsDefault);
            var address = addr.FirstOrDefault();
            if (address == null) return NotFound();
            return Ok(new
            {
                address.Id,
                address.Name,
                address.Phone,
                FullAddress = $"{address.Province}{address.City}{address.County}{address.Detail}",
                address.Province,
                address.City,
                address.County,
                address.Detail,
                address.Label,
                address.IsDefault
            });
        }
        [HttpPost("addaddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto dto)
        {
            // 1. 如果要设为默认，先取消旧默认
            if (dto.IsDefault)
            {
                var olds = await _addressService.GetManyByAsync(
                               a => a.UserId == dto.UserId && a.IsDefault);
                foreach (var old in olds)
                {
                    old.IsDefault = false;
                    await _addressService.UpdateAsync(old);
                }
            }

            // 2. 插入新地址
            var entity = new AddressEntity
            {
                UserId = dto.UserId,
                Name = dto.Name.Trim(),
                Phone = dto.Phone.Trim(),
                Province = dto.Province.Trim(),
                City = dto.City.Trim(),
                County = dto.County.Trim(),
                Detail = dto.Detail.Trim(),
                Label = string.IsNullOrWhiteSpace(dto.Label) ? null : dto.Label.Trim(),
                IsDefault = dto.IsDefault
            };

            var result = await _addressService.CreateAsync(entity);
            return Ok(result);
        }
        [HttpPost("updateaddress/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressDto dto)
        {
            var address = await _addressService.GetOneByIdAsync(id);
            if (address == null) return NotFound();

            // 如果本次要设为默认
            if (dto.IsDefault)
            {
                var olds = await _addressService.GetManyByAsync(
                               a => a.UserId == dto.UserId && a.IsDefault && a.Id != id);
                foreach (var old in olds)
                {
                    old.IsDefault = false;
                    await _addressService.UpdateAsync(old);
                }
            }

            // 更新字段
            address.UserId = dto.UserId;
            address.Name = dto.Name.Trim();
            address.Phone = dto.Phone.Trim();
            address.Province = dto.Province.Trim();
            address.City = dto.City.Trim();
            address.County = dto.County.Trim();
            address.Detail = dto.Detail.Trim();
            address.Label = string.IsNullOrWhiteSpace(dto.Label) ? null : dto.Label.Trim();
            address.IsDefault = dto.IsDefault;

            var result = await _addressService.UpdateAsync(address);
            return Ok(result);
        }
        [HttpPost("deladdress/{id}")]
        public async Task<IActionResult> SoftDeleteAddress(int id, [FromBody] AddressDelDTO dto)
        {
            var address = await _addressService.GetOneByIdAsync(id);
            if (address == null) return NotFound();

            address.IsDeleted = dto.IsDeleted;
            await _addressService.UpdateAsync(address);

            // 若删掉的是默认且非删除（恢复场景不触发）
            if (address.IsDefault && dto.IsDeleted)
            {
                var newDefault = (await _addressService.GetManyByAsync(
                                    a => a.UserId == address.UserId &&
                                         !a.IsDeleted &&
                                         a.Id != id))
                                 .OrderBy(a => a.Id)
                                 .FirstOrDefault();

                if (newDefault != null)
                {
                    newDefault.IsDefault = true;
                    await _addressService.UpdateAsync(newDefault);
                }
            }

            return Ok(address);
        }
    }
}  

