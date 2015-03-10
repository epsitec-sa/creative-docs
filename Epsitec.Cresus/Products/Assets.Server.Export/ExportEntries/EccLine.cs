//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Cette classe est reflète le contenu d'une ligne quelconque d'un fichier .ecc.
	/// Par exemple:
	/// #FSC	9.3 	ECC
	///	#ECF	1; 10/6/2010 11:06:53; "toto.ecf"; 123
	///	#BACKUP	salaires-exemple-2010|%1.salx|%1.ecs/n|%1 (01-01-2010 ~ 31-12-2010).ecs/n
	///	#END
	///	Les lignes autres que #ECF/#ECS/#ECA sont simplement bypassées.
	/// </summary>
	public class EccLine
	{
		public EccLine(string line)
		{
			this.OriginalLine = line;

			var x = line.Split ('\t');

			this.Tag      = x[0];
			this.N        = null;
			this.Date     = null;
			this.Filename = null;
			this.Uid      = null;

			if (x.Length == 2)
			{
				var y = x[1].Split (new string[] { "; " }, System.StringSplitOptions.None);
				if (y.Length == 4)
				{
					this.N        = y[0].ParseInt ();
					this.Date     = EccLine.ParseEccDate (y[1]);
					this.Filename = y[2];
					this.Uid      = y[3].ParseInt ();
				}
			}
		}

		public EccLine(string tag, int n, System.DateTime date, string filename, int uid)
		{
			this.OriginalLine = null;
			this.Tag          = tag;
			this.N            = n;
			this.Date         = date;
			this.Filename     = filename;
			this.Uid          = uid;
		}


		public string							Line
		{
			get
			{
				if (this.IsBody)
				{
					return string.Concat (
						this.Tag, "\t",
						this.N.ToStringIO (), "; ",
						EccLine.ToEccStringIO (this.Date), "; ",
						this.Filename, "; ",
						this.Uid.ToStringIO ());
				}
				else
				{
					return this.OriginalLine;
				}
			}
		}

		public bool								IsBody
		{
			get
			{
				return this.N.HasValue
					&& this.Date.HasValue
					&& !string.IsNullOrEmpty (this.Filename)
					&& this.Uid.HasValue;
			}
		}


		private static string ToEccStringIO(System.DateTime? date)
		{
			if (date.HasValue)
			{
				return date.Value.ToString ("dd/MM/yyyy HH:mm:ss");
			}
			else
			{
				return null;
			}
		}

		private static System.DateTime ParseEccDate(string s)
		{
			return System.DateTime.ParseExact (s, "dd/M/yyyy HH:mm:ss", null);
		}


		private readonly string					OriginalLine;	// ligne originale
		public string							Tag;			// par exemple #FSC, #ECA ou #END
		public int?								N;				// normalement 1
		public System.DateTime?					Date;			// date
		public string							Filename;		// nom du fichier d'écritures, sans les dossiers
		public int?								Uid;			// identificateur unique
	}
}
