using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KK_Wardrobe
{
	public abstract class BaseWindow
	{
		public abstract string Title { get; }
		public abstract float Height { get; }
		public abstract float Width { get; }

		public bool visible = false;

		public int _id;
		public Rect _rect;

		public Rect Rect => new Rect(_rect.x, _rect.y, Width, Height);

		public BaseWindow()
		{
			_id = GetHashCode();
			InitRect();
		}

		public abstract void Draw(int id);

		public void InitRect()
		{
			_rect = new Rect(
				Screen.width / 2 - Width / 2,
				Screen.height / 2 - Height / 2,
				Width,
				Height
			);
		}

		public void Display()
		{
			if (visible)
				_rect = GUILayout.Window(_id, Rect, Draw, Title);
		}
	}
}
