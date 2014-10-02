//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Settings
{
	public struct SearchInfo
	{
		public SearchInfo(SearchDefinition definition)
		{
			this.Definition = definition;
			this.LastPatterns = new List<string> ();
		}

		public SearchInfo FromDefinition(SearchDefinition definition)
		{
			var info = new SearchInfo (definition);
			info.LastPatterns.AddRange (this.LastPatterns);

			return info;
		}

		public static SearchInfo Default = new SearchInfo (SearchDefinition.Default);

		public readonly SearchDefinition		Definition;
		public readonly List<string>			LastPatterns;
	}
}
