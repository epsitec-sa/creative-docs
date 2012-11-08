//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class NaturalPersonBusinessRules : GenericBusinessRule<NaturalPersonEntity>
	{
		public override void ApplySetupRule(NaturalPersonEntity person)
		{
			var businessContext = this.GetBusinessContext ();

			var example = new LanguageEntity();
			example.IsoLanguageCode = "fr";

			var language = businessContext.DataContext.GetByExample<LanguageEntity> (example).FirstOrDefault();

			if (language != null)
			{
				person.PreferredLanguage = language;
			}
		}

		public override void ApplyUpdateRule(NaturalPersonEntity entity)
		{
			entity.DisplayName1 = StringExtensions.JoinNonEmpty (" ", entity.Firstname, entity.Lastname);
			entity.DisplayName2 = StringExtensions.JoinNonEmpty (", ", entity.Lastname, entity.Firstname);
		}
	}
}