using Epsitec.Common.Support;
using Epsitec.Common.Types;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public enum EntityStatus
	{
		Unknown = -1,
		Empty,
		EmptyAndValid,		// un champ optionnel vide est à la fois vide et valide
		Valid,
		Invalid,
	}
}
