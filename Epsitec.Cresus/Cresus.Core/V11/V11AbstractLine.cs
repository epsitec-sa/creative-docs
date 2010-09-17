//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public abstract class V11AbstractLine
	{
		public V11AbstractLine(V11LineType type)
		{
			this.type = type;
		}


		public V11LineType Type
		{
			get
			{
				return this.type;
			}
		}

		public V11LineGenreTransaction GenreTransaction
		{
			get;
			set;
		}

		public string NoClient
		{
			get;
			set;
		}

		public V11LineGenreRemise GenreRemise
		{
			get;
			set;
		}

		public decimal? Montant
		{
			get;
			set;
		}

		public string MonnaieMontant
		{
			get;
			set;
		}

		public decimal? Taxes
		{
			get;
			set;
		}

		public string MonnaieTaxes
		{
			get;
			set;
		}


		public virtual bool IsValid
		{
			get
			{
				if (this.Type == V11LineType.Unknown)
				{
					return false;
				}

				return
					this.GenreTransaction != V11LineGenreTransaction.Unknown &&
					this.GenreRemise      != V11LineGenreRemise.Unknown &&
					this.Montant          != null &&
					V11AbstractLine.CheckMonnaie (this.MonnaieMontant) &&
					this.Taxes            != null &&
					V11AbstractLine.CheckMonnaie (this.MonnaieTaxes);
			}
		}


		protected static bool CheckMonnaie(string monnaie)
		{
			return monnaie != null && monnaie.Length == 3;
		}


		private readonly V11LineType type;
	}
}
