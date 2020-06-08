using System;
using System.Collections.Generic;
using System.Linq;

namespace KK_Wardrobe
{
	public class CheckList_Array : CheckList
	{
		public int[] indices;

		public override string Key =>
			key != null ?
				$"{key}[{string.Join(",", indices.Select(v => v.ToString()).ToArray())}]" :
				null;

		public CheckList_Array(object obj,
							   Type type,
							   string key = null,
							   int[] indices = null,
							   Dictionary<string, bool> data = null,
							   string link = "")
		{
			this.key = key;
			this.type = type;
			this.indices = new int[indices?.Length ?? 0];

			if (indices != null)
				for (int i = 0; i < indices.Length; i++)
					this.indices[i] = indices[i];

			linkedKey = GetLinkedKey(link);

			Load(obj, data);
		}
	}
}
