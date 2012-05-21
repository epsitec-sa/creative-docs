//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Permanents.Data
{
	/// <summary>
	/// Cette classe décrit les paramètress permanents d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeComptePermanents : AbstractPermanents
	{
		public override void SetComptaEntity(ComptaEntity compta)
		{
			base.SetComptaEntity (compta);

			this.Clear ();
		}


		public override void Clear()
		{
			base.Clear ();

			//	Utilise le premier compte normal par défaut.
			if (this.compta != null)
			{
				var compte = this.compta.PlanComptable.Where (x => x.Type == TypeDeCompte.Normal).FirstOrDefault ();

				if (compte != null)
				{
					this.NuméroCompte = compte.Numéro;
				}
			}
		}


		public FormattedText NuméroCompte
		{
			//	Numéro du compte dont on affiche l'extrait.
			get;
			set;
		}


		public override AbstractPermanents CopyFrom()
		{
			var options = new ExtraitDeComptePermanents ();
			options.SetComptaEntity (this.compta);
			this.CopyTo (options);
			return options;
		}

		public override void CopyTo(AbstractPermanents dst)
		{
			var d = dst as ExtraitDeComptePermanents;

			d.NuméroCompte = this.NuméroCompte;

			base.CopyTo (dst);
		}

		public override bool CompareTo(AbstractPermanents other)
		{
			if (!base.CompareTo (other))
			{
				return false;
			}

			return true;
		}
	}
}
