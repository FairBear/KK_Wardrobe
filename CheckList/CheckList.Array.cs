using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public class CheckList_Array : CheckList
	{
		public int rank;
		public int index;

		public override string Key =>
			key != null ?
				$"{key}[{rank},{index}]" :
				null;

		public CheckList_Array(object obj,
							   Type type,
							   string key = null,
							   int rank = 0,
							   int index = 0,
							   Dictionary<string, bool> data = null,
							   string link = "")
		{
			this.key = key;
			this.type = type;
			this.rank = rank;
			this.index = index;

			Load(obj, data, link);
		}
	}
}
