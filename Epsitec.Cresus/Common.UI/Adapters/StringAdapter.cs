//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for StringAdapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.StringController))]
	
	public class StringAdapter : AbstractStringAdapter
	{
		public StringAdapter()
		{
		}
		
		public StringAdapter(Binders.IBinder binder) : base (binder)
		{
		}
	}
}
