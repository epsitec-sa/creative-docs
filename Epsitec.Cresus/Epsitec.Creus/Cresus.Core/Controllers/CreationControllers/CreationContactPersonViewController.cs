//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationContactPersonViewController : CreationViewController<ContactPersonEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreatePanelTitleTile ("Data.ContactPerson", "Contact à créer...");

				this.CreateUINewNaturalPersonButton (builder);
				this.CreateUINewLegalPersonButton (builder);

				builder.EndPanelTitleTile ();
			}
		}

		private void CreateUINewNaturalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButtonWithInitializer<ContactPersonEntity> (this, "Personne privée", "Crée un contact de type personne privée", CreationContactPersonViewController.SetupNaturalPerson);
		}

		private void CreateUINewLegalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButtonWithInitializer<ContactPersonEntity> (this, "Entreprise", "Crée un contact de type entreprise", CreationContactPersonViewController.SetupLegalPerson);
		}

		private static void SetupNaturalPerson(BusinessContext context, ContactPersonEntity contact)
		{
			contact.Person = context.CreateEntity<NaturalPersonEntity> ();
		}

		private static void SetupLegalPerson(BusinessContext context, ContactPersonEntity contact)
		{
			contact.Person = context.CreateEntity<LegalPersonEntity> ();
		}
	}
}
