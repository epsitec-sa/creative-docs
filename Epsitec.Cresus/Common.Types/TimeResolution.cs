//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TimeResolution</c> enumeration defines the resolution of a time
	/// or a date (seconds, milliseconds, days, weeks, etc.)
	/// </summary>
	public enum TimeResolution
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
