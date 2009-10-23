//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class ChartSeries
	{
		public ChartSeries()
		{
			this.values = new List<ChartValue> ();
		}

		public ChartSeries(IEnumerable<ChartValue> collection)
			: this ()
		{
			this.values.AddRange (collection);
		}

		public ChartSeries(string label, IEnumerable<ChartValue> collection)
			: this (collection)
		{
			this.Label = label;
		}

		
		public string Label
		{
			get;
			set;
		}
		
		public IList<ChartValue> Values
		{
			get
			{
				return this.values;
			}
		}


		public XElement SaveSettings(XElement xml)
		{
			xml.Add (new XAttribute ("label", this.Label ?? ""));
			xml.Add (new XElement ("values",
				this.Values.Select (value =>
					new XElement ("value",
						new XAttribute ("x", value.Label ?? ""),
						new XAttribute ("y", value.Value)))));

			return xml;
		}

		public void RestoreSettings(XElement xml)
		{
			this.Label = (string) xml.Attribute ("label");
			this.values.AddRange (xml.Elements ("value").Select (value => new ChartValue ((string) value.Attribute ("x"), (double) value.Attribute ("y"))));
		}


		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (this.Label);
			buffer.Append (">");

			bool first = true;

			foreach (var value in this.values)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					buffer.Append (":");
				}

				buffer.Append (value.ToString ());
			}

			return buffer.ToString ();
		}


		private readonly List<ChartValue> values;
	}
}
