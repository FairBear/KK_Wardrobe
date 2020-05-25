using System.Linq;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public Vector2 home_Scroll = Vector2.zero;

		public void Draw_Home()
		{
			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(Strings.NEW_WARDROBE, GUILayout.ExpandWidth(false)))
				{
					int i;

					bool predicate(Wardrobe v) =>
						v.name.ToLower() == string.Format(Strings.WARDROBE_N, i).ToLower();

					for (i = 1; Controller.wardrobes.Any(predicate); i++) ;

					Wardrobe wardrobe = new Wardrobe { name = string.Format(Strings.WARDROBE_N, i) };
					Controller.wardrobes.Add(wardrobe);
				}
			}
			GUILayout.EndHorizontal();

			home_Scroll = GUILayout.BeginScrollView(home_Scroll);
			{
				foreach (Wardrobe wardrobe in Controller.wardrobes)
					Draw_Home_Item(wardrobe);
			}
			GUILayout.EndScrollView();
		}

		public void Draw_Home_Item(Wardrobe wardrobe)
		{
			GUILayout.BeginVertical(GUI.skin.box);
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Label(wardrobe.name);

					if (GUILayout.Button(Strings.EDIT, GUILayout.ExpandWidth(false)))
						config_Folder = wardrobe;

					if (GUILayout.Button(Strings.OPEN, GUILayout.ExpandWidth(false)))
						address.Push(wardrobe);
				}
				GUILayout.EndHorizontal();

				float maxWeight = Controller.wardrobes.Aggregate(0, (a, b) => a + b.weight) / 100f;
				float chance = maxWeight > 0 ? wardrobe.weight / maxWeight : 0;

				GUILayout.Label(string.Format(Strings.COUNT_LAYERS, wardrobe.layers.Count));
				GUILayout.Label(string.Format(Strings.WARDROBE_WEIGHT, wardrobe.weight, chance));
			}
			GUILayout.EndVertical();
		}
	}
}
