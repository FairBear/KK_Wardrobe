using System.Collections.Generic;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public const int ADDRESS_MAX = 4;

		public Stack<Folder> address = new Stack<Folder>();

		public void Draw_Address()
		{
			GUILayout.BeginHorizontal();
			{
				if ((address.Count > 0 || config_Folder != null) &&
					GUILayout.Button(Strings.ARROW_LEFT, GUILayout.ExpandWidth(false)))
				{
					if (config_Folder != null)
					{
						config_Folder = null;
						config_Address.Clear();
					}
					else
						address.Pop();
				}

				GUILayout.BeginHorizontal(GUI.skin.box);
				{
					if (address.Count > 0)
					{
						Folder pop = null;
						Folder[] folders = address.ToArray();
						int n = Mathf.Min(folders.Length, ADDRESS_MAX) - 1;

						for (int i = n; i >= 0; i--)
						{
							Folder folder = folders[i];

							if (i < n)
								GUILayout.Label(Strings.ARROW_RIGHT, GUILayout.Width(10f));

							if (GUILayout.Button(folder.name, GUILayout.ExpandWidth(false)))
							{
								pop = folder;
								config_Address.Clear();
								break;
							}
						}

						if (pop != null)
							while (config_Folder != null || pop != address.Peek())
								if (config_Folder != null)
								{
									config_Folder = null;
									config_Address.Clear();
								}
								else
									address.Pop();
					}
					else if (config_Folder == null)
						GUILayout.Label(string.Empty);

					if (config_Folder != null)
					{
						if (address.Count > 0)
							GUILayout.Label(Strings.ARROW_RIGHT, GUILayout.Width(10f));

						string label = string.Format(Strings.EDIT_FOLDERNAME, config_Folder.name);
						GUILayout.Button(label, GUILayout.ExpandWidth(false));
					}
				}
				GUILayout.EndHorizontal();

				if (GUILayout.Button(Strings.X, GUILayout.ExpandWidth(false)))
					visible = false;
			}
			GUILayout.EndHorizontal();
		}
	}
}
