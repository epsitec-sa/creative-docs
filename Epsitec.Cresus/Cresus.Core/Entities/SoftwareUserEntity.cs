//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

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

				if (Business.UserManagement.UserManager.IsPasswordRequired (this) == false)
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
			var s1 = this.LoginName.GetEntityStatus ();
			var s2 = this.DisplayName.GetEntityStatus ();
			var s3 = (this.AuthenticationMethod == Business.UserManagement.UserAuthenticationMethod.Password && string.IsNullOrWhiteSpace (this.LoginPasswordHash)) ? EntityStatus.None : EntityStatus.Valid;
			var s4 = this.UserGroups.Select (x => x.GetEntityStatus ()).ToArray ();

			return Helpers.EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3, s4);
		}
		
#if false
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
#endif


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
