﻿namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;

	public static class ArrayExtensions
	{
		/// <summary>Set all values in array to a specified <paramref name="value"/>.</summary>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Array to initialize.</param>
		/// <param name="value">Value to set.</param>
		public static void Initialize<T>(this T[] array, T value)
		{
			Verify.Argument.IsNotNull(array, "array");

			for(int i = 0; i < array.Length; ++i)
			{
				array[i] = value;
			}
		}

		/// <summary>Set all values in array to a result returned by specified <paramref name="getValue"/>.</summary>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Array to initialize.</param>
		/// <param name="getValue">Function which returns value for each array element.</param>
		public static void Initialize<T>(this T[] array, Func<int, T> getValue)
		{
			Verify.Argument.IsNotNull(array, "array");

			for(int i = 0; i < array.Length; ++i)
			{
				array[i] = getValue(i);
			}
		}

		/// <summary>Set all values in array to a result returned by specified <paramref name="getValue"/>.</summary>
		/// <typeparam name="T">Type of array elements.</typeparam>
		/// <param name="array">Array to initialize.</param>
		/// <param name="getValue">Function which returns value for each array element.</param>
		public static void Initialize<T>(this T[] array, Func<int, T, T> getValue)
		{
			Verify.Argument.IsNotNull(array, "array");

			for(int i = 0; i < array.Length; ++i)
			{
				array[i] = getValue(i, array[i]);
			}
		}

		public static bool TrueForAll<T>(this T[] array, Predicate<T> match)
		{
			return Array.TrueForAll<T>(array, match);
		}

		public static bool TrueForAll<T>(this T[] array, Func<int, T, bool> match)
		{
			Verify.Argument.IsNotNull(array, "array");
			Verify.Argument.IsNotNull(match, "match");

			for(int i = 0; i < array.Length; ++i)
			{
				if(!match(i, array[i])) return false;
			}
			return true;
		}

		public static ArraySegment<T> GetSegment<T>(this T[] array, int offset, int count)
		{
			return new ArraySegment<T>(array, offset, count);
		}

		public static T[] CloneSegment<T>(this T[] array, int offset, int count)
		{
			Verify.Argument.IsNotNull(array, "array");
			Verify.Argument.IsValidIndex(offset, array.Length, "offset");
			Verify.Argument.IsValidIndex(count, array.Length - offset + 1, "count");

			var res = new T[count];
			Array.Copy(array, offset, res, 0, count);
			return res;
		}

		public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.AsReadOnly<T>(array);
		}

		public static T Find<T>(this T[] array, Predicate<T> match)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.Find<T>(array, match);
		}

		public static T[] FindAll<T>(this T[] array, Predicate<T> match)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.FindAll<T>(array, match);
		}

		public static void ForEach<T>(this T[] array, Action<T> action)
		{
			Verify.Argument.IsNotNull(array, "array");

			Array.ForEach<T>(array, action);
		}

		public static void ForEach<T>(this T[] array, Action<int, T> action)
		{
			Verify.Argument.IsNotNull(array, "array");
			Verify.Argument.IsNotNull(action, "action");

			for(int i = 0; i < array.Length; ++i)
			{
				action(i, array[i]);
			}
		}

		public static bool Contains<T>(this T[] array, T value)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.IndexOf<T>(array, value) != -1;
		}

		public static TOutput[] Convert<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.ConvertAll<TInput, TOutput>(array, converter);
		}

		public static void Sort<T>(this T[] array)
		{
			Verify.Argument.IsNotNull(array, "array");

			Array.Sort<T>(array);
		}

		public static void Sort<T>(this T[] array, int index, int length)
		{
			Verify.Argument.IsNotNull(array, "array");

			Array.Sort<T>(array, index, length);
		}

		public static void Sort<T>(this T[] array, Comparison<T> comparison)
		{
			Verify.Argument.IsNotNull(array, "array");

			Array.Sort<T>(array, comparison);
		}

		public static void Sort<T>(this T[] array, IComparer<T> comparer)
		{
			Verify.Argument.IsNotNull(array, "array");

			Array.Sort<T>(array, comparer);
		}

		public static void Sort<T>(this T[] array, int index, int length, IComparer<T> comparer)
		{
			Verify.Argument.IsNotNull(array, "array");

			Array.Sort<T>(array, index, length, comparer);
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> match)
		{
			Verify.Argument.IsNotNull(array, "array");
			Verify.Argument.IsNotNull(match, "match");

			for(int i = 0; i < array.Length; ++i)
			{
				if(match(array[i]))
					return i;
			}
			return -1;
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> match, int offset)
		{
			Verify.Argument.IsNotNull(array, "array");
			Verify.Argument.IsNotNull(match, "match");
			Verify.Argument.IsValidIndex(offset, array.Length, "offset");

			for(int i = offset; i < array.Length; ++i)
			{
				if(match(array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> match, int offset, int count)
		{
			Verify.Argument.IsNotNull(array, "array");
			Verify.Argument.IsNotNull(match, "match");
			Verify.Argument.IsValidIndex(offset, array.Length, "offset");
			Verify.Argument.IsValidIndex(count, array.Length - offset + 1, "count");

			int end = offset + count;
			for(int i = offset; i < end; ++i)
			{
				if(match(array[i]))
					return i;
			}
			return -1;
		}

		public static int IndexOf<T>(this T[] array, T value)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.IndexOf<T>(array, value);
		}

		public static int IndexOf<T>(this T[] array, T value, int startIndex)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.IndexOf<T>(array, value, startIndex);
		}

		public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.IndexOf<T>(array, value, startIndex, count);
		}

		public static int LastIndexOf<T>(this T[] array, T value)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.LastIndexOf<T>(array, value);
		}

		public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.LastIndexOf<T>(array, value, startIndex);
		}

		public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
		{
			Verify.Argument.IsNotNull(array, "array");

			return Array.LastIndexOf<T>(array, value, count);
		}
	}
}
