//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
