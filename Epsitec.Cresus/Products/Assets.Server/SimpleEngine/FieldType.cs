//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
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
		GuidGroup,		// Guid d'un groupe
		GuidPerson,		// Guid d'une personne
		GuidAccount,	// Guid d'un compte
		GuidEntry,		// Guid d'une écriture
		GuidRatio,		// couple Guid + Ratio (decimal)
	}
}