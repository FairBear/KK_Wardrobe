using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow
	{
		public bool config_Busy = false;
		public CheckList config_Clipboard = null;
		public Folder config_Clipboard_Folder = null;

		public void Draw_Config_Layer(Layer layer)
		{
			GUILayout.BeginVertical(GUI.skin.box);
			{
				Draw_Config_Field("Name",
					() => layer.name = GUILayout.TextField(layer.name)
				);
			}
			GUILayout.EndVertical();

			Draw_Config_Layer_Address();

			config_Scroll = GUILayout.BeginScrollView(config_Scroll);
			{
				if (config_Address.Count <= 0)
				{
					Draw_Config_Layer_CheckList(layer.checkList);
					Draw_Config_Layer_CheckList(layer.checkList_KKABMX);
					Draw_Config_Layer_CheckList(layer.checkList_KCOX);
					Draw_Config_Layer_CheckList(layer.checkList_KSOX);
				}
				else
					Draw_Config_Layer_CheckList(config_Address.Peek());
			}
			GUILayout.EndScrollView();
		}

		public void Draw_Config_Layer_Address()
		{
			GUILayout.BeginHorizontal();
			{
				bool flag = config_Address.Count > 0;

				if (flag && GUILayout.Button(Strings.ARROW_LEFT, GUILayout.ExpandWidth(false)))
					config_Address.Pop();

				GUILayout.BeginHorizontal(GUI.skin.box);
				{
					if (flag)
					{
						CheckList pop = null;
						CheckList[] list = config_Address.ToArray();
						int n = Mathf.Min(list.Length, CONFIG_MAX) - 1;

						for (int i = n; i >= 0; i--)
						{
							CheckList node = list[i];

							if (i < n)
								GUILayout.Label(Strings.ARROW_RIGHT, GUILayout.Width(10f));

							if (GUILayout.Button(node.LabelKey, GUILayout.ExpandWidth(false)))
							{
								pop = node;
								break;
							}
						}

						if (pop != null)
							while (pop != config_Address.Peek())
								config_Address.Pop();
					}
					else
						GUILayout.Label(string.Empty);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndHorizontal();
		}

		public void Draw_Config_Layer_CheckList(CheckList checkList)
		{
			HashSet<string> multiple = new HashSet<string>();

			foreach (CheckList node in checkList)
			{
				bool next = Draw_Config_Layer_CheckList_Item(
					node.LabelKey,
					node.Value,
					node.Count,
					node.Sum(v => v._value ? 1 : 0),
					node
				);

				if (config_Busy)
					config_Busy = false;
				else
					node.Value = next;
			}
		}

		public bool Draw_Config_Layer_CheckList_Item(string key,
													 bool value,
													 int fields,
													 int fieldsActive,
													 CheckList node = null)
		{
			bool result;

			GUILayout.BeginHorizontal(GUI.skin.box);
			{
				result = GUILayout.Toggle(value, " " + key);

				if (fields > 0)
				{
					GUILayout.Label(
						$"{(value ? fieldsActive : 0)}/{fields} ",
						GUILayout.ExpandWidth(false)
					);

					if (GUILayout.Button(Strings.RESET, config_ResetStyle, GUILayout.ExpandWidth(false)))
					{
						config_Busy = true;
						Draw_Config_Layer_CheckList_Reset(node);
					}

					if (config_Clipboard != node &&
						config_Clipboard?.type == node.type)
					{
						string label;

						if (config_Clipboard_Folder != config_Folder)
							label = string.Format(
								Strings.PASTE_CHECKLIST_OTHER,
								config_Clipboard_Folder.name,
								config_Clipboard.LabelKey
							);
						else
							label = string.Format(Strings.PASTE_CHECKLIST, config_Clipboard.LabelKey);

						if (GUILayout.Button(label, GUILayout.ExpandWidth(false)))
						{
							config_Busy = true;
							Draw_Config_Layer_CheckList_Copy(config_Clipboard, node);
						}
					}

					if (config_Clipboard == node)
					{
						if (GUILayout.Button(Strings.COPY, config_CopyStyle, GUILayout.ExpandWidth(false)))
						{
							config_Clipboard = null;
							config_Clipboard_Folder = null;
						}
					}
					else if (GUILayout.Button(Strings.COPY, GUILayout.ExpandWidth(false)))
					{
						config_Clipboard = node;
						config_Clipboard_Folder = address.Peek();
					}

					if (value && GUILayout.Button(Strings.ARROW_RIGHT, GUILayout.ExpandWidth(false)))
						config_Address.Push(node);
				}
			}
			GUILayout.EndHorizontal();

			return result;
		}

		public void Draw_Config_Layer_CheckList_Copy(CheckList from, CheckList to)
		{
			if (from == null || to == null)
				return;

			to.Value = from._value;

			foreach (CheckList _from in from)
				Draw_Config_Layer_CheckList_Copy(_from, to.FirstOrDefault(v => v.Key == _from.Key));
		}

		public void Draw_Config_Layer_CheckList_Reset(CheckList node)
		{
			node.Value = true;

			foreach (CheckList _node in node)
				Draw_Config_Layer_CheckList_Reset(_node);
		}
	}
}
