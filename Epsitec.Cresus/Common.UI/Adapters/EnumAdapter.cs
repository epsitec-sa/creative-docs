//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for EnumAdapter.
	/// </summary>
	
	[Controller (1, typeof (Controllers.EnumController))]
	
	public class EnumAdapter : AbstractEnumAdapter
	{
		public EnumAdapter(Types.IEnumType enum_type) : base (enum_type)
		{
		}
		
		public EnumAdapter(Types.IEnumType enum_type, Binders.IBinder binder) : base (enum_type, binder)
		{
		}
	}
}
