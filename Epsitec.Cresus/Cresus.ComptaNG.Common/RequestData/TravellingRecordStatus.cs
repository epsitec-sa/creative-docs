using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.ComptaNG.Common.RecordAccessor;
using Epsitec.Common.Types;
using Epsitec.Cresus.ComptaNG.Common.TextAccessor;

namespace Epsitec.Cresus.ComptaNG.Common.RequestData
{
	public enum TravellingRecordStatus
	{
		None,
		Created,	// Record fraichement créé
		Updated,	// Record modifié
		Deleted,	// Record supprimé
	}
}
