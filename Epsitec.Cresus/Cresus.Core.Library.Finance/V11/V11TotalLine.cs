//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.V11
{
	public class V11TotalLine : V11AbstractLine
	{
		public V11TotalLine(V11LineType type)
			: base (type)
		{
		}


		public int? NbTransactions
		{
			get;
			set;
		}

		public Date? DateEtablissement
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
					this.NbTransactions    != null &&
					this.DateEtablissement != null;
			}
		}
	}
}
