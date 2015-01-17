//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
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
		GuidMethod,		// Guid d'une méthode d'amortissement
		GuidRatio,		// couple Guid + Ratio (decimal)
	}
}