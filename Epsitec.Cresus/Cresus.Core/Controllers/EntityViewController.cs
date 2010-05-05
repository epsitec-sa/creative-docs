//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

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
		}

		public T Entity
		{
			get
			{
				return this.entity;
			}
		}


		private void CreateEntity(Widgets.AbstractTile tile)
		{
			EntitiesAccessors.AbstractAccessor accessor = tile.EntitiesAccessor;
			AbstractEntity newEntity = accessor.Create ();
			CoreViewController controller = EntityViewController.CreateEntityViewController ("ViewController", newEntity, tile.ChildrenMode, this.Orchestrator);

			this.Orchestrator.RebuildView ();
			this.Orchestrator.ShowSubView (this, controller);
		}

		private void RemoveEntity(Widgets.AbstractTile tile)
		{
			EntitiesAccessors.AbstractAccessor accessor = tile.EntitiesAccessor;

			Common.Dialogs.DialogResult result = Common.Dialogs.MessageDialog.ShowQuestion (accessor.RemoveQuestion, tile.Window);

			if (result == Common.Dialogs.DialogResult.Yes)
			{
				accessor.Remove ();

				this.Orchestrator.RebuildView ();
			}
		}

		private void CloseTile()
		{
			this.Orchestrator.CloseView (this);
		}


		private void HandleTileCreateEntity(object sender)
		{
			//	Appelé lorsque le bouton "+" d'une tuile est cliqué.
			var tile = sender as Widgets.AbstractTile;
			this.CreateEntity (tile);
		}

		private void HandleTileRemoveEntity(object sender)
		{
			//	Appelé lorsque le bouton "-" d'une tuile est cliqué.
			var tile = sender as Widgets.AbstractTile;
			this.RemoveEntity (tile);
		}

		private void HandleCloseButtonClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton "Fermer" d'une tuile est cliqué.
			this.CloseTile ();
		}


		private readonly T entity;
	}
}
