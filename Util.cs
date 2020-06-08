using ExtensibleSaveFormat;
using KKABMX.Core;
using KKAPI.Utilities;
using KoiClothesOverlayX;
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
				filter,
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

			ChaFileControl chara = new ChaFileControl();

			if (chara.LoadCharaFile(path))
			{
				result = chara;
				pngData = chara.pngData;
				return CardType.Character;
			}

			ChaFileCoordinate coord = new ChaFileCoordinate();

			if (coord.LoadFile(path))
			{
				KKABMXCoordToChara(coord, chara);
				KCOXCoordToChara(coord, chara);

				for (int i = 0; i < chara.coordinate.Length; i++)
					chara.coordinate[i] = coord;

				result = chara;
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

		public static void KCOXCoordToChara(ChaFileCoordinate coord, ChaFileControl chara)
		{
			Dictionary<string, ClothesTexData> coordData = coord.KCOXData().KCOXCoordinateDictionary();
			Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>> charaData =
				chara.KCOXData().KCOXDictionary();
			ChaFileDefine.CoordinateType[] coordinates =
				Enum.GetValues(typeof(ChaFileDefine.CoordinateType)) as ChaFileDefine.CoordinateType[];

			foreach (ChaFileDefine.CoordinateType coordinate in coordinates)
				charaData[coordinate] = coordData;

			ExtendedSave.SetExtendedDataById(chara, KoiClothesOverlayMgr.GUID, charaData.KCOXData());
		}
	}
}
