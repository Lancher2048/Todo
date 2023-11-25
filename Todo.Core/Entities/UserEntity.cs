namespace Todo.Core
{
    public class UserEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; } = "";
        /// <summary>
        /// 姓名
        /// </summary>
        public string UserName { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";
        /// <summary>
        /// 性别
        /// </summary>
        public int? Sex { get; set; } = 0;
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = "";
    }
}
