//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

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

		public string MonnaieTransaction
		{
			get;
			set;
		}

		public string FormatedNoClient
		{
			//	Retourne le numéro de client au format "01-069444-3".
			get
			{
				string no = this.NoClient;

				if (!string.IsNullOrEmpty (no) && no.Length == 9)
				{
					string s1 = no.Substring (0, 2);
					string s2 = no.Substring (2, 6);
					string s3 = no.Substring (8, 1);

					return string.Concat (s1, "-", s2, "-", s3);
				}

				return no;
			}
		}

		public bool CheckNoClient
		{
			get
			{
				string no = this.NoClient;

				if (!string.IsNullOrEmpty (no) && no.Length == 9)
				{
					char c = Isr.ComputeCheckDigit (no.Substring (0, 8));
					return no[8] == c;
				}

				return false;
			}
		}

		public string NoClient
		{
			//	Numéro de client à 9 chiffres. Par exemple "010694443".
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
					V11AbstractLine.CheckMonnaie (this.MonnaieTransaction) &&
					!string.IsNullOrEmpty(this.NoClient) &&
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
