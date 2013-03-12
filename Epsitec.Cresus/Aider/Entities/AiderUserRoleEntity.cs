//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderUserRoleEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "\n", this.DefaultScopes);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public static AiderUserRoleEntity GetRole(BusinessContext businessContext, string name)
		{
			var example = new AiderUserRoleEntity ()
			{
				Name = name
			};

			var result = businessContext.DataContext.GetByExample (example);
			
			return result.Single ();
		}

		public static readonly string AdminRole =  "Administrateur";
		public static readonly string AleRole = "Accès Ale";
		public static readonly string CountyRole = "Accès cantonal";
		public static readonly string RegionRole = "Accès régional";
		public static readonly string ParishRole = "Accès paroissial";
	}
}
