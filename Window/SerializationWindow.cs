using UnityEngine;

namespace KK_Wardrobe
{
	public class SerializationWindow : BaseWindow
	{
		public override string Title => Strings.WARDROBE_MANAGER;
		public override float Height => 320f;
		public override float Width => 480f;

		public Vector2 scroll = Vector2.zero;

		public bool closeable = false;
		public int wardrobes = 0;
		public int wardrobes_error = 0;
		public int wardrobes_deleted = 0;
		public int layers = 0;
		public int layers_error = 0;
		public int layers_deleted = 0;
		public int cards = 0;
		public int cards_error = 0;
		public int cards_deleted = 0;
		public string _texts = string.Empty;


		public override void Draw(int id)
		{
			GUILayout.BeginVertical();
			{
				GUILayout.BeginVertical();
				{
					GUILayout.Label(string.Format(
						Strings.FOLDER_ACTIVE_ERROR_DELETED,
						Strings.WARDROBES,
						wardrobes,
						wardrobes_error,
						wardrobes_deleted
					));
					GUILayout.Label(string.Format(
						Strings.FOLDER_ACTIVE_ERROR_DELETED,
						Strings.LAYERS,
						layers,
						layers_error,
						layers_deleted
					));
					GUILayout.Label(string.Format(
						Strings.FOLDER_ACTIVE_ERROR_DELETED,
						Strings.CARDS,
						cards,
						cards_error,
						cards_deleted
					));
				}
				GUILayout.EndVertical();

				scroll = GUILayout.BeginScrollView(scroll);
				{
					GUILayout.TextField(_texts, GUILayout.ExpandHeight(true));
				}
				GUILayout.EndScrollView();

				if (closeable)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.FlexibleSpace();

						if (GUILayout.Button(Strings.CLOSE))
							visible = false;

						GUILayout.FlexibleSpace();
					}
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		public void Initialize()
		{
			closeable = false;
			wardrobes =
				wardrobes_error =
				wardrobes_deleted =
				layers =
				layers_error =
				layers_deleted =
				cards =
				cards_error =
				cards_deleted = 0;
			_texts = string.Empty;

			InitRect();
		}

		public void Write(string text) => _texts = text + Strings.NEWLINE + _texts;
	}
}
