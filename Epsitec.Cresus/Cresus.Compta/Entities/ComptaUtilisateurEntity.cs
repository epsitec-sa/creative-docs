//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaUtilisateurEntity
	{
		public FormattedText ShortDescription
		{
			get
			{
				//	Retourne la description à afficher dans une liste.
				FormattedText text;

				if (this.NomComplet == this.Utilisateur)
				{
					if (this.Utilisateur.IsNullOrEmpty)
					{
						text = "Nouvel utilisateur";
					}
					else
					{
						text = this.Utilisateur;
					}
				}
				else
				{
					text = TextFormatter.FormatText (this.NomComplet, "(", this.Utilisateur, ")");
				}

				return text;
			}
		}

		public FormattedText GetPasswordIcon()
		{
			string icon;

			if (string.IsNullOrEmpty(this.MotDePasse))
			{
				icon = "User.Unlocked";
			}
			else
			{
				icon = "User.Locked";
			}

			return string.Format (@"<img src=""{0}"" voff=""-10"" dx=""32"" dy=""32""/>", UIBuilder.GetResourceIconUri (icon));
		}

		public FormattedText GetAccessSummary()
		{
			//	Retourne un résumé sur les droits d'accès de l'utilisateur.
			if (this.Admin)
			{
				return "Toutes les présentations";
			}

			int n = Helpers.Présentations.PrésentationTypeCount (this.Présentations);

			if (n == 0)
			{
				return "Aucune présentation";
			}
			else if (n == 1)
			{
				return "1 présentation";
			}
			else
			{
				return string.Format ("{0} présentations", n.ToString ());
			}
		}


		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Utilisateur.GetEntityStatus ());
				a.Accumulate (this.NomComplet.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
