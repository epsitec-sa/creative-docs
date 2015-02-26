//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

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
						case X.Definitions:
							{
								this.Range             =               reader.ReadDateRangeAttribute (X.Attr.Range);
								this.Periodicity       = (Periodicity) reader.ReadTypeAttribute      (X.Attr.Periodicity, typeof (Periodicity));
								this.InitialAmount     =               reader.ReadDecimalAttribute   (X.Attr.InitialAmount).GetValueOrDefault ();
								this.ExtraDate         =               reader.ReadDateAttribute      (X.Attr.ExtraDate);
								this.ExtraAmount       =               reader.ReadDecimalAttribute   (X.Attr.ExtraAmount);
								this.AmortizationSuppl =               reader.ReadBoolAttribute      (X.Attr.AmortizationSuppl);
								this.AdjustDate        =               reader.ReadDateAttribute      (X.Attr.AdjustDate);
								this.AdjustAmount      =               reader.ReadDecimalAttribute   (X.Attr.AdjustAmount);

								reader.Read ();  // on avance sur le noeud suivant
							}
							break;

						case X.Arguments:
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
			writer.WriteStartElement (X.Definitions);

			writer.WriteDateRangeAttribute (X.Attr.Range,             this.Range);
			writer.WriteTypeAttribute      (X.Attr.Periodicity,       this.Periodicity);
			writer.WriteDecimalAttribute   (X.Attr.InitialAmount,     this.InitialAmount);
			writer.WriteDateAttribute      (X.Attr.ExtraDate,         this.ExtraDate);
			writer.WriteDecimalAttribute   (X.Attr.ExtraAmount,       this.ExtraAmount);
			writer.WriteBoolAttribute      (X.Attr.AmortizationSuppl, this.AmortizationSuppl);
			writer.WriteDateAttribute      (X.Attr.AdjustDate,        this.AdjustDate);
			writer.WriteDecimalAttribute   (X.Attr.AdjustAmount,      this.AdjustAmount);

			writer.WriteEndElement ();
		}

		private void SerializeArguments(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Arguments);

			foreach (var pair in this.arguments)
			{
				writer.WriteStartElement (X.Argument);

				writer.WriteObjectFieldAttribute (X.Attr.ObjectField, pair.Key);
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
					writer.WriteDecimalAttribute (X.Attr.Decimal, (decimal) value);
				}
				else if (value is int)
				{
					writer.WriteIntAttribute (X.Attr.Int, (int) value);
				}
				else if (value is bool)
				{
					if ((bool) value)
					{
						writer.WriteBoolAttribute (X.Attr.Bool, (bool) value);
					}
				}
				else if (value is System.DateTime)
				{
					writer.WriteDateAttribute (X.Attr.Date, (System.DateTime) value);
				}
				else if (value is string)
				{
					writer.WriteStringAttribute (X.Attr.String, (string) value);
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
						case X.Argument:
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
			var field = (ObjectField) reader.ReadObjectFieldAttribute (X.Attr.ObjectField);
			var obj = this.DeserializeArgumentValue (reader);

			if (obj != null)
			{
				this.arguments.Add (field, obj);
			}

			reader.Read ();  // on avance sur le noeud suivant
		}

		private object DeserializeArgumentValue(System.Xml.XmlReader reader)
		{
			var d = reader.ReadDecimalAttribute (X.Attr.Decimal);
			if (d.HasValue)
			{
				return d;
			}

			var i = reader.ReadIntAttribute (X.Attr.Int);
			if (i.HasValue)
			{
				return d;
			}

			var b = reader.ReadBoolAttribute (X.Attr.Bool);
			if (b)
			{
				return b;
			}

			var date = reader.ReadDateAttribute (X.Attr.Date);
			if (date.HasValue)
			{
				return date;
			}

			var s = reader.ReadStringAttribute (X.Attr.String);
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
