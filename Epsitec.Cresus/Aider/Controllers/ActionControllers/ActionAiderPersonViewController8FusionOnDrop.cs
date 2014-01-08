//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (8)]
	public sealed class ActionAiderPersonViewController8FusionOnDrop : TemplateActionViewController<AiderPersonEntity, AiderContactEntity>
	{
		public override bool IsEnabled
		{
			get
			{
				return true; // this.Entity.eCH_Person.DataSource == Enumerations.DataSource.Government;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.Text ("Fusionner les personnes");
		}

		private FormattedText GetText()
		{
			var p1 = this.Entity;
			var p2 = this.AdditionalEntity.Person;

			var p1Summary = TextFormatter.FormatText (p1.eCH_Person.PersonFirstNames, p1.eCH_Person.PersonOfficialName, "(~", p1.Age, "~), ", p1.MainContact.Address.GetDisplayAddress ());
			var p2Summary = TextFormatter.FormatText (p2.eCH_Person.PersonFirstNames, p2.eCH_Person.PersonOfficialName, "(~", p2.Age, "~), ", p2.MainContact.Address.GetDisplayAddress ());
			//classic case
			if (p1.IsGovernmentDefined && !p2.IsGovernmentDefined)
			{
				return TextFormatter.FormatText ("Fusion des données non-officelles de:\n",
				p2Summary,
				"\navec les données officielles de:\n",
				p1Summary,
				"\nRappel:\n",
				"Ce contact sera supprimé:\n",
				p2Summary,
				"\nCe contact sera conservé:\n",
				p1Summary
				);
			}
			else if (!p1.IsGovernmentDefined && p2.IsGovernmentDefined)
			{
				return TextFormatter.FormatText ("Fusion des données non-officelles de:\n",
				p1Summary,
				"\navec les données officielles de:\n",
				p2Summary,
				"\nRappel:\n",
				"Ce contact sera supprimé:\n",
				p1Summary,
				"\nCe contact sera conservé:\n",
				p2Summary
				);
			}
			else if (!p1.IsGovernmentDefined && !p2.IsGovernmentDefined)
			{
				return TextFormatter.FormatText ("Aucun des contacts n'appartient au registre ECh!\n",
												 "Fusion des données non-officelles de:\n",
				p1Summary,
				"\navec les données non-officielles de:\n",
				p2Summary,
				"\nRappel:\n",
				"Aucun des contacts n'appartient au registre ECh!\n",
				"Ce contact sera supprimé:\n",
				p2Summary,
				"\nCe contact sera conservé:\n",
				p1Summary
				);
			}
			else
			{
				return TextFormatter.FormatText ("Les deux contacts ont des données officielles.\n",
												 "Il s'agit probablement d'une erreur sur la personne.\n",
												 "La Fusion va echouer.");
			}

			
		}

		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<bool>(this.Execute);
		}

		private void Execute(bool doFusion)
		{
			if (doFusion)
			{
				var businessContext = this.BusinessContext;

				var p1	= this.Entity;
				var p2  = this.AdditionalEntity.Person;

				var contactKeyId = EntityBag.GetId (p2.MainContact);

				if (AiderPersonEntity.MergePersons (businessContext, p1, p2))
				{
					EntityBag.Remove (contactKeyId);
				}
			}
		}

		protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
		{
			form.Title("Fusion de données")
				.Text(this.GetText())
				.Field<bool>()
					.Title("Oui, je souhaite fusionner ces deux personnes")
				.End()
			.End();
		}
	}
}