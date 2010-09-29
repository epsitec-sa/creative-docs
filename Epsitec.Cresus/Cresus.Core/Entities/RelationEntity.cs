//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class RelationEntity
	{
		public FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"N�", this.IdA, "\n",
					this.Person.GetSummary (), "\n",
					"Repr�sentant: ~", this.SalesRepresentative.GetCompactSummary ()
				);
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("N�", this.IdA, "\n", this.Person.GetCompactSummary ());
		}
	}
}
