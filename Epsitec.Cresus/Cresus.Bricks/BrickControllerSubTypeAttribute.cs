//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	/// <summary>
	/// The <c>ControllerSubTypeAttribute</c> is used to decorate special versions
	/// of some view controller implementations. This can be used to split the UI
	/// for the edition of large entities into several view controllers.
	/// </summary>
	public abstract class BrickControllerSubTypeAttribute : System.Attribute
	{
		/// <summary>
		/// Gets the id of the view controller.
		/// </summary>
		/// <value>The id of the view controller.</value>
		public abstract int Id
		{
			get;
		}
	}
}
