//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderContactEntity
	{
		public override FormattedText GetSummary()
		{
			switch (this.ContactType)
			{
				case Enumerations.ContactType.None:
					return Resources.FormattedText ("Ce contact n'est pas défini.<br/><br/>Utilisez une <i>action</i> pour associer ce contact à une personne ou à une entreprise.");
			}

			return TextFormatter.FormatText (this.DisplayName, "\n", this.DisplayParish, "\n", this.DisplayAddress);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
		}

		public void RefreshCache()
		{
			if (this.Person.IsNotNull ())
			{
				this.DisplayName = this.Person.DisplayName;
			}
			else
			{
				this.DisplayName = "—";
			}

			if (this.Address.IsNotNull ())
			{
				this.DisplayAddress = this.Address.GetCompactSummary ().ToSimpleText ();
			}
			else
			{
				this.DisplayAddress = "";
			}

			if (this.Household.IsNotNull ())
			{
				this.DisplayAddress = this.Household.Address.GetCompactSummary ().ToSimpleText ();
			}
		}
	}
}
