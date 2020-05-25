using KoiSkinOverlayX;
using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public class KSOXHelper
	{
		public Dictionary<string, OverlayTexture> KSOX;

		public KSOXHelper(ChaFileControl chaFileCtrl = null)
		{
			KSOX = new Dictionary<string, OverlayTexture>();

			TexType[] types = Enum.GetValues(typeof(TexType)) as TexType[];
			Dictionary<TexType, OverlayTexture> data = chaFileCtrl?.KSOXData().KSOXDictionary();

			foreach (TexType type in types)
			{
				if (type == TexType.Unknown)
					continue;

				string key = type.ToString();

				if (data != null && data.ContainsKey(type))
					KSOX[key] = data[type];
				else
					KSOX[key] = null;
			}
		}
	}
}
