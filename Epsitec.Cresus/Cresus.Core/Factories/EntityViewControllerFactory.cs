//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
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
		/// <param name="controllerSubTypeId">The controller sub-type ID.</param>
		/// <param name="navigationPathElement">The navigation path element.</param>
		/// <returns>The <see cref="EntityViewController"/> if one could be found.</returns>
		/// <exception cref="System.InvalidOperationException">Throws <see cref="System.InvalidOperationException"/> if no controller could be found.</exception>
		public static EntityViewController Create(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, int controllerSubTypeId = -1, NavigationPathElement navigationPathElement = null)
		{
			if (entity == null)
			{
				return null;
			}

			EntityViewControllerFactory.defaults = new DefaultSettings
			{
				Orchestrator = orchestrator,
				Mode = mode,
				NavigationPathElement = navigationPathElement,
			};

			EntityViewController controller;

			try
			{
				controller = EntityViewControllerResolver.Resolve (orchestrator, name, entity, mode, controllerSubTypeId, navigationPathElement);
			}
			finally
			{
				EntityViewControllerFactory.defaults = null;
			}

			if ((controller == null) &&
				(mode == ViewControllerMode.Creation))
			{
				return EntityViewControllerFactory.Create (name, entity, ViewControllerMode.Summary, orchestrator, controllerSubTypeId, navigationPathElement);
			}
			else
			{
				return controller;
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

			public ViewControllerMode			Mode
			{
				get;
				set;
			}

			public NavigationPathElement		NavigationPathElement
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
