using ExtensibleSaveFormat;
using KKABMX.Core;
using KKAPI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KK_Wardrobe
{
	public static class Util
	{
		public static void PromptCards(Action<string[]> onAccept, string filter)
		{
			OpenFileDialog.Show(
				onAccept,
				Strings.SELECT_CARDS,
				Strings.CHARA_PATH,
				Strings.CHARA_COORD_FILTER,
				Strings.CARD_EXTENSION,
				OpenFileDialog.OpenSaveFileDialgueFlags.OFN_ALLOWMULTISELECT |
				OpenFileDialog.OpenSaveFileDialgueFlags.OFN_FILEMUSTEXIST |
				OpenFileDialog.OpenSaveFileDialgueFlags.OFN_EXPLORER |
				OpenFileDialog.OpenSaveFileDialgueFlags.OFN_LONGNAMES
			);
		}

		public static CardType LoadFile(string path, out byte[] saveData, out ChaFileControl result, out byte[] pngData)
		{
			if (Path.GetExtension(path) != Strings.CARD_EXTENSION)
				goto UNKNOWN;

			saveData = File.ReadAllBytes(path);

			ChaFileControl chaFileCtrl = new ChaFileControl();

			if (chaFileCtrl.LoadCharaFile(path))
			{
				result = chaFileCtrl;
				pngData = chaFileCtrl.pngData;
				return CardType.Character;
			}

			ChaFileCoordinate chaFileCoord = new ChaFileCoordinate();

			if (chaFileCoord.LoadFile(path))
			{
				KKABMXCoordToChara(chaFileCoord, chaFileCtrl);

				for (int i = 0; i < chaFileCtrl.coordinate.Length; i++)
					chaFileCtrl.coordinate[i] = chaFileCoord;

				result = chaFileCtrl;
				pngData = PngFile.LoadPngBytes(path);
				return CardType.Coordinate;
			}

		UNKNOWN:
			saveData = null;
			result = null;
			pngData = null;
			return CardType.Unknown;
		}

		public static void KKABMXCoordToChara(ChaFileCoordinate coord, ChaFileControl chara)
		{
			Dictionary<string, BoneModifierData> coordData = coord.KKABMXData().BoneModifierData();
			List<BoneModifier> charaData = chara.KKABMXData().BoneModifiers();

			foreach (KeyValuePair<string, BoneModifierData> pair in coordData)
			{
				BoneModifier modifier = charaData.FirstOrDefault(v => v.BoneName == pair.Key);

				if (modifier == null)
					charaData.Add(modifier = new BoneModifier(pair.Key));

				modifier.MakeCoordinateSpecific();

				for (int i = 0; i < modifier.CoordinateModifiers.Length; i++)
					modifier.CoordinateModifiers[i] = pair.Value;
			}

			ExtendedSave.SetExtendedDataById(chara, KKABMX_Core.ExtDataGUID, charaData.KKABMXData());
		}
	}
}
