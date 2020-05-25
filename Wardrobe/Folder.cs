using MessagePack;
using System.Collections.Generic;
using System.IO;

namespace KK_Wardrobe
{
	public abstract class Folder
	{
		public string name;

		public virtual void Load(string path)
		{
			name = Path.GetFileName(path);
		}

		public Dictionary<string, object> Load_State(string path)
		{
			if (!File.Exists(path))
				return new Dictionary<string, object>();

			byte[] bytes = File.ReadAllBytes(path);

			try
			{
				return LZ4MessagePackSerializer.Deserialize<Dictionary<string, object>>(bytes);
			}
			catch { }

			return new Dictionary<string, object>();
		}
	}
}
