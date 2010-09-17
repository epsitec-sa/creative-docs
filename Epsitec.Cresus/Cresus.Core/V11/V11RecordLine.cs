//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public class V11RecordLine : V11AbstractLine
	{
#region Enumerations
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

		public enum OrigineEnum
		{
			Unknown,
			OfficePoste,
			OPA,
			yellownet,
			EuroSIC,
		}

		public enum CodeRejetEnum
		{
			Unknown,
			Aucun,
			Rejet,
			RejetMasse,
		}
#endregion


		public V11RecordLine(TypeEnum type)
			: base (type)
		{
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

		public OrigineEnum Origine
		{
			get;
			set;
		}

		public string NoReference
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


		public override bool IsValid
		{
			get
			{
				if (!base.IsValid)
				{
					return false;
				}

				return
					this.CodeTransaction  != CodeTransactionEnum.Unknown &&
					this.BVRTransaction   != BVRTransactionEnum.Unknown &&
					this.Origine          != OrigineEnum.Unknown &&
					this.DateDepot        != null &&
					this.DateTraitement   != null &&
					this.DateCredit       != null &&
					this.CodeRejet        != CodeRejetEnum.Unknown;
			}
		}
	}
}
