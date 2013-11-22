//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				.Title ("Cr�er une nouvelle extraction par groupe")
				.Field<string> ()
					.Title ("Nom")
				.End ()
				.Field<AiderGroupEntity> ()
					.Title ("Groupe de r�f�rence")
					.WithSpecialField<AiderGroupSpecialField<AiderGroupExtractionEntity>> ()
				.End ()
				.Field<GroupExtractionMatch> ()
					.Title ("Filtre pour l'extraction")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, AiderGroupEntity, GroupExtractionMatch, AiderGroupExtractionEntity> (this.Execute);
		}

		private AiderGroupExtractionEntity Execute(string name, AiderGroupEntity group, GroupExtractionMatch match)
		{
			if (string.IsNullOrEmpty (name))
			{
				Logic.BusinessRuleException ("Le nom est obligatoire.");
			}
			if (group.IsNull () && match != GroupExtractionMatch.Path)
			{
				Logic.BusinessRuleException ("Le groupe de r�f�rence est obligatoire.");
			}

			var extraction = this.BusinessContext.CreateAndRegisterEntity<AiderGroupExtractionEntity> ();

			extraction.Name        = name;
			extraction.SearchGroup = group;
			extraction.SearchPath  = group.IsNull () ? "" : group.Path;
			extraction.Match       = match;

			return extraction;
		}
	}
}
