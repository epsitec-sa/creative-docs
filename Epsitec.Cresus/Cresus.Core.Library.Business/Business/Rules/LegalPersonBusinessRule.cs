//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class LegalPersonBusinessRule : GenericBusinessRule<LegalPersonEntity>
	{
		public override void ApplyUpdateRule(LegalPersonEntity entity)
		{
			var name = entity.Name.ToSimpleText ();

			entity.DisplayName1 = name;
			entity.DisplayName2 = name;
		}
	}
}
