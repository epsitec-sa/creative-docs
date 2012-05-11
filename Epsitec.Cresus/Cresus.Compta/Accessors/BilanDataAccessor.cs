//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Graph;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du bilan de la comptabilité.
	/// </summary>
	public class BilanDataAccessor : DoubleDataAccessor
	{
		public BilanDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<BilanOptions> ("Présentation.Bilan.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Bilan.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.Bilan.Filter");

			this.arrayGraphOptions = new GraphOptions ();

			this.UpdateAfterOptionsChanged ();
		}


		protected override CatégorieDeCompte CatégorieGauche
		{
			get
			{
				return CatégorieDeCompte.Actif;
			}
		}

		protected override CatégorieDeCompte CatégorieDroite
		{
			get
			{
				return CatégorieDeCompte.Passif;
			}
		}


		protected override FormattedText DifferenceGaucheDescription
		{
			get
			{
				return "Différence (découvert)";
			}
		}

		protected override FormattedText DifferenceDroiteDescription
		{
			get
			{
				return "Différence (capital)";
			}
		}

	}
}