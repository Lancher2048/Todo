using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Todo.Core;
using Todo.Commons;
using Todo.WebAPI.App_Start;

namespace Todo.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly JwtHelper _jwtHelper;
        private IDistributedCache _cache;

        public UserController(DataContext context, IDistributedCache cache, JwtHelper jwtHelper)
        {
            _context = context;
            _cache = cache;
            _jwtHelper = jwtHelper;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetList()
        {
            var list = new List<UserEntity> { new UserEntity { Id = "2323", Password = "" } };

            //var list = await _context.User.ToListAsync();
            return Ok(list);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetEntity(string keyValue)
        {
            var entity = await _context.User.FindAsync(keyValue);
            return Ok(entity);
        }

        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetCacheKey(string key)
        {
            var val = await _cache.GetStringAsync(key);
            return Ok(val);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteEntity(string keyValue)
        {
            var list = await _context.User.Where(n => n.Id == keyValue).ToListAsync();
            if (list.Any())
            {
                foreach(var item in list)
                {
                    _context.User.Remove(item);
                }
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="name"></param>
        /// <param name="passWord"></param>
        /// <param name="sex"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddEntity(string name, string passWord, int? sex, string remark)
        {
            var entity = new UserEntity();
            entity.Id = Guid.NewGuid().ToString();
            entity.UserName = name;
            entity.Password = passWord;
            entity.Remark = remark;
            entity.Sex = sex;
            _context.User.Add(entity);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult> GetToken()
        {
            return Ok(_jwtHelper.CreateToken());
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetSnowId()
        {
            return Ok(SnowflakeHelper.GetId());
        }
    }
}
