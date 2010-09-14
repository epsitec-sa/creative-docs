//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public static class EntityViewControllerFactory
	{
		public static EntityViewController Create(string name, Marshaler marshaler, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement = null)
		{
			var entity = marshaler.GetValue<AbstractEntity> ();
			int index  = marshaler.GetCollectionIndex ();
			string path;

			if (index < 0)
			{
				path = marshaler.GetGetterExpression ().ToString ();
			}
			else
			{
				path = string.Concat (marshaler.GetGetterExpression ().ToString (), "[", marshaler.GetCollectionIndex ().ToString (System.Globalization.CultureInfo.InvariantCulture), "]");
			}

			System.Diagnostics.Debug.WriteLine ("EntityViewController --> " + path);

			return EntityViewControllerFactory.Create (name, entity, mode, orchestrator, navigationPathElement: navigationPathElement);
		}

		public static EntityViewController Create(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, int controllerSubTypeId = -1, NavigationPathElement navigationPathElement = null)
		{
			if (entity == null)
			{
				return null;
			}

			var controller = EntityViewControllerResolver.Resolve (orchestrator, name, entity, mode, controllerSubTypeId, navigationPathElement);

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
	}
}
