//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public abstract class V11AbstractLine
	{
		#region Enumerations
		public enum TypeEnum
		{
			Unknown,
			Type3,
			Type4,
		}

		public enum GenreTransactionEnum
		{
			Unknown,
			Credit,
			ContrePrestation,
			Correction,
		}

		public enum GenreRemiseEnum
		{
			Unknown,
			Original,
			Reconstruction,
			Test,
		}
		#endregion


		public V11AbstractLine(TypeEnum type)
		{
			this.type = type;
		}


		public TypeEnum Type
		{
			get
			{
				return this.type;
			}
		}

		public GenreTransactionEnum GenreTransaction
		{
			get;
			set;
		}

		public string NoClient
		{
			get;
			set;
		}

		public GenreRemiseEnum GenreRemise
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
				if (this.Type == TypeEnum.Unknown)
				{
					return false;
				}

				return
					this.GenreTransaction != GenreTransactionEnum.Unknown &&
					this.GenreRemise      != GenreRemiseEnum.Unknown &&
					this.Montant          != null &&
					this.Taxes            != null;
			}
		}


		private readonly TypeEnum type;
	}
}
