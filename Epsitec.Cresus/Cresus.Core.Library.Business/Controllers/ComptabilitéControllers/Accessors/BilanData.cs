//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Données pour la balance de la comptabilité.
	/// </summary>
	public class BilanData : AbstractData
	{
		public BilanData()
		{
		}


		public FormattedText NuméroGauche
		{
			get;
			set;
		}

		public FormattedText TitreGauche
		{
			get;
			set;
		}

		public int NiveauGauche
		{
			get;
			set;
		}

		public decimal? SoldeGauche
		{
			get;
			set;
		}


		public FormattedText NuméroDroite
		{
			get;
			set;
		}

		public FormattedText TitreDroite
		{
			get;
			set;
		}

		public int NiveauDroite
		{
			get;
			set;
		}

		public decimal? SoldeDroite
		{
			get;
			set;
		}
	}
}