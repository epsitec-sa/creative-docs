//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for Num2Adapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.Num2Controller))]
	
	public class Num2Adapter : AbstractStringAdapter
	{
		public Num2Adapter()
		{
		}
		
		public Num2Adapter(Binders.IBinder binder) : base (binder)
		{
		}
	}
}
