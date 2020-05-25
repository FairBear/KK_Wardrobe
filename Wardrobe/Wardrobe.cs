using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KK_Wardrobe
{
	public class Wardrobe : Folder
	{
		public List<Layer> layers;
		public int weight = 10;
		public int nameMode = 0; // 0 = blacklist; 1 = whitelist
		public List<string> nameList = new List<string>();

		public Wardrobe(string path = null)
		{
			layers = new List<Layer>();

			if (path != null)
				Load(path);
		}

		public override void Load(string path)
		{
			base.Load(path);

			if (Controller.Busy)
			{
				Controller.serializationWindow.wardrobes++;
				Controller.serializationWindow.Write(string.Format(
					Strings.LOADED_FILENAME,
					Strings.WARDROBE.ToLower(),
					name
				));
			}

			layers.Clear();

			Dictionary<string, object> state = Load_State($"{path}\\{Strings.STATE_BIN}");

			if (state.TryGetValue(Strings.STATE_WARDROBE_WEIGHT, out int weight))
				this.weight = weight;

			foreach (string _path in Directory.GetDirectories(path))
				layers.Add(new Layer(_path));

			if (state.TryGetValue(Strings.STATE_WARDROBE_ORDER, out string order))
			{
				string[] names = order.Split(Strings.STATE_WARDROBE_NAME_LIST_SEPARATOR);
				Layer[] layers = this.layers.ToArray();
				this.layers.Clear();

				foreach (string name in names)
				{
					if (name.Length == 0)
						continue;

					Layer layer = layers.FirstOrDefault(v => v.name.ToLower() == name);

					if (layer != null)
						this.layers.Add(layer);
				}

				foreach (Layer layer in layers)
					if (!this.layers.Contains(layer))
						this.layers.Add(layer);
			}

			if (state.TryGetValue(Strings.STATE_WARDROBE_NAME_MODE, out int nameMode))
				this.nameMode = nameMode;

			for (int i = 0; state.TryGetValue(Strings.STATE_WARDROBE_NAME_LIST + i, out string name); i++)
				nameList.Add(name);
		}
	}
}
