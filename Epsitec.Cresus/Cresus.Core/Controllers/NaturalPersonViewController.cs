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
	public class NaturalPersonViewController : EntityViewController
	{
		public NaturalPersonViewController(string name)
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
			var person = this.Entity as Entities.NaturalPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			if (this.Mode == ViewControllerMode.NaturalPersonEdition)
			{
				var naturalPerson = person as Entities.NaturalPersonEntity;
				this.CreateSummaryTile (this.Entity, ViewControllerMode.None, "Data.NaturalPerson", "Edition de la personne physique", EntitySummary.GetNaturalPersonSummary (naturalPerson));
			}
			else
			{
				//	Une première tuile pour l'identité de la personne.
				var naturalPerson = person as Entities.NaturalPersonEntity;
				this.CreateSummaryTile (this.Entity, ViewControllerMode.NaturalPersonEdition, "Data.NaturalPerson", "Personne physique", EntitySummary.GetNaturalPersonSummary (naturalPerson));

				//	Une tuile distincte par adresse postale.
				foreach (Entities.AbstractContactEntity contact in naturalPerson.Contacts)
				{
					if (contact is Entities.MailContactEntity)
					{
						var mailContact = contact as Entities.MailContactEntity;
						this.CreateSummaryTile (mailContact, ViewControllerMode.GenericEdition, "Data.Mail", EntitySummary.GetMailTitle (mailContact), EntitySummary.GetMailSummary (mailContact));
					}
				}

				//	Une tuile commune pour tous les numéros de téléphone.
				string telecomContent = EntitySummary.GetTelecomSummary (naturalPerson.Contacts);
				if (!string.IsNullOrEmpty (telecomContent))
				{
					this.CreateSummaryTile (this.Entity, ViewControllerMode.TelecomsEdition, "Data.Telecom", "Téléphones", telecomContent);
				}

				//	Une tuile commune pour toutes les adresses mail.
				string uriContent = EntitySummary.GetUriSummary (naturalPerson.Contacts);
				if (!string.IsNullOrEmpty (uriContent))
				{
					this.CreateSummaryTile (this.Entity, ViewControllerMode.UrisEdition, "Data.Uri", "Mails", uriContent);
				}
			}

			this.AdjustLastTile ();
		}
	}
}
