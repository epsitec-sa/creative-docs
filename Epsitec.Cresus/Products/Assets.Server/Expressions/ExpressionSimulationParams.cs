﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public struct ExpressionSimulationParams
	{
		public ExpressionSimulationParams(DateRange range, Periodicity periodicity, decimal initialAmount,
			System.DateTime? extraDate, decimal? extraAmount, bool amortizationSuppl,
			System.DateTime? adjustDate, decimal? adjustAmount)
		{
			this.Range             = range;
			this.Periodicity       = periodicity;
			this.InitialAmount     = initialAmount;
			this.ExtraDate         = extraDate;
			this.ExtraAmount       = extraAmount;
			this.AmortizationSuppl = amortizationSuppl;
			this.AdjustDate        = adjustDate;
			this.AdjustAmount      = adjustAmount;

			this.arguments = new Dictionary<ObjectField, object> ();
		}

		public Dictionary<ObjectField, object> Arguments
		{
			get
			{
				return this.arguments;
			}
		}

		public bool HasExtra
		{
			get
			{
				return this.ExtraDate.HasValue && this.ExtraAmount.HasValue;
			}
		}

		public bool HasAdjust
		{
			get
			{
				return this.AdjustDate.HasValue && this.AdjustAmount.HasValue;
			}
		}

		public ExpressionSimulationParams(System.Xml.XmlReader reader)
		{
			// Todo...
			this.Range             = DateRange.Empty;
			this.Periodicity       = Periodicity.Unknown;
			this.InitialAmount     = 0.0m;
			this.ExtraDate         = null;
			this.ExtraAmount       = null;
			this.AmortizationSuppl = false;
			this.AdjustDate        = null;
			this.AdjustAmount      = null;

			this.arguments = new Dictionary<ObjectField, object> ();
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			// Todo...
		}


		public static ExpressionSimulationParams Default = new ExpressionSimulationParams (
			new DateRange (new System.DateTime (2000, 1, 1), new System.DateTime (2020, 1, 1)),
			Periodicity.Annual, 10000.0m, null, null, false, null, null);

		public readonly DateRange				Range;
		public readonly Periodicity				Periodicity;
		public readonly decimal					InitialAmount;
		public readonly System.DateTime?		ExtraDate;
		public readonly decimal?				ExtraAmount;
		public readonly bool					AmortizationSuppl;
		public readonly System.DateTime?		AdjustDate;
		public readonly decimal?				AdjustAmount;
		private readonly Dictionary<ObjectField, object> arguments;
	}
}
