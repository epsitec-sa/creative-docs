//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class EnumExtensions
	{
#if DOTNET35
		public static bool HasFlag(this System.Enum value, System.Enum mask)
		{
			var enumValue = EnumExtensions.ToUInt64 (value);
			var enumMask  = EnumExtensions.ToUInt64 (mask);
			
			return (enumValue & enumMask) == enumMask;
		}
#endif

		public static T SetFlag<T>(this System.Enum value, T mask)
			where T : struct
		{
			var enumValue = EnumExtensions.ToUInt64 (value);
			var enumMask  = EnumExtensions.ToUInt64 (mask);

			return EnumExtensions.FromUInt64<T> (enumValue | enumMask);
		}

		public static T ClearFlag<T>(this System.Enum value, T mask)
			where T : struct
		{
			var enumValue = EnumExtensions.ToUInt64 (value);
			var enumMask  = EnumExtensions.ToUInt64 (mask);

			return EnumExtensions.FromUInt64<T> (enumValue & ~enumMask);
		}


		public static string GetQualifiedName(this System.Enum value)
		{
			var type = value.GetType ();
			var name = type.Name;
			var symbols = System.Enum.Format (type, value, "f").Split (',').Select (x => string.Concat (name, ".", x.TrimStart ())).ToArray ();

			return string.Join (" | ", symbols);
		}


		internal static ulong ToUInt64(object value)
		{
			//	TODO: optimize this code

			//	I guess we could do the same with this very simple IL, provided the value is
			//	represented using 32-bit (or less) :
			//
			//		ldarg.0
			//		ret
			//
			//	For 64-bit, maybe :
			//
			//		ldarg.0
			//		conv.i4
			//		ret
			
			var type = value.GetType ().GetEnumUnderlyingType ();

			if (type == typeof (int))
			{
				return (ulong) (int) (object) value;
			}
			if (type == typeof (uint))
			{
				return (ulong) (uint) (object) value;
			}
			if (type == typeof (long))
			{
				return (ulong) (long) (object) value;
			}
			if (type == typeof (ulong))
			{
				return (ulong) (object) value;
			}
			if (type == typeof (sbyte))
			{
				return (ulong) (sbyte) (object) value;
			}
			if (type == typeof (byte))
			{
				return (ulong) (byte) (object) value;
			}
			if (type == typeof (short))
			{
				return (ulong) (short) (object) value;
			}
			if (type == typeof (ushort))
			{
				return (ulong) (ushort) (object) value;
			}

			throw new System.NotSupportedException (string.Format ("Enum value {0} has unexpected underlying type {1}", value, type.FullName));
		}

		internal static T FromUInt64<T>(ulong value)
			where T : struct
		{
			var type = typeof (T).GetEnumUnderlyingType ();

			if (type == typeof (int))
			{
				return (T) (object) (int) value;
			}
			if (type == typeof (uint))
			{
				return (T) (object) (uint) value;
			}
			if (type == typeof (long))
			{
				return (T) (object) (long) value;
			}
			if (type == typeof (ulong))
			{
				return (T) (object) value;
			}
			if (type == typeof (sbyte))
			{
				return (T) (object) (sbyte) value;
			}
			if (type == typeof (byte))
			{
				return (T) (object) (byte) value;
			}
			if (type == typeof (short))
			{
				return (T) (object) (short) value;
			}
			if (type == typeof (ushort))
			{
				return (T) (object) (ushort) value;
			}

			throw new System.NotSupportedException (string.Format ("Enum type {0} has unexpected underlying type {1}", typeof (T).FullName, type.FullName));
		}
	}
}
