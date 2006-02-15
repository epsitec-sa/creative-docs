//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IChange définit un événement Changed.
	/// </summary>
	public interface IChange
	{
		event Support.EventHandler	Changed;
	}
}
