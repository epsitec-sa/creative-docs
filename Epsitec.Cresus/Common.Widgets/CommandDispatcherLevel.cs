//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Text.RegularExpressions;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'énumération CommandDispatcherLevel détermine à quel niveau se trouve
	/// un CommandDispatcher.
	/// </summary>
	public enum CommandDispatcherLevel
	{
		Unknown			= 0,
		
		Root			= 1,					//	au niveau application
		Primary			= 2,					//	au niveau document
		Secondary		= 3,					//	au niveau dialogue
	}
}
