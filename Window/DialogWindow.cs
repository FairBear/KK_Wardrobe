using System;
using UnityEngine;

namespace KK_Wardrobe
{
	public class DialogWindow : BaseWindow
	{
		public string _title;
		public string _description;
		public string[] _choices;
		public Action<int> _callback;

		public GUIStyle _labelStyle;

		public override string Title => _title;

		public override float Height => 160;

		public override float Width => 320;

		public override void Draw(int id)
		{
			if (_labelStyle == null)
				_labelStyle = new GUIStyle(GUI.skin.box)
				{
					alignment = TextAnchor.UpperCenter,
					wordWrap = true
				};

			GUILayout.BeginVertical();
			{
				GUILayout.Label(_description, _labelStyle, GUILayout.ExpandHeight(true));

				GUILayout.BeginHorizontal(GUILayout.ExpandHeight(false));
				{
					GUILayout.FlexibleSpace();

					for (int i = 0; i < _choices.Length; i++)
					{
						string choice = _choices[i];

						if (GUILayout.Button(choice, GUILayout.ExpandWidth(false)))
						{
							_callback(i);
							visible = false;
						}

						GUILayout.FlexibleSpace();
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		public void Show(string title,
						 string description,
						 Action<int> callback,
						 params string[] choices)
		{
			InitRect();
			_title = title;
			_description = description;
			_callback = callback;
			_choices = choices;
			visible = true;
		}
	}
}
