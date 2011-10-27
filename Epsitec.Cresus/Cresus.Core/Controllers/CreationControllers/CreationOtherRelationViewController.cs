//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationOtherRelationViewController : CreationViewController<OtherRelationEntity>
	{
		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreatePanelTitleTile ("Data.OtherRelation", "Contact à créer...");

				this.CreateUINewNaturalPersonButton (builder);
				this.CreateUINewLegalPersonButton (builder);

				builder.EndPanelTitleTile ();
			}
		}

		private void CreateUINewNaturalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButtonWithInitializer<OtherRelationEntity> (this, "Personne privée", "Crée un contact de type personne privée", this.SetupNaturalPerson);
		}

		private void CreateUINewLegalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButtonWithInitializer<OtherRelationEntity> (this, "Entreprise", "Crée un contact de type entreprise", this.SetupLegalPerson);
		}

		private void SetupNaturalPerson(BusinessContext context, OtherRelationEntity otherRelation)
		{
			otherRelation.Person = context.CreateEntity<NaturalPersonEntity> ();
		}

		private void SetupLegalPerson(BusinessContext context, OtherRelationEntity otherRelation)
		{
			otherRelation.Person = context.CreateEntity<LegalPersonEntity> ();
		}
	}
}
