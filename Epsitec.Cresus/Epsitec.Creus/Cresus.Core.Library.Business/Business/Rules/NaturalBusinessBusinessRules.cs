//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Workflows;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class NaturalPersonBusinessRules : GenericBusinessRule<NaturalPersonEntity>
	{
		public override void ApplySetupRule(NaturalPersonEntity person)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();
			var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();

			var example = new LanguageEntity();
			example.IsoLanguageCode = "fr";

			var language = businessContext.DataContext.GetByExample<LanguageEntity> (example).FirstOrDefault();

			if (language != null)
			{
				person.PreferredLanguage = language;
			}
		}
	}
}
