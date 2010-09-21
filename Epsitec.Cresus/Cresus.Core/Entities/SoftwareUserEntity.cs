//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class SoftwareUserEntity
	{
		public bool IsActive
		{
			get
			{
				var now = System.DateTime.Now;

				return (this.IsArchive == false)
					&& (this.Disabled == false)
					&& (Misc.IsDateInRange (now, this.BeginDate, this.EndDate))
					&& (this.UserGroups.Where (group => !group.IsArchive).All (group => group.Disabled == false));
			}
		}

		public bool CheckPassword(string plaintextPassword)
		{
			return Epsitec.Common.Identity.BCrypt.CheckPassword (plaintextPassword, this.LoginPasswordHash);
		}

		public void SetPassword(string plaintextPassword)
		{
			if (plaintextPassword == null)
			{
				this.LoginPasswordHash = null;
			}
			else
			{
				this.LoginPasswordHash = Epsitec.Common.Identity.BCrypt.HashPassword (plaintextPassword);
			}
		}
	}
}
