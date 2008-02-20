//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IValidator permet de d�terminer si un objet est dans un �tat
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
	/// L'�num�ration ValidationState repr�sente les �tats possibles pour un
	/// validateur.
	/// </summary>
	public enum ValidationState
	{
		Unknown,			//	�tat inconnu (non d�fini)
		Ok,					//	�tat valide
		Error,				//	�tat non valide
		Dirty				//	�tat pas � jour => n�cessite une validation
	}
}
