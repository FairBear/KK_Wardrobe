using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public Vector2 config_wardrobe_Scroll = Vector2.zero;

		public void Draw_Config_Wardrobe(Wardrobe wardrobe)
		{
			GUILayout.BeginVertical(GUI.skin.box);
			{
				Draw_Config_Field(Strings.NAME,
					() => wardrobe.name = GUILayout.TextField(wardrobe.name)
				);
				Draw_Config_Field(Strings.WEIGHT,
					() => int.TryParse(
						GUILayout.TextField(wardrobe.weight.ToString()),
						out wardrobe.weight
					)
				);
				Draw_Config_Field(Strings.NAME_MODE,
					() => wardrobe.nameMode = GUILayout.Toolbar(
						wardrobe.nameMode,
						Strings.WARDROBE_NAME_MODE,
						GUILayout.ExpandWidth(false)
					)
				);
			}
			GUILayout.EndVertical();

			Draw_Config_Wardrobe_NameList(wardrobe);
		}

		public void Draw_Config_Wardrobe_NameList(Wardrobe wardrobe)
		{
			switch (wardrobe.nameMode)
			{
				case 0:
					GUILayout.Label(Strings.WARDROBE_NAME_MODE_BLACKLIST);
					break;

				case 1:
					GUILayout.Label(Strings.WARDROBE_NAME_MODE_WHITELIST);
					break;
			}

			GUILayout.Label(Strings.WARDROBE_NAME_MODE_TIP);

			config_wardrobe_Scroll = GUILayout.BeginScrollView(config_wardrobe_Scroll);
			{
				int i;
				bool flag = false;

				for (i = 0; i < wardrobe.nameList.Count; i++)
					if (flag = Draw_Config_Wardrobe_NameList_Item(wardrobe, wardrobe.nameList[i], i))
						break;

				if (flag)
					wardrobe.nameList.RemoveAt(i);

				GUILayout.BeginHorizontal(GUI.skin.box);
				{
					if (GUILayout.Button(Strings.PLUS, GUILayout.ExpandWidth(false)))
						wardrobe.nameList.Add(string.Empty);

					if (GUILayout.Button(Strings.BROWSE, GUILayout.ExpandWidth(false)))
						Util.PromptCards(
							paths => Draw_Config_Wardrobe_NameList_onAccept(wardrobe, paths),
							Strings.CHARA_FILTER
						);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
		}

		public bool Draw_Config_Wardrobe_NameList_Item(Wardrobe wardrobe, string name, int key)
		{
			bool flag;

			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				flag = GUILayout.Button(Strings.X, GUILayout.ExpandWidth(false));
				wardrobe.nameList[key] = GUILayout.TextField(name);
			}
			GUILayout.EndHorizontal();

			return flag;
		}

		public void Draw_Config_Wardrobe_NameList_onAccept(Wardrobe wardrobe, string[] paths)
		{
			ChaFileControl chaFileCtrl = new ChaFileControl();

			foreach (string path in paths)
				if (chaFileCtrl.LoadCharaFile(path))
					wardrobe.nameList.Add(chaFileCtrl.parameter.fullname);
		}
	}
}
