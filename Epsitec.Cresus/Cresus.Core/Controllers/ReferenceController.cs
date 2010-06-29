//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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
		public ReferenceController(System.Func<AbstractEntity> entityGetter = null, System.Func<DataContext, NewValue<AbstractEntity>> creator = null, ViewControllerMode mode = ViewControllerMode.Summary)
		{
			this.entityGetter       = entityGetter;
			this.viewControllerMode = mode;
			this.creator            = creator;
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

		public System.Func<DataContext, NewValue<AbstractEntity>> ValueCreator
		{
			get
			{
				return this.creator;
			}
		}		
		
		public EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
		{
			var entity = this.Entity;
			var mode   = this.Mode;
			var ctrl   = EntityViewController.CreateEntityViewController ("ReferenceViewController", entity, mode, orchestrator);

			return ctrl;
		}

		
		private readonly System.Func<AbstractEntity> entityGetter;
		private readonly ViewControllerMode viewControllerMode;
		private readonly System.Func<DataContext, NewValue<AbstractEntity>> creator;
	}
}
