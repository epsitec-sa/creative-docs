//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public struct AmortizedAmount2 : System.IEquatable<AmortizedAmount2>
	{
		public AmortizedAmount2
		(
			AmortizationMethod	amortizationMethod,
			string				expression,
			decimal?			rate,
			decimal				yearRank,
			decimal				yearCount,
			decimal				periodRank,
			Periodicity			periodicity,
			decimal?			forcedAmount,
			decimal?			previousAmount,
			decimal?			initialAmount,
			decimal?			startYearAmount,
			decimal?			baseAmount,
			decimal?			prorataNumerator,
			decimal?			prorataDenominator,
			decimal?			roundAmount,
			decimal?			residualAmount,
			EntryScenario		entryScenario,
			System.DateTime		date,
			Guid				assetGuid,
			Guid				eventGuid,
			Guid				entryGuid,
			int					entrySeed
		)
		{
			this.AmortizationMethod = amortizationMethod;
			this.Expression         = expression;
			this.Rate               = rate;
			this.YearRank           = yearRank;
			this.YearCount          = yearCount;
			this.PeriodRank         = periodRank;
			this.Periodicity        = periodicity;
			this.ForcedAmount       = forcedAmount;
			this.PreviousAmount     = previousAmount;
			this.InitialAmount      = initialAmount;
			this.StartYearAmount    = startYearAmount;
			this.BaseAmount         = baseAmount;
			this.ProrataNumerator   = prorataNumerator;
			this.ProrataDenominator = prorataDenominator;
			this.RoundAmount        = roundAmount;
			this.ResidualAmount     = residualAmount;
			this.EntryScenario      = entryScenario;
			this.Date               = date;
			this.AssetGuid          = assetGuid;
			this.EventGuid          = eventGuid;
			this.EntryGuid          = entryGuid;
			this.EntrySeed          = entrySeed;
		}

		public AmortizedAmount2(decimal? initialAmount)
		{
			this.AmortizationMethod = AmortizationMethod.Unknown;
			this.Expression         = null;
			this.Rate               = 0;
			this.YearRank           = 0;
			this.YearCount          = 0;
			this.PeriodRank         = 0;
			this.Periodicity        = Periodicity.Unknown;
			this.ForcedAmount       = null;
			this.PreviousAmount     = null;
			this.InitialAmount      = initialAmount;
			this.StartYearAmount    = null;
			this.BaseAmount         = null;
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.ResidualAmount     = null;
			this.EntryScenario      = EntryScenario.None;
			this.Date               = System.DateTime.MinValue;
			this.AssetGuid          = Guid.Empty;
			this.EventGuid          = Guid.Empty;
			this.EntryGuid          = Guid.Empty;
			this.EntrySeed          = 0;
		}

		public AmortizedAmount2(decimal? initialAmount, EntryScenario scenario, System.DateTime date, Guid objGuid, Guid eventGuid)
		{
			this.AmortizationMethod = AmortizationMethod.Unknown;
			this.Expression         = null;
			this.Rate               = 0;
			this.YearRank           = 0;
			this.YearCount          = 0;
			this.PeriodRank         = 0;
			this.Periodicity        = Periodicity.Unknown;
			this.ForcedAmount       = null;
			this.PreviousAmount     = null;
			this.InitialAmount      = initialAmount;
			this.StartYearAmount    = null;
			this.BaseAmount         = null;
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.ResidualAmount     = null;
			this.EntryScenario      = scenario;
			this.Date               = date;
			this.AssetGuid          = objGuid;
			this.EventGuid          = eventGuid;
			this.EntryGuid          = Guid.Empty;
			this.EntrySeed          = 0;
		}

		public AmortizedAmount2(EntryScenario scenario)
		{
			this.AmortizationMethod = AmortizationMethod.Unknown;
			this.Expression         = null;
			this.Rate               = 0;
			this.YearRank           = 0;
			this.YearCount          = 0;
			this.PeriodRank         = 0;
			this.Periodicity        = Periodicity.Unknown;
			this.ForcedAmount       = null;
			this.PreviousAmount     = null;
			this.InitialAmount      = null;
			this.StartYearAmount    = null;
			this.BaseAmount         = null;
			this.ProrataNumerator   = null;
			this.ProrataDenominator = null;
			this.RoundAmount        = null;
			this.ResidualAmount     = null;
			this.EntryScenario      = scenario;
			this.Date               = System.DateTime.MinValue;
			this.AssetGuid          = Guid.Empty;
			this.EventGuid          = Guid.Empty;
			this.EntryGuid          = Guid.Empty;
			this.EntrySeed          = 0;
		}

		public AmortizedAmount2(System.Xml.XmlReader reader)
		{
			this.AmortizationMethod = (AmortizationMethod) IOHelpers.ReadTypeAttribute (reader, "AmortizationMethod", typeof (AmortizationMethod));
			this.Expression         = IOHelpers.ReadStringAttribute (reader, "Expression");
			this.Rate               = IOHelpers.ReadDecimalAttribute (reader, "Rate");
			this.YearRank           = IOHelpers.ReadDecimalAttribute (reader, "YearRank").GetValueOrDefault ();
			this.YearCount          = IOHelpers.ReadDecimalAttribute (reader, "YearCount").GetValueOrDefault (1.0m);
			this.PeriodRank         = IOHelpers.ReadDecimalAttribute (reader, "PeriodRank").GetValueOrDefault ();
			this.Periodicity        = (Periodicity) IOHelpers.ReadTypeAttribute (reader, "Periodicity", typeof (Periodicity));
			this.ForcedAmount       = IOHelpers.ReadDecimalAttribute (reader, "ForcedAmount");
			this.PreviousAmount     = IOHelpers.ReadDecimalAttribute (reader, "PreviousAmount");
			this.InitialAmount      = IOHelpers.ReadDecimalAttribute (reader, "InitialAmount");
			this.StartYearAmount    = IOHelpers.ReadDecimalAttribute (reader, "StartYearAmount");
			this.BaseAmount         = IOHelpers.ReadDecimalAttribute (reader, "BaseAmount");
			this.ProrataNumerator   = IOHelpers.ReadDecimalAttribute (reader, "ProrataNumerator");
			this.ProrataDenominator = IOHelpers.ReadDecimalAttribute (reader, "ProrataDenominator");
			this.RoundAmount        = IOHelpers.ReadDecimalAttribute (reader, "RoundAmount");
			this.ResidualAmount     = IOHelpers.ReadDecimalAttribute (reader, "ResidualAmount");
			this.EntryScenario      = (EntryScenario) IOHelpers.ReadTypeAttribute (reader, "EntryScenario", typeof (EntryScenario));
			this.Date               = IOHelpers.ReadDateAttribute (reader, "Date").GetValueOrDefault ();
			this.AssetGuid          = IOHelpers.ReadGuidAttribute (reader, "AssetGuid");
			this.EventGuid          = IOHelpers.ReadGuidAttribute (reader, "EventGuid");
			this.EntryGuid          = IOHelpers.ReadGuidAttribute (reader, "EntryGuid");
			this.EntrySeed          = IOHelpers.ReadIntAttribute (reader, "EntrySeed").GetValueOrDefault ();

			reader.Read ();  // on avance plus loin
		}


		public readonly AmortizationMethod		AmortizationMethod;
		public readonly string					Expression;
		public readonly decimal?				Rate;
		public readonly decimal					YearRank;
		public readonly decimal					YearCount;
		public readonly decimal					PeriodRank;
		public readonly Periodicity				Periodicity;
		public readonly decimal?				ForcedAmount;
		public readonly decimal?				PreviousAmount;
		public readonly decimal?				InitialAmount;
		public readonly decimal?				StartYearAmount;
		public readonly decimal?				BaseAmount;
		public readonly decimal?				ProrataNumerator;
		public readonly decimal?				ProrataDenominator;
		public readonly decimal?				RoundAmount;
		public readonly decimal?				ResidualAmount;
		public readonly EntryScenario			EntryScenario;
		public readonly System.DateTime			Date;
		public readonly Guid					AssetGuid;
		public readonly Guid					EventGuid;
		public readonly Guid					EntryGuid;
		public readonly int						EntrySeed;


		public decimal							ProrataFactor
		{
			//	Retourne le facteur multiplicateur "au prorata", compris entre 0 et 1.
			get
			{
				if (this.ProrataNumerator.HasValue &&
					this.ProrataDenominator.HasValue &&
					this.ProrataDenominator.Value != 0.0m)
				{
					var prorata = this.ProrataNumerator.Value/ this.ProrataDenominator.Value;

					prorata = System.Math.Max (prorata, 0.0m);
					prorata = System.Math.Min (prorata, 1.0m);  // garde-fou

					return prorata;
				}
				else
				{
					return 1.0m;  // 100%
				}
			}
		}

		public decimal							PeriodicityFactor
		{
			//	Retourne le facteur multiplicateur pour la périodicité, compris entre 0 et 1.
			//	Annuel      -> 12/12
			//	Semestriel  ->  6/12
			//	Trimestriel ->  3/12
			//	Mensuel     ->  1/12
			get
			{
				return this.PeriodMonthCount / 12.0m;
			}
		}

		public int								PeriodMonthCount
		{
			get
			{
				return AmortizedAmount2.GetPeriodMonthCount (this.Periodicity);
			}
		}


		#region IEquatable<AmortizedAmount2> Members
		public bool Equals(AmortizedAmount2 other)
		{
			return this.AmortizationMethod == other.AmortizationMethod
				&& this.Expression         == other.Expression
				&& this.Rate               == other.Rate
				&& this.YearRank           == other.YearRank
				&& this.YearCount          == other.YearCount
				&& this.PeriodRank         == other.PeriodRank
				&& this.Periodicity        == other.Periodicity
				&& this.ForcedAmount       == other.ForcedAmount
				&& this.PreviousAmount     == other.PreviousAmount
				&& this.InitialAmount      == other.InitialAmount
				&& this.StartYearAmount    == other.StartYearAmount
				&& this.BaseAmount         == other.BaseAmount
				&& this.ProrataNumerator   == other.ProrataNumerator
				&& this.ProrataDenominator == other.ProrataDenominator
				&& this.RoundAmount        == other.RoundAmount
				&& this.ResidualAmount     == other.ResidualAmount
				&& this.EntryScenario      == other.EntryScenario
				&& this.Date               == other.Date
				&& this.AssetGuid          == other.AssetGuid
				&& this.EventGuid          == other.EventGuid
				&& this.EntryGuid          == other.EntryGuid
				&& this.EntrySeed          == other.EntrySeed;
		}
		#endregion

		public override bool Equals(object obj)
		{
			if (obj is AmortizedAmount2)
			{
				return this.Equals ((AmortizedAmount2) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.AmortizationMethod.GetHashCode ()
				 ^ this.Expression        .GetHashCode ()
				 ^ this.Rate              .GetHashCode ()
				 ^ this.YearRank          .GetHashCode ()
				 ^ this.YearCount         .GetHashCode ()
				 ^ this.PeriodRank        .GetHashCode ()
				 ^ this.Periodicity       .GetHashCode ()
				 ^ this.ForcedAmount      .GetHashCode ()
				 ^ this.PreviousAmount    .GetHashCode ()
				 ^ this.InitialAmount     .GetHashCode ()
				 ^ this.StartYearAmount   .GetHashCode ()
				 ^ this.BaseAmount        .GetHashCode ()
				 ^ this.ProrataNumerator  .GetHashCode ()
				 ^ this.ProrataDenominator.GetHashCode ()
				 ^ this.RoundAmount       .GetHashCode ()
				 ^ this.ResidualAmount    .GetHashCode ()
				 ^ this.EntryScenario     .GetHashCode ()
				 ^ this.Date              .GetHashCode ()
				 ^ this.AssetGuid         .GetHashCode ()
				 ^ this.EventGuid         .GetHashCode ()
				 ^ this.EntryGuid         .GetHashCode ()
				 ^ this.EntrySeed         .GetHashCode ();
		}

		public static bool operator ==(AmortizedAmount2 a, AmortizedAmount2 b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(AmortizedAmount2 a, AmortizedAmount2 b)
		{
			return !(a == b);
		}


		#region Constructors helpers
		public static AmortizedAmount2 SetAmortizationMethod(AmortizedAmount2 model, AmortizationMethod method)
		{
			return new AmortizedAmount2
			(
				method,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetExpression(AmortizedAmount2 model, string expression)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetInitialAmount(AmortizedAmount2 model, decimal? value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				value,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetRate(AmortizedAmount2 model, decimal? value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				value,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetForcedAmount(AmortizedAmount2 model, decimal? forcedAmount)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				forcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetProrataNumerator(AmortizedAmount2 model, decimal? value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				value,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetProrataDenominator(AmortizedAmount2 model, decimal? value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				value,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetRoundAmount(AmortizedAmount2 model, decimal? value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				value,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetResidualAmount(AmortizedAmount2 model, decimal? value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				value,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetEntryScenario(AmortizedAmount2 model, EntryScenario value)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				value,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetEntry(AmortizedAmount2 model, Guid entryGuid, int entrySeed)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				entryGuid,
				entrySeed
			);
		}

		public static AmortizedAmount2 SetPreview(AmortizedAmount2 model, decimal? rate,
			decimal? prorataNumerator, decimal? prorataDenominator,
			Periodicity periodicity,
			decimal? roundAmount, decimal? residualAmount, EntryScenario entryScenario)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				prorataNumerator,
				prorataDenominator,
				roundAmount,
				residualAmount,
				entryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetPreviousAmount(AmortizedAmount2 model, decimal? previousAmount)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				previousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetAmortizedAmount2(AmortizedAmount2 model, decimal? initialAmount, decimal? startYearAmount, decimal? baseAmount)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				model.YearRank,
				model.YearCount,
				model.PeriodRank,
				model.Periodicity,
				model.ForcedAmount,
				initialAmount,
				initialAmount,
				startYearAmount,
				baseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount2 SetRanks(AmortizedAmount2 model, decimal yearRank, decimal periodRank, Periodicity periodicity)
		{
			return new AmortizedAmount2
			(
				model.AmortizationMethod,
				model.Expression,
				model.Rate,
				yearRank,
				model.YearCount,
				periodRank,
				periodicity,
				model.ForcedAmount,
				model.PreviousAmount,
				model.InitialAmount,
				model.StartYearAmount,
				model.BaseAmount,
				model.ProrataNumerator,
				model.ProrataDenominator,
				model.RoundAmount,
				model.ResidualAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}
		#endregion


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteTypeAttribute    (writer, "AmortizationMethod", this.AmortizationMethod);
			IOHelpers.WriteStringAttribute  (writer, "Expression",         this.Expression);
			IOHelpers.WriteDecimalAttribute (writer, "Rate",               this.Rate);
			IOHelpers.WriteDecimalAttribute (writer, "YearRank",           this.YearRank);
			IOHelpers.WriteDecimalAttribute (writer, "YearCount",          this.YearCount);
			IOHelpers.WriteDecimalAttribute (writer, "PeriodRank",         this.PeriodRank);
			IOHelpers.WriteTypeAttribute    (writer, "Periodicity",        this.Periodicity);

			IOHelpers.WriteDecimalAttribute (writer, "ForcedAmount",       this.ForcedAmount);
			IOHelpers.WriteDecimalAttribute (writer, "PreviousAmount",     this.PreviousAmount);
			IOHelpers.WriteDecimalAttribute (writer, "InitialAmount",      this.InitialAmount);
			IOHelpers.WriteDecimalAttribute (writer, "StartYearAmount",    this.StartYearAmount);
			IOHelpers.WriteDecimalAttribute (writer, "BaseAmount",         this.BaseAmount);
			IOHelpers.WriteDecimalAttribute (writer, "ProrataNumerator",   this.ProrataNumerator);
			IOHelpers.WriteDecimalAttribute (writer, "ProrataDenominator", this.ProrataDenominator);
			IOHelpers.WriteDecimalAttribute (writer, "RoundAmount",        this.RoundAmount);
			IOHelpers.WriteDecimalAttribute (writer, "ResidualAmount",     this.ResidualAmount);

			IOHelpers.WriteTypeAttribute (writer, "EntryScenario", this.EntryScenario);

			IOHelpers.WriteDateAttribute (writer, "Date", this.Date);
			IOHelpers.WriteGuidAttribute (writer, "AssetGuid", this.AssetGuid);
			IOHelpers.WriteGuidAttribute (writer, "EventGuid", this.EventGuid);
			IOHelpers.WriteGuidAttribute (writer, "EntryGuid", this.EntryGuid);
			IOHelpers.WriteIntAttribute  (writer, "EntrySeed", this.EntrySeed);

			writer.WriteEndElement ();
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
	}
}
