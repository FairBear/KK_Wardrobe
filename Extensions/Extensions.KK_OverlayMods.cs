using ExtensibleSaveFormat;
using KoiClothesOverlayX;
using KoiSkinOverlayX;
using MessagePack;
using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public static partial class Extensions
	{
		public static string KCOX_KEY = "Overlays";
		public static PluginData NewKSOXData => new PluginData { version = 1 };
		public static PluginData NewKCOXData => new PluginData { version = 1 };

		public static PluginData KSOXData(this ChaFileControl ChaFile)
		{
			return ExtendedSave.GetExtendedDataById(ChaFile, KoiSkinOverlayMgr.GUID) ?? NewKSOXData;
		}

		public static PluginData KSOXData(this Dictionary<TexType, OverlayTexture> overlays)
		{
			PluginData data = NewKSOXData;

			foreach (KeyValuePair<TexType, OverlayTexture> pair in overlays)
				if (pair.Value != null)
					data.data[pair.Key.ToString()] = pair.Value.Data;

			return data;
		}

		public static PluginData KCOXData(this ChaFileControl ChaFile)
		{
			return ExtendedSave.GetExtendedDataById(ChaFile, KoiClothesOverlayMgr.GUID) ?? NewKCOXData;
		}

		public static PluginData KCOXData(this Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>> overlays)
		{
			PluginData data = NewKCOXData;

			data.data.Add(KCOX_KEY, MessagePackSerializer.Serialize(overlays));

			return data;
		}

		public static Dictionary<TexType, OverlayTexture> KSOXDictionary(this PluginData data)
		{
			Dictionary<TexType, OverlayTexture> overlays = new Dictionary<TexType, OverlayTexture>();

			foreach (TexType type in Enum.GetValues(typeof(TexType)) as TexType[])
			{
				if (type == TexType.Unknown)
					continue;

				string key = type.ToString();

				if (data.data.TryGetValue(type.ToString(), out object texData) &&
					texData is byte[] bytes &&
					bytes.Length > 0)
					overlays[type] = new OverlayTexture(bytes);
			}

			return overlays;
		}

		public static Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>> KCOXDictionary(this PluginData data)
		{
			try
			{
				return LZ4MessagePackSerializer
					.Deserialize<Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>>>
					((byte[])data.data[KCOX_KEY]);
			}
			catch { }

			return new Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>>();
		}
	}
}
