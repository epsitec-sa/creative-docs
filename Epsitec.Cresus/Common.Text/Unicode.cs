//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Unicode encapsule certaines informations au sujet des
	/// caractères codés en UTF-32.
	/// </summary>
	public sealed class Unicode
	{
		private Unicode()
		{
		}
		
		
		#region Bits Class
		public class Bits
		{
			public const ulong	CodeMask		= 0x001FFFFF;
			public const ulong	CombiningFlag	= 0x00200000;
			public const ulong	ReorderingFlag	= 0x00400000;
			public const ulong	ReservedFlag	= 0x00800000;
			
			
			public static int  GetCode(ulong value)
			{
				return (int) (value & Bits.CodeMask);
			}
			
			public static void SetCode(ref ulong value, int code)
			{
				if ((code < 0) || (code > 0x0010FFFF))
				{
					throw new Unicode.IllegalCodeException ();
				}
				
				value = (value & Bits.CodeMask) | (uint) code;
			}
			
			
			public static bool GetCombiningFlag(ulong value)
			{
				return (value & Bits.CombiningFlag) != 0;
			}
			
			public static void SetCombiningFlag(ref ulong value, bool flag)
			{
				if (flag)
				{
					value = value | Bits.CombiningFlag;
				}
				else
				{
					value = value & ~Bits.CombiningFlag;
				}
			}
			
			
			public static bool GetReorderingFlag(ulong value)
			{
				return (value & Bits.ReorderingFlag) != 0;
			}
			
			public static void SetReorderingFlag(ref ulong value, bool flag)
			{
				if (flag)
				{
					value = value | Bits.ReorderingFlag;
				}
				else
				{
					value = value & ~Bits.ReorderingFlag;
				}
			}
		}
		#endregion
		
		#region IllegalCodeException Class
		public class IllegalCodeException : System.ApplicationException
		{
			public IllegalCodeException()
			{
			}
			
			public IllegalCodeException(string message) : base (message)
			{
			}
		}
		#endregion
		
		#region Surrogate Constants
		public const char	SurrogateLowMin		= (char) 0xD800;
		public const char	SurrogateLowMax		= (char) 0xDBFF;
		public const char	SurrogateHighMin	= (char) 0xDC00;
		public const char	SurrogateHighMax	= (char) 0xDFFF;
		
		public const char	SurrogateMin		= SurrogateLowMin;
		public const char	SurrogateMax		= SurrogateHighMax;
		#endregion
	}
}
