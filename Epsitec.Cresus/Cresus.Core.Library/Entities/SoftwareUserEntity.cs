//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Extensions;
using Epsitec.Cresus.Core.Business.UserManagement;

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
					&& (System.DateTime.UtcNow.InRange (this))
					&& (this.UserGroups.Where (group => !group.IsArchive).All (group => group.Disabled == false));
			}
		}

		public bool IsPasswordRequired
		{
			get
			{
				switch (this.AuthenticationMethod)
				{
					case UserAuthenticationMethod.Password:
						return true;

					case UserAuthenticationMethod.None:
						return false;

					case UserAuthenticationMethod.System:
						return string.Compare (this.LoginName, System.Environment.UserName, System.StringComparison.CurrentCultureIgnoreCase) != 0;

					case UserAuthenticationMethod.Disabled:
						return false;

					default:
						throw new System.NotSupportedException (string.Format ("Authentication method {0} not supported", this.AuthenticationMethod));
				}
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

				if (this.IsPasswordRequired == false)
				{
					text = TextFormatter.FormatText (text, "*");
				}

				if (this.IsEntityValid == false)
				{
					//	Affiche en italique les comptes qui ont une erreur.
					text = TextFormatter.FormatText ("<i>", text, "</i>");
				}

				return text;
			}
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Code.GetEntityStatus ());
				a.Accumulate (this.LoginName.GetEntityStatus ());
				a.Accumulate (this.DisplayName.GetEntityStatus ());
				a.Accumulate ((this.AuthenticationMethod == UserAuthenticationMethod.Password && string.IsNullOrWhiteSpace (this.LoginPasswordHash)) ? EntityStatus.None : EntityStatus.Valid);
				a.Accumulate (this.UserGroups.Select (x => x.GetEntityStatus ()));

				return a.EntityStatus;
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

		/// <summary>
		/// Checks whether the user has at least the given power level. So this function will return
		/// true when asked for the power level "regular" if the user is an administrator.
		/// </summary>
		public bool HasPowerLevel(UserPowerLevel powerLevel)
		{
			return this.UserGroups.Any
			(
				g => g.UserPowerLevel != UserPowerLevel.None && g.UserPowerLevel <= powerLevel
			);
		}
	}
}
