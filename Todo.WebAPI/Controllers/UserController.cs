using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Todo.Core;

namespace Todo.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private IDistributedCache _cache;

        public UserController(DataContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        public async Task<ActionResult> GetList()
        {
            var list = await _context.User.ToListAsync();
            return Ok(list);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetEntity")]
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
        [Route("GetCacheKey")]
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
        [Route("DeleteEntity")]
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
        [Route("AddEntity")]
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
    }
}
