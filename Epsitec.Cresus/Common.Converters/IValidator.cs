//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/03/2004

namespace Epsitec.Common.Converters
{
	/// <summary>
	/// L'interface IValidator permet de déterminer si un objet est dans un état
	/// valide (ou non).
	/// </summary>
	public interface IValidator
	{
		bool	IsValid				{ get; set; }
		string	ErrorMessage		{ get; set; }
		
		void Validate();
	}
}
