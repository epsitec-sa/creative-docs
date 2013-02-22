using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.ComptaNG.Common.RequestData
{
	public class ErrorField
	{
		public FieldType Field;
		public ErrorType Error;
		public FormattedText Description;
	}
}
