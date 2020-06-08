using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace KK_Wardrobe
{
	[BepInProcess("Koikatu")]
	[BepInProcess("Koikatsu Party")]
	[BepInPlugin(GUID, PluginName, Version)]
	public class KK_Wardrobe : BaseUnityPlugin
	{
		public const string GUID = "com.fairbair.kk_wardrobe";
		public const string PluginName = "KK_Wardrobe";
		public const string Version = "1.0.1";


		const string SECTION_GENERAL = "General";


		const string DESCRIPTION_CHANGE_ON_START =
			"If the girls change their outfits at the start of the day.";
		const string DESCRIPTION_INCLUDE_PLAYER =
			"If the player should also be able to change outfits.";

		public static ConfigEntry<bool> ChangeOnStart { get; set; }
		public static ConfigEntry<bool> IncludePlayer { get; set; }

		public static ConfigEntry<KeyboardShortcut> ManagerKey { get; set; }

		public void Awake()
		{
			ChangeOnStart = Config.Bind(SECTION_GENERAL, "Change Outfits on Start", true, DESCRIPTION_CHANGE_ON_START);
			IncludePlayer = Config.Bind(SECTION_GENERAL, "Include Player", true, DESCRIPTION_INCLUDE_PLAYER);

			ManagerKey = Config.Bind(SECTION_GENERAL, "Show/Hide Manager Key", new KeyboardShortcut(KeyCode.F10));

			Strings.Awake();
		}

		public void Start()
		{
			Controller.Start();
		}

		public void Update()
		{
			Controller.Update();
		}

		public void OnGUI()
		{
			Controller.OnGUI();
		}
    }
}
