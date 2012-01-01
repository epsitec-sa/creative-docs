//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class ComptabilitéCompteEntity
	{
		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Numéro.ToSimpleText (), this.Titre.ToSimpleText () };
		}
		
		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}
		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Numéro, this.Titre, this.Catégorie);
		}
	}
}
