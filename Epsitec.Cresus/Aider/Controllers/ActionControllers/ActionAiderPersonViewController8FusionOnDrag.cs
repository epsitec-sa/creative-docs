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

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (8)]
	public sealed class ActionAiderPersonViewController8FusionOnDrag : TemplateActionViewController<AiderPersonEntity, AiderContactEntity>
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

			return TextFormatter.FormatText ("Cette action fusionne les données des deux personnes.\n \n",
				"Le contact", p2.eCH_Person.PersonFirstNames, p2.eCH_Person.PersonOfficialName, "(~", p2.Age, "~)",
				"que vous venez de glisser sur", p1.eCH_Person.PersonFirstNames, p1.eCH_Person.PersonOfficialName, "(~", p1.Age, "~)",
				"sera supprimé.");
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

				var officialPerson = this.Entity;
				var otherContact   = this.AdditionalEntity;
				var otherPerson    = otherContact.Person;

				AiderPersonEntity.MergePersons (businessContext, officialPerson, otherPerson);
				AiderContactEntity.Delete (businessContext, otherContact);
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