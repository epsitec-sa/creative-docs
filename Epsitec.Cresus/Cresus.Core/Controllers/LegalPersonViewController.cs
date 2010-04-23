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

namespace Epsitec.Cresus.Core.Controllers
{
	public class LegalPersonViewController : EntityViewController
	{
		public LegalPersonViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var person = this.Entity as Entities.LegalPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			if (this.Mode == ViewControllerMode.LegalPersonEdition)
			{
				var legalPerson = person as Entities.LegalPersonEntity;
				this.CreateSummaryTile (this.Entity, ViewControllerMode.None, "Data.LegalPerson", "Personne morale", "[ <i>Ici prendra place l'édition de la personne morale</i> ]");
			}
			else
			{
				//	Une première tuile pour l'identité de la personne.
				var legalPerson = person as Entities.LegalPersonEntity;
				this.CreateSummaryTile (this.Entity, ViewControllerMode.LegalPersonEdition, "Data.LegalPerson", "Personne morale", EntitySummary.GetLegalPersonSummary (legalPerson));

				//	Une tuile distincte par adresse postale.
				foreach (Entities.AbstractContactEntity contact in legalPerson.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var mailContact = contact as Entities.MailContactEntity;
						this.CreateSummaryTile (mailContact, ViewControllerMode.GenericEdition, "Data.Mail", EntitySummary.GetMailTitle (mailContact), EntitySummary.GetMailSummary (mailContact));
					}
				}

				//	Une tuile commune pour tous les numéros de téléphone.
				string telecomContent = EntitySummary.GetTelecomSummary (legalPerson.Contacts);
				if (!string.IsNullOrEmpty (telecomContent))
				{
					this.CreateSummaryTile (this.Entity, ViewControllerMode.TelecomsEdition, "Data.Telecom", "Téléphones", telecomContent);
				}

				//	Une tuile commune pour toutes les adresses mail.
				string uriContent = EntitySummary.GetUriSummary (legalPerson.Contacts);
				if (!string.IsNullOrEmpty (uriContent))
				{
					this.CreateSummaryTile (this.Entity, ViewControllerMode.UrisEdition, "Data.Uri", "Mails", uriContent);
				}
			}

			this.AdjustLastTile ();
		}
	}
}
