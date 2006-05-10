//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IValidator permet de déterminer si un objet est dans un état
	/// valide (ou non).
	/// </summary>
	public interface IValidator
	{
		bool				IsValid				{ get; }
		ValidationState		State				{ get; }
		string				ErrorMessage		{ get; }
		
		void Validate();
		void MakeDirty(bool deep);
		
		event Support.EventHandler	BecameDirty;
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
