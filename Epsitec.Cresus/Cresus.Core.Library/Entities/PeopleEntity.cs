//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PeopleEntity
	{
		public override FormattedText GetSummary()
		{
			return this.GetCompactSummary ();
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.NaturalPerson.Firstname, this.NaturalPerson.Lastname);
		}
	}
}
