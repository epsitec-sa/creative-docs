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

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public abstract class AbstractSummaryPersonViewController : EntityViewController
	{
		public AbstractSummaryPersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
		}


		protected void CreateUITiles(Entities.AbstractPersonEntity person)
		{
			//	Subtil système en 4 passes, pour essayer d'abord de ne mettre que des tuiles distinctes, puis de grouper petit à petit.
			//	En cas de manque de place, ou groupe d'abord les mails, puis les téléphones, puis finalement les adresses.
			if (!this.container.IsActualGeometryValid)
			{
				Common.Widgets.Layouts.LayoutContext.SyncArrange (this.container);
			}

			for (int pass = 2; pass < 4; pass++)
			{
				this.container.Children.Clear ();  // supprime les widgets générés à la passe précédente
				this.CreateUITiles (person, pass > 2, pass > 1, pass > 0);

				Common.Widgets.Layouts.LayoutContext.SyncMeasure (this.container);

				//	Calcule la hauteur des tuiles qu'on vient de générer.
				double currentHeight = 0;
				foreach (Widget widget in this.container.Children)
				{
					currentHeight += Epsitec.Common.Widgets.Layouts.LayoutMeasure.GetHeight (widget).Desired;
					currentHeight += widget.Margins.Height;
				}

				if (currentHeight <= this.container.ActualHeight)  // assez de place ?
				{
					break;
				}
			}
		}

		private void CreateUITiles(Entities.AbstractPersonEntity person, bool groupMail, bool groupTelecom, bool groupUri)
		{
			Widgets.GroupingTile group = null;
			int count;

			this.CreateHeaderEditorTile ();

			//	Une première tuile pour l'identité de la personne, physique ou morale.
			this.CreatePersonUI ();

			//	Crée les tuiles pour les adresses postales.
			if (groupMail)
			{
				group = this.CreateGroupingTile (this.Container, "Data.Mail", "Adresse", false);
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.MailContactEntity)
				{
					var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, contact as Entities.MailContactEntity, groupMail);

					if (!groupMail)
					{
						group = this.CreateGroupingTile (this.Container, "Data.Mail", accessor.Title, false);
					}

					this.CreateSummaryTile (group, accessor, true, ViewControllerMode.GenericEdition);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupMail)
				{
					group = this.CreateGroupingTile (this.Container, "Data.Mail", "Adresse", false);
				}

				var emptyEntity = new Entities.MailContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, emptyEntity, false);
				this.CreateSummaryTile (group, accessor, false, ViewControllerMode.GenericEdition);
			}

			//	Crée les tuiles pour les numéros de téléphone.
			if (groupTelecom)
			{
				group = this.CreateGroupingTile (this.Container, "Data.Telecom", "Téléphone", false);
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.TelecomContactEntity)
				{
					var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, contact as Entities.TelecomContactEntity, groupTelecom);

					if (!groupTelecom)
					{
						group = this.CreateGroupingTile (this.Container, "Data.Telecom", accessor.Title, false);
					}

					this.CreateSummaryTile (group, accessor, true, ViewControllerMode.GenericEdition);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupTelecom)
				{
					group = this.CreateGroupingTile (this.Container, "Data.Telecom", "Téléphone", false);
				}

				var emptyEntity = new Entities.TelecomContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, emptyEntity, false);
				this.CreateSummaryTile (group, accessor, false, ViewControllerMode.GenericEdition);
			}

			//	Crée les tuiles pour les adresses mail.
			if (groupUri)
			{
				group = this.CreateGroupingTile (this.Container, "Data.Uri", "Mail", false);
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.UriContactEntity)
				{
					var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, contact as Entities.UriContactEntity, groupUri);

					if (!groupUri)
					{
						group = this.CreateGroupingTile (this.Container, "Data.Uri", accessor.Title, false);
					}

					this.CreateSummaryTile (group, accessor, true, ViewControllerMode.GenericEdition);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupUri)
				{
					group = this.CreateGroupingTile (this.Container, "Data.Uri", "Mail", false);
				}

				var emptyEntity = new Entities.UriContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, emptyEntity, false);
				this.CreateSummaryTile (group, accessor, false, ViewControllerMode.GenericEdition);
			}

			this.CreateFooterEditorTile ();
		}

		protected virtual void CreatePersonUI()
		{
		}
	}
}
