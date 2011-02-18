//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Platform
{
	/// <summary>
	/// The <c>IFileOperationWindow</c> interface is used to get access to the
	/// platform window handle, given a window. This interface is required as
	/// we cannot add a reference to <c>Epsitec.Common.Widgets</c>, yet we must
	/// be able to get data from it.
	/// </summary>
	public interface IFileOperationWindow
	{
		/// <summary>
		/// Gets the platform window handle.
		/// </summary>
		/// <returns>The platform window handle</returns>
		System.IntPtr GetPlatformHandle();
	}
}
