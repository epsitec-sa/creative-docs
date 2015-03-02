//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Cette structure définit ce qu'il faut chercher, c'est-à-dire la chaîne
	/// cherchée (pattern) et comment chercher (mode).
	/// </summary>
	public struct SearchDefinition
	{
		public SearchDefinition(string pattern, SearchOptions options)
		{
			this.Pattern = pattern;
			this.Options = options;
		}

		public SearchDefinition(System.Xml.XmlReader reader)
		{
			this.Pattern = IOHelpers.ReadStringAttribute (reader, X.Attr.Pattern);
			this.Options = (SearchOptions) IOHelpers.ReadTypeAttribute (reader, X.Attr.Options, typeof (SearchOptions));

			reader.Read ();
		}


		public bool IsActive
		{
			get
			{
				return !string.IsNullOrEmpty (this.Pattern);
			}
		}


		public SearchDefinition FromPattern(string pattern)
		{
			return new SearchDefinition (pattern, this.Options);
		}

		public SearchDefinition FromOptions(SearchOptions options)
		{
			return new SearchDefinition (this.Pattern, options);
		}


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteStartElement (name);

			IOHelpers.WriteStringAttribute (writer, X.Attr.Pattern, this.Pattern);
			IOHelpers.WriteTypeAttribute (writer, X.Attr.Options, this.Options);

			writer.WriteEndElement ();
		}


		public static SearchDefinition Default = new SearchDefinition (null, SearchOptions.IgnoreCase | SearchOptions.IgnoreDiacritic);


		public readonly string					Pattern;
		public readonly SearchOptions			Options;
	}
}
