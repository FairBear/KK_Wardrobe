using ActionGame.Chara;
using ExtensibleSaveFormat;
using KKABMX.Core;
using KoiClothesOverlayX;
using KoiSkinOverlayX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KK_Wardrobe
{
	public class Girl
	{
		//public Heroine data;

		public Func<Base> _charaBase = null;
		public Func<ChaControl> _chaCtrl = null; // Overworld Data
		public Func<ChaFileControl> _charFile = null; // Save Data

		// Mod-Related Data
		public BoneController kkabmx;
		public KoiClothesOverlayController kcox;
		public KoiSkinOverlayController ksox;

		// Logic
		public bool _initialized = false;
		public bool _reloadPending = false;
		public int? _lastRedressLocation = null;

		public Base CharaBase => _charaBase?.Invoke();
		public ChaControl ChaCtrl => _chaCtrl?.Invoke();
		public ChaFileControl CharFile => _charFile?.Invoke();

		public Girl(Func<Base> charaBase, Func<ChaControl> chaCtrl, Func<ChaFileControl> charFile)
		{
			_charaBase = charaBase;
			_chaCtrl = chaCtrl;
			_charFile = charFile;

			// The actual girl's ChaFileControl is `data.charFile`.
			// This is only used as reference and SHOULD NOT be modified.
			// Use `data.chaCtrl.chaFile` for the overworld and H-scene model.
		}

		public bool Update()
		{
			Base charaBase = CharaBase;

			if (charaBase == null)
				return false;

			ChaControl chaCtrl = ChaCtrl;

			if (!chaCtrl.loadEnd)
				return true;

			if (!_lastRedressLocation.HasValue || _lastRedressLocation.Value != charaBase.mapNo)
				if ((!_initialized && KK_Wardrobe.ChangeOnStart.Value) ||
					Controller.locations.Contains(charaBase.mapNo))
					ChangeOutfit();
				else if (_lastRedressLocation.HasValue)
					_lastRedressLocation = null;

			// Only update the model when the girl is in the same location.
			if (_reloadPending && charaBase.mapNo == Controller.player.charaBase.mapNo)
			{
				_reloadPending = false;
				Reload();
			}

			return true;
		}

		public PluginData GetBones()
		{
			try
			{
				return ChaCtrl.GetComponent<BoneController>().Modifiers.KKABMXData();
			} catch { }

			return null;
		}

		public PluginData GetSkinOverlay()
		{
			try
			{
				PluginData data = Extensions.NewKSOXData;
				IEnumerable<KeyValuePair<TexType, OverlayTexture>> overlays =
					ChaCtrl.GetComponent<KoiSkinOverlayController>().Overlays;

				foreach (KeyValuePair<TexType, OverlayTexture> overlay in overlays)
					if (overlay.Value != null)
						data.data.Add(overlay.Key.ToString(), overlay.Value.Data);

				return data;
			} catch { }

			return null;
		}

		public PluginData GetClothesOverlay()
		{
			return ChaCtrl.chaFile.KCOXData();
		}

		public void Reload()
		{
			CharaBase.ChangeNowCoordinate(); // Reload clothes.
			ChaCtrl.Reload(); // Reload body.
		}

		public void ChangeOutfit(bool reset = false)
		{
			if (!_initialized)
			{
				ChaControl chaCtrl = ChaCtrl;

				kkabmx = chaCtrl.GetComponent<BoneController>();
				kcox = chaCtrl.GetComponent<KoiClothesOverlayController>();
				ksox = chaCtrl.GetComponent<KoiSkinOverlayController>();

				_initialized = true;
			}

			_lastRedressLocation = CharaBase.mapNo;
			ChangeOutfit_Internal(reset);
		}

		public void ChangeOutfit_Internal(bool reset)
		{
			// Reset all before changing outfits.
			ChaFileControl chaFileCtrl = ChaCtrl.chaFile;
			ChaFileControl charFile = CharFile;
			string fullname = chaFileCtrl.parameter.fullname;

			chaFileCtrl.CopyCoordinate(charFile.coordinate);
			chaFileCtrl.CopyCustom(charFile.custom);

			KKABMXHelper kkabmxHelper = new KKABMXHelper(chaFileCtrl);
			KCOXHelper kcoxHelper = new KCOXHelper(chaFileCtrl);
			KSOXHelper ksoxHelper = new KSOXHelper(chaFileCtrl);

			// Failing to roll will use original outfit instead.
			if (!reset && Controller.RollForOutfit(out List<Layer> layers, out List<Card> cards, fullname))
				for (int i = 0; i < layers.Count; i++)
				{
					Layer layer = layers[i];
					Card card = cards[i];
					layer.checkList.Apply(card.chaFileCtrl, chaFileCtrl);
					layer.checkList_KKABMX.Apply(card.KKABMX, kkabmxHelper);
					layer.checkList_KCOX.Apply(card.KCOX, kcoxHelper);
					layer.checkList_KSOX.Apply(card.KSOX, ksoxHelper);
				}

			ChangeOutfit_Internal_KKABMX(kkabmxHelper);
			ChangeOutfit_Internal_KCOX(kcoxHelper);
			ChangeOutfit_Internal_KSOX(ksoxHelper);

			_reloadPending = true;
		}

		public void ChangeOutfit_Internal_KKABMX(KKABMXHelper helper)
		{
			List<BoneModifier> data = new List<BoneModifier>();

			foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, BoneModifier>>> a in helper.KKABMX)
				foreach (KeyValuePair<string, Dictionary<string, BoneModifier>> b in a.Value)
					foreach (KeyValuePair<string, BoneModifier> c in b.Value)
						if (c.Value != null)
							data.Add(c.Value);

			for (int i = 0; i < helper.KKABMXCoordinate.Length; i++)
				foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, BoneModifier>>> a in helper.KKABMXCoordinate[i])
					foreach (KeyValuePair<string, Dictionary<string, BoneModifier>> b in a.Value)
						foreach (KeyValuePair<string, BoneModifier> c in b.Value)
							if (c.Value != null)
							{
								BoneModifier bone = data.FirstOrDefault(v => v.BoneName == c.Key);

								if (bone == null)
									data.Add(bone = new BoneModifier(c.Key));

								if (!bone.IsCoordinateSpecific())
									bone.MakeCoordinateSpecific();

								bone.CoordinateModifiers[i] = c.Value.CoordinateModifiers[i];
							}

			kkabmx.SetExtendedData(data.KKABMXData());
		}

		public void ChangeOutfit_Internal_KCOX(KCOXHelper helper)
		{
			Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>> data =
				new Dictionary<ChaFileDefine.CoordinateType, Dictionary<string, ClothesTexData>>();

			foreach (KeyValuePair<string, Dictionary<string, ClothesTexData>> a in helper.KCOX)
				try
				{
					ChaFileDefine.CoordinateType key = (ChaFileDefine.CoordinateType)Enum.Parse(
						typeof(ChaFileDefine.CoordinateType),
						a.Key,
						true
					);

					Dictionary<string, ClothesTexData> entry = data[key] =
						new Dictionary<string, ClothesTexData>();

					foreach (KeyValuePair<string, ClothesTexData> b in a.Value)
						if (b.Value != null)
							entry[b.Key] = b.Value;
				} catch { }

			kcox.SetExtendedData(data.KCOXData());
		}

		public void ChangeOutfit_Internal_KSOX(KSOXHelper helper)
		{
			Dictionary<TexType, OverlayTexture> data = new Dictionary<TexType, OverlayTexture>();

			foreach (KeyValuePair<string, OverlayTexture> a in helper.KSOX)
				try
				{
					TexType key = (TexType)Enum.Parse(typeof(TexType), a.Key, true);

					if (a.Value != null)
						data[key] = a.Value;
				} catch { }

			ksox.SetExtendedData(data.KSOXData());
		}
	}
}
