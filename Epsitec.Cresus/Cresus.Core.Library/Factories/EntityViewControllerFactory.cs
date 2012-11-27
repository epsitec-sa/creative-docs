//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	/// <summary>
	/// The <c>EntityViewControllerFactory</c> class creates <see cref="EntityViewController"/> instances
	/// based on the entity and <see cref="ViewControllerMode"/> specified by the caller.
	/// </summary>
	public static class EntityViewControllerFactory
	{
		/// <summary>
		/// Gets the default settings for the controller currently being
		/// created.
		/// </summary>
		/// <value>The default settings.</value>
		public static DefaultSettings Default
		{
			get
			{
				return EntityViewControllerFactory.defaults;
			}
		}


		/// <summary>
		/// Creates the specified controller.
		/// </summary>
		/// <param name="name">The controller name (for debugging purposes only).</param>
		/// <param name="entity">The entity to bind to.</param>
		/// <param name="mode">The controller mode.</param>
		/// <param name="orchestrator">The orchestrator.</param>
		/// <param name="parentController">The parent controller.</param>
		/// <param name="controllerSubTypeId">The controller sub-type ID.</param>
		/// <param name="navigationPathElement">The navigation path element.</param>
		/// <param name="resolutionMode">The resolution mode.</param>
		/// <returns>
		/// The <see cref="EntityViewController"/> if one could be found.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">Throws <see cref="System.InvalidOperationException"/> if no controller could be found.</exception>
		public static EntityViewController Create(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, CoreViewController parentController,
			int?                  controllerSubTypeId   = null,
			NavigationPathElement navigationPathElement = null,
			ResolutionMode        resolutionMode        = ResolutionMode.ThrowOnError)
		{
			if (entity.IsNull ())
			{
				return null;
			}

			var callerDefaults = EntityViewControllerFactory.defaults;

			try
			{
				EntityViewControllerFactory.defaults = new DefaultSettings
				{
					Orchestrator = orchestrator,
					Mode = mode,
					ControllerSubTypeId = controllerSubTypeId,
					NavigationPathElement = navigationPathElement,
					ParentController = parentController,
					ControllerName = name,
					Entity = entity,
				};

				return EntityViewControllerResolver.Resolve (entity.GetType (), mode, controllerSubTypeId, resolutionMode);
			}
			finally
			{
				//	Don't forget to clear the defaults before leaving:
				EntityViewControllerFactory.defaults = callerDefaults;
			}
		}


		#region DefaultSettings Class

		/// <summary>
		/// The <c>DefaultSettings</c> class is used to store the values of the
		/// <see cref="EntityViewControllerFactory.Default"/> property.
		/// </summary>
		public sealed class DefaultSettings
		{
			public DataViewOrchestrator			Orchestrator
			{
				get;
				set;
			}

			public CoreViewController			ParentController
			{
				get;
				set;
			}

			public ViewControllerMode			Mode
			{
				get;
				set;
			}

			public int?							ControllerSubTypeId
			{
				get;
				set;
			}

			public NavigationPathElement		NavigationPathElement
			{
				get;
				set;
			}

			public string						ControllerName
			{
				get;
				set;
			}

			public AbstractEntity				Entity
			{
				get;
				set;
			}
		}

		#endregion

		[System.ThreadStatic]
		private static DefaultSettings defaults;
	}
}
