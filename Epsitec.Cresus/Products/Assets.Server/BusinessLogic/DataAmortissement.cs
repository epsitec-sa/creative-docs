//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct DataAmortissement
	{
		public DataAmortissement(decimal rate, TypeAmortissement type, int period, decimal rest)
		{
			this.Rate   = rate;
			this.Type   = type;
			this.Period = period;
			this.Rest   = rest;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Rate   == 0.0m
					&& this.Type   == TypeAmortissement.Unknown
					&& this.Period == 0
					&& this.Rest   == 0.0m;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.Rate   != 0.0m
					&& this.Type   != TypeAmortissement.Unknown
					&& this.Period != 0;
			}
		}

		public static DataAmortissement Empty = new DataAmortissement (0.0m, TypeAmortissement.Unknown, 0, 0.0m);

		public readonly decimal				Rate;
		public readonly TypeAmortissement	Type;
		public readonly int					Period;
		public readonly decimal				Rest;
	}
}
