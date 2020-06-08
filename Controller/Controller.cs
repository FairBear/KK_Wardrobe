using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace KK_Wardrobe
{
	public static partial class Controller
	{
		public static ChaFileControl dummyChaFileCtrl = new ChaFileControl();

		public static ManagerWindow managerWindow = new ManagerWindow();
		public static SerializationWindow serializationWindow = new SerializationWindow();
		public static DialogWindow dialogWindow = new DialogWindow();

		public static Random random = new Random(dummyChaFileCtrl.GetHashCode());
		public static HashSet<Wardrobe> wardrobes = new HashSet<Wardrobe>();
		public static HashSet<int> locations = new HashSet<int>();

		public static bool _busy = false;

		public static bool Busy => _busy;

		public static void Start()
		{
			foreach (int i in Enum.GetValues(typeof(Place)))
				locations.Add(i);

			ReloadWardrobes();
			Start_Girls();
		}

		public static void Update()
		{
			if (KK_Wardrobe.ManagerKey.Value.IsDown())
			{
				if (Busy)
				{
					managerWindow.visible = serializationWindow.visible =
						!serializationWindow.visible;

					if (!serializationWindow.visible)
						serializationWindow.InitRect();

					return;
				}

				managerWindow.visible = !managerWindow.visible;

				if (!managerWindow.visible)
					managerWindow.InitRect();
			}

			if (Busy)
				return;

			Update_Girls();
		}

		public static void OnGUI()
		{
			if (dialogWindow.visible)
			{
				dialogWindow.Display();
				return;
			}

			if (serializationWindow.visible)
			{
				serializationWindow.Display();
				return;
			}

			managerWindow.Display();
		}

		public static void ReloadWardrobes()
		{
			string path = Path.GetFullPath(Strings.DATA_PATH);

			wardrobes.Clear();

			if (!Directory.Exists(path))
				return;

			_busy = true;
			serializationWindow.Initialize();
			serializationWindow.visible = managerWindow.visible;

			serializationWindow.Write(Strings.LOADING);

			new Thread(() => ReloadWardrobes_Internal()).Start();
		}

		public static void ReloadWardrobes_Internal()
		{
			string path = Path.GetFullPath(Strings.DATA_PATH);

			foreach (string _path in Directory.GetDirectories(path))
				wardrobes.Add(new Wardrobe(_path));

			_busy = false;
			serializationWindow.Write(Strings.LOADING_DONE);
			serializationWindow.closeable = true;
		}

		public static bool RollForOutfit(out List<Layer> layers,
										 out List<Card> outfits,
										 string fullname = null)
		{
			layers = new List<Layer>();
			outfits = new List<Card>();

			bool predicate(Wardrobe v) =>
				v.nameMode == 0 ?
					!v.nameList.Contains(fullname) :
				v.nameMode == 1 ?
					v.nameList.Contains(fullname) :
					true;

			bool flag0 = wardrobes.RollWeight(
				wardrobes.Sum(v => predicate(v) ? v.weight : 0),
				out Wardrobe wardrobe,
				v => predicate(v) ? v.weight : 0
			);

			if (!flag0)
				return false;

			foreach (Layer layer in wardrobe.layers)
			{
				bool flag1 = layer.cards.RollWeight(
					layer.cards.Sum(v => v.weight),
					out Card outfit,
					v => v.weight
				);

				if (!flag1)
					continue;

				layers.Add(layer);
				outfits.Add(outfit);
			}

			return true;
		}

		/// <summary>
		/// Check if there are folders/cards that are not included
		/// in the manager. This happens if the user deleted them within
		/// the manager, or a new folder/card is copied outside the
		/// manager.
		/// </summary>
		public static bool HasUnusedAssets()
		{
			string path0 = Path.GetFullPath(Strings.DATA_PATH) + "\\";

			if (!Directory.Exists(path0))
				return false;

			foreach (string path1 in Directory.GetDirectories(path0))
			{
				Wardrobe wardrobe = wardrobes.FirstOrDefault(v =>
					v.name.ToLower() == Path.GetFileName(path1).ToLower()
				);

				if (wardrobe == null)
					return true;

				foreach (string path2 in Directory.GetDirectories(path1))
				{
					Layer layer = wardrobe.layers.FirstOrDefault(v =>
						v.name.ToLower() == Path.GetFileName(path2).ToLower()
					);

					if (layer == null)
						return true;

					foreach (string path3 in Directory.GetFiles(path2))
					{
						if (Path.GetExtension(path3).ToLower() != Strings.CARD_EXTENSION)
							continue;

						if (!layer.cards.Any(v =>
								v.name.ToLower() == Path.GetFileName(path3).ToLower()
							))
							return true;
					}
				}
			}

			return false;
		}

		public static void Save()
		{
			serializationWindow.Initialize();
			serializationWindow.visible = true;

			serializationWindow.Write(Strings.SAVING);

			new Thread(() => Save_Internal()).Start();
		}

		public static void Save_Internal()
		{
			string path0 = Path.GetFullPath(Strings.DATA_PATH) + "\\";

			foreach (Wardrobe wardrobe in wardrobes)
				Save_Wardrobe(wardrobe, path0);

			// Delete unused wardrobes.

			foreach (string path1 in Directory.GetDirectories(path0))
				if (!wardrobes.Any(v => v.name.ToLower() == Path.GetFileName(path1).ToLower()))
				{
					Directory.Delete(path1, true);
					serializationWindow.Write(string.Format(
						Strings.DELETE_UNUSED,
						Strings.WARDROBE.ToLower(),
						Path.GetFileName(path1)
					));
					serializationWindow.wardrobes_deleted++;
				}

			serializationWindow.Write(Strings.SAVING_DONE);
			serializationWindow.closeable = true;
		}

		public static void Save_Wardrobe(Wardrobe wardrobe, string path0)
		{
			if (wardrobe.name.Length == 0)
			{
				serializationWindow.Write(string.Format(
					Strings.UNABLE_TO_SAVE,
					Strings.WARDROBE.ToLower()
				));
				serializationWindow.wardrobes_error++;
				return;
			}

			char[] invalidChars = Path.GetInvalidFileNameChars();

			if (wardrobe.name.Any(v => invalidChars.Contains(v)))
			{
				serializationWindow.Write(string.Format(
					Strings.CONTAINS_INVALID_CHAR,
					Strings.WARDROBE.ToLower(),
					wardrobe.name
				));
				serializationWindow.wardrobes_error++;
				return;
			}

			serializationWindow.wardrobes++;

			string path1 = $"{path0}{wardrobe.name}\\";
			Dictionary<string, object> state = new Dictionary<string, object>
			{
				[Strings.STATE_WARDROBE_WEIGHT] = wardrobe.weight,
				[Strings.STATE_WARDROBE_ORDER] = wardrobe.layers.Aggregate(
					string.Empty,
					(a, b) => $"{a}\\{b.name.ToLower()}"
				),
				[Strings.NAME_MODE] = wardrobe.nameMode
			};

			for (int i = 0; i < wardrobe.nameList.Count; i++)
				state[Strings.STATE_WARDROBE_NAME_LIST + i] = wardrobe.nameList[i];

			if (!Directory.Exists(path1))
			{
				Directory.CreateDirectory(path1);
				serializationWindow.Write(string.Format(
					Strings.CREATED_NEW_FOLDER,
					Strings.WARDROBE.ToLower(),
					wardrobe.name
				));
			}

			File.WriteAllBytes(path1 + Strings.STATE_BIN, LZ4MessagePackSerializer.Serialize(state));

			foreach (Layer layer in wardrobe.layers)
				Save_Layer(layer, path1);

			// Delete unused layers.

			foreach (string path2 in Directory.GetDirectories(path1))
				if (!wardrobe.layers.Any(v => v.name.ToLower() == Path.GetFileName(path2).ToLower()))
				{
					Directory.Delete(path2, true);
					serializationWindow.Write(string.Format(
						Strings.DELETE_UNUSED,
						Strings.LAYER.ToLower(),
						Path.GetFileName(path2)
					));
					serializationWindow.layers_deleted++;
				}
		}

		public static void Save_Layer(Layer layer, string path0)
		{
			if (layer.name.Length == 0)
			{
				serializationWindow.Write(string.Format(
					Strings.UNABLE_TO_SAVE,
					Strings.LAYER.ToLower()
				));
				serializationWindow.layers_error++;
				return;
			}

			char[] invalidChars = Path.GetInvalidFileNameChars();

			if (layer.name.Any(v => invalidChars.Contains(v)))
			{
				serializationWindow.Write(string.Format(
					Strings.CONTAINS_INVALID_CHAR,
					Strings.LAYER.ToLower(),
					layer.name
				));
				serializationWindow.layers_error++;
				return;
			}

			serializationWindow.layers++;

			string path1 = $"{path0}{layer.name}\\";
			Dictionary<string, object> state = new Dictionary<string, object>();

			if (!Directory.Exists(path1))
			{
				Directory.CreateDirectory(path1);
				serializationWindow.Write(string.Format(
					Strings.CREATED_NEW_FOLDER,
					Strings.LAYER.ToLower(),
					layer.name
				));
			}

			File.WriteAllBytes(path1 + Strings.CHECK_LIST_BIN, layer.checkList.ToBytes());
			File.WriteAllBytes(path1 + Strings.CHECK_LIST_KKABMX_BIN, layer.checkList_KKABMX.ToBytes());
			File.WriteAllBytes(path1 + Strings.CHECK_LIST_KSOX_BIN, layer.checkList_KSOX.ToBytes());
			File.WriteAllBytes(path1 + Strings.CHECK_LIST_KCOX_BIN, layer.checkList_KCOX.ToBytes());

			foreach (Card card in layer.cards)
			{
				string path2 = path1 + card.name;
				state[card.name.ToLower()] = card.weight;

				if (File.Exists(path2))
					serializationWindow.Write(string.Format(
						Strings.CARD_OVERWRITE,
						card.name
					));
				else
					serializationWindow.Write(string.Format(
						Strings.CARD_SAVED,
						card.name
					));

				File.WriteAllBytes(path2, card.saveData);
				serializationWindow.cards++;
			}

			File.WriteAllBytes(path1 + Strings.STATE_BIN, LZ4MessagePackSerializer.Serialize(state));

			// Delete unused cards.

			foreach (string path2 in Directory.GetFiles(path1))
			{
				if (Path.GetExtension(path2).ToLower() != Strings.CARD_EXTENSION)
					continue;

				if (!layer.cards.Any(v => v.name.ToLower() == Path.GetFileName(path2).ToLower()))
				{
					File.Delete(path2);
					serializationWindow.Write(string.Format(
						Strings.DELETE_UNUSED,
						Strings.CARD.ToLower(),
						path2
					));
					serializationWindow.cards_deleted++;
				}
			}
		}
	}
}
