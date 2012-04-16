//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données pour le résumé périodique de la comptabilité.
	/// </summary>
	public class RésuméPériodiqueData : AbstractData
	{
		public RésuméPériodiqueData()
		{
			this.soldes = new List<decimal?> ();
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

		public decimal? GetSolde(int rank)
		{
			if (rank >= 0 && rank < this.soldes.Count)
			{
				return this.soldes[rank];
			}
			else
			{
				return null;
			}
		}

		public void SetSolde(int rank, decimal? solde)
		{
			while (rank >= this.soldes.Count)
			{
				this.soldes.Add (null);
			}

			this.soldes[rank] = solde;
		}


		private readonly List<decimal?>		soldes;
	}
}