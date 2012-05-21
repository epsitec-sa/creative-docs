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
					&& ((softId.Substring (1, 2) == "20") || (softId.Substring (0, 3) == "025") || (softId.Substring (1, 2) == "23"))
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

				if ((fileExt == ".cre") ||
					(fileExt == ".nmc"))
				{
					var compta = new Epsitec.CresusToolkit.CresusComptaDocument (sourcePath);

					if (compta.CheckMetadata ())
					{
						//	Try to derive the start/end year of the file from the associated *.crp
						//	metadata.

						var date1 = compta.BeginDate.Year;
						var date2 = compta.EndDate.Year;

						if ((date1 >= 1980) &&
							(date1 <= date2) &&
							(date2 <= 2999))
						{
							if (date1 == date2)
							{
								sourceName = date1.ToString ("D4");
							}
							else
							{
								sourceName = date1.ToString ("D4") + "-" + date2.ToString ("D4");
							}
						}
					}
					
					if ((string.IsNullOrEmpty (sourceName)) &&
						(fileName.Length > 4))
					{
						int year;

						if (int.TryParse (fileName.Substring (fileName.Length-4), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out year))
						{
							sourceName = year.ToString ("D4");
						}
					}
				}
			}

			if (string.IsNullOrEmpty (sourceName))
			{
				sourceName = "?";
			}
			return sourceName;
		}

		protected static string MakeFullDate(string text, string year)
		{
			if (text == null)
			{
				return text;
			}

			string fullDate = Compta.ExtractDate (text, year);

			if (fullDate == null)
			{
				if (text.Contains (year))
				{
					return text;
				}
				else
				{
					return string.Concat (text, " (", year, ")");
				}
			}
			else
			{
				string[] words = text.Split (' ');
				words[words.Length-1] = fullDate;
				return string.Join (" ", words);
			}
		}

		protected static string RemoveLineBreaks(string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return text;
			}

			string[] lines = text.Split (new string[] { "\n", "\\n" }, System.StringSplitOptions.RemoveEmptyEntries);

			if (lines.Length == 1)
			{
				return text;
			}

			if ((lines[0].Length > 4) &&
				(text.Length > 10))
			{
				string[] words = lines[0].Split (' ');

				if (words[0].Length > 3)
				{
					words[0] = words[0].Substring (0, 1) + ".";
					lines[0] = string.Join (" ", words);
				}
			}

			return string.Join (" ", lines);
		}

		protected static string ExtractDate(string textWithDateSuffix, string year)
		{
			string dayMonth = Compta.ExtractDate (textWithDateSuffix);

			if (dayMonth != null)
			{
				return string.Concat (dayMonth, ".", year);
			}

			return null;
		}

		protected static string ExtractDate(string textWithDateSuffix)
		{
			if (string.IsNullOrEmpty (textWithDateSuffix))
			{
				return null;
			}

			string lastWord = textWithDateSuffix.Split (' ').Last ();
			string[] numbers = lastWord.Split ('.', '/', ';', ':');

			if (numbers.Length == 2)
			{
				int n1, n2;

				if ((int.TryParse (numbers[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out n1)) &&
					(int.TryParse (numbers[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out n2)))
				{
					return string.Format ("{0:00}.{1:00}", n1, n2);
				}
			}
			
			return null;
		}

		public override GraphDataCategory GetCategory(ChartSeries series)
		{
			string label  = series.Label;
			string number = label.Split ('\t')[0];

			if (this.Meta != null)
			{
				if (this.compta == null)
				{
					if (this.Meta.ContainsKey ("Path"))
					{
						var path = this.Meta["Path"];
						this.compta = new Epsitec.CresusToolkit.CresusComptaDocument (path);
					}
				}

				if (this.compta != null)
				{
					var account = this.compta.GetAccounts (x => x.Number == number, true).FirstOrDefault ();

					if (account != null)
					{
						switch (account.Category)
						{
							case Epsitec.CresusToolkit.CresusComptaCategory.Actif:
								return new GraphDataCategory (1, "Actif");
							
							case Epsitec.CresusToolkit.CresusComptaCategory.Passif:
								return new GraphDataCategory (2, "Passif");
							
							case Epsitec.CresusToolkit.CresusComptaCategory.Produit:
								return new GraphDataCategory (3, "Produit");
							
							case Epsitec.CresusToolkit.CresusComptaCategory.Charge:
								return new GraphDataCategory (4, "Charge");
							
							case Epsitec.CresusToolkit.CresusComptaCategory.Exploitation:
								return new GraphDataCategory (5, "Exploitation");
						}
					}
				}
			}

			return base.GetCategory (series);
		}

		public static System.DateTime ParseDate(string text)
		{
			return Epsitec.CresusToolkit.CresusComptaDocument.ParseDate (text);
		}

		private readonly string view;
		private Epsitec.CresusToolkit.CresusComptaDocument compta;
	}
}
