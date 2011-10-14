//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationCustomerViewController : CreationViewController<CustomerEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreatePanelTitleTile ("Data.Customer", "Client à créer...");

				this.CreateUINewNaturalPersonButton (builder);
				this.CreateUINewLegalPersonButton (builder);

				builder.EndPanelTitleTile ();
			}
		}

		private void CreateUINewNaturalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButtonWithInitializer<CustomerEntity> (this, "Personne privée", "Crée un client de type personne privée", this.SetupNaturalPersonCustomer);
		}

		private void CreateUINewLegalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButtonWithInitializer<CustomerEntity> (this, "Entreprise", "Crée un client de type entreprise", this.SetupLegalPersonCustomer);
		}

		private void SetupNaturalPersonCustomer(BusinessContext context, CustomerEntity customer)
		{
			customer.MainRelation = context.CreateEntity<RelationEntity> ();
			customer.MainRelation.Person = context.CreateEntity<NaturalPersonEntity> ();
		}

		private void SetupLegalPersonCustomer(BusinessContext context, CustomerEntity customer)
		{
			customer.MainRelation = context.CreateEntity<RelationEntity> ();
			customer.MainRelation.Person = context.CreateEntity<LegalPersonEntity> ();
		}
	}
}
