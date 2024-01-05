using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core;

public class UserService
{
    private readonly DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取全部用户数据
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserEntity>> GetAsync()
    {
        var data = await _context.User.ToListAsync();
        return data;
    }
}
