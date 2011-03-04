//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Documents
{
	public static class DocumentOptions
	{
		public static string ToString(DocumentOption option)
		{
			return option.ToString ();
		}

		public static DocumentOption Parse(string name)
		{
			return name.ToEnum<DocumentOption> (DocumentOption.None);
		}
	}
}
