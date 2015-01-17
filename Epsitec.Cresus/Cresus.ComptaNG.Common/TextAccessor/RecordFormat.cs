using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;
using Epsitec.Common.Types;
using Epsitec.Cresus.ComptaNG.Common.Records;

namespace Epsitec.Cresus.ComptaNG.Common.TextAccessor
{
	/// <summary>
	/// Cette classe décrit la typographie à appliquer aux champs d'un Record.
	/// </summary>
	public class RecordFormat
	{
		public Guid					RecordGuid;
		public List<FieldFormat>	FieldFormats;
	}
}
