using UnityEngine;

namespace KK_Wardrobe
{
	public partial class ManagerWindow : BaseWindow
	{
		public const int IMAGE_WIDTH = 63; // 252
		public const int IMAGE_HEIGHT = 88; // 352

		public override string Title => Strings.WARDROBE_MANAGER;
		public override float Height => 640f;
		public override float Width => 560f;

		public ManagerWindow()
		{
			_id = GetHashCode();
			InitRect();
		}

		public override void Draw(int id)
		{
			GUILayout.BeginVertical();
			{
				Draw_Address();

				if (config_Folder == null)
				{
					switch (address.Count)
					{
						case 0:
							Draw_Home();
							break;

						case 1:
							Draw_Wardrobe();
							break;

						case 2:
							Draw_Layer();
							break;
					}
				}
				else
					Draw_Config();

				GUILayout.FlexibleSpace();

				Draw_Menu();
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}
