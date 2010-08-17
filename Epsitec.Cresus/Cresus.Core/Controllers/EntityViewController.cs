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
	public abstract class EntityViewController : CoreViewController
	{
		protected EntityViewController(string name)
			: base (name)
		{
		}



		/// <summary>
		/// Gets the edition status of the entity.
		/// </summary>
		/// <value>The edition status.</value>
		public EditionStatus EditionStatus
		{
			get
			{
				return this.GetEditionStatus ();
			}
		}

		public TileContainer TileContainer
		{
			get;
			protected set;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public abstract AbstractEntity GetEntity();

		protected virtual EditionStatus GetEditionStatus()
		{
			return EditionStatus.Unknown;
		}

		protected abstract void CreateUI();

		public T NotifyChildItemCreated<T>(T entity)
			where T : AbstractEntity
		{
			this.OnChildItemCreated (entity);
			return entity;
		}

		public void NotifyChildItemDeleted<T>(T entity)
			where T : AbstractEntity
		{
		}

		
		public static EntityViewController CreateEntityViewController(string name, Marshaler marshaler, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, NavigationPathElement navigationPathElement = null)
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

			return EntityViewController.CreateEntityViewController (name, entity, mode, orchestrator, navigationPathElement: navigationPathElement);
		}

		public static EntityViewController CreateEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator, int controllerSubTypeId = -1, NavigationPathElement navigationPathElement = null)
		{
			var controller = EntityViewController.ResolveEntityViewController (name, entity, mode, controllerSubTypeId);

			if (controller == null)
			{
				if (mode == ViewControllerMode.Creation)
				{
					return EntityViewController.CreateEntityViewController (name, entity, ViewControllerMode.Summary, orchestrator, controllerSubTypeId, navigationPathElement);
				}

				return null;
			}

			controller.ParentController = orchestrator.GetLeafViewController ();
			controller.Orchestrator = orchestrator;
			controller.Mode = mode;
			controller.NavigationPathElement = navigationPathElement;

			return controller;
		}


		protected virtual void OnChildItemCreated(AbstractEntity entity)
		{
		}

		private static string GetViewControllerPrefix(ViewControllerMode mode)
		{
			switch (mode)
			{
				case ViewControllerMode.Summary:
					return "Summary";
				case ViewControllerMode.Edition:
					return "Edition";
				case ViewControllerMode.Creation:
					return "Creation";

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}
		}

		private static System.Type FindViewControllerType(System.Type entityType, ViewControllerMode mode, int controllerSubTypeId)
		{
			var baseTypePrefix = EntityViewController.GetViewControllerPrefix (mode);
			var baseTypeName   = string.Concat (baseTypePrefix, "ViewController`1");

			//	Find all concrete classes which use either the generic SummaryViewController or the
			//	generic EditionViewController base classes, which match the entity type (usually,
			//	there should be exactly one such type).

			var types = from type in typeof (EntityViewController).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			if (controllerSubTypeId < 0)
			{
				types = types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Length == 0);
			}
			else
			{
				types = types.Where (type => type.GetCustomAttributes (typeof (ControllerSubTypeAttribute), false).Cast<ControllerSubTypeAttribute> ().Any (attribute => attribute.Id == controllerSubTypeId));
			}

			return types.FirstOrDefault ();
		}

		private static EntityViewController ResolveEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode, int controllerSubTypeId)
		{
			switch (mode)
			{
				case ViewControllerMode.None:
					return null;

				case ViewControllerMode.Summary:
				case ViewControllerMode.Edition:
				case ViewControllerMode.Creation:
					break;

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}

			var type = EntityViewController.FindViewControllerType (entity.GetType (), mode, controllerSubTypeId);

			if (type == null)
			{
				if (mode == ViewControllerMode.Creation)
				{
					return null;
				}

				throw new System.InvalidOperationException (string.Format ("Cannot create controller {0} for entity of type {1} using ViewControllerMode.{2}", name, entity.GetType (), mode));
			}

			return System.Activator.CreateInstance (type, new object[] { name, entity }) as EntityViewController;
		}
	}
}
