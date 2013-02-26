using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.TextAccessor
{
	/// <summary>
	/// Cette classe décrit la typographie à appliquer à un segment de texte.
	/// </summary>
	public class TextFormat
	{
		public int			StartIndex;
		public int			Length;
		public Typo			Typo;

		public FormattedText GetFormattedText(FormattedText text)
		{
			return text;
		}
	}
}
