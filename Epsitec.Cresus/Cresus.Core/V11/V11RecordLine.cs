//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public class V11RecordLine : V11AbstractLine
	{
		public V11RecordLine(V11LineType type)
			: base (type)
		{
		}


		public V11LineCodeTransaction CodeTransaction
		{
			get;
			set;
		}

		public V11LineBVRTransaction BVRTransaction
		{
			get;
			set;
		}

		public V11LineOrigine Origine
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

		public V11LineCodeRejet CodeRejet
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
					this.CodeTransaction  != V11LineCodeTransaction.Unknown &&
					this.BVRTransaction   != V11LineBVRTransaction.Unknown &&
					this.Origine          != V11LineOrigine.Unknown &&
					this.DateDepot        != null &&
					this.DateTraitement   != null &&
					this.DateCredit       != null &&
					this.CodeRejet        != V11LineCodeRejet.Unknown;
			}
		}
	}
}
