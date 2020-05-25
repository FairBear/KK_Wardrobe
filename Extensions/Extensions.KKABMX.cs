using ExtensibleSaveFormat;
using KKABMX.Core;
using System.Collections.Generic;
using MessagePack;

namespace KK_Wardrobe
{
	public static partial class Extensions
	{
		public static string KKABMX_KEY = "boneData";
		public static PluginData NewPluginData => new PluginData { version = 2 };

		public static PluginData KKABMXData(this ChaFileControl ChaFile)
		{
			return ExtendedSave.GetExtendedDataById(ChaFile, KKABMX_Core.ExtDataGUID) ?? NewPluginData;
		}

		public static PluginData KKABMXData(this ChaFileCoordinate ChaCoordinate)
		{
			return ExtendedSave.GetExtendedDataById(ChaCoordinate, KKABMX_Core.ExtDataGUID) ?? NewPluginData;
		}

		public static PluginData KKABMXData(this List<BoneModifier> modifiers)
		{
			PluginData data = NewPluginData;
			data.data[KKABMX_KEY] = LZ4MessagePackSerializer.Serialize(modifiers);

			return data;
		}

		public static List<BoneModifier> BoneModifiers(this PluginData data)
		{
			try
			{
				return LZ4MessagePackSerializer.Deserialize<List<BoneModifier>>((byte[])data.data[KKABMX_KEY]);
			}
			catch { }

			return new List<BoneModifier>();
		}

		public static Dictionary<string, BoneModifierData> BoneModifierData(this PluginData data)
		{
			try
			{
				return LZ4MessagePackSerializer.Deserialize<Dictionary<string, BoneModifierData>>((byte[])data.data[KKABMX_KEY]);
			}
			catch { }

			return new Dictionary<string, BoneModifierData>();
		}
	}
}
