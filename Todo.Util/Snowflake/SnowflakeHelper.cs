using Snowflake.Core;

namespace Todo.Commons
{
    public static class SnowflakeHelper
    {
        private static IdWorker _IdWorker = null;

        public static void SetIdWorker(IdWorkOptions options)
        {
            _IdWorker = new IdWorker(options.workId, options.datacenterId);
        }
        /// <summary>
        /// 获取雪花ID
        /// </summary>
        /// <returns></returns>
        public static long GetId()
        {
            if(_IdWorker == null)
            {
                _IdWorker = new IdWorker(0, 0);
            }
            return _IdWorker.NextId();
        }
    }
}
