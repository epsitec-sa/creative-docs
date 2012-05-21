//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class IDateMetadataBusinessRules : GenericBusinessRule<IDateMetadata>
	{
		public override void ApplySetupRule(IDateMetadata entity)
		{
			var now = System.DateTime.UtcNow;

			entity.CreationDate         = now;
			entity.LastModificationDate = now;
		}

		public override void ApplyUpdateRule(IDateMetadata entity)
		{
			entity.LastModificationDate = System.DateTime.UtcNow;
		}
	}
}
