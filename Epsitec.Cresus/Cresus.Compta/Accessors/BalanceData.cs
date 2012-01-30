//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données pour la balance de vérification de la comptabilité.
	/// </summary>
	public class BalanceData : AbstractData
	{
		public FormattedText Numéro
		{
			get;
			set;
		}

		public FormattedText Titre
		{
			get;
			set;
		}

		public CatégorieDeCompte Catégorie
		{
			get;
			set;
		}

		public TypeDeCompte Type
		{
			get;
			set;
		}

		public decimal? Débit
		{
			get;
			set;
		}

		public decimal? Crédit
		{
			get;
			set;
		}

		public decimal? SoldeDébit
		{
			get;
			set;
		}

		public decimal? SoldeCrédit
		{
			get;
			set;
		}

		public decimal? Budget
		{
			get;
			set;
		}

		public int Niveau
		{
			get;
			set;
		}
	}
}