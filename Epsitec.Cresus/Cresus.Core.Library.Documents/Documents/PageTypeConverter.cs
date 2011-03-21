//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Documents
{
	public static class PageTypeConverter
	{
		public static string ToString(PageType type)
		{
			return type.ToString ();
		}

		public static PageType Parse(string name)
		{
			return name.ToEnum<PageType> (PageType.Unknown);
		}
	}
}
