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
			return TextFormatter.FormatText (this.Name, FormattedText.Concat ("<br/> <br/>", this.GetPageTypesSummary ()));
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

		private FormattedText GetPageTypesSummary()
		{
			var builder = new System.Text.StringBuilder ();
			builder.Append ("Pages pouvant être imprimées par cette unité :<br/>");

			var list = this.GetPageTypes ();

			if (list.Count == 0)
			{
				builder.Append ("● <i>Aucune</i><br/>");
			}
			else
			{
				foreach (var pageType in list)
				{
					builder.Append ("● ");
					builder.Append (pageType.ToString ());  // TODO: Comment accéder à Epsitec.Cresus.Core.Documents.Verbose.VerbosePageType pour afficher le PageType en clair ?
					builder.Append ("<br/>");
				}
			}

			return builder.ToString ();
		}

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
