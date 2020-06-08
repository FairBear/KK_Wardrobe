using System.Collections.Generic;

namespace KK_Wardrobe
{
	public static partial class Strings
	{
		public static Dictionary<string, string> checkList_Strings = new Dictionary<string, string>();

		public delegate delegateWriteString delegateWriteString(string key, string value);

		public static delegateWriteString WriteString(string key, string value)
		{
			checkList_Strings[key] = value;

			return WriteString;
		}

		public static void WriteString(string key, params string[] values)
		{
			string[] keys = key.Split('.');
			key += "." + keys[keys.Length - 1];

			for (int i = 0; i < values.Length; i++)
				WriteString($"{key}[{i}]", values[i]);
		}
	}
}
