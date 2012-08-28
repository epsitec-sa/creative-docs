//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	/// <summary>
	/// The <c>IItemCodeBusinessRules</c> class initializes all <see cref="IItemCode"/> based
	/// entities when they are created, so that they get a default unique <c>Code</c> property.
	/// </summary>
	[BusinessRule]
	internal class IItemCodeBusinessRules : GenericBusinessRule<IItemCode>
	{
		public override void ApplySetupRule(IItemCode itemCode)
		{
			itemCode.Code = (string) ItemCodeGenerator.NewCode ();
		}
	}
}
