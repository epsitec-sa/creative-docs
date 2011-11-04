//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Factories;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	using InitializerAction		= System.Action<BusinessContext, AbstractEntity>;
	using EntityCreatorFunction	= System.Func<System.Func<BusinessContext, AbstractEntity>, System.Action<BusinessContext, AbstractEntity>, AbstractEntity>;

	public abstract class CreationViewController<T> : EntityViewController<T>, ICreationController
		where T : AbstractEntity, new ()
	{
		protected CreationViewController()
		{
			System.Diagnostics.Debug.Assert (this.Orchestrator != null);
			System.Diagnostics.Debug.Assert (this.IsDummyEntity || this.IsPartiallyCreatedEntity);
		}


		public bool								IsDummyEntity
		{
			get
			{
				return this.Orchestrator.Data.IsDummyEntity (this.Entity);
			}
		}

		public bool								IsPartiallyCreatedEntity
		{
			get
			{
				return this.Entity.IsEntityPartiallyCreated;
			}
		}

		#region ICreationController Members

		void ICreationController.RegisterDisposeAction(System.Action disposeAction)
		{
			this.disposeAction = disposeAction;
		}

		void ICreationController.RegisterEntityCreator(EntityCreatorFunction entityCreator)
		{
			this.entityCreator = entityCreator;
		}

		#endregion


		/// <summary>
		/// Creates the real entity based on the current (template or example) entity.
		/// </summary>
		/// <typeparam name="TDerived">The type of the entity.</typeparam>
		/// <param name="initializer">The entity initializer.</param>
		internal void CreateRealEntity<TDerived>(System.Action<BusinessContext, TDerived> initializer = null)
			where TDerived : T, new ()
		{
			var initAction = CreationViewController<T>.GetCompatibleInitializer (initializer);

			if (this.IsDummyEntity)
			{
				//	The entity is not yet the real one, so let the creator replace it
				//	with the real one:

				this.CreateEntityUsingEntityCreator<TDerived> (initAction);

				//	The simple fact of creating the real entity is sufficient to update
				//	the user interface; usually, the entity creator is tightly related
				//	to the BrowserViewController class. Creating an item will therefore
				//	update the selected item in the browser and trigger the UI updates.
			}
			else
			{
				//	The entity is already a real entity (usually in a not-yet-fully-created
				//	state) and the creator must finish its initialization:

				initAction (this.BusinessContext, this.Entity);

				//	Now, re-navigate to the current sub-view, using the freshly initialized
				//	entity, which should use the edition (or summary) controller instead.

				this.ReopenSubView ();
			}

			System.Diagnostics.Debug.Assert (this.IsDisposed);
		}

		

		private static InitializerAction GetCompatibleInitializer<TDerived>(System.Action<BusinessContext, TDerived> initializer)
			where TDerived : T, new ()
		{
			if (initializer == null)
			{
				return null;
			}
			else
			{
				return (businessContext, entity) => initializer (businessContext, (TDerived) entity);
			}
		}

		private T CreateEntityUsingEntityCreator<TDerived>(InitializerAction initializer)
			where TDerived : T, new ()
		{
			if (this.entityCreator == null)
			{
				throw new System.InvalidOperationException ("Cannot create entity in CreationViewController without an entity creator");
			}

			return this.entityCreator (context => context.CreateEntity<TDerived> (), initializer) as T;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.disposeAction != null)
				{
					this.disposeAction ();
				}
			}

			base.Dispose (disposing);
		}


		private System.Action					disposeAction;
		private EntityCreatorFunction			entityCreator;
	}
}