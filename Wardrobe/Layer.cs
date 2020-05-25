using MessagePack;
using System.Collections.Generic;
using System.IO;

namespace KK_Wardrobe
{
	public class Layer : Folder
	{
		public HashSet<Card> cards;
		public CheckList_Generic checkList;
		public CheckList_Generic checkList_KKABMX;
		public CheckList_Generic checkList_KCOX;
		public CheckList_Generic checkList_KSOX;

		public Layer(string path = null)
		{
			cards = new HashSet<Card>();
			checkList = new CheckList_Generic(
				Controller.dummyChaFileCtrl,
				Controller.dummyChaFileCtrl.GetType(),
				data: Load_CheckList(path, Strings.CHECK_LIST_BIN)
			);
			KKABMXHelper bones = new KKABMXHelper();
			checkList_KKABMX = new CheckList_Generic(
				bones,
				bones.GetType(),
				data: Load_CheckList(path, Strings.CHECK_LIST_KKABMX_BIN)
			);
			KCOXHelper clothes = new KCOXHelper();
			checkList_KCOX = new CheckList_Generic(
				clothes,
				clothes.GetType(),
				data: Load_CheckList(path, Strings.CHECK_LIST_KCOX_BIN)
			);
			KSOXHelper skin = new KSOXHelper();
			checkList_KSOX = new CheckList_Generic(
				skin,
				skin.GetType(),
				data: Load_CheckList(path, Strings.CHECK_LIST_KSOX_BIN)
			);

			if (path != null)
				Load(path);
		}

		public override void Load(string path)
		{
			base.Load(path);

			if (Controller.Busy)
			{
				Controller.serializationWindow.layers++;
				Controller.serializationWindow.Write(string.Format(
					Strings.LOADED_FILENAME,
					Strings.LAYER.ToLower(),
					name
				));
			}

			cards.Clear();

			Dictionary<string, object> state = Load_State($"{path}\\{Strings.STATE_BIN}");

			foreach (string _path in Directory.GetFiles(path))
			{
				Card card = AddCard(_path);

				if (card == null)
					continue;

				if (Controller.Busy)
				{
					Controller.serializationWindow.cards++;
					Controller.serializationWindow.Write(string.Format(
						Strings.LOADED_FILENAME,
						Strings.CARD.ToLower(),
						card.name
					));
				}

				if (state.TryGetValue(card.name.ToLower(), out int weight))
					card.weight = weight;
			}
		}

		public Dictionary<string, bool> Load_CheckList(string path, string filename)
		{
			if (path == null || filename == null)
				return null;

			string _path = $"{path}\\{filename}";

			if (!File.Exists(_path))
				return null;

			byte[] bytes = File.ReadAllBytes(_path);

			try
			{
				return LZ4MessagePackSerializer.Deserialize<Dictionary<string, bool>>(bytes);
			}
			catch { }

			return null;
		}

		public Card AddCard(string path)
		{
			Card card = new Card(path);

			if (card.type == CardType.Unknown)
				return null;

			cards.Add(card);
			return card;
		}
	}
}
