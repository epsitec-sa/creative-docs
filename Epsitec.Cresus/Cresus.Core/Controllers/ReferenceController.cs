//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>ReferenceController</c> class manages how following a reference will
	/// behave in the data view.
	/// </summary>
	public class ReferenceController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceController"/> class.
		/// </summary>
		/// <param name="entityGetter">The entity getter.</param>
		/// <param name="mode">The view controller mode.</param>
		public ReferenceController(System.Func<AbstractEntity> entityGetter = null, System.Func<AbstractEntity, AbstractEntity> entityMapper = null, System.Func<DataContext, NewEntityReference> creator = null, ViewControllerMode mode = ViewControllerMode.Summary)
		{
			this.entityGetter       = entityGetter;
			this.entityMapper       = entityMapper;
			this.viewControllerMode = mode;
			this.creator            = creator;
		}

		public static ReferenceController Create<T1, T2>(System.Func<T1> entityGetter, System.Func<T1, T2> entityMapper,
			System.Func<DataContext, NewEntityReference> creator = null,
			ViewControllerMode mode = ViewControllerMode.Summary)
			where T1 : AbstractEntity
			where T2 : AbstractEntity
		{
			System.Func<AbstractEntity, AbstractEntity> mapper = x => entityMapper (x as T1);
			System.Func<AbstractEntity> getter = () => entityGetter ();
			
			return new ReferenceController (entityGetter == null ? null : getter, entityMapper == null ? null : mapper, creator, mode);
		}


		public AbstractEntity Entity
		{
			get
			{
				return this.entityGetter == null ? null : this.entityGetter ();
			}
		}

		public ViewControllerMode Mode
		{
			get
			{
				return this.viewControllerMode;
			}
		}

		public bool HasCreator
		{
			get
			{
				return this.creator != null;
			}
		}

		public System.Func<DataContext, NewEntityReference> ValueCreator
		{
			get
			{
				return this.creator;
			}
		}

		public AbstractEntity Map(AbstractEntity entity)
		{
			if (this.entityMapper == null)
			{
				return entity;
			}

			if (entity.IsNull ())
			{
				return null;
			}

			return this.entityMapper (entity);
		}
		
		public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
		{
			var entity = this.Map (this.Entity);
			var mode   = this.Mode;
			var ctrl   = EntityViewController.CreateEntityViewController ("ReferenceViewController", entity, mode, orchestrator);

			return ctrl;
		}

		
		private readonly System.Func<AbstractEntity> entityGetter;
		private readonly System.Func<AbstractEntity, AbstractEntity> entityMapper;
		private readonly ViewControllerMode viewControllerMode;
		private readonly System.Func<DataContext, NewEntityReference> creator;
	}
}
