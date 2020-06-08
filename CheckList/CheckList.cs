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
		public string linkedKey;
		public bool _value = true; // Value specifically set for this node.
		public bool _subValue = true; // Value with respect to the parent node.
		public CheckList parent = null;

		public virtual string Key => key;

		public string LabelKey =>
			Strings.checkList_Strings.ContainsKey(linkedKey) ?
				Strings.checkList_Strings[linkedKey] :
				Key;

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

		public string GetLinkedKey(string link) =>
			link.IsNullOrEmpty() ? Key : $"{link}.{Key}";

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

		public void Load(object obj, Dictionary<string, bool> data)
		{
			if (data != null && linkedKey != null && data.ContainsKey(linkedKey))
				Value = data[linkedKey];


			if (obj == null || _applicable.Contains(type))
				return;

			if (Load_Dictionary(obj, data))
				return;

			if (Load_Array(obj, data))
				return;

			Load_Generic(obj, data);
		}

		public bool Load_Dictionary(object obj, Dictionary<string, bool> data)
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

				Add(new CheckList_Dictionary(
					_value,
					type.GetGenericArguments()[1],
					key,
					dictionaryKey,
					data,
					linkedKey
				));
			}

			return true;
		}

		public bool Load_Array(object obj, Dictionary<string, bool> data)
		{
			if (!type.IsArray)
				return false;

			Type _type = type.GetElementType();
			Array array = obj as Array;
			int[] indices = new int[array.Rank];
			int length = array.GetLength(0);

			while (indices[0] < length)
			{
				Add(new CheckList_Array(
					array.GetValue(indices),
					_type,
					key,
					indices,
					data,
					linkedKey
				));

				for (int i = array.Rank - 1; i >= 0; i--)
				{
					indices[i]++;

					if (indices[i] < array.GetLength(i))
						break;

					if (i > 0)
						indices[i] = 0;
				}
			}

			return true;
		}

		public void Load_Generic(object obj, Dictionary<string, bool> data)
		{
			foreach (FieldInfo field in type.GetFields())
				Load_Generic(
					field.FieldType,
					field.GetValue(obj),
					field.Name,
					data
				);

			foreach (PropertyInfo property in type.GetProperties())
				if (property.GetSetMethod() != null)
					Load_Generic(
						property.PropertyType,
						property.GetValue(obj, null),
						property.Name,
						data
					);
		}

		public void Load_Generic(Type type,
								 object value,
								 string name,
								 Dictionary<string, bool> data)
		{
			if (value == null || _indices_Ignore.Contains(name))
				return;

			Add(new CheckList_Generic(
				value,
				type,
				name,
				data,
				linkedKey
			));
		}

		public byte[] ToBytes()
		{
			Dictionary<string, bool> data = new Dictionary<string, bool>();

			ToBytes_Internal(ref data);

			return LZ4MessagePackSerializer.Serialize(data);
		}

		public void ToBytes_Internal(ref Dictionary<string, bool> data)
		{
			foreach (CheckList node in this)
			{
				data[node.linkedKey] = node._value;
				node.ToBytes_Internal(ref data);
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

			object _from;

			_from = aFrom.GetValue(array.indices);

			bool flag = _applicable.Contains(from.GetType().GetElementType());

			if (flag || _from == null)
				aTo.SetValue(_from, array.indices);
			else
				array.Apply(_from, aTo.GetValue(array.indices));
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
