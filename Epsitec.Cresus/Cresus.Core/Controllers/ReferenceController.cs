//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
		/// <param name="entityMapper">The entity mapper.</param>
		/// <param name="creator">The entity creator.</param>
		/// <param name="mode">The view controller mode.</param>
		private ReferenceController(System.Func<AbstractEntity> entityGetter = null, System.Func<AbstractEntity, AbstractEntity> entityMapper = null, System.Func<DataContext, NewEntityReference> creator = null, ViewControllerMode mode = ViewControllerMode.Summary)
		{
			this.entityGetter       = entityGetter;
			this.entityMapper       = entityMapper;
			this.viewControllerMode = mode;
			this.creator            = creator;
		}

		public ReferenceController(System.Func<AbstractEntity> entityGetter)
			: this (entityGetter, null, null, ViewControllerMode.Summary)
		{
		}

		public ReferenceController(System.Func<AbstractEntity, AbstractEntity> entityMapper = null, System.Func<DataContext, NewEntityReference> creator = null, ViewControllerMode mode = ViewControllerMode.Summary)
			: this (null, entityMapper, creator, mode)
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="ReferenceController"/> class.
		/// </summary>
		public static ReferenceController Create<T1, T2, T3>(System.Func<T1> rootEntityGetter, Expression<System.Func<T1, T2>> rootToFieldMapper, System.Func<T1, T3> rootToFinalMapper,
			System.Func<DataContext, NewEntityReference> creator = null,
			ViewControllerMode mode = ViewControllerMode.Summary)
			where T1 : AbstractEntity
			where T2 : AbstractEntity
			where T3 : AbstractEntity
		{
			return new ReferenceController (rootEntityGetter, x => ReferenceController.Apply (x as T1, rootToFinalMapper), creator, mode);
		}



		private AbstractEntity Entity
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

		public NewEntityReference CreateNewValue(DataContext context)
		{
			if (this.creator == null)
			{
				return new NewEntityReference (null);
			}
			else
			{
				return this.creator (context);
			}
		}

		
		public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
		{
			var entity = ReferenceController.Apply (this.Entity, this.entityMapper);
			var mode   = this.Mode;
			var ctrl   = EntityViewController.CreateEntityViewController ("ReferenceViewController", entity, mode, orchestrator);

			return ctrl;
		}

		private static T2 Apply<T1, T2>(T1 value, System.Func<T1, T2> map1)
			where T1 : AbstractEntity
			where T2 : AbstractEntity
		{
			if (map1 == null)
			{
				return value as T2;
			}

			if (value.IsNull ())
			{
				return null;
			}

			return map1 (value);
		}

		
		private readonly System.Func<AbstractEntity> entityGetter;
		private readonly System.Func<AbstractEntity, AbstractEntity> entityMapper;
		private readonly ViewControllerMode viewControllerMode;
		private readonly System.Func<DataContext, NewEntityReference> creator;
	}
}
