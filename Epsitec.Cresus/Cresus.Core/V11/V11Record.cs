//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public class V11Record
	{
#region Enumerations
		public enum TypeEnum
		{
			Unknown,
			Type3,
			Type4,
		}

		public enum CodeTransactionEnum
		{
			Unknown,
			Normal,
			Remboursement,
			PropreCompte,
		}

		public enum BVRTransactionEnum
		{
			Unknown,
			BVR,
			BVRPlus,
		}

		public enum GenreTransactionEnum
		{
			Unknown,
			Credit,
			ContrePrestation,
			Correction,
		}

		public enum OrigineEnum
		{
			Unknown,
			OfficePoste,
			OPA,
			yellownet,
			EuroSIC,
		}

		public enum GenreRemiseEnum
		{
			Unknown,
			Original,
			Reconstruction,
			Test,
		}

		public enum CodeRejetEnum
		{
			Unknown,
			Aucun,
			Rejet,
			RejetMasse,
		}
#endregion


		public TypeEnum Type
		{
			get;
			set;
		}

		public CodeTransactionEnum CodeTransaction
		{
			get;
			set;
		}

		public BVRTransactionEnum BVRTransaction
		{
			get;
			set;
		}

		public string MonnaieTransaction
		{
			get;
			set;
		}

		public GenreTransactionEnum GenreTransaction
		{
			get;
			set;
		}

		public OrigineEnum Origine
		{
			get;
			set;
		}

		public GenreRemiseEnum GenreRemise
		{
			get;
			set;
		}

		public string NoReference
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

		public string RefDepot
		{
			get;
			set;
		}

		public Date? DateDepot
		{
			get;
			set;
		}

		public Date? DateTraitement
		{
			get;
			set;
		}

		public Date? DateCredit
		{
			get;
			set;
		}

		public string NoMicrofilm
		{
			get;
			set;
		}

		public CodeRejetEnum CodeRejet
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

		public bool IsValid
		{
			get
			{
				return
					this.Type             != TypeEnum.Unknown &&
					this.CodeTransaction  != CodeTransactionEnum.Unknown &&
					this.BVRTransaction   != BVRTransactionEnum.Unknown &&
					this.GenreTransaction != GenreTransactionEnum.Unknown &&
					this.Origine          != OrigineEnum.Unknown &&
					this.GenreRemise      != GenreRemiseEnum.Unknown &&
					this.Montant          != null &&
					this.DateDepot        != null &&
					this.DateTraitement   != null &&
					this.DateCredit       != null &&
					this.CodeRejet        != CodeRejetEnum.Unknown &&
					this.Taxes            != null;
			}
		}
	}
}
