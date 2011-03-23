//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library.Internal
{
	static internal class StringBuilderExtension
	{
		public static char LastCharacter(this System.Text.StringBuilder builder)
		{
			string text = builder.ToString ();
			return text.RemoveTag ().LastCharacter ();
		}
	}
}
