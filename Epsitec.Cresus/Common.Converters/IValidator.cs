//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 08/03/2004

namespace Epsitec.Common.Converters
{
	/// <summary>
	/// L'interface IValidator permet de d�terminer si un objet est dans un �tat
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
