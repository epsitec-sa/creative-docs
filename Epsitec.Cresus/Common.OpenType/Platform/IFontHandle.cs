//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
