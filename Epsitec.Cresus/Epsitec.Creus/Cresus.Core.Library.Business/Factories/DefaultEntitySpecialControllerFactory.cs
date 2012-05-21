//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>DefaultEntitySpecialControllerFactory&lt;T&gt;</c> class is the base class
	/// for all special controller factories.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	public abstract class DefaultEntitySpecialControllerFactory<T> : IEntitySpecialControllerFactory
			where T : AbstractEntity
	{
		#region IEntitySpecialControllerFactory Members

		public virtual bool CanRepresent(AbstractEntity entity, int mode)
		{
			return entity is T;
		}

		IEntitySpecialController IEntitySpecialControllerFactory.Create(TileContainer container, AbstractEntity entity, int mode)
		{
			var controller = this.Create (container, entity as T, mode);
			var disposable = controller as System.IDisposable;
			var updater    = controller as IWidgetUpdater;

			if (disposable != null)
			{
				//	Make sure we dispose the controller when the container gets deleted:

				container.Disposed += x => disposable.Dispose ();
			}

			if (updater != null)
			{
				//	Automatically update the controller when something changes in the
				//	business context :

				container.Add (updater);
			}

			return controller;
		}

		#endregion

		protected abstract IEntitySpecialController Create(TileContainer container, T entity, int mode);
	}
}
