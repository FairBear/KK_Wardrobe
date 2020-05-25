using System.Collections.Generic;

namespace KK_Wardrobe
{
	public static partial class Strings
	{
		public static Dictionary<string, Dictionary<string, string>> checkList_Strings =
			new Dictionary<string, Dictionary<string, string>>();

		public delegate delegateWriteString delegateWriteString(string key, string value, string parent = null);

		public static delegateWriteString WriteString(string key, string value, string parent = null)
		{
			Dictionary<string, string> dict;
			parent = parent ?? string.Empty;

			if (!checkList_Strings.ContainsKey(parent))
				dict = checkList_Strings[parent] = new Dictionary<string, string>();
			else
				dict = checkList_Strings[parent];

			dict.Add(key, value);

			return WriteString;
		}

		public static void WriteString(string parent, string prefix, int rank, params string[] values)
		{
			for (int i = 0; i < values.Length; i++)
				WriteString($"{prefix}[{rank},{i}]", values[i], parent);
		}
	}
}
