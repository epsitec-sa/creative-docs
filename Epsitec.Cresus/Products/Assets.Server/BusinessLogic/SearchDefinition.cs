//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{

	public struct SearchDefinition
	{
		public SearchDefinition(string pattern, SearchMode mode)
		{
			this.Pattern = pattern;
			this.Mode    = mode;
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
			return new SearchDefinition (pattern, this.Mode);
		}

		public SearchDefinition FromMode(SearchMode mode)
		{
			return new SearchDefinition (this.Pattern, mode);
		}


		public static SearchDefinition Default = new SearchDefinition (null, SearchMode.IgnoreCase | SearchMode.IgnoreDiacritic | SearchMode.Fragment);


		public readonly string					Pattern;
		public readonly SearchMode				Mode;
	}
}
