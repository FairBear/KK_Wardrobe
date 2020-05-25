using System.IO;
using System.Linq;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public static GUIStyle layer_DeleteStyle;

		public Vector2 layer_Scroll = Vector2.zero;

		public void Draw_Layer()
		{
			if (layer_DeleteStyle == null)
			{
				layer_DeleteStyle = new GUIStyle(GUI.skin.button)
				{
					fontStyle = FontStyle.Bold
				};
				layer_DeleteStyle.normal.textColor = Color.red;
			}

			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(Strings.ADD_CARDS, GUILayout.ExpandWidth(false)))
					Util.PromptCards(Layer_Add_onAccept, Strings.CHARA_COORD_FILTER);
			}
			GUILayout.EndHorizontal();

			Layer layer = address.Peek() as Layer;

			layer_Scroll = GUILayout.BeginScrollView(layer_Scroll);
			{
				foreach (Card card in layer.cards.ToArray())
					Draw_Layer_Item(layer, card);
			}
			GUILayout.EndScrollView();
		}
		
		public void Draw_Layer_Item(Layer layer, Card card)
		{
			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label(
						card.Image,
						GUILayout.Width(IMAGE_WIDTH),
						GUILayout.Height(IMAGE_HEIGHT)
					);

					if (GUILayout.Button(Strings.DELETE, layer_DeleteStyle, GUILayout.Width(IMAGE_WIDTH - 3f)))
						layer.cards.Remove(card);
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					Draw_Layer_Item_Field(Strings.NAME, card.name);

					Draw_Layer_Item_Field(Strings.CARD_TYPE, card.type.ToString());

					string weight = card.weight.ToString();
					Draw_Layer_Item_Field(Strings.WEIGHT, card.weight.ToString(), out string _weight);
					if (weight != _weight &&
						int.TryParse(_weight, out int __weight) &&
						__weight >= 0)
						card.weight = __weight;

					float maxWeight = layer.cards.Aggregate(0, (a, b) => a + b.weight) / 100f;
					float chance = maxWeight > 0 ? card.weight / maxWeight : 0;

					Draw_Layer_Item_Field(string.Empty, string.Format(Strings.CHANCE, chance));
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		public void Draw_Layer_Item_Field(string label, string value)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(label, GUILayout.Width(100f));
				GUILayout.Label(value);
			}
			GUILayout.EndHorizontal();
		}

		public void Draw_Layer_Item_Field(string label, string curr, out string next)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(label, GUILayout.Width(100f));
				next = GUILayout.TextField(curr);
			}
			GUILayout.EndHorizontal();
		}

		public void Layer_Add_onAccept(string[] paths)
		{
			if (paths?.Length > 0)
			{
				Layer layer = address.Peek() as Layer;

				foreach (string path in paths)
				{
					// Overwrite existing cards with the same name.
					Card card = layer.cards.FirstOrDefault(v =>
						v.name.ToLower() == Path.GetFileName(path).ToLower()
					);
					int weight = 10;

					if (card != null)
					{
						weight = card.weight;
						layer.cards.Remove(card);
					}

					layer.AddCard(path).weight = weight;
				}
			}
		}
	}
}
