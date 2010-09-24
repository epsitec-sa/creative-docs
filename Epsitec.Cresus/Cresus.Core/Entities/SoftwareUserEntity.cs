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


		public FormattedText ShortDescription
		{
			get
			{
				//	Retourne la description à afficher dans une liste.
				FormattedText text;

				if (this.DisplayName == this.LoginName)
				{
					if (string.IsNullOrEmpty (this.LoginName))
					{
						text = "Nouveau compte";
					}
					else
					{
						text = this.LoginName;
					}
				}
				else
				{
					text = TextFormatter.FormatText (this.DisplayName, "(", this.LoginName, ")");
				}

				if (this.CurrentState != SoftwareUserEntityState.OK)
				{
					//	Affiche en italique les comptes qui ont une erreur.
					text = string.Concat ("<i>", text, "</i>");
				}

				return text;
			}
		}

		public SoftwareUserEntityState CurrentState
		{
			get
			{
				bool b1 = string.IsNullOrWhiteSpace (this.LoginName);
				bool b2 = this.DisplayName.IsNullOrWhiteSpace;
				bool b3 = this.AuthenticationMethod == Business.UserManagement.UserAuthenticationMethod.Password && string.IsNullOrWhiteSpace (this.LoginPasswordHash);
				bool b4 = this.UserGroups.Count == 0;

				if (b1 && b2 && b3 && b4)
				{
					return SoftwareUserEntityState.Empty;
				}

				if (b1 || b2 || b3 || b4)
				{
					return SoftwareUserEntityState.Error;
				}

				return SoftwareUserEntityState.OK;
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
