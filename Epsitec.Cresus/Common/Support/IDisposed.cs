//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>IDisposed</c> interface provides the <see cref="Disposed"/> event, used to
	/// notify the users that the object was disposed.
	/// </summary>
	public interface IDisposed : System.IDisposable
	{
		event EventHandler Disposed;
	}
}
