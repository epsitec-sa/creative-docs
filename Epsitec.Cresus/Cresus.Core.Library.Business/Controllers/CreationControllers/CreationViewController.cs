//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
//			System.Diagnostics.Debug.Assert (this.Orchestrator.Data.IsDummyEntity (this.Entity));
		}


		public bool IsDummyEntity
		{
			get
			{
				return this.Orchestrator.Data.IsDummyEntity (this.Entity);
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

		
		internal void CreateRealEntity(System.Action<BusinessContext, T> initializer = null)
		{
			var initAction = CreationViewController<T>.GetCompatibleInitializer (initializer);
			
			this.CreateEntityUsingEntityCreator<T> (initAction);

			//	The simple fact of creating the real entity is sufficient to update
			//	the user interface; usually, the entity creator is tightly related
			//	to the BrowserViewController class. Creating an item will therefore
			//	update the selected item in the browser and trigger the UI updates.

			System.Diagnostics.Debug.Assert (this.IsDisposed);
		}

		internal void CreateRealEntity<TDerived>(System.Action<BusinessContext, TDerived> initializer = null)
			where TDerived : T, new ()
		{
			var initAction = CreationViewController<TDerived>.GetCompatibleInitializer (initializer);

			this.CreateEntityUsingEntityCreator<TDerived> (initAction);

			//	The simple fact of creating the real entity is sufficient to update
			//	the user interface; usually, the entity creator is tightly related
			//	to the BrowserViewController class. Creating an item will therefore
			//	update the selected item in the browser and trigger the UI updates.

			System.Diagnostics.Debug.Assert (this.IsDisposed);
		}

		private static InitializerAction GetCompatibleInitializer(System.Action<BusinessContext, T> initializer)
		{
			if (initializer == null)
			{
				return null;
			}
			else
			{
				return
					delegate (BusinessContext businessContext, AbstractEntity entity)
					{
						initializer (businessContext, (T) entity);
					};
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