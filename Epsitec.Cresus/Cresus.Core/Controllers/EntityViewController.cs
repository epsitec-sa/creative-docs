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

		public sealed override void CreateUI(Widget container)
		{
			this.CreateUI (container as TileContainer);
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

		private static EntityViewController ResolveEntityViewController(string name, AbstractEntity entity, ViewControllerMode mode)
		{
			if (mode == ViewControllerMode.None)
			{
				return null;
			}

			if (entity is Entities.NaturalPersonEntity)
			{
				if (mode == ViewControllerMode.Summary)
				{
					return new SummaryNaturalPersonViewController (name, entity as Entities.NaturalPersonEntity);
				}
				else
				{
					return new EditionNaturalPersonViewController (name, entity as Entities.NaturalPersonEntity);
				}
			}

			if (entity is Entities.LegalPersonEntity)
			{
				if (mode == ViewControllerMode.Summary)
				{
					return new SummaryLegalPersonViewController (name, entity as Entities.LegalPersonEntity);
				}
				else
				{
					return new EditionLegalPersonViewController (name, entity as Entities.LegalPersonEntity);
				}
			}

			if (entity is Entities.PersonTitleEntity)
			{
				if (mode == ViewControllerMode.Summary)
				{
					return new SummaryTitleViewController (name, entity as Entities.PersonTitleEntity);
				}
				else
				{
					return new EditionTitleViewController (name, entity as Entities.PersonTitleEntity);
				}
			}

			if (entity is Entities.CountryEntity)
			{
				if (mode == ViewControllerMode.Summary)
				{
					return new SummaryCountryViewController (name, entity as Entities.CountryEntity);
				}
				else
				{
					return new EditionCountryViewController (name, entity as Entities.CountryEntity);
				}
			}

			if (entity is Entities.LocationEntity)
			{
				if (mode == ViewControllerMode.Summary)
				{
					return new SummaryLocationViewController (name, entity as Entities.LocationEntity);
				}
				else
				{
					return new EditionLocationViewController (name, entity as Entities.LocationEntity);
				}
			}

			//	Doit être avant les tests sur MailContactEntity, TelecomContactEntity et UriContactEntity !
			if (entity is Entities.AbstractContactEntity && mode == ViewControllerMode.RolesEdition)
			{
				return new EditionRolesContactViewController (name, entity as Entities.AbstractContactEntity);
			}

			if (entity is Entities.TelecomContactEntity && mode == ViewControllerMode.TelecomTypeEdition)
			{
				return new EditionTelecomTypeViewController (name, entity as Entities.TelecomContactEntity);
			}

			if (entity is Entities.UriContactEntity && mode == ViewControllerMode.UriSchemeEdition)
			{
				return new EditionUriSchemeViewController (name, entity as Entities.UriContactEntity);
			}

			//	Après...
			if (entity is Entities.MailContactEntity)
			{
				return new EditionMailContactViewController (name, entity as Entities.MailContactEntity);
			}

			if (entity is Entities.TelecomContactEntity)
			{
				return new EditionTelecomContactViewController (name, entity as Entities.TelecomContactEntity);
			}

			if (entity is Entities.UriContactEntity)
			{
				return new EditionUriContactViewController (name, entity as Entities.UriContactEntity);
			}

			// TODO: Compléter ici au fur et à mesure des besoins...

			return null;
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

#if false
		private void CreateEntity(GenericTile tile)
		{
			var accessor = tile.EntitiesAccessor;
			var newEntity = accessor.Create ();
			CoreViewController controller = EntityViewController.CreateEntityViewController ("ViewController", newEntity, tile.ChildrenMode, this.Orchestrator);

			this.Orchestrator.RebuildView ();
			this.Orchestrator.ShowSubView (this, controller);
		}

		private void RemoveEntity(GenericTile tile)
		{
			var accessor = tile.EntitiesAccessor;

			Common.Dialogs.DialogResult result = Common.Dialogs.MessageDialog.ShowQuestion (accessor.RemoveQuestion, tile.Window);

			if (result == Common.Dialogs.DialogResult.Yes)
			{
				accessor.Remove ();

				this.Orchestrator.RebuildView ();
			}
		}
#endif

		private void HandleTileCreateEntity(object sender)
		{
			//	Appelé lorsque le bouton "+" d'une tuile est cliqué.
			var tile = sender as GenericTile;
//-			this.CreateEntity (tile);
		}

		private void HandleTileRemoveEntity(object sender)
		{
			//	Appelé lorsque le bouton "-" d'une tuile est cliqué.
			var tile = sender as GenericTile;
//-			this.RemoveEntity (tile);
		}


		private readonly T entity;
	}
}
