//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AbstractArticleParameterDefinitionEntity
	{
		public const char SeparatorChar = (char) 0x2219;					// '∙'
		public const string Separator = SeparatorChar.ToString ();			// "∙"

		public static string[] Split(string text)
		{
			return text.Split (AbstractArticleParameterDefinitionEntity.SeparatorChar);
		}

		public static string Join(params string[] values)
		{
			return string.Join (AbstractArticleParameterDefinitionEntity.Separator, values);
		}
	}
}
