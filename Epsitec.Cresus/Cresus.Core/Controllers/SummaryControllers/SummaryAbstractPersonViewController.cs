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
	public abstract class SummaryAbstractPersonViewController<T> : EntityViewController<T> where T : Entities.AbstractPersonEntity
	{
		public SummaryAbstractPersonViewController(string name, T entity)
			: base (name, entity)
		{
		}

		public override void CreateUI(Widget container)
		{
			var person = this.Entity;
			
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

				//	Calcule la hauteur des tuiles qu'on vient de générer.
				double currentHeight = UIBuilder.GetContentHeight (container);
				if (currentHeight <= container.ActualHeight)  // assez de place ?
				{
					break;
				}
			}
		}

		protected abstract void CreatePersonUI(UIBuilder builder);

		private void CreateUITiles(Widget container, Entities.AbstractPersonEntity person, bool groupMail, bool groupTelecom, bool groupUri)
		{
			UIBuilder builder = new UIBuilder (container, this);
			Widgets.GroupingTile group = null;
			int count;

			builder.CreateHeaderEditorTile ();

			//	Une première tuile pour l'identité de la personne, physique ou morale.
			this.CreatePersonUI (builder);

			//	Crée les tuiles pour les adresses postales.
			if (groupMail)
			{
				group = builder.CreateSummaryGroupingTile ("Data.Mail", "Adresse");
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.MailContactEntity)
				{
					var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, contact as Entities.MailContactEntity, groupMail)
					{
						EnableAddAndRemove = true,
						ViewControllerMode = ViewControllerMode.Edition
					};


					if (!groupMail)
					{
						group = builder.CreateSummaryGroupingTile ("Data.Mail", accessor.Title);
					}

					builder.CreateSummaryTile (group, accessor);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupMail)
				{
					group = builder.CreateSummaryGroupingTile ("Data.Mail", "Adresse");
				}

				var emptyEntity = new Entities.MailContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.MailContactAccessor (person.Contacts, emptyEntity, false)
				{
					ViewControllerMode = ViewControllerMode.Edition
				};

				builder.CreateSummaryTile (group, accessor);
			}

			//	Crée les tuiles pour les numéros de téléphone.
			if (groupTelecom)
			{
				group = builder.CreateSummaryGroupingTile ("Data.Telecom", "Téléphone");
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.TelecomContactEntity)
				{
					var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, contact as Entities.TelecomContactEntity, groupTelecom)
					{
						EnableAddAndRemove = true,
						ViewControllerMode = ViewControllerMode.Edition
					};

					if (!groupTelecom)
					{
						group = builder.CreateSummaryGroupingTile ("Data.Telecom", accessor.Title);
					}

					builder.CreateSummaryTile (group, accessor);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupTelecom)
				{
					group = builder.CreateSummaryGroupingTile ("Data.Telecom", "Téléphone");
				}

				var emptyEntity = new Entities.TelecomContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.TelecomContactAccessor (person.Contacts, emptyEntity, false)
				{
					ViewControllerMode = ViewControllerMode.Edition
				};

				builder.CreateSummaryTile (group, accessor);
			}

			//	Crée les tuiles pour les adresses mail.
			if (groupUri)
			{
				group = builder.CreateSummaryGroupingTile ("Data.Uri", "Mail");
			}

			count = 0;
			foreach (Entities.AbstractContactEntity contact in person.Contacts)
			{
				if (contact is Entities.UriContactEntity)
				{
					var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, contact as Entities.UriContactEntity, groupUri)
					{
						EnableAddAndRemove = true,
						ViewControllerMode = ViewControllerMode.Edition
					};


					if (!groupUri)
					{
						group = builder.CreateSummaryGroupingTile ("Data.Uri", accessor.Title);
					}

					builder.CreateSummaryTile (group, accessor);

					count++;
				}
			}

			if (count == 0)
			{
				if (!groupUri)
				{
					group = builder.CreateSummaryGroupingTile ("Data.Uri", "Mail");
				}

				var emptyEntity = new Entities.UriContactEntity ();
				person.Contacts.Add (emptyEntity);

				var accessor = new EntitiesAccessors.UriContactAccessor (person.Contacts, emptyEntity, false)
				{
					ViewControllerMode = ViewControllerMode.Edition
				};

				builder.CreateSummaryTile (group, accessor);
			}

			builder.CreateFooterEditorTile ();
		}
	}
}
