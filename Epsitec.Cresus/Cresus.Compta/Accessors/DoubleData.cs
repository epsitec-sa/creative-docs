//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données doubles (bilan avec actif/passif ou PP avec charge/produit) de la comptabilité.
	/// </summary>
	public class DoubleData : AbstractData
	{
		public DoubleData()
		{
		}


		public bool Gauche
		{
			//	true  -> actif ou charge
			//	false -> passif ou produit
			get;
			set;
		}

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

		public int Niveau
		{
			get;
			set;
		}

		public decimal? Solde
		{
			get;
			set;
		}

		public decimal? Budget
		{
			get;
			set;
		}

		public decimal? BudgetProrata
		{
			get;
			set;
		}

		public decimal? BudgetFutur
		{
			get;
			set;
		}

		public decimal? BudgetFuturProrata
		{
			get;
			set;
		}

		public decimal? PériodePrécédente
		{
			get;
			set;
		}

		public decimal? PériodePénultième
		{
			get;
			set;
		}
	}
}