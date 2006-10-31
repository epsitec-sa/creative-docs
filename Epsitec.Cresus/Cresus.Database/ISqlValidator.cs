//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Interface de validation d'éléments SQL (noms, valeurs, chaînes, etc.)
	/// </summary>
	public interface ISqlValidator
	{
		bool ValidateName(string value);
		bool ValidateQualifiedName(string value);
		bool ValidateString(string value);
		bool ValidateNumber(string value);
		//	Vérification de la validité de noms et de valeurs SQL :
		
		
	}
}
