//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// Summary description for Status.
	/// </summary>
	public enum Status
	{
		Undefined,
		
		Ok,
		OkFitEnded,
		
		SwitchLayout,
		
		ErrorNeedMoreText,
		ErrorNeedMoreRoom,
		ErrorCannotFit,
	}
}
