//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Graph
{
	/// <summary>
	/// Cette classe gère les échanges de données graphiques avec StringList.
	/// </summary>
	public class GraphicData
	{
		public GraphicData(GraphicMode mode, FormattedText name, decimal minValue, decimal maxValue)
		{
			//	Construit une nouvelle instance à partir des paramètres distincts.
			this.mode     = mode;
			this.name     = name;
			this.minValue = minValue;
			this.maxValue = maxValue;

			this.values = new List<decimal> ();
		}

		public GraphicData(string text)
		{
			//	Construit une nouvelle instance à partir des données sérialisées.
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (text));
			System.Diagnostics.Debug.Assert (text.StartsWith (StringArray.SpecialContentGraphicValue));

			var words = text.Split (GraphicData.separator);
			System.Diagnostics.Debug.Assert (words.Length >= 6);

			this.mode     = (GraphicMode) System.Enum.Parse (typeof (GraphicMode), words[1]);
			this.name     = words[2];
			this.minValue = Converters.ParseDecimal (words[3]).GetValueOrDefault ();
			this.maxValue = Converters.ParseDecimal (words[4]).GetValueOrDefault ();

			this.values = new List<decimal> ();

			for (int i = 5; i < words.Length; i++)
			{
				decimal value = Converters.ParseDecimal (words[i]).GetValueOrDefault ();
				this.values.Add (value);
			}
		}


		public GraphicMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		public FormattedText Name
		{
			get
			{
				return this.name;
			}
		}

		public decimal MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public decimal MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public List<decimal> Values
		{
			get
			{
				return this.values;
			}
		}


		public new string ToString()
		{
			//	Retourne les toutes données sérialisées sous forme d'une string.
			var builder = new System.Text.StringBuilder ();

			builder.Append (StringArray.SpecialContentGraphicValue);
			builder.Append (GraphicData.separator);
			builder.Append (this.mode.ToString ());
			builder.Append (GraphicData.separator);
			builder.Append (this.name);
			builder.Append (GraphicData.separator);
			builder.Append (Converters.DecimalToString (this.minValue, null));
			builder.Append (GraphicData.separator);
			builder.Append (Converters.DecimalToString (this.maxValue, null));

			foreach (var value in this.values)
			{
				builder.Append (GraphicData.separator);
				builder.Append (Converters.DecimalToString (value, null));
			}

			return builder.ToString ();
		}

		public void Parse(string text)
		{
		}


		private static readonly char separator = '◊';

		private readonly GraphicMode		mode;
		private readonly FormattedText		name;
		private readonly decimal			minValue;
		private readonly decimal			maxValue;
		private readonly List<decimal>		values;
	}
}
