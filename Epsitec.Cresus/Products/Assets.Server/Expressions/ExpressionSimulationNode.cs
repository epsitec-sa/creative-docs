//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public struct ExpressionSimulationNode
	{
		public ExpressionSimulationNode(int? rank, System.DateTime date, EventType eventType, TimelineGlyphMode mode,
			decimal? initialAmount, decimal? finalAmount, string trace, string error,
			AmortizationDetails details)
		{
			this.Rank          = rank;
			this.Date          = date;
			this.EventType     = eventType;
			this.Mode          = mode;
			this.InitialAmount = initialAmount;
			this.FinalAmount   = finalAmount;
			this.Trace         = trace;
			this.Error         = error;
			this.Details       = details;
		}

		public decimal? Amortization
		{
			get
			{
				if (this.IsAmortization &&
					this.InitialAmount.HasValue &&
					this.FinalAmount.HasValue)
				{
					return this.InitialAmount.Value - this.FinalAmount.Value;
				}
				else
				{
					return null;
				}
			}
		}

		public bool IsAmortizationEnded
		{
			get
			{
				return this.Mode == TimelineGlyphMode.Dimmed;
			}
		}

		private bool IsAmortization
		{
			get
			{
				return this.EventType == Data.EventType.AmortizationExtra
					|| this.EventType == Data.EventType.AmortizationSuppl
					|| this.EventType == Data.EventType.AmortizationPreview
					|| this.EventType == Data.EventType.AmortizationAuto;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.EventType == EventType.Unknown;
			}
		}

		public static ExpressionSimulationNode Empty = new ExpressionSimulationNode (null, System.DateTime.MinValue, EventType.Unknown, TimelineGlyphMode.Undefined, null, null, null, null, AmortizationDetails.Empty);

		public readonly int?					Rank;
		public readonly System.DateTime			Date;
		public readonly EventType				EventType;
		public readonly TimelineGlyphMode		Mode;
		public readonly decimal?				InitialAmount;
		public readonly decimal?				FinalAmount;
		public readonly string					Trace;
		public readonly string					Error;
		public readonly AmortizationDetails		Details;
	}
}
