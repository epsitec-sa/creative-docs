//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Controllers.CreationControllers;
using Epsitec.Aider.Controllers.SpecialFieldControllers;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderGroupExtractionViewController0 : BrickCreationViewController<AiderGroupExtractionEntity>
	{
		protected override void GetForm(ActionBrick<AiderGroupExtractionEntity, SimpleBrick<AiderGroupExtractionEntity>> action)
		{
			action
				.Title ("Créer une nouvelle extraction par groupe")
				.Field<string> ()
					.Title ("Nom (laisser vide pour une création automatique)")
				.End ()
				.Field<bool> ()
					.Title ("Ajouter le nom du groupe parent")
					.InitialValue(true)
				.End ()
				.Field<AiderGroupEntity> ()
					.Title ("Groupe de référence")
					.WithSpecialField<AiderGroupSpecialField<AiderGroupExtractionEntity>> ()
				.End ()
				.Field<GroupExtractionMatch> ()
					.Title ("Filtre pour l'extraction")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string,bool, AiderGroupEntity, GroupExtractionMatch, AiderGroupExtractionEntity> (this.Execute);
		}

		private AiderGroupExtractionEntity Execute(string name,bool suffixWithParentGroupName, AiderGroupEntity group, GroupExtractionMatch match)
		{
			if (group.IsNull () && match != GroupExtractionMatch.Path)
			{
				Logic.BusinessRuleException ("Le groupe de référence est obligatoire.");
			}

			var extraction = this.BusinessContext.CreateAndRegisterEntity<AiderGroupExtractionEntity> ();
			var buildName = "";
			if (string.IsNullOrEmpty (name))
			{
				buildName = group.Name;
			}
			else
			{
				buildName = name;
			}

			if (suffixWithParentGroupName)
			{
				buildName +=  ", " + group.Parent.Name;
			}

			extraction.Name		   = buildName;
			extraction.SearchGroup = group;
			extraction.SearchPath  = group.IsNull () ? "" : group.Path;
			extraction.Match       = match;

			return extraction;
		}
	}
}
