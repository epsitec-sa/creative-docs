//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Interface de validation d'�l�ments SQL (noms, valeurs, cha�nes, etc.)
	/// </summary>
	public interface ISqlValidator
	{
		bool ValidateName(string value);
		bool ValidateQualifiedName(string value);
		bool ValidateString(string value);
		bool ValidateNumber(string value);
		void ThrowError(string message);
		//	V�rification de la validit� de noms et de valeurs SQL :
		
		
	}
}
