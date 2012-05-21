//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class NumericValueArticleParameterDefinitionBusinessRules : GenericBusinessRule<NumericValueArticleParameterDefinitionEntity>
	{
		public override void ApplySetupRule(NumericValueArticleParameterDefinitionEntity entity)
		{
//-			entity.Code = (string) ItemCodeGenerator.NewCode ();
		}
	}
}