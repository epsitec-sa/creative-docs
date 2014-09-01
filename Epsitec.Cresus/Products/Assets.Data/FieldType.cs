//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Assets.Data
{
	[DesignerVisible]
	public enum FieldType
	{
		Unknown,

		String,
		Decimal,
		ComputedAmount,
		AmortizedAmount,
		Int,
		Date,
		Account,		// compte
		GuidGroup,		// Guid d'un groupe
		GuidPerson,		// Guid d'une personne
		GuidRatio,		// couple Guid + Ratio (decimal)
	}
}