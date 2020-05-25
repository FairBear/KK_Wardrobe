using KKABMX.Core;
using KoiClothesOverlayX;
using KoiSkinOverlayX;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KK_Wardrobe
{
	public abstract class CheckList : HashSet<CheckList>
	{
		public const BindingFlags BINDINGS =
			BindingFlags.IgnoreCase |
			BindingFlags.Public |
			BindingFlags.Instance;

		public static string[] _indices_Ignore = new string[]
		{
			"skipRangeCheck",
			"loadProductNo",
			"loadVersion",
			"BlockName",
			"version",
			"parameter",
			"status",
			"enableMakeup",
			"makeup",
			"coordinateName",
			"hideBraOpt",
			"hideShortsOpt"
		};
		public static Type[] _applicable = new Type[]
		{
			typeof(byte),
			typeof(short),
			typeof(int),
			typeof(long),
			typeof(float),
			typeof(bool),
			typeof(string),
			typeof(Color),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(BoneModifier),
			typeof(BoneModifierData),
			typeof(OverlayTexture),
			typeof(ClothesTexData)
		};

		public Type type;
		public string key;
		public bool _value = true; // Value specifically set for this node.
		public bool _subValue = true; // Value with respect to the parent node.
		public CheckList parent = null;

		public virtual string Key => key;

		public string LabelKey
		{
			get
			{
				string key = Key;

				if (key == null)
					return null;

				string _parent = parent?.key ?? string.Empty;

				if (Strings.checkList_Strings.ContainsKey(_parent))
				{
					Dictionary<string, string> dict = Strings.checkList_Strings[_parent];

					if (dict.ContainsKey(key))
						key = dict[key];
				}

				return key;
			}
		}

		public bool Value
		{
			get => _subValue;
			set => SubValue = _value = value;
		}

		public bool SubValue
		{
			set
			{
				bool flag = value && _value;

				if (parent != null)
					flag = flag && parent.Value;

				if (_subValue != flag)
				{
					_subValue = flag;

					foreach (CheckList child in this)
						child.SubValue = flag;
				}
			}
		}

		public delegate void Apply_Info(object info,
										object to,
										object _from,
										object _to,
										int rank,
										int index);
		
		public new void Add(CheckList item)
		{
			base.Add(item);

			item.parent = this;
			item.SubValue = item.Value;
		}

		public void Load(object obj, Dictionary<string, bool> data, string link)
		{
			string key = Key;

			if (data != null && !key.IsNullOrEmpty())
			{
				if (link.IsNullOrEmpty())
					link = key;
				else
					link += "." + key;

				if (data.ContainsKey(link))
					Value = data[link];
			}

			if (obj == null || _applicable.Contains(type))
				return;

			if (Load_Dictionary(obj, data, link))
				return;

			if (Load_Array(obj, data, link))
				return;

			Load_Generic(obj, data, link);
		}

		public bool Load_Dictionary(object obj, Dictionary<string, bool> data, string link)
		{
			if (!type.IsGenericType ||
				!type.GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>)) ||
				type.GetGenericArguments()[0] != typeof(string))
				return false;

			IDictionary dictionary = obj as IDictionary;

			foreach (object pair in dictionary)
			{
				Type _type = pair.GetType();
				string dictionaryKey = _type.GetProperty("Key").GetValue(pair, null) as string;
				object _value = _type.GetProperty("Value").GetValue(pair, null);

				string key;

				if (this is CheckList_Dictionary _dictionary)
					key = _dictionary.dictionaryKey;
				else
					key = this.key;

				Add(new CheckList_Dictionary(
					_value,
					type.GetGenericArguments()[1],
					key,
					dictionaryKey,
					data,
					link
				));
			}

			return true;
		}

		public bool Load_Array(object obj, Dictionary<string, bool> data, string link)
		{
			if (!type.IsArray)
				return false;

			Type _type = type.GetElementType();
			Array array = obj as Array;

			if (array.Rank > 1)
			{
				for (int y = 0; y < array.Rank; y++)
					for (int x = 0; x < array.GetLength(y); x++)
					{
						object _value = array.GetValue(y, x);

						if (_value != null)
							Add(new CheckList_Array(_value, _type, key, y, x, data, link));
					}

				return true;
			}

			for (int i = 0; i < array.Length; i++)
			{
				object _value = array.GetValue(i);

				if (_value != null)
					Add(new CheckList_Array(_value, _type, key, 0, i, data, link));
			}

			return true;
		}

		public void Load_Generic(object obj, Dictionary<string, bool> data, string link)
		{
			foreach (FieldInfo field in type.GetFields())
				Load_Generic(
					field.FieldType,
					field.GetValue(obj),
					field.Name,
					data,
					link
				);

			foreach (PropertyInfo property in type.GetProperties())
				if (property.GetSetMethod() != null)
					Load_Generic(
						property.PropertyType,
						property.GetValue(obj, null),
						property.Name,
						data,
						link
					);
		}

		public void Load_Generic(Type type,
								 object value,
								 string name,
								 Dictionary<string, bool> data,
								 string link)
		{
			if (value == null || _indices_Ignore.Contains(name))
				return;

			Add(new CheckList_Generic(value, type, name, data, link));
		}

		public byte[] ToBytes()
		{
			Dictionary<string, bool> data = new Dictionary<string, bool>();

			ToBytes_Internal(ref data);

			return LZ4MessagePackSerializer.Serialize(data);
		}

		public void ToBytes_Internal(ref Dictionary<string, bool> data, string link = "")
		{
			foreach (CheckList node in this)
			{
				string name = node.Key;

				string _link = link + (link.Length > 0 ? "." : "") + name;
				data[_link] = node._value;
				node.ToBytes_Internal(ref data, _link);
			}
		}

		public void Apply(object from, object to)
		{
			foreach (CheckList node in this)
			{
				if (!node.Value)
					continue;

				if (node is CheckList_Dictionary dictionary)
				{
					Apply_Dictionary(dictionary, from, to);
					continue;
				}

				if (node is CheckList_Array array)
				{
					Apply_Array(array, from, to);
					continue;
				}

				Apply_Generic(node, from, to);
			}
		}

		public void Apply_Dictionary(CheckList_Dictionary dictionary, object from, object to)
		{
			IDictionary dFrom = from as IDictionary;
			IDictionary dTo = to as IDictionary;

			if (!dFrom.Contains(dictionary.dictionaryKey) ||
				!dTo.Contains(dictionary.dictionaryKey))
				return;

			object _from = dFrom[dictionary.dictionaryKey];
			bool flag = _applicable.Contains(from.GetType().GetGenericArguments()[1]);

			if (flag || _from == null)
				dTo[dictionary.dictionaryKey] = _from;
			else
				dictionary.Apply(_from, dTo[dictionary.dictionaryKey]);
		}

		public void Apply_Array(CheckList_Array array, object from, object to)
		{
			Array aFrom = from as Array;
			Array aTo = to as Array;

			bool ranked = aFrom.Rank > 1;
			object _from;

			if (ranked)
				_from = aFrom.GetValue(array.rank, array.index);
			else
				_from = aFrom.GetValue(array.index);

			bool flag = _applicable.Contains(from.GetType().GetElementType());

			if (flag || _from == null)
			{
				if (ranked)
					aTo.SetValue(_from, array.rank, array.index);
				else
					aTo.SetValue(_from, array.index);
			}
			else
			{
				object _to;

				if (ranked)
					_to = aTo.GetValue(array.rank, array.index);
				else
					_to = aTo.GetValue(array.index);

				array.Apply(_from, _to);
			}
		}

		public void Apply_Generic(CheckList node, object from, object to)
		{
			bool flag = _applicable.Contains(node.type);
			object _info = null;
			object _from = null;
			object _to = null;
			Apply_Info act = null;

			FieldInfo field = type.GetField(node.key, BINDINGS);

			if (field != null)
			{
				_info = field;
				_from = field.GetValue(from);
				_to = field.GetValue(to);
				act = Apply_Generic_Field;
			}
			else
			{
				PropertyInfo property = type.GetProperty(node.key, BINDINGS);

				if (property != null)
				{
					_info = property;
					_from = property.GetValue(from, null);
					_to = property.GetValue(to, null);
					act = Apply_Generic_Property;
				}
			}

			if (flag || _from == null)
				act?.Invoke(_info, to, _from, _to, 0, 0);
			else
				node.Apply(_from, _to);
		}

		public void Apply_Generic_Field(object info,
										object to,
										object _from,
										object _to,
										int rank,
										int index)
		{
			FieldInfo field = info as FieldInfo;

			field.SetValue(to, _from);
		}

		public void Apply_Generic_Property(object info,
										   object to,
										   object _from,
										   object _to,
										   int rank,
										   int index)
		{
			(info as PropertyInfo).SetValue(to, _from, null);
		}

		public void Apply_Info_Array(object _from, object _to, int rank, int index)
		{
			Array arrayFrom = _from as Array;
			Array arrayTo = _to as Array;

			if (arrayFrom.Rank > 1)
				arrayTo.SetValue(arrayFrom.GetValue(rank, index), rank, index);
			else
				arrayTo.SetValue(arrayFrom.GetValue(index), index);
		}
	}
}
