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
				return (this.IsArchive == false)
					&& (this.Disabled == false)
					&& (Misc.IsDateInRange (System.DateTime.Now, this.BeginDate, this.EndDate))
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

				if (this.CurrentStatus != Controllers.EditionStatus.Valid)
				{
					//	Affiche en italique les comptes qui ont une erreur.
					text = TextFormatter.FormatText ("<i>", text, "</i>");
				}

				return text;
			}
		}

		public Controllers.EditionStatus CurrentStatus
		{
			get
			{
				bool b1 = string.IsNullOrWhiteSpace (this.LoginName);
				bool b2 = this.DisplayName.IsNullOrWhiteSpace;
				bool b3 = this.AuthenticationMethod == Business.UserManagement.UserAuthenticationMethod.Password && string.IsNullOrWhiteSpace (this.LoginPasswordHash);
				bool b4 = this.UserGroups.Count == 0;

				if (b1 && b2 && b3 && b4)
				{
					return Controllers.EditionStatus.Empty;
				}

				if (b1 || b2 || b3 || b4)
				{
					return Controllers.EditionStatus.Invalid;
				}

				return Controllers.EditionStatus.Valid;
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
