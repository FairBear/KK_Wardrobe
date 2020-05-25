using System.Linq;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public GUIStyle wardrobe_IndexStyle;
		public Vector2 wardrobe_Scroll = Vector2.zero;

		public void Draw_Wardrobe()
		{
			if (wardrobe_IndexStyle == null)
			{
				wardrobe_IndexStyle = new GUIStyle(GUI.skin.label)
				{
					alignment = TextAnchor.MiddleCenter,
					fontStyle = FontStyle.Bold
				};
			}

			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(Strings.NEW_LAYER, GUILayout.ExpandWidth(false)))
				{
					int i;
					Wardrobe wardrobe = address.Peek() as Wardrobe;

					bool predicate(Layer v) =>
						v.name.ToLower() == string.Format(Strings.LAYER_N, i).ToLower();

					for (i = 1; wardrobe.layers.Any(predicate); i++) ;

					Layer layer = new Layer { name = string.Format(Strings.LAYER_N, i) };
					wardrobe.layers.Add(layer);
				}
			}
			GUILayout.EndHorizontal();

			wardrobe_Scroll = GUILayout.BeginScrollView(wardrobe_Scroll);
			{
				if (address.Peek() is Wardrobe wardrobe)
					for (int i = 0; i < wardrobe.layers.Count; i++)
						Draw_Wardrobe_Item(i, wardrobe.layers[i], wardrobe);
			}
			GUILayout.EndScrollView();
		}

		public void Draw_Wardrobe_Item(int index, Layer layer, Wardrobe wardrobe)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
				{
					GUILayout.Label(
						(index + 1).ToString(),
						wardrobe_IndexStyle,
						GUILayout.Width(20f)
					);

					if (index > 1 && GUILayout.Button(Strings.T, GUILayout.Width(20f)))
					{
						wardrobe.layers.Remove(layer);
						wardrobe.layers.Insert(0, layer);
					}

					if (index > 0 && GUILayout.Button(Strings.ARROW_UP, GUILayout.Width(20f)))
					{
						wardrobe.layers.Remove(layer);
						wardrobe.layers.Insert(index - 1, layer);
					}

					int max = wardrobe.layers.Count - 1;

					if (index < max && GUILayout.Button(Strings.ARROW_DOWN, GUILayout.Width(20f)))
					{
						wardrobe.layers.Remove(layer);
						wardrobe.layers.Insert(index + 1, layer);
					}

					if (index < max - 1 && GUILayout.Button(Strings.B, GUILayout.Width(20f)))
					{
						wardrobe.layers.Remove(layer);
						wardrobe.layers.Insert(max, layer);
					}
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical(GUI.skin.box);
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Label(layer.name);

						if (GUILayout.Button(Strings.EDIT, GUILayout.ExpandWidth(false)))
							config_Folder = layer;

						if (GUILayout.Button(Strings.OPEN, GUILayout.ExpandWidth(false)))
							address.Push(layer);
					}
					GUILayout.EndHorizontal();

					GUILayout.Label(string.Format(Strings.COUNT_CARDS, layer.cards.Count));
					Draw_Wardrobe_Item_Images(layer);
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		public void Draw_Wardrobe_Item_Images(Layer layer)
		{
			GUILayout.BeginHorizontal();
			{
				int n = 0;

				foreach (Card card in layer.cards)
				{
					if (n > 6)
						break;

					GUILayout.Label(
						card.Image,
						GUILayout.Width(IMAGE_WIDTH),
						GUILayout.Height(IMAGE_HEIGHT)
					);

					n++;
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}
