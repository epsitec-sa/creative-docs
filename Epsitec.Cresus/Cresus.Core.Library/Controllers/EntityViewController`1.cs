//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>EntityViewController</c> class is the base class for all view controllers
	/// (such as the summary view controller, edition view controller and creation view
	/// controller). The view controller is bound to an entity of type <c>T</c>.
	/// </summary>
	/// <typeparam name="T">The type of the entity.</typeparam>
	public abstract class EntityViewController<T> : EntityViewController
		where T : AbstractEntity, new ()
	{
		protected EntityViewController()
			: base (EntityViewControllerFactory.Default.ControllerName)
		{
			this.uiControllers = new List<EntityViewController> ();
			this.entity        = EntityViewControllerFactory.Default.Entity as T;

			System.Diagnostics.Debug.Assert (this.DataContext != null, "No DataContext");
			System.Diagnostics.Debug.Assert (this.CheckDummyEntity (), "Invalid entity");

			EntityNullReferenceVirtualizer.PatchNullReferences (this.entity);
		}

		
		public T								Entity
		{
			get
			{
				return this.entity;
			}
		}

		public System.Func<T>					EntityGetter
		{
			get
			{
				return () => this.entity;
			}
		}

		private bool CheckDummyEntity()
		{
			// Here we check that the entity is not a dummy entity, i.e. that it comes from our
			// DataContext. There are two exceptions.
			// - If it is registered as a dummy entity in the orchestrator, if we have an
			//   orchestrator.
			// - If the controller is an IBrickCreationViewController. That's because in this case,
			//   the job of the controller is precisely to create the entity, so there is no way we
			//   can possibly have an entity which is not dummy at this point.

			return this.DataContext.Contains (this.entity)
				|| this is BrickCreationViewController<T>
				|| (this.Orchestrator != null && this.Orchestrator.Data.IsDummyEntity (this.entity));
		}


		public sealed override AbstractEntity GetEntity()
		{
			return this.Entity;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			return this.uiControllers;
		}

		public sealed override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (container is TileContainer);

			this.TileContainer = container as TileContainer;
			base.CreateUI (container);

			using (new BridgeContext (this))
			{
				this.CreateUI ();
			}
		}

		
		internal void AddUIController(EntityViewController controller)
		{
			this.uiControllers.Add (controller);
			controller.CreateBridgeAndBuildBricks ();
		}
		
		internal sealed override Bridge CreateBridgeAndBuildBricks()
		{
			var bridge = BridgeContext.Instance.CreateBridge<T> (this);
			var wall   = bridge.CreateBrickWall ();

			this.CreateBricks (wall);

			return bridge;
		}

		public sealed override BrickWall BuildBrickWall()
		{
			return new BrickWall<T> ();
		}

		public sealed override void BuildBricks(BrickWall wall)
		{
			this.CreateBricks ((BrickWall<T>) wall);
		}

		
		protected override void CreateUI()
		{
		}

		protected virtual void CreateBricks(BrickWall<T> wall)
		{
		}


		protected internal Marshaler<T> CreateEntityMarshaler()
		{
			return Marshaler.Create (this.entity, x => x, null);
		}
		
		public Accessor<FormattedText> CreateAccessor(System.Func<T, FormattedText> formatter)
		{
			return Accessor.Create (this.EntityGetter, formatter);
		}

		protected CollectionAccessor CreateCollectionAccessor<T1, T2>(CollectionTemplate<T1> template, System.Func<T, IList<T2>> collectionResolver)
			where T1 : T2, new ()
			where T2 : AbstractEntity, new ()
		{
			return CollectionAccessor.Create (this.EntityGetter, collectionResolver, template);
		}

		protected CollectionAccessor CreateCollectionAccessor<T1>(CollectionTemplate<T1> template)
			where T1 : AbstractEntity, new ()
		{
			return CollectionAccessor.Create (this.EntityGetter, template);
		}

		protected override sealed void AboutToCreateUI()
		{
			//	This method might be called more than once. Don't do anything if it has already
			//	been called :

			if (this.masterEntities == null)
			{
				var businessContext = this.BusinessContext;

				System.Diagnostics.Debug.Assert (businessContext != null);

				this.masterEntities = this.GetMasterEntities ().ToArray ();
				this.masterEntities.ForEach (x => businessContext.AddMasterEntity (x));

				base.AboutToCreateUI ();
			}
		}
		
		protected override void AboutToCloseUI()
		{
			this.UpgradeEmptyEntity ();
			base.AboutToCloseUI ();

			var businessContext = this.BusinessContext;

			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (this.masterEntities != null);

			this.masterEntities.ForEach (x => businessContext.RemoveMasterEntity (x));
			this.masterEntities = null;
		}
		
		protected virtual IEnumerable<AbstractEntity> GetMasterEntities()
		{
			yield return this.Entity;
		}


		protected override sealed EntityStatus GetEditionStatus()
		{
			return this.Entity.GetEntityStatus ();
		}

	
		/// <summary>
		/// If the current entity was registered in the <see cref="DataContext"/> as an empty
		/// entity, upgrade it to a real entity if its content is valid.
		/// </summary>
		private void UpgradeEmptyEntity()
		{
			var data = this.BusinessContext.Data;
			var entity  = this.Entity;
			var context = data.DataContextPool.FindDataContext (entity);

			bool isEmpty = (this.GetEditionStatus () & EntityStatus.Empty) != 0;

			this.UpdateEmptyEntityStatus (context, isEmpty);
		}

		private void UpdateEmptyEntityStatus(DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			if ((context != null) &&
				(entity != null))
			{
				context.UpdateEmptyEntityStatus (entity, isEmpty);
			}
		}

		private readonly List<EntityViewController>	uiControllers;
		private T									entity;
		private AbstractEntity[]					masterEntities;
	}
}
