//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;

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

		public ExpressionSimulationParams(System.Xml.XmlReader reader)
		{
			this.Range             = DateRange.Empty;
			this.Periodicity       = Periodicity.Unknown;
			this.InitialAmount     = 0.0m;
			this.ExtraDate         = null;
			this.ExtraAmount       = null;
			this.AmortizationSuppl = false;
			this.AdjustDate        = null;
			this.AdjustAmount      = null;

			this.arguments = new Dictionary<ObjectField, object> ();

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Definitions":
							{
								this.Range             =               IOHelpers.ReadDateRangeAttribute (reader, "Range");
								this.Periodicity       = (Periodicity) IOHelpers.ReadTypeAttribute      (reader, "Periodicity", typeof (Periodicity));
								this.InitialAmount     =               IOHelpers.ReadDecimalAttribute   (reader, "InitialAmount").GetValueOrDefault ();
								this.ExtraDate         =               IOHelpers.ReadDateAttribute      (reader, "ExtraDate");
								this.ExtraAmount       =               IOHelpers.ReadDecimalAttribute   (reader, "ExtraAmount");
								this.AmortizationSuppl =               IOHelpers.ReadBoolAttribute      (reader, "AmortizationSuppl");
								this.AdjustDate        =               IOHelpers.ReadDateAttribute      (reader, "AdjustDate");
								this.AdjustAmount      =               IOHelpers.ReadDecimalAttribute   (reader, "AdjustAmount");

								reader.Read ();  // on avance sur le noeud suivant
							}
							break;

						case "Arguments":
							this.DeserializeArguments (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
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


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			this.SerializeDefinitions (writer);
			this.SerializeArguments   (writer);

			writer.WriteEndElement ();
		}

		private void SerializeDefinitions(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Definitions");

			IOHelpers.WriteDateRangeAttribute (writer, "Range",             this.Range);
			IOHelpers.WriteTypeAttribute      (writer, "Periodicity",       this.Periodicity);
			IOHelpers.WriteDecimalAttribute   (writer, "InitialAmount",     this.InitialAmount);
			IOHelpers.WriteDateAttribute      (writer, "ExtraDate",         this.ExtraDate);
			IOHelpers.WriteDecimalAttribute   (writer, "ExtraAmount",       this.ExtraAmount);
			IOHelpers.WriteBoolAttribute      (writer, "AmortizationSuppl", this.AmortizationSuppl);
			IOHelpers.WriteDateAttribute      (writer, "AdjustDate",        this.AdjustDate);
			IOHelpers.WriteDecimalAttribute   (writer, "AdjustAmount",      this.AdjustAmount);

			writer.WriteEndElement ();
		}

		private void SerializeArguments(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Arguments");

			foreach (var pair in this.arguments)
			{
				writer.WriteStartElement ("Argument");

				IOHelpers.WriteObjectFieldAttribute (writer, "ObjectField", pair.Key);
				this.SerializeArgumentValue (writer, pair.Value);

				writer.WriteEndElement ();
			}

			writer.WriteEndElement ();
		}

		private void SerializeArgumentValue(System.Xml.XmlWriter writer, object value)
		{
			if (value != null)
			{
				if (value is decimal)
				{
					IOHelpers.WriteDecimalAttribute (writer, "Decimal", (decimal) value);
				}
				else if (value is int)
				{
					IOHelpers.WriteIntAttribute (writer, "Int", (int) value);
				}
				else if (value is bool)
				{
					if ((bool) value)
					{
						IOHelpers.WriteBoolAttribute (writer, "Bool", (bool) value);
					}
				}
				else if (value is System.DateTime)
				{
					IOHelpers.WriteDateAttribute (writer, "Date", (System.DateTime) value);
				}
				else if (value is string)
				{
					IOHelpers.WriteStringAttribute (writer, "String", (string) value);
				}
				else
				{
					throw new System.InvalidOperationException (string.Format ("Invalid ArgumentType {0}", value.GetType ()));
				}
			}
		}

		private void DeserializeArguments(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Argument":
							this.DeserializeArgument (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeArgument(System.Xml.XmlReader reader)
		{
			var field = (ObjectField) IOHelpers.ReadObjectFieldAttribute (reader, "ObjectField");
			var obj = this.DeserializeArgumentValue (reader);

			if (obj != null)
			{
				this.arguments.Add (field, obj);
			}

			reader.Read ();  // on avance sur le noeud suivant
		}

		private object DeserializeArgumentValue(System.Xml.XmlReader reader)
		{
			var d = IOHelpers.ReadDecimalAttribute (reader, "Decimal");
			if (d.HasValue)
			{
				return d;
			}

			var i = IOHelpers.ReadIntAttribute (reader, "Int");
			if (i.HasValue)
			{
				return d;
			}

			var b = IOHelpers.ReadBoolAttribute (reader, "Bool");
			if (b)
			{
				return b;
			}

			var date = IOHelpers.ReadDateAttribute (reader, "Date");
			if (date.HasValue)
			{
				return date;
			}

			var s = IOHelpers.ReadStringAttribute (reader, "String");
			if (!string.IsNullOrEmpty (s))
			{
				return s;
			}

			return null;
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
