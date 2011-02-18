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
					&& ((softId.Substring (1, 2) == "20") || (softId.Substring (0, 3) == "025"))
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

				if (fileExt == ".cre")
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
