//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 27/04/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface IChangedSource définit un événement Changed, et rien
	/// d'autre.
	/// </summary>
	public interface IChangedSource
	{
		event Support.EventHandler	Changed;
	}
}
