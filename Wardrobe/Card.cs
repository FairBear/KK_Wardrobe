using System.IO;
using UnityEngine;

namespace KK_Wardrobe
{
	public class Card
	{
		public string name;
		public int weight = 10;
		public CardType type = CardType.Unknown;
		public ChaFileControl chaFileCtrl;
		public KKABMXHelper _kkabmx;
		public KCOXHelper _kcox;
		public KSOXHelper _ksox;
		public byte[] pngData;
		public byte[] saveData;
		public Texture _image;

		public Texture Image
		{
			get
			{
				if (_image == null)
					_image = PngAssist.ChangeTextureFromByte(
						pngData,
						ManagerWindow.IMAGE_WIDTH,
						ManagerWindow.IMAGE_HEIGHT
					);

				return _image;
			}
		}

		public KKABMXHelper KKABMX
		{
			get
			{
				if (_kkabmx == null)
					_kkabmx = new KKABMXHelper(chaFileCtrl);

				return _kkabmx;
			}
		}

		public KCOXHelper KCOX
		{
			get
			{
				if (_kcox == null)
					_kcox = new KCOXHelper(chaFileCtrl);

				return _kcox;
			}
		}

		public KSOXHelper KSOX
		{
			get
			{
				if (_ksox == null)
					_ksox = new KSOXHelper(chaFileCtrl);

				return _ksox;
			}
		}

		public Card(string path)
		{
			CardType type = Util.LoadFile(
				path,
				out byte[] saveData,
				out ChaFileControl chaFileCtrl,
				out byte[] pngData
			);

			name = Path.GetFileName(path);

			if (type == CardType.Unknown)
				return;

			this.type = type;
			this.chaFileCtrl = chaFileCtrl;
			this.pngData = pngData;
			this.saveData = saveData;
		}
	}
}
