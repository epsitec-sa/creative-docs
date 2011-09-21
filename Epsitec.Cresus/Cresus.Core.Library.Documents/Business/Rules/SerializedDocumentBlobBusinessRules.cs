//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class SerializedDocumentBlobBusinessRules : GenericBusinessRule<SerializedDocumentBlobEntity>
	{
		public override void ApplySetupRule(SerializedDocumentBlobEntity blob)
		{
		}
	}
}
