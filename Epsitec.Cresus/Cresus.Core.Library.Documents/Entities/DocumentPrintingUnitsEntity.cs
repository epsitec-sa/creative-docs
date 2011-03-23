//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Documents;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

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
			var builder = new TextBuilder ();

			builder.Append ("Pages imprimables par cette unité :<br/>");

			var list = this.GetPageTypes ();

			if (list.Count == 0)
			{
				builder.Append ("● <i>Aucune</i><br/>");
			}
			else
			{
				foreach (var pageType in list)
				{
					builder.Append ("●");
					builder.Append (pageType);  // VerbosePageType.PrettyPrinter convertit PageType en texte clair (ShortDescription en l'occurrence)
					builder.Append ("<br/>");
				}
			}

			return builder.ToFormattedText (detailLevel: TextFormatterDetailLevel.Default);
		}

		public List<PageType> GetPageTypes()
		{
			var pageTypes = new List<PageType> ();

			if (this.SerializedData != null)
			{
				foreach (var item in StringPacker.UnpackFromBytes (this.SerializedData))
				{
					PageType pageType = InvariantConverter.ToEnum<PageType> (item, PageType.Unknown);

					if (pageType != PageType.Unknown)
					{
						pageTypes.Add (pageType);
					}
				}
			}

			return pageTypes;
		}

		public void SetPageTypes(IEnumerable<PageType> pageTypes)
		{
			if ((pageTypes == null) || (pageTypes.Any () == false))
			{
				this.SerializedData = null;
			}
			else
			{
				this.SerializedData = StringPacker.PackToBytes (pageTypes.Select (x => x.ToString ()));
			}
		}
	}
}
