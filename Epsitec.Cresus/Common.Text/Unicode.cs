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
		
		public class Bits
		{
			public const ulong	CodeMask			= 0x001FFFFF;
			public const ulong	CombiningFlag		= 0x00200000;
			
			
			public static bool IsCombining(ulong value)
			{
				return (value & Bits.CombiningFlag) != 0;
			}
		}
		
		public const char		SurrogateLowMin		= (char) 0xD800;
		public const char		SurrogateLowMax		= (char) 0xDBFF;
		public const char		SurrogateHighMin	= (char) 0xDC00;
		public const char		SurrogateHighMax	= (char) 0xDFFF;
		
		public const char		SurrogateMin		= SurrogateLowMin;
		public const char		SurrogateMax		= SurrogateHighMax;
	}
}
