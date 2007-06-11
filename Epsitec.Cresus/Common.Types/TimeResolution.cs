//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TimeResolution</c> enumeration defines the resolution of a time
	/// or a date (seconds, milliseconds, days, weeks, etc.)
	/// </summary>
	[DesignerVisible]
	public enum TimeResolution : byte
	{
		Default,
		
		Milliseconds,
		Seconds,
		Minutes,
		Hours,
		
		Days,
		Weeks,
		Months,
		Years
	}
}
