//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Library.Internal
{
	public static class StringBuilderExtensions
	{
		public static char LastCharacter(this System.Text.StringBuilder builder)
		{
			string text = builder.ToString ();
			return text.RemoveTag ().LastCharacter ();
		}
	}
}
