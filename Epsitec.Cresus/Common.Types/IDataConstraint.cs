//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// L'interface IDataConstraint permet de d�terminer une contrainte pour un
	/// objet, et de v�rifier si la contrainte est respect�e.
	/// </summary>
	public interface IDataConstraint
	{
		bool CheckConstraint(object value);
	}
}
