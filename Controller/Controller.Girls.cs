using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace KK_Wardrobe
{
	public static partial class Controller
	{
		public static HashSet<Girl> girls = new HashSet<Girl>();
		public static SaveData.Player player;

		public static void Start_Girls()
		{
			SceneManager.sceneUnloaded += SceneUnloaded_Girls;
		}

		public static void SceneUnloaded_Girls(Scene scene)
		{
			// Compile the girls.
			Manager.Game game = Manager.Game.Instance;

			if (game == null || scene.name != "MyRoom")
				return;

			player = game.Player;

			if (KK_Wardrobe.IncludePlayer.Value)
				girls.Add(new Girl(
					() => player.charaBase,
					() => player.chaCtrl,
					() => player.charFile
				));

			foreach (SaveData.Heroine girl in game.HeroineList)
				if (!girl.isTeacher)
					girls.Add(new Girl(
						() => girl.charaBase,
						() => girl.chaCtrl,
						() => girl.charFile
					));
		}

		public static void Update_Girls()
		{
			if (girls.Count == 0)
				return;

			if (player?.charaBase == null)
			{
				player = null;
				girls.Clear();
				return;
			}

			foreach (Girl girl in girls)
				girl.Update();
		}
	}
}
