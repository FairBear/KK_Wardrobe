using System;
using System.Collections.Generic;

namespace KK_Wardrobe
{
	public static partial class Extensions
	{
		public static bool TryGetValue<T>(this Dictionary<string, object> dictionary, string key, out T value)
		{
			value = default;

			if (!dictionary.ContainsKey(key))
				return false;

			if (!(dictionary[key] is T _value))
				return false;

			value = _value;
			return true;
		}

		public static bool RollWeight<T>(this IEnumerable<T> list,
										 int maxWeight,
										 out T value,
										 Func<T, int> predicate)
		{
			if (maxWeight <= 0)
				goto EMPTY;

			int num = Controller.random.Next(maxWeight);
			int min = 0;

			foreach (T obj in list)
			{
				int weight = predicate(obj);

				if (weight == 0)
					continue;

				int max = min + weight;

				if (num >= min && num <= max)
				{
					value = obj;
					return true;
				}

				min = max;
			}

		EMPTY:
			value = default;
			return false;
		}
	}
}
