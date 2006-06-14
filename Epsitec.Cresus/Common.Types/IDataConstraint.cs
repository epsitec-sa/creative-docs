//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataConstraint permet de déterminer une contrainte pour un
	/// objet, et de vérifier si la contrainte est respectée.
	/// </summary>
	public interface IDataConstraint
	{
		bool IsValidValue(object value);
	}
}
