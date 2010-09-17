﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController<T> : EntityViewController
		where T : AbstractEntity, new ()
	{
		protected EntityViewController(string name, T entity)
			: base (name)
		{
			this.entity = entity;

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

		public Marshaler<T> EntityMarshaler
		{
			get
			{
				return Marshaler.Create (this.entity, x => x, null);
			}
		}

		public sealed override void CreateUI(Widget container)
		{
			var context       = this.DataContext;
			var entity        = this.Entity;
			var tileContainer = container as TileContainer;
			
			System.Diagnostics.Debug.Assert (tileContainer != null);

			this.TileContainer = tileContainer;
			this.CreateUI ();
		}


		public sealed override AbstractEntity GetEntity()
		{
			return this.Entity;
		}


		protected override void AboutToCloseUI()
		{
			this.UpgradeEmptyEntity ();
			base.AboutToCloseUI ();
		}


		/// <summary>
		/// If the current entity was registered in the <see cref="DataContext"/> as an empty
		/// entity, upgrade it to a real entity if its content is valid.
		/// </summary>
		private void UpgradeEmptyEntity()
		{
			var entity  = this.Entity;
			var context = DataContextPool.Instance.FindDataContext (entity);

			bool isEmpty = this.EditionStatus == EditionStatus.Empty;

			this.UpdateEmptyEntityStatus (context, isEmpty);
		}

		protected virtual void UpdateEmptyEntityStatus(DataContext context, bool isEmpty)
		{
			var entity = this.Entity;

			if ((context != null) &&
				(entity != null))
			{
				context.UpdateEmptyEntityStatus (entity, isEmpty);
			}
		}

		private T entity;
		private BusinessLogic.BusinessContext businessContext;
	}
}
