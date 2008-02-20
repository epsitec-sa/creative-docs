//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceBundleSaver</c> is a callback used by the resource accessors
	/// to save or delete a resource bundle.
	/// </summary>
	public delegate void ResourceBundleSaver(ResourceManager manager, ResourceBundle bundle, ResourceSetMode mode);
}
