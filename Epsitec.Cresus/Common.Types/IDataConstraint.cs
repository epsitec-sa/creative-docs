//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataConstraint permet de déterminer une contrainte pour un
	/// objet, et de vérifier si la contrainte est respectée.
	/// </summary>
	public interface IDataConstraint
	{
		bool CheckConstraint(object value);
	}
}
