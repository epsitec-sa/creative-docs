//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.ImportConverters
{
	public abstract class Compta : AbstractImportConverter
	{
		protected Compta(string name, string view)
			: base (name)
		{
			this.view = view;
		}

		public override bool CheckCompatibleMeta(IDictionary<string, string> meta)
		{
			string softId;
			string viewNumber;

			if ((meta.TryGetValue ("SoftID", out softId)) &&
				(meta.TryGetValue ("View", out viewNumber)))
			{
				return (softId.Length == 24)
					&& (softId.Substring (1, 2) == "20")
					&& (viewNumber == this.view);
			}

			return false;
		}

		protected static string GetSourceName(string sourcePath)
		{
			string sourceName = "";

			if (string.IsNullOrEmpty (sourcePath))
			{
				//	Unknown source - probably the clipboard
			}
			else
			{
				var fileExt  = System.IO.Path.GetExtension (sourcePath).ToLowerInvariant ();
				var fileName = System.IO.Path.GetFileNameWithoutExtension (sourcePath);

				if ((fileExt == ".cre") &&
					(fileName.Length > 4))
				{
					int year;

					if (int.TryParse (fileName.Substring (fileName.Length-4), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out year))
					{
						sourceName = year.ToString ("D4");
					}
				}
			}

			if (string.IsNullOrEmpty (sourceName))
			{
				sourceName = "?";
			}
			return sourceName;
		}

		public override GraphDataCategory GetCategory(ChartSeries series)
		{
			string cat = series.Label.Substring (0, 1);

			switch (cat)
			{
				case "1":
					return new GraphDataCategory (1, "Actif");

				case "2":
					return new GraphDataCategory (2, "Passif");

				case "3":
				case "7":
					return new GraphDataCategory (3, "Produit");

				case "4":
				case "5":
				case "6":
				case "8":
					return new GraphDataCategory (4, "Charge");

				case "9":
					return new GraphDataCategory (5, "Exploitation");
			}

			return base.GetCategory (series);
		}

		public static System.DateTime ParseDate(string text)
		{
			if ((text.Length == 8) &&
				(text[2] == '.') &&
				(text[5] == '.'))
			{
				int day   = int.Parse (text.Substring (0, 2), System.Globalization.CultureInfo.InvariantCulture);
				int month = int.Parse (text.Substring (3, 2), System.Globalization.CultureInfo.InvariantCulture);
				int year  = int.Parse (text.Substring (6, 2), System.Globalization.CultureInfo.InvariantCulture);

				if (year < 80)
				{
					year += 2000;
				}
				else
				{
					year += 1900;
				}

				return new System.DateTime (year, month, day);
			}
			else
			{
				throw new System.FormatException ("Invalid date format");
			}
		}

        private readonly string view;
	}
}
