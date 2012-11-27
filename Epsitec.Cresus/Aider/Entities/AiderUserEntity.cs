//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business.UserManagement;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderUserEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.DisplayName, "\n", this.LoginName, "\n", "Rôle: ", this.Role.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.DisplayName);
		}

		partial void GetIsAdministrator(ref bool value)
		{
			if (!this.isAdministrator.HasValue)
			{
				this.isAdministrator = this.HasPowerLevel (UserPowerLevel.Administrator);
			}

			value = this.isAdministrator.Value;
		}

		partial void SetIsAdministrator(bool value)
		{
			this.isAdministrator = value;
		}

		private bool? isAdministrator;
	}
}
