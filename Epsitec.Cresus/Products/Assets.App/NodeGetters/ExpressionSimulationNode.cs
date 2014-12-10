//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.NodeGetters
{
	public struct ExpressionSimulationNode
	{
		public ExpressionSimulationNode(int rank, System.DateTime date, decimal initialAmount, decimal finalAmount, string trace)
		{
			this.Rank          = rank;
			this.Date          = date;
			this.InitialAmount = initialAmount;
			this.FinalAmount   = finalAmount;
			this.Trace         = trace;
		}

		public bool IsEmpty
		{
			get
			{
				return this.Rank == -1;
			}
		}

		public static ExpressionSimulationNode Empty = new ExpressionSimulationNode (-1, System.DateTime.MinValue, 0.0m, 0.0m, null);

		public readonly int						Rank;
		public readonly System.DateTime			Date;
		public readonly decimal					InitialAmount;
		public readonly decimal					FinalAmount;
		public readonly string					Trace;
	}
}
