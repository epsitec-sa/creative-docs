//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Data
{
	public struct AmortizedAmount : System.IEquatable<AmortizedAmount>
	{
		public AmortizedAmount(decimal? initialAmount, decimal? finalAmount,
			EntryScenario entryScenario, System.DateTime date,
			Guid assetGuid, Guid eventGuid, Guid entryGuid, int entrySeed)
		{
			this.InitialAmount = initialAmount;
			this.FinalAmount   = finalAmount;

			this.EntryScenario = entryScenario;
			this.Date          = date;
			this.AssetGuid     = assetGuid;
			this.EventGuid     = eventGuid;
			this.EntryGuid     = entryGuid;
			this.EntrySeed     = entrySeed;
		}

		public AmortizedAmount(decimal? finalValue)
		{
			this.InitialAmount = null;
			this.FinalAmount   = finalValue;

			this.EntryScenario = EntryScenario.None;
			this.Date          = System.DateTime.MinValue;
			this.AssetGuid     = Guid.Empty;
			this.EventGuid     = Guid.Empty;
			this.EntryGuid     = Guid.Empty;
			this.EntrySeed     = 0;
		}

		public AmortizedAmount(EntryScenario entryScenario)
		{
			this.InitialAmount = null;
			this.FinalAmount   = null;

			this.EntryScenario = entryScenario;
			this.Date          = System.DateTime.MinValue;
			this.AssetGuid     = Guid.Empty;
			this.EventGuid     = Guid.Empty;
			this.EntryGuid     = Guid.Empty;
			this.EntrySeed     = 0;
		}

		public AmortizedAmount(System.Xml.XmlReader reader)
		{
			this.InitialAmount = IOHelpers.ReadDecimalAttribute (reader, "InitialAmount");
			this.FinalAmount   = IOHelpers.ReadDecimalAttribute (reader, "FinalAmount");

			this.EntryScenario = (EntryScenario) IOHelpers.ReadTypeAttribute (reader, "EntryScenario", typeof (EntryScenario));
			this.Date          = IOHelpers.ReadDateAttribute (reader, "Date").GetValueOrDefault ();
			this.AssetGuid     = IOHelpers.ReadGuidAttribute (reader, "AssetGuid");
			this.EventGuid     = IOHelpers.ReadGuidAttribute (reader, "EventGuid");
			this.EntryGuid     = IOHelpers.ReadGuidAttribute (reader, "EntryGuid");
			this.EntrySeed     = IOHelpers.ReadIntAttribute  (reader, "EntrySeed").GetValueOrDefault ();

			reader.Read ();  // on avance plus loin
		}


		public readonly decimal?				InitialAmount;
		public readonly decimal?				FinalAmount;

		public readonly EntryScenario			EntryScenario;
		public readonly System.DateTime			Date;
		public readonly Guid					AssetGuid;
		public readonly Guid					EventGuid;
		public readonly Guid					EntryGuid;
		public readonly int						EntrySeed;


		#region Constructors helpers
		public static AmortizedAmount SetFinalAmount(AmortizedAmount model, decimal? finalAmount)
		{
			return new AmortizedAmount
			(
				model.InitialAmount,
				finalAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
				model.EntryGuid,
				model.EntrySeed
			);
		}

		public static AmortizedAmount SetAmounts(AmortizedAmount model, decimal? initialAmount, decimal? finalAmount)
		{
			return new AmortizedAmount
			(
				initialAmount,
				finalAmount,
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
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
				scenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
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
				model.EntryScenario,
				model.Date,
				model.AssetGuid,
				model.EventGuid,
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
				&& this.EntryScenario == other.EntryScenario
				&& this.Date          == other.Date
				&& this.AssetGuid     == other.AssetGuid
				&& this.EventGuid     == other.EventGuid
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
				 ^ this.EntryScenario.GetHashCode ()
				 ^ this.Date         .GetHashCode ()
				 ^ this.AssetGuid    .GetHashCode ()
				 ^ this.EventGuid    .GetHashCode ()
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

			IOHelpers.WriteTypeAttribute    (writer, "EntryScenario", this.EntryScenario);
			IOHelpers.WriteDateAttribute    (writer, "Date",          this.Date);
			IOHelpers.WriteGuidAttribute    (writer, "AssetGuid",     this.AssetGuid);
			IOHelpers.WriteGuidAttribute    (writer, "EventGuid",     this.EventGuid);
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
