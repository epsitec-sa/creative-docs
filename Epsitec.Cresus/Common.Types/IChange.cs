//	Copyright � 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IChange d�finit un �v�nement Changed.
	/// </summary>
	public interface IChange
	{
		event Support.EventHandler	Changed;
	}
}
