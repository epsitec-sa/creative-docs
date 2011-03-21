//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.CreationControllers
{
	public class CreationRelationViewController : CreationViewController<RelationEntity>
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
			builder.CreateCreationButton<RelationEntity> (this, "Personne privée", "Crée un client de type personne privée", this.SetupNaturalPersonRelation);
		}

		private void CreateUINewLegalPersonButton(UIBuilder builder)
		{
			builder.CreateCreationButton<RelationEntity> (this, "Entreprise", "Crée un client de type entreprise", this.SetupLegalPersonRelation);
		}

		private void SetupNaturalPersonRelation(BusinessContext context, RelationEntity relation)
		{
			relation.Person = context.CreateEntity<NaturalPersonEntity> ();
		}

		private void SetupLegalPersonRelation(BusinessContext context, RelationEntity relation)
		{
			relation.Person = context.CreateEntity<LegalPersonEntity> ();
		}
	}
}
