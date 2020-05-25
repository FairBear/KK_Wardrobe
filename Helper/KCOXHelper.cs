using KoiClothesOverlayX;
using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public class KCOXHelper
	{
		public Dictionary<string, Dictionary<string, ClothesTexData>> KCOX;

		public KCOXHelper(ChaFileControl chaFileCtrl = null)
		{
			KCOX = new Dictionary<string, Dictionary<string, ClothesTexData>>();

			ChaFileDefine.CoordinateType[] coordinates =
				Enum.GetValues(typeof(ChaFileDefine.CoordinateType)) as ChaFileDefine.CoordinateType[];
			Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>> data =
				chaFileCtrl?.KCOXData().KCOXDictionary();

			foreach (ChaFileDefine.CoordinateType type in coordinates)
			{
				Dictionary<string, ClothesTexData> dictionary = KCOX[type.ToString()] =
					new Dictionary<string, ClothesTexData>();

				foreach (string idx in KoiClothesOverlayMgr.MainClothesNames)
					dictionary[idx] = null;

				foreach (string idx in KoiClothesOverlayMgr.SubClothesNames)
					dictionary[idx] = null;

				if (data == null || !data.ContainsKey(type))
					continue;

				foreach (KeyValuePair<string, ClothesTexData> pair in data[type])
					dictionary[pair.Key] = pair.Value;
			}
		}
	}
}
