namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Interface de validation d'�l�ments SQL (noms, valeurs, cha�nes, etc.)
	/// </summary>
	public interface ISqlValidator
	{
		//	V�rification de la validit� de noms et de valeurs SQL :
		
		bool ValidateName(string value);
		bool ValidateString(string value);
		bool ValidateNumber(string value);
		
		void ThrowError(string message);
	}
}
