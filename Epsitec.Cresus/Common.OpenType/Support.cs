//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe Support offre des routines de base pour la manipulation de
	/// tables de fontes OpenType.
	/// </summary>
	public sealed class Support
	{
		public static uint ReadInt16(byte[] data, int offset)
		{
			return (uint) (data[offset+0] << 8) | (data[offset+1]);
		}
		
		public static uint ReadInt32(byte[] data, int offset)
		{
			return (uint) (data[offset+0] << 24) | (uint) (data[offset+1] << 16) | (uint) (data[offset+2] << 8) | (data[offset+3]);
		}
	}
}
