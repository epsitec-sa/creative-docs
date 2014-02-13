//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct AmortizationDefinition
	{
		public AmortizationDefinition(decimal rate, AmortizationType type, Periodicity periodicity, ProrataType prorataType, decimal round, decimal residual)
		{
			this.Rate        = rate;
			this.Type        = type;
			this.Periodicity = periodicity;
			this.ProrataType = prorataType;
			this.Round       = round;
			this.Residual    = residual;
		}

		public decimal							EffectiveRate
		{
			get
			{
				return this.Rate * this.PeriodMonthCount / 12.0m;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return this.Rate        == 0.0m
					&& this.Type        == AmortizationType.Unknown
					&& this.Periodicity == 0
					&& this.ProrataType == 0
					&& this.Round       == 0.0m
					&& this.Residual    == 0.0m;
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.Error == ErrorType.Ok;
			}
		}

		public ErrorType						Error
		{
			get
			{
				if (this.Rate < 0.0m || this.Rate > 1.0m)
				{
					return ErrorType.AmortizationInvalidRate;
				}

				if (this.Type == AmortizationType.Unknown)
				{
					return ErrorType.AmortizationInvalidType;
				}

				if (this.PeriodMonthCount == -1)
				{
					return ErrorType.AmortizationInvalidPeriod;
				}

				return ErrorType.Ok;
			}
		}


		public int								PeriodMonthCount
		{
			get
			{
				return AmortizationDefinition.GetPeriodMonthCount (this.Periodicity);
			}
		}

		public static int GetPeriodMonthCount(Periodicity period)
		{
			switch (period)
			{
				case Periodicity.Annual:
					return 12;

				case Periodicity.Semestrial:
					return 6;

				case Periodicity.Trimestrial:
					return 3;

				case Periodicity.Mensual:
					return 1;

				default:
					return -1;
			}
		}


		public void AddAdditionnalFields(DataEvent e)
		{
			//	Ajoute le contenu de la structure dans un événement, sous forme de
			//	champs additionnels.
			if (this.IsEmpty)
			{
				return;
			}

			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsDefRate, this.Rate);
				e.AddProperty (p);
			}

			{
				var p = new DataIntProperty (ObjectField.AmortizationDetailsDefType, (int) this.Type);
				e.AddProperty (p);
			}

			{
				var p = new DataIntProperty (ObjectField.AmortizationDetailsDefPeriodicity, (int) this.Periodicity);
				e.AddProperty (p);
			}

			{
				var p = new DataIntProperty (ObjectField.AmortizationDetailsDefProrataType, (int) this.ProrataType);
				e.AddProperty (p);
			}

			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsDefRound, this.Round);
				e.AddProperty (p);
			}

			{
				var p = new DataDecimalProperty (ObjectField.AmortizationDetailsDefResidual, this.Residual);
				e.AddProperty (p);
			}
		}


		public static AmortizationDefinition Empty = new AmortizationDefinition (0.0m, AmortizationType.Unknown, 0, 0.0m, 0.0m, 0.0m);

		public readonly decimal					Rate;
		public readonly AmortizationType		Type;
		public readonly Periodicity				Periodicity;
		public readonly ProrataType				ProrataType;
		public readonly decimal					Round;
		public readonly decimal					Residual;
	}
}
