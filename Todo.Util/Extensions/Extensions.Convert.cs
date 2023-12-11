using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Util
{
	public static partial class Extensions
	{
		public static int ToInt(this object data)
		{
			if (data == null) return 0;
			if (int.TryParse(data.ToString, out int result))
				return result;

			try
			{
				return Convert.ToInt32(ToDouble(data, 0));
			}
			catch (Exception)
			{
				return 0;
			}
		}
    }
}
