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
		bool				IsValid				{ get; }
		ValidationState		ValidationState		{ get; }
		string				ErrorMessage		{ get; }
		
		void Validate();
	}
	
	/// <summary>
	/// L'énumération ValidationState représente les états possibles pour un
	/// validateur.
	/// </summary>
	public enum ValidationState
	{
		Unknown,			//	état inconnu (non défini)
		Ok,					//	état valide
		Error,				//	état non valide
		Dirty				//	état pas à jour => nécessite une validation
	}
}
