//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Interface de validation d'�l�ments SQL (noms, valeurs, cha�nes, etc.)
	/// </summary>
	public interface ISqlValidator
	{
		//	V�rification de la validit� de noms et de valeurs SQL :
		
		bool ValidateName(string value);
		bool ValidateQualifiedName(string value);
		bool ValidateString(string value);
		bool ValidateNumber(string value);
		
		void ThrowError(string message);
	}
}
