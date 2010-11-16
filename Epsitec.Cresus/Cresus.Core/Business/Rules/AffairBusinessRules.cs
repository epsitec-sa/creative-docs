//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class AffairBusinessRules : GenericBusinessRule<AffairEntity>
	{
		public override void ApplySetupRule(AffairEntity affair)
		{
			var pool = Logic.Current.Data.RefIdGeneratorPool;
			var generator = pool.GetGenerator<AffairEntity> ();
			var nextId    = generator.GetNextId ();

			affair.IdA = string.Format ("{0:000000}", nextId);

			//	TODO: ...compléter...
		}
	}
}