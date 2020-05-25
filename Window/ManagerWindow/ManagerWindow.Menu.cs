using System.Linq;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public GUIStyle menu_DeleteStyle;

		public void Draw_Menu()
		{
			if (menu_DeleteStyle == null)
			{
				menu_DeleteStyle = new GUIStyle(GUI.skin.button)
				{
					fontStyle = FontStyle.Bold
				};
				menu_DeleteStyle.normal.textColor = Color.red;
			}

			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				if (GUILayout.Button(Strings.SAVE, GUILayout.ExpandWidth(false)))
				{
					if (Controller.HasUnusedAssets())
						Controller.dialogWindow.Show(
							Strings.SAVE,
							Strings.SAVE_CONFLICT,
							Draw_Menu_Save,
							Strings.SAVE,
							Strings.CANCEL
						);
					else
						Controller.Save();
				}

				if (config_Folder == null)
				{
					GUILayout.Space(10f);

					if (address.Count > 0)
					{
						if (GUILayout.Button(Strings.EDIT_CONFIGURATION, GUILayout.ExpandWidth(false)))
							config_Folder = address.Peek();
					}
					else if (GUILayout.Button(Strings.RELOAD, GUILayout.ExpandWidth(false)))
					{
						config_Folder = null;
						config_Address.Clear();
						address.Clear();
						Controller.ReloadWardrobes();
					}
				}
				else
				{
					GUILayout.FlexibleSpace();

					Draw_Menu_Delete();
				}
			}
			GUILayout.EndHorizontal();
		}

		public void Draw_Menu_Delete()
		{
			if (config_Folder == null)
				return;

			if (config_Folder is Wardrobe wardrobe)
				Draw_Menu_Delete_Wardrobe(wardrobe);
			else if (config_Folder is Layer layer)
				Draw_Menu_Delete_Layer(layer);
		}

		public void Draw_Menu_Delete_Wardrobe(Wardrobe wardrobe)
		{
			if (GUILayout.Button(Strings.DELETE_WARDROBE, menu_DeleteStyle, GUILayout.ExpandWidth(false)))
			{
				Controller.wardrobes.Remove(wardrobe);
				Draw_Menu_Delete_Loop(wardrobe);
				config_Folder = null;
			}
		}

		public void Draw_Menu_Delete_Layer(Layer layer)
		{
			if (GUILayout.Button(Strings.DELETE_LAYER, menu_DeleteStyle, GUILayout.ExpandWidth(false)))
			{
				Controller.wardrobes.FirstOrDefault(v => v.layers.Contains(layer)).layers.Remove(layer);
				Draw_Menu_Delete_Loop(layer);
				config_Folder = null;
			}
		}

		public void Draw_Menu_Delete_Loop(Folder folder)
		{
			if (address.Contains(folder))
				while (address.Count > 0 && address.Pop() != folder) ;
		}

		public void Draw_Menu_Save(int i)
		{
			if (i == 0)
				Controller.Save();
		}
	}
}
