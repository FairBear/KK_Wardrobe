using System;
using System.Collections.Generic;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public const int CONFIG_MAX = 3;

		public Folder config_Folder = null;
		public Stack<CheckList> config_Address = new Stack<CheckList>();

		public Vector2 config_Scroll = Vector2.zero;

		public static GUIStyle config_CopyStyle;
		public static GUIStyle config_ResetStyle;

		public void Draw_Config()
		{
			if (config_CopyStyle == null)
			{
				config_CopyStyle = new GUIStyle(GUI.skin.button);
				config_CopyStyle.normal.textColor = Color.yellow;
				config_ResetStyle = new GUIStyle(GUI.skin.button)
				{
					fontStyle = FontStyle.Bold
				};
				config_ResetStyle.normal.textColor = Color.red;
			}

			if (config_Folder is Wardrobe wardrobe)
				Draw_Config_Wardrobe(wardrobe);
			else if (config_Folder is Layer layer)
				Draw_Config_Layer(layer);
			else
			{
				config_Folder = null;
				config_Address.Clear();
			}
		}

		public void Draw_Config_Field(string label, Action act)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(label, GUILayout.Width(120f));
				act();
			}
			GUILayout.EndHorizontal();
		}
	}
}
