using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App
{
	public delegate void EventHandler<T1>(object sender, T1 val1);
	public delegate void EventHandler<T1, T2>(object sender, T1 val1, T2 val2);
}
