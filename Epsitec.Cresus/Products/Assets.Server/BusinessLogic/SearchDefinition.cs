//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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


		public static SearchDefinition Default = new SearchDefinition (null, SearchOptions.IgnoreCase | SearchOptions.IgnoreDiacritic);


		public readonly string					Pattern;
		public readonly SearchOptions			Options;
	}
}
