//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// La classe Assert offre des facilit�s pour le debugging.
	/// </summary>
	public sealed class Assert
	{
		public static void Fail()
		{
			throw new AssertFailedException ("Exceptional condition found.");
		}
		
		public static void Fail(string format, params object[] args)
		{
			throw new AssertFailedException (string.Format (format, args));
		}
		
		
		public static void IsInBounds(int value, int min, int max)
		{
			if ((value < min) || (value > max))
			{
				throw new AssertFailedException (string.Format ("Value {0} out of bounds [{1}..{2}].", value, min, max));
			}
		}
		
		public static void IsInBounds(uint value, uint min, uint max)
		{
			if ((value < min) || (value > max))
			{
				throw new AssertFailedException (string.Format ("Value {0} out of bounds [{1}..{2}].", value, min, max));
			}
		}
		
		public static void IsInBounds(ulong value, ulong min, ulong max)
		{
			if ((value < min) || (value > max))
			{
				throw new AssertFailedException (string.Format ("Value {0} out of bounds [{1}..{2}].", value, min, max));
			}
		}
		
		public static void IsInBounds(double value, double min, double max)
		{
			if ((value < min) || (value > max))
			{
				throw new AssertFailedException (string.Format ("Value {0} out of bounds [{1}..{2}].", value, min, max));
			}
		}
		
		
		public static void IsTrue(bool condition)
		{
			if (condition != true)
			{
				throw new AssertFailedException ("Condition not true.");
			}
		}
		
		public static void IsTrue(bool condition, string format, params object[] args)
		{
			if (condition != true)
			{
				throw new AssertFailedException (string.Format (format, args));
			}
		}
		
		public static void IsFalse(bool condition)
		{
			if (condition != false)
			{
				throw new AssertFailedException ("Condition not false.");
			}
		}
		
		public static void IsFalse(bool condition, string format, params object[] args)
		{
			if (condition != false)
			{
				throw new AssertFailedException (string.Format (format, args));
			}
		}
		
		
		public static void IsNull(object value)
		{
			if (value != null)
			{
				throw new AssertFailedException ("Value is not null.");
			}
		}
		
		public static void IsNull(object value, string format, params object[] args)
		{
			if (value != null)
			{
				throw new AssertFailedException (string.Format (format, args));
			}
		}
		
		public static void IsNotNull(object value)
		{
			if (value == null)
			{
				throw new AssertFailedException ("Value is null.");
			}
		}
		
		public static void IsNotNull(object value, string format, params object[] args)
		{
			if (value == null)
			{
				throw new AssertFailedException (string.Format (format, args));
			}
		}
	}
}
