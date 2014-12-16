﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public struct AmortizedAmount : System.IEquatable<AmortizedAmount>
	{
		public AmortizedAmount(decimal? initialAmount, decimal? finalAmount, string trace, string error,
			EntryScenario entryScenario, Guid entryGuid, int entrySeed)
		{
			this.InitialAmount = initialAmount;
			this.FinalAmount   = finalAmount;
			this.Trace         = trace;
			this.Error         = error;

			this.EntryScenario = entryScenario;
			this.EntryGuid     = entryGuid;
			this.EntrySeed     = entrySeed;
		}

		public AmortizedAmount(decimal? finalValue)
		{
			this.InitialAmount = null;
			this.FinalAmount   = finalValue;
			this.Trace         = null;
			this.Error         = null;

			this.EntryScenario = EntryScenario.None;
			this.EntryGuid     = Guid.Empty;
			this.EntrySeed     = 0;
		}

		public AmortizedAmount(EntryScenario entryScenario)
		{
			this.InitialAmount = null;
			this.FinalAmount   = null;
			this.Trace         = null;
			this.Error         = null;

			this.EntryScenario = entryScenario;
			this.EntryGuid     = Guid.Empty;
			this.EntrySeed     = 0;
		}

		public AmortizedAmount(System.Xml.XmlReader reader)
		{
			this.InitialAmount = IOHelpers.ReadDecimalAttribute (reader, "InitialAmount");
			this.FinalAmount   = IOHelpers.ReadDecimalAttribute (reader, "FinalAmount");
			this.Trace         = IOHelpers.ReadStringAttribute  (reader, "Trace");
			this.Error         = IOHelpers.ReadStringAttribute  (reader, "Error");

			this.EntryScenario = (EntryScenario) IOHelpers.ReadTypeAttribute (reader, "EntryScenario", typeof (EntryScenario));
			this.EntryGuid     = IOHelpers.ReadGuidAttribute (reader, "EntryGuid");
			this.EntrySeed     = IOHelpers.ReadIntAttribute  (reader, "EntrySeed").GetValueOrDefault ();

			reader.Read ();  // on avance plus loin
		}


		public readonly decimal?				InitialAmount;
		public readonly decimal?				FinalAmount;
		public readonly string					Trace;
		public readonly string					Error;

		public readonly EntryScenario			EntryScenario;
		public readonly Guid					EntryGuid;
		public readonly int						EntrySeed;


		#region Constructors helpers
		public static AmortizedAmount SetInitialAmount(AmortizedAmount model, decimal? initialAmount)
		{
			return new AmortizedAmount
			(
				initialAmount,
				model.FinalAmount,
				model.Trace,
				model.Error,
				model.EntryScenario,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetFinalAmount(AmortizedAmount model, decimal? finalAmount)
		{
			return new AmortizedAmount
			(
				model.InitialAmount,
				finalAmount,
				model.Trace,
				model.Error,
				model.EntryScenario,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetAmounts(AmortizedAmount model, decimal? initialAmount, decimal? finalAmount, string trace, string error)
		{
			return new AmortizedAmount
			(
				initialAmount,
				finalAmount,
				trace,
				error,
				model.EntryScenario,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetEntryScenario(AmortizedAmount model, EntryScenario scenario)
		{
			return new AmortizedAmount
			(
				model.InitialAmount,
				model.FinalAmount,
				model.Trace,
				model.Error,
				scenario,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetEntry(AmortizedAmount model, Guid entryGuid, int entrySeed)
		{
			return new AmortizedAmount
			(
				model.InitialAmount,
				model.FinalAmount,
				model.Trace,
				model.Error,
				model.EntryScenario,
				entryGuid,
				entrySeed
			);
		}
		#endregion


		#region IEquatable<AmortizedAmount2> Members
		public bool Equals(AmortizedAmount other)
		{
			return this.InitialAmount == other.InitialAmount
				&& this.FinalAmount   == other.FinalAmount
				&& this.Trace         == other.Trace
				&& this.Error         == other.Error
				&& this.EntryScenario == other.EntryScenario
				&& this.EntryGuid     == other.EntryGuid
				&& this.EntrySeed     == other.EntrySeed;
		}
		#endregion

		public override bool Equals(object obj)
		{
			if (obj is AmortizedAmount)
			{
				return this.Equals ((AmortizedAmount) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.InitialAmount.GetHashCode ()
				 ^ this.FinalAmount  .GetHashCode ()
				 ^ this.Trace        .GetHashCode ()
				 ^ this.Error        .GetHashCode ()
				 ^ this.EntryScenario.GetHashCode ()
				 ^ this.EntryGuid    .GetHashCode ()
				 ^ this.EntrySeed    .GetHashCode ();
		}

		public static bool operator ==(AmortizedAmount a, AmortizedAmount b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(AmortizedAmount a, AmortizedAmount b)
		{
			return !(a == b);
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteDecimalAttribute (writer, "InitialAmount", this.InitialAmount);
			IOHelpers.WriteDecimalAttribute (writer, "FinalAmount",   this.FinalAmount);
			IOHelpers.WriteStringAttribute  (writer, "Trace",         this.Trace);
			IOHelpers.WriteStringAttribute  (writer, "Error",         this.Error);

			IOHelpers.WriteTypeAttribute    (writer, "EntryScenario", this.EntryScenario);
			IOHelpers.WriteGuidAttribute    (writer, "EntryGuid",     this.EntryGuid);
			IOHelpers.WriteIntAttribute     (writer, "EntrySeed",     this.EntrySeed);

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
