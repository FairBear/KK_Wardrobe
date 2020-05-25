using KKABMX.Core;
using KKABMX.GUI;
using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public class KKABMXHelper
	{
		public Dictionary<string, Dictionary<string, Dictionary<string, BoneModifier>>> KKABMX;
		public Dictionary<string, Dictionary<string, Dictionary<string, BoneModifier>>>[] KKABMXCoordinate;

		public KKABMXHelper(ChaFileControl chaFileCtrl = null)
		{
			int count = Enum.GetValues(typeof(ChaFileDefine.CoordinateType)).Length;
			KKABMX = new Dictionary<string, Dictionary<string, Dictionary<string, BoneModifier>>>();
			KKABMXCoordinate = new Dictionary<string, Dictionary<string, Dictionary<string, BoneModifier>>>[count];
			List<BoneModifier> data = chaFileCtrl?.KKABMXData().BoneModifiers();

			for (int i = 0; i < count; i++)
				KKABMXCoordinate[i] =
					new Dictionary<string, Dictionary<string, Dictionary<string, BoneModifier>>>();

			foreach (BoneMeta meta in InterfaceData.BoneControls)
			{
				if (meta.UniquePerCoordinate)
					foreach (var dictionary in KKABMXCoordinate)
						Set(dictionary, meta, data);
				else
					Set(KKABMX, meta, data);
			}
		}

		public void Set(Dictionary<string, Dictionary<string, Dictionary<string, BoneModifier>>> dictionary,
						BoneMeta meta,
						List<BoneModifier> data = null)
		{
			bool flag0 = dictionary.TryGetValue(
					meta.Category.CategoryName,
					out Dictionary<string, Dictionary<string, BoneModifier>> category
				);

			if (!flag0)
				dictionary[meta.Category.CategoryName] = category =
					new Dictionary<string, Dictionary<string, BoneModifier>>();

			bool flag1 = category.TryGetValue(
				meta.Category.SubCategoryName,
				out Dictionary<string, BoneModifier> subcategory
			);

			if (!flag1)
				category[meta.Category.SubCategoryName] = subcategory =
					new Dictionary<string, BoneModifier>();

			if (!meta.BoneName.IsNullOrEmpty() && !subcategory.ContainsKey(meta.BoneName))
				subcategory[meta.BoneName] = null;

			if (!meta.RightBoneName.IsNullOrEmpty() && !subcategory.ContainsKey(meta.RightBoneName))
				subcategory[meta.RightBoneName] = null;

			if (data == null)
				return;

			List<BoneModifier> list = data.FindAll(v =>
				v.BoneName == meta.BoneName ||
				v.BoneName == meta.RightBoneName
			);

			if (list == null)
				return;

			foreach (BoneModifier modifier in list)
				subcategory[modifier.BoneName] = modifier;
		}
	}
}
