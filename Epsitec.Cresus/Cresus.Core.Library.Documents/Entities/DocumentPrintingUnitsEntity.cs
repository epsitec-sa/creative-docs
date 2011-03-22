//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Documents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentPrintingUnitsEntity
	{
		public override FormattedText GetSummary()
		{
			//	L'espace entre les <br/> est nécessaire, à cause de FormatText qui fait du zèle !
			return TextFormatter.FormatText (this.Name, FormattedText.Concat ("<br/>________________________________________<br/> <br/>"
				/*, this.GetPrintingUnitsSummary ()*/));
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}

#if false
		private FormattedText GetPrintingUnitsSummary()
		{
			var dict = this.GetPrintingUnits ();
			var all = VerbosePageType.GetAll ();
			var builder = new System.Text.StringBuilder ();

			foreach (var pageType in all)
			{
				if (dict.ContainsPageType (pageType.Type))
				{
					var unit = dict.GetPrintingUnit (pageType.Type);

					if (string.IsNullOrEmpty (unit))
					{
						unit = "<i>(aucune)</i>";
					}

					builder.Append (pageType.ShortDescription);
					builder.Append (" = ");
					builder.Append (unit);
					builder.Append ("<br/>");
				}
			}

			return builder.ToString ();
		}
#endif

#if false
		public PrintingUnitDictionary GetPrintingUnits()
		{
			//	Retourne le dictionnaire "type de pages" / "unité d'impression".
			// TODO: Ajouter un cache pour accélérer l'accès !
			var dict = new PrintingUnitDictionary ();

			if (this.SerializedData != null)
			{
				string s = System.Text.Encoding.UTF8.GetString (this.SerializedData);

				if (!string.IsNullOrEmpty (s))
				{
					// Exemple de table obtenue: "ForAllPages", "Blanc", "ForPagesCopy", "Recyclé", ""
					string[] split = s.Split ('◊');

					for (int i = 0; i < split.Length-1; i+=2)
					{
						var type = PageTypeConverter.Parse (split[i]);
						dict[type] = split[i+1];
					}
				}
			}

			return dict;
		}

		public void SetPrintingUnits(PrintingUnitDictionary options)
		{
			//	Spécifie le dictionnaire "type de pages" / "unité d'impression".
			if (options.Count == 0)
			{
				this.SerializedData = null;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				foreach (var pair in options.ContentPair)
				{
					builder.Append (pair.Key);
					builder.Append ("◊");
					builder.Append (pair.Value);
					builder.Append ("◊");
				}

				// Exemple de chaîne obtenue: "ForAllPages◊Blanc◊ForPagesCopy◊Recyclé◊"
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes (builder.ToString ());
				this.SerializedData = bytes;
			}
		}
#endif

		public List<PageType> GetPageTypes()
		{
			var pageTypes = new List<PageType> ();

			if (this.SerializedData != null)
			{
				string s = System.Text.Encoding.UTF8.GetString (this.SerializedData);

				if (!string.IsNullOrEmpty (s))
				{
					string[] split = s.Split ('◊');

					for (int i=0; i<split.Length; i++)
					{
						PageType pageType;
						if (System.Enum.TryParse (split[i], out pageType))
						{
							pageTypes.Add (pageType);
						}
					}
				}
			}

			return pageTypes;
		}

		public void SetPageTypes(List<PageType> pageTypes)
		{
			if (pageTypes == null || pageTypes.Count == 0)
			{
				this.SerializedData = null;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				for (int i=0; i<pageTypes.Count; i++)
				{
					builder.Append (pageTypes[i].ToString ());

					if (i < pageTypes.Count-1)
					{
						builder.Append ("◊");
					}
				}

				byte[] bytes = System.Text.Encoding.UTF8.GetBytes (builder.ToString ());
				this.SerializedData = bytes;
			}
		}
	}
}
