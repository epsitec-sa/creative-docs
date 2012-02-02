//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

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
			this.options    = this.mainWindowController.GetSettingsOptions<DoubleOptions> ("Présentation.Bilan.Options", this.comptaEntity);
			this.searchData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Bilan.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Bilan.Filter");

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