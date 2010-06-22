//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

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


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		protected abstract void CreateUI(TileContainer container);


		public static EntityViewController CreateEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator)
		{
			EntityViewController controller = EntityViewController.ResolveEntityViewController (name, entity, mode);

			if (controller == null)
			{
				return null;
			}

			controller.Orchestrator = orchestrator;

			return controller;
		}

		private static System.Type FindViewControllerType(System.Type entityType, ViewControllerMode mode)
		{
			var baseTypePrefix = mode == ViewControllerMode.Summary ? "Summary" : "Edition";
			var baseTypeName   = string.Concat (baseTypePrefix, "ViewController`1");

			//	Find all concrete classes which use either the generic SummaryViewController or the
			//	generic EditionViewController base classes, which match the entity type (usually,
			//	there should be exactly one such type).

			var types = from type in typeof (EntityViewController).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types.FirstOrDefault ();
		}

		private static EntityViewController ResolveEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode)
		{
			switch (mode)
			{
				case ViewControllerMode.None:
					return null;

				case ViewControllerMode.Summary:
				case ViewControllerMode.Edition:
					break;

				default:
					throw new System.NotSupportedException (string.Format ("ViewControllerMode.{0} not supported", mode));
			}

			var type = EntityViewController.FindViewControllerType (entity.GetType (), mode);

			if (type == null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot create controller {0} for entity of type {1} using ViewControllerMode.{2}", name, entity.GetType (), mode));
			}

			return System.Activator.CreateInstance (type, new object[] { name, entity }) as EntityViewController;
		}
	}


	public abstract class EntityViewController<T> : EntityViewController where T : AbstractEntity
	{
		protected EntityViewController(string name, T entity)
			: base (name)
		{
			this.entity = entity;
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

		public sealed override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.DataContext.Contains (this.Entity));
			this.CreateUI (container as TileContainer);
		}

		private readonly T entity;
	}
}
