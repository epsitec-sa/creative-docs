using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;
using Epsitec.Cresus.ComptaNG.Common.Records;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;

namespace Epsitec.Cresus.ComptaNG.Common.TextAccessor
{
	/// <summary>
	/// Cette classe permet d'accéder aux champs d'un Record sous une forme textuelle riche.
	/// </summary>
	public class TextAccessor
	{
		public TextAccessor(AbstractRecord record, RecordFormat format)
		{
			this.record = record;
			this.format = format;
		}

		//	Retourne le contenu textuel d'un champ, en tenant compte ou pas du 'FieldFormat'.
		public FormattedText GetFormattedText(FieldType fieldType, bool useFieldFormat = false)
		{
			FormattedText text = this.record.GetFormattedText (fieldType);

			if (useFieldFormat && this.format != null)
			{
				var fieldFormat = this.format.FieldFormats.Where (x => x.FieldType == fieldType).FirstOrDefault ();

				if (fieldFormat != null)
				{
					text = fieldFormat.GetFormattedText (text);
				}
			}

			return text;
		}


		private readonly AbstractRecord record;
		private readonly RecordFormat format;
	}
}
