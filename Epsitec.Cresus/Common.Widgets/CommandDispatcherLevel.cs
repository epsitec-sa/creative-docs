//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>CommandDispatcherLevel</c> enumeration specifies at which level in
	/// a dispatch chain the command dispatcher works.
	/// </summary>
	public enum CommandDispatcherLevel
	{
		/// <summary>
		/// No or unknown dispatcher level.
		/// </summary>
		Unknown			= 0,
		
		/// <summary>
		/// Root dispatcher; this is an application level dispatcher.
		/// </summary>
		Root			= 1,

		/// <summary>
		/// Primary dispatcher; this is a document view level dispatcher.
		/// </summary>
		Primary			= 2,
		
		/// <summary>
		/// Secondary dispatcher; this is a dialog level dispatcher.
		/// </summary>
		Secondary		= 3,
	}
}
