//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// La classe Assert offre des facilités pour le debugging.
	/// </summary>
	public sealed class Assert
	{
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
	}
}
