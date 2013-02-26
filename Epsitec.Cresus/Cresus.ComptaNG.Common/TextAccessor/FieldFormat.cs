using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.TextAccessor
{
	/// <summary>
	/// Cette classe décrit la typographie à appliquer au texte d'un champ.
	/// La typographique peut contenir plusieurs segments différents.
	/// </summary>
	public class FieldFormat
	{
		public FieldType			FieldType;
		public List<TextFormat>		TextFormats;

		public FormattedText GetFormattedText(FormattedText text)
		{
			//	Juste une ébauche de ce qu'il faudra faire :
			//	foreach (var format in this.TextFormats)
			//	{
			//		text = Merge (text, format.GetFormattedText (text));
			//	}

			return text;
		}
	}
}
