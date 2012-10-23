//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Factories;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>ReferenceController</c> class manages how following a reference will
	/// behave in the data view.
	/// </summary>
	public class ReferenceController : ITileController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceController"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="entityGetter">The entity getter.</param>
		/// <param name="entityMapper">The entity mapper.</param>
		/// <param name="creator">The entity creator.</param>
		/// <param name="mode">The view controller mode.</param>
		/// <param name="viewControllerSubTypeId">The sub-type ID of the view controller.</param>
		public ReferenceController(
			string id,
			System.Func<AbstractEntity> entityGetter,
			System.Func<AbstractEntity, AbstractEntity> entityMapper = null,
			System.Func<DataContext, NewEntityReference> creator = null,
			ViewControllerMode mode = ViewControllerMode.Summary,
			int viewControllerSubTypeId = -1)
		{
			this.entityGetter			 = entityGetter ?? (() => null);
			this.entityMapper			 = entityMapper;
			this.viewControllerMode		 = mode;
			this.creator				 = creator;
			this.viewControllerSubTypeId = viewControllerSubTypeId;
			this.id                      = id;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReferenceController"/> class.
		/// </summary>
		/// <param name="entityGetter">The entity getter.</param>
		/// <param name="entityMapper">The entity mapper.</param>
		/// <param name="creator">The entity creator.</param>
		/// <param name="mode">The view controller mode.</param>
		/// <param name="viewControllerSubTypeId">The sub-type ID of the view controller.</param>
		public ReferenceController(
			Expression<System.Func<AbstractEntity>> entityGetterExpression,
			System.Func<AbstractEntity, AbstractEntity> entityMapper = null,
			System.Func<DataContext, NewEntityReference> creator = null,
			ViewControllerMode mode = ViewControllerMode.Summary,
			int viewControllerSubTypeId = -1,
			string id = null)
		{
			var entityGetter = entityGetterExpression == null ? null : entityGetterExpression.Compile ();
			var entitySource = entityGetterExpression == null ? null : entityGetterExpression.ToString ();

			this.entityGetter			 = entityGetter ?? (() => null);
			this.entityMapper			 = entityMapper;
			this.viewControllerMode		 = mode;
			this.creator				 = creator;
			this.viewControllerSubTypeId = viewControllerSubTypeId;
			this.id                      = id ?? entitySource;
		}


		/// <summary>
		/// Creates a new instance of the <see cref="ReferenceController"/> class.
		/// </summary>
		/// <typeparam name="T1">The type of the root entity.</typeparam>
		/// <typeparam name="T2">The type of the field entity.</typeparam>
		/// <typeparam name="T3">The type of the final entity.</typeparam>
		/// <param name="rootEntityGetter">The getter for the root entity.</param>
		/// <param name="rootToFieldMapper">The mapper used to reach the field entity (the one which is referenced) from the root entity.</param>
		/// <param name="rootToFinalMapper">The mapper used to reach the final entity (the one which will be displayed in the UI) from the root entity.</param>
		/// <param name="creator">The creator.</param>
		/// <param name="mode">The mode.</param>
		/// <param name="viewControllerSubTypeId">The sub-type ID of the view controller.</param>
		/// <returns></returns>
		public static ReferenceController Create<T1, T2, T3>(
			System.Func<T1> rootEntityGetter,
			Expression<System.Func<T1, T2>> rootToFieldMapper,
			System.Func<T1, T3> rootToFinalMapper,
			System.Func<DataContext, NewEntityReference> creator = null,
			ViewControllerMode mode = ViewControllerMode.Summary,
			int viewControllerSubTypeId = -1)
			where T1 : AbstractEntity
			where T2 : AbstractEntity
			where T3 : AbstractEntity
		{
			string id = rootToFieldMapper.ToString ();
			return new ReferenceController (id, rootEntityGetter, x => ReferenceController.Apply (x as T1, rootToFinalMapper), creator, mode, viewControllerSubTypeId);
		}


		public string Id
		{
			get
			{
				return this.id;
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

		public int ViewControllerSubTypeId
		{
			get
			{
				return this.viewControllerSubTypeId;
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

		public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController, NavigationPathElement navigationPathElement)
		{
			var entity = ReferenceController.Apply (this.entityGetter (), this.entityMapper);
			var mode   = this.Mode;
			var ctrl   = EntityViewControllerFactory.Create ("ReferenceViewController", entity, mode, orchestrator, parentController, navigationPathElement: navigationPathElement);

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
		private readonly int viewControllerSubTypeId;
		private readonly string id;
	}
}
