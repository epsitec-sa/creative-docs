using Epsitec.Aider.eCH;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using System;


namespace Epsitec.Aider.Entities
{


	public partial class AiderPersonEntity
	{


		public override FormattedText GetSummary()
		{
			var text = string.Join
			(
				" ",
				this.eCH_Person.PersonFirstNames,
				this.eCH_Person.PersonOfficialName
			);
			
			return TextFormatter.FormatText (text);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}


		public bool IsGovernmentDefined()
		{
			return this.eCH_Person.DataSource == eCH.DataSource.Government;
		}


	}


}
