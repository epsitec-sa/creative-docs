//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType.Platform
{
	/// <summary>
	/// Summary description for IFontHandle.
	/// </summary>
	public interface IFontHandle : System.IDisposable
	{
		System.IntPtr Handle { get; }
	}
}
