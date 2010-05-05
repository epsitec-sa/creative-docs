//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>CoreController</c> class is the base class for every controller
	/// in the application. Note: a controller is responsible for the management
	/// of a specific piece of UI.
	/// </summary>
	public abstract class CoreController : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CoreController"/> class.
		/// </summary>
		/// <param name="name">The name of the controller.</param>
		public CoreController(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Gets the name of this controller.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the collection of sub-controllers (or an empty collection
		/// if there is no child controllers used by this controller).
		/// </summary>
		/// <returns>The collection of controllers.</returns>
		public abstract IEnumerable<CoreController> GetSubControllers();

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
		}

		readonly string name;
	}
}
