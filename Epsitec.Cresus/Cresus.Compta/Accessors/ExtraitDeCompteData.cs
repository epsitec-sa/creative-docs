//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données pour un extrait de compte de la comptabilité.
	/// </summary>
	public class ExtraitDeCompteData : AbstractData
	{
		public ExtraitDeCompteData()
		{
		}


		public bool IsDébit
		{
			get;
			set;
		}

		public Date? Date
		{
			get;
			set;
		}

		public ComptaCompteEntity CP
		{
			get;
			set;
		}

		public FormattedText Pièce
		{
			get;
			set;
		}

		public FormattedText Libellé
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

		public decimal? Solde
		{
			get;
			set;
		}

		public ComptaCodeTVAEntity CodeTVA
		{
			get;
			set;
		}

		public decimal? TauxTVA
		{
			get;
			set;
		}

		public FormattedText Journal
		{
			get;
			set;
		}
	}
}