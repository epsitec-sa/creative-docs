//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.PlugIns
{
	/// <summary>
	/// The <c>ICorePlugIn</c> interface must be implemented by every plug-in
	/// decorated with the <see cref="PlugInAttribute"/> attribute.
	/// </summary>
	public interface ICorePlugIn : System.IDisposable
	{
	}
}
