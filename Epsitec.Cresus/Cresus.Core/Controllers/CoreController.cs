//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>CoreController</c> class is the base class for every controller
	/// in the application. Note: a controller is responsible for the management
	/// of a specific piece of UI.
	/// </summary>
	public abstract class CoreController : IIsDisposed
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
		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		#region IIsDisposed Members

		/// <summary>
		/// Gets a value indicating whether this instance was disposed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance was disposed; otherwise, <c>false</c>.
		/// </value>
		public bool								IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		#endregion

		/// <summary>
		/// Gets the collection of sub-controllers (or an empty collection
		/// if there is no child controllers used by this controller).
		/// </summary>
		/// <returns>The collection of controllers.</returns>
		public abstract IEnumerable<CoreController> GetSubControllers();


		/// <summary>
		/// Gets all sub controllers, going down level by level (using a
		/// non-recursive breadth first descent algorithm).
		/// </summary>
		/// <returns>The collection of controllers.</returns>
		public IEnumerable<CoreController> GetAllSubControllers()
		{
			var controllers = new Queue<CoreController> ();
			controllers.Enqueue (this);

			while (controllers.Count > 0)
			{
				var item = controllers.Dequeue ();

				if (item != this)
				{
					yield return item;
				}

				controllers.EnqueueRange (item.GetSubControllers ());
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}
		}

		#endregion

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.OnDisposing ();
				this.Disposing = null;

				this.DisposeSubControllers ();
			}
		}

		private void DisposeSubControllers()
		{
			var controllers = this.GetSubControllers ().Where (controller => controller.IsDisposed == false).ToList ();
			controllers.ForEach (controller => controller.Dispose ());
		}
		
		private void OnDisposing()
		{
			var handler = this.Disposing;

			if (handler != null)
            {
				handler (this);
            }
		}


		public event EventHandler				Disposing;

		private readonly string					name;
		private bool							isDisposed;
	}
}
