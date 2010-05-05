//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public abstract class SummaryAbstractPersonViewController : EntityViewController
	{
		public SummaryAbstractPersonViewController(string name, Entities.AbstractPersonEntity entity)
			: base (name, entity)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.Entity != null);
			var person = this.Entity as Entities.AbstractPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			this.CreateUITiles (container, person);
		}


		protected abstract void CreatePersonUI(UIBuilder builder);



		private void CreateUITiles(Widget container, Entities.AbstractPersonEntity person)
		{
			//	Subtil système en 4 passes, pour essayer d'abord de ne mettre que des tuiles distinctes, puis de grouper petit à petit.
			//	En cas de manque de place, ou groupe d'abord les mails, puis les téléphones, puis finalement les adresses.
			if (!container.IsActualGeometryValid)
			{
				Common.Widgets.Layouts.LayoutContext.SyncArrange (container);
			}

			for (int pass = 2; pass < 4; pass++)
			{
				container.Children.Clear ();  // supprime les widgets générés à la passe précédente
				this.CreateUITiles (container, person, pass > 2, pass > 1, pass > 0);

				Common.Widgets.Layouts.LayoutContext.SyncMeasure (container);

				//	Calcule la hauteur des tuiles qu'on vient de générer.
				double currentHeight = 0;
				foreach (Widget widget in container.Children)
				{
					currentHeight += Epsitec.Common.Widgets.Layouts.LayoutMeasure.GetHeight (widget).Desired;
					currentHeight += widget.Margins.Height;
				}

				if (currentHeight <= container.ActualHeight)  // assez de place ?
				{
					break;
				}
			}
		}

		private void CreateUITiles(Widget container, Entities.AbstractPersonEntity person, bool groupMail, bool groupTelecom, bool groupUri)
		{
			UIBuilder builder = new UIBuilder (container);
			Widgets.GroupingTile group = null;
			int count;

			builder.CreateHeaderEditorTile ();

			//	Une première tuile pour l'identité de la personne, physique ou morale.
			this.CreatePersonUI (builder);

			//	Crée les tuiles pour les adresses postales.
			if (groupMail)
			{
				group = builder.CreateGroupingTile ("Data.Mail", "Adresse", false);
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.MailContactEntity)
				{
					var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, contact as Entities.MailContactEntity, groupMail);

					if (!groupMail)
					{
						group = builder.CreateGroupingTile ("Data.Mail", accessor.Title, false);
					}

					builder.CreateSummaryTile (group, accessor, true, ViewControllerMode.Edition);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupMail)
				{
					group = builder.CreateGroupingTile ("Data.Mail", "Adresse", false);
				}

				var emptyEntity = new Entities.MailContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, emptyEntity, false);
				builder.CreateSummaryTile (group, accessor, false, ViewControllerMode.Edition);
			}

			//	Crée les tuiles pour les numéros de téléphone.
			if (groupTelecom)
			{
				group = builder.CreateGroupingTile ("Data.Telecom", "Téléphone", false);
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.TelecomContactEntity)
				{
					var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, contact as Entities.TelecomContactEntity, groupTelecom);

					if (!groupTelecom)
					{
						group = builder.CreateGroupingTile ("Data.Telecom", accessor.Title, false);
					}

					builder.CreateSummaryTile (group, accessor, true, ViewControllerMode.Edition);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupTelecom)
				{
					group = builder.CreateGroupingTile ("Data.Telecom", "Téléphone", false);
				}

				var emptyEntity = new Entities.TelecomContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, emptyEntity, false);
				builder.CreateSummaryTile (group, accessor, false, ViewControllerMode.Edition);
			}

			//	Crée les tuiles pour les adresses mail.
			if (groupUri)
			{
				group = builder.CreateGroupingTile ("Data.Uri", "Mail", false);
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.UriContactEntity)
				{
					var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, contact as Entities.UriContactEntity, groupUri);

					if (!groupUri)
					{
						group = builder.CreateGroupingTile ("Data.Uri", accessor.Title, false);
					}

					builder.CreateSummaryTile (group, accessor, true, ViewControllerMode.Edition);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupUri)
				{
					group = builder.CreateGroupingTile ("Data.Uri", "Mail", false);
				}

				var emptyEntity = new Entities.UriContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, emptyEntity, false);
				builder.CreateSummaryTile (group, accessor, false, ViewControllerMode.Edition);
			}

			builder.CreateFooterEditorTile ();
		}
	}
}
