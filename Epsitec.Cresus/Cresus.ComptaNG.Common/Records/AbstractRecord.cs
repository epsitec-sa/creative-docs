using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Types;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;

namespace Epsitec.Cresus.ComptaNG.Common.Records
{
	/// <summary>
	/// Partie commune à toutes les données comptables.
	/// </summary>
	public abstract class AbstractRecord
	{
		public System.Guid Guid;

		public FormattedText GetFormattedText(FieldType fieldType)
		{
			return FormattedText.Empty;
		}
	}
}
