namespace Epsitec.Cresus.Support.Implementation
{
	using System.Globalization;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// La classe FileProvider donne acc�s aux ressources stock�es dans des
	/// fichiers.
	/// </summary>
	public class FileProvider : Epsitec.Cresus.Support.IResourceProvider
	{
		public FileProvider()
		{
			//	Un ID valide pour une ressource stock�e dans un fichier se compose comme
			//	suit :
			//
			//	- Une lettre, un chiffre ou le "_".
			//
			//	puis :
			//	
			//	- Z�ro � n occurrences de l'un des suivants :
			//	  - lettres, chiffres ou le "_"
			//	  - " ", "+", "-" ou ".", pour autant que :
			//		- le prochain caract�re n'est pas le dernier "." ou " "
			//		- ce caract�re ne se r�p�te pas (la clause ?!\k<X> emp�che le doublon)
			//
			//	Ainsi, sont valides : "abc", "_123", "a.b+c-2", "a+- .x".
			//	Sont refus�s : ".abc", "a++b", "a.", "b ".
			
			this.id_regex = new Regex (@"^([a-zA-Z0-9_]((?![ \.]$)(?<X>[ \+\-\.])(?!\k<X>))*)+$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		}
		
		#region IResourceProvider Members
		public string Prefix
		{
			get
			{
				return "file";
			}
		}
		
		
		public void SelectLocale(System.Globalization.CultureInfo culture)
		{
			this.culture = culture;
		}
		
		
		public bool ValidateId(string id)
		{
			return this.id_regex.IsMatch (id);
		}
		
		public bool Contains(string id)
		{
			// TODO:  Add FileProvider.Contains implementation
			return false;
		}
		
		public System.IO.Stream GetDataStream(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.GetDataStream implementation
			return null;
		}
		
		public void Remove(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Remove implementation
		}
		
		public void Create(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Create implementation
		}
		
		public byte[] GetData(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.GetData implementation
			return null;
		}
		
		public void Update(string id, Epsitec.Cresus.Support.ResourceLevel level, byte[] data)
		{
			// TODO:  Add FileProvider.Update implementation
		}
		#endregion
		
		protected CultureInfo			culture;
		protected Regex					id_regex;
	}
}
