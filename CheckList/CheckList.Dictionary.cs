using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public class CheckList_Dictionary : CheckList
	{
		public string dictionaryKey;

		public override string Key =>
			key != null ?
				$"{key}{{{dictionaryKey}}}" :
				null;

		public CheckList_Dictionary(object obj,
									Type type,
									string key = null,
									string dictionaryKey = null,
									Dictionary<string, bool> data = null,
									string link = "")
		{
			this.key = key;
			this.dictionaryKey = dictionaryKey;
			this.type = type;

			Load(obj, data, link);
		}
	}
}
