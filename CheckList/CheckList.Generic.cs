using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public class CheckList_Generic : CheckList
	{
		public CheckList_Generic(object obj,
								 Type type,
								 string key = null,
								 Dictionary<string, bool> data = null,
								 string link = "")
		{
			this.key = key;
			this.type = type;

			Load(obj, data, link);
		}
	}
}
