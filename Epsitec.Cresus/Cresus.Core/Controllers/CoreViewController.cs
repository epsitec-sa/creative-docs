//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>CoreViewController</c> class is the base class for every view
	/// controller in the application.
	/// </summary>
	public abstract class CoreViewController : CoreController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreViewController"/> class.
		/// </summary>
		/// <param name="name">The name of the controller.</param>
		public CoreViewController(string name)
			: base (name)
		{
		}

		/// <summary>
		/// Creates the UI managed by this controller.
		/// </summary>
		/// <param name="container">The container.</param>
		public abstract void CreateUI(Widget container);
	}
}
