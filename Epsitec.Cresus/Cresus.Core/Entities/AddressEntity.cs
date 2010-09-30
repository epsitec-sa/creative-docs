//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AddressEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					this.Street.StreetName, "\n",
					this.Location.PostalCode, this.Location.Name
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Street.StreetName, ", ", this.Location.Country.Code, "-", this.Location.PostalCode, this.Location.Name);
		}

		public override string[] GetTextArray()
		{
			return new string[] { this.Street.StreetName.ToSimpleText (), this.Location.Country.Code, this.Location.PostalCode.ToSimpleText (), this.Location.Name.ToSimpleText () };
		}

		public override EntityStatus Status
		{
			get
			{
				EntityStatus ok1 = this.Street.Status;
				EntityStatus ok2 = this.PostBox.Status;
				EntityStatus ok3 = this.Location.Status;

				if (ok1 == EntityStatus.Valid && ok2 == EntityStatus.Valid && ok3 == EntityStatus.Valid)
				{
					return EntityStatus.Valid;
				}

				if (ok1 == EntityStatus.Invalid || ok2 == EntityStatus.Invalid || ok3 == EntityStatus.Invalid)
				{
					return EntityStatus.Invalid;
				}

				return EntityStatus.Empty;
			}
		}
	}
}
