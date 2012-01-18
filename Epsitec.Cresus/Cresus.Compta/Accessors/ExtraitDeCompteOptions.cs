//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta
{
	/// <summary>
	/// Cette classe décrit les options d'affichage d'un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteOptions : AbstractOptions
	{
		public ExtraitDeCompteOptions(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
			//	Utilise le premier compte normal par défaut.
			var compte = this.comptabilitéEntity.PlanComptable.OrderBy (x => x.Numéro).Where (x => x.Type == TypeDeCompte.Normal).FirstOrDefault ();

			if (compte != null)
			{
				this.NuméroCompte = compte.Numéro;
			}

			this.HasGraphics = true;
		}


		public FormattedText NuméroCompte
		{
			//	Numéro du compte dont on affiche l'extrait.
			get;
			set;
		}

		public bool HasGraphics
		{
			get;
			set;
		}
	}
}
