//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for Num1Adapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.Num1Controller))]
	
	public class Num1Adapter : AbstractStringAdapter
	{
		public Num1Adapter()
		{
		}
		
		public Num1Adapter(Binders.IBinder binder) : base (binder)
		{
		}
	}
}
