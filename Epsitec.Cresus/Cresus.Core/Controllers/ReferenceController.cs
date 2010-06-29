//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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
		/// <param name="controllerCreatedCallback">The callback which will be invoked when the controller is created.</param>
		/// <param name="mode">The view controller mode.</param>
		public ReferenceController(System.Func<AbstractEntity> entityGetter = null, System.Action<ReferenceController, CoreViewController> controllerCreatedCallback = null, ViewControllerMode mode = ViewControllerMode.Summary)
		{
			this.entityGetter = entityGetter;
			this.controllerCreatedCallback = controllerCreatedCallback;
			this.mode         = mode;
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
				return this.mode;
			}
		}

		private readonly System.Func<AbstractEntity> entityGetter;
		private readonly System.Action<ReferenceController, CoreViewController> controllerCreatedCallback;
		private readonly ViewControllerMode mode;
	}
}
