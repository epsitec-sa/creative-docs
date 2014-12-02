//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Settings
{
	public struct ExpressionSimulationParams
	{
		public ExpressionSimulationParams(DateRange range,
			decimal initialAmount, decimal residualAmount, decimal roundAmount,
			decimal rate, decimal yearCount, Periodicity periodicity)
		{
			this.Range          = range;
			this.InitialAmount  = initialAmount;
			this.ResidualAmount = residualAmount;
			this.RoundAmount    = roundAmount;
			this.Rate           = rate;
			this.YearCount      = yearCount;
			this.Periodicity    = periodicity;
		}

		//??public ExpressionSimulationParams(System.Xml.XmlReader reader)
		//??{
		//??}


		//??public void Serialize(System.Xml.XmlWriter writer, string name)
		//??{
		//??}


		public static ExpressionSimulationParams Default = new ExpressionSimulationParams (
			new DateRange (new System.DateTime (2000, 1, 1), new System.DateTime (2100, 1, 1)),
			10000.0m, 1.0m, 1.0m, 0.1m, 10, Periodicity.Annual);

		public readonly DateRange				Range;
		public readonly decimal					InitialAmount;
		public readonly decimal					ResidualAmount;
		public readonly decimal					RoundAmount;
		public readonly decimal					Rate;
		public readonly decimal					YearCount;
		public readonly Periodicity				Periodicity;
	}
}
