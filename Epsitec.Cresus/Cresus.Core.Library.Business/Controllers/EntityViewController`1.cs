﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController<T> : EntityViewController
		where T : AbstractEntity, new ()
	{
		protected EntityViewController()
			: base (EntityViewControllerFactory.Default.ControllerName)
		{
			this.uiControllers = new List<EntityViewController> ();
			this.entity        = EntityViewControllerFactory.Default.Entity as T;

			System.Diagnostics.Debug.Assert (this.DataContext != null, "No DataContext");
			System.Diagnostics.Debug.Assert ((this.Orchestrator.Data.IsDummyEntity (this.entity)) || (this.DataContext.Contains (this.entity)), "Invalid entity");

			EntityNullReferenceVirtualizer.PatchNullReferences (this.entity);
		}

		public T Entity
		{
			get
			{
				return this.entity;
			}
		}

		public System.Func<T> EntityGetter
		{
			get
			{
				return () => this.entity;
			}
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			return this.uiControllers;
		}

        public sealed override void CreateUI(Widget container)
		{
			base.CreateUI (container);

			var context       = this.DataContext;
			var entity        = this.Entity;
			var tileContainer = container as TileContainer;
			
			System.Diagnostics.Debug.Assert (tileContainer != null);

			this.TileContainer = tileContainer;
			this.CreateUI ();
		}

		protected override void CreateUI()
		{
			var bridge = this.CreateBridgeAndBuildBricks ();

			if (bridge.ContainsBricks)
			{
				using (var data = TileContainerController.Setup (this))
				{
					bridge.CreateTileDataItems (data);

					this.uiControllers.ForEach (x => x.NotifyAboutToCreateUI ().CreateBridgeAndBuildBricks ().CreateTileDataItems (data));
				}
			}
		}

		public void AddUIController(EntityViewController controller)
		{
			this.uiControllers.Add (controller);
		}

		protected EntityViewController CreateEditionSubController<TField>(Expression<System.Func<T, TField>> entityGetter)
			where TField : AbstractEntity
		{
			var name = EntityInfo.GetFieldCaption (entityGetter).Name;
			var func = entityGetter.Compile ();

			return EntityViewControllerFactory.Create (this.Name + name, func (this.Entity), ViewControllerMode.Edition, this.Orchestrator);
		}

		internal sealed override Bridge CreateBridgeAndBuildBricks()
		{
			var bridge = new Bridge<T> (this);
			var wall   = bridge.CreateBrickWall ();

			this.CreateBricks (wall);

			return bridge;
		}

		protected virtual void CreateBricks(Bricks.BrickWall<T> wall)
		{
		}

		public sealed override AbstractEntity GetEntity()
		{
			return this.Entity;
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
			var businessContext = this.BusinessContext;

			System.Diagnostics.Debug.Assert (businessContext != null);
			System.Diagnostics.Debug.Assert (this.masterEntities == null);

			this.masterEntities = this.GetMasterEntities ().ToArray ();
			this.masterEntities.ForEach (x => businessContext.AddMasterEntity (x));

			base.AboutToCreateUI ();
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

		protected void UpdateEmptyEntityStatus(DataContext context, bool isEmpty)
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
