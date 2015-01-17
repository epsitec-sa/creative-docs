//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Rules
{
	[BusinessRule]
	internal class AiderGroupExtractionBusinessRules : GenericBusinessRule<AiderGroupExtractionEntity>
	{
		public override void ApplyValidateRule(AiderGroupExtractionEntity extraction)
		{
			if (string.IsNullOrEmpty (extraction.Name))
			{
				Logic.BusinessRuleException (extraction, "Le nom est obligatoire.");
			}

			if (extraction.SearchGroup.IsNull () && extraction.Match != Enumerations.GroupExtractionMatch.Path)
			{
				Logic.BusinessRuleException (extraction, "Le groupe de r�f�rence est obligatoire.");
			}

			if ((extraction.Match == Enumerations.GroupExtractionMatch.SameFunction) &&
				(extraction.SearchGroup.GroupDef.Classification != Enumerations.GroupClassification.Function))
			{
				Logic.BusinessRuleException (extraction, "Le groupe s�lectionn� ne correspond pas � une fonction.");
			}
		}

		public override void ApplyUpdateRule(AiderGroupExtractionEntity extraction)
		{
			if (extraction.Match != Enumerations.GroupExtractionMatch.Path)
			{
				extraction.SearchPath = extraction.SearchGroup.Path ?? "";
			}
		}
	}
}

