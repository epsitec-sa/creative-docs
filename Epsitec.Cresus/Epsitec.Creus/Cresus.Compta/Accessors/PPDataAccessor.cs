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
	/// Gère l'accès aux données des pertes et profits de la comptabilité.
	/// </summary>
	public class PPDataAccessor : DoubleDataAccessor
	{
		public PPDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.PP.ViewSettings");
			this.searchData       = this.mainWindowController.GetSettingsSearchData ("Présentation.PP.Search");
			this.filterData       = this.viewSettingsList.Selected.CurrentFilter;
			this.options          = this.viewSettingsList.Selected.CurrentOptions;

			this.arrayGraphOptions = new GraphOptions ();

			this.UpdateAfterOptionsChanged ();
		}


		protected override CatégorieDeCompte CatégorieGauche
		{
			get
			{
				return CatégorieDeCompte.Charge;
			}
		}

		protected override CatégorieDeCompte CatégorieDroite
		{
			get
			{
				return CatégorieDeCompte.Produit;
			}
		}


		protected override FormattedText DifferenceGaucheDescription
		{
			get
			{
				return "Différence (bénéfice)";
			}
		}

		protected override FormattedText DifferenceDroiteDescription
		{
			get
			{
				return "Différence (perte)";
			}
		}

	}
}