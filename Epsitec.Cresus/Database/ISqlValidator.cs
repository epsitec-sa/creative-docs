namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Interface de validation d'éléments SQL (noms, valeurs, chaînes, etc.)
	/// </summary>
	public interface ISqlValidator
	{
		//	Vérification de la validité de noms et de valeurs SQL :
		
		bool ValidateName(string value);
		bool ValidateString(string value);
		bool ValidateNumber(string value);
		
		void ThrowError(string message);
	}
}
