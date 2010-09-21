//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic.Rules
{
	[BusinessRule (RuleType.Setup)]
	internal class AffairSetupRule : GenericBusinessRule<AffairEntity>
	{
		protected override void Apply(AffairEntity affair)
		{
			var pool = Logic.Current.Data.RefIdGeneratorPool;
			var generator = pool.GetGenerator<AffairEntity> ();
			var nextId    = generator.GetNextId ();

			affair.IdA = string.Format ("{0:000000}", nextId);

			//	TODO: ...compléter...
		}
	}
}