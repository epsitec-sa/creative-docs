//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Support.Implementation
{
	using System.Globalization;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// La classe FileProvider donne accès aux ressources stockées dans des
	/// fichiers.
	/// </summary>
	public class FileProvider : Epsitec.Cresus.Support.IResourceProvider
	{
		public FileProvider()
		{
			//	Un ID valide pour une ressource stockée dans un fichier se compose comme
			//	suit :
			//
			//	- Une lettre, un chiffre ou le "_".
			//
			//	puis :
			//	
			//	- Zéro à n occurrences de l'un des suivants :
			//	  - lettres, chiffres ou le "_"
			//	  - " ", "+", "-" ou ".", pour autant que :
			//		- le prochain caractère n'est pas le dernier "." ou " "
			//		- ce caractère ne se répète pas (la clause ?!\k<X> empêche le doublon)
			//
			//	Ainsi, sont valides : "abc", "_123", "a.b+c-2", "a+- .x".
			//	Sont refusés : ".abc", "a++b", "a.", "b ".
			
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
			return (id != null) && (id != "") && (id.Length < 100) && this.id_regex.IsMatch (id);
		}
		
		public bool Contains(string id)
		{
			if (this.ValidateId (id))
			{
				//	On valide toujours le nom avant, pour éviter des mauvaises surprises si
				//	l'appelant est malicieux.
				
				//	TODO: vérifie si la ressource existe, en cherchant uniquement le niveau
				//	ResourceLevel.Default.
			}
			
			return false;
		}
		
		public byte[] GetData(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.GetData implementation
			return null;
		}
		
		public System.IO.Stream GetDataStream(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			byte[] data = this.GetData (id, level);
			return (data == null) ? null : new System.IO.MemoryStream (data, false);
		}
		
		public void Create(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Create implementation
			throw new ResourceException ("Not implemented");
		}
		
		public void Update(string id, Epsitec.Cresus.Support.ResourceLevel level, byte[] data)
		{
			// TODO:  Add FileProvider.Update implementation
			throw new ResourceException ("Not implemented");
		}
		
		public void Remove(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
		}
		#endregion
		
		protected CultureInfo			culture;
		protected Regex					id_regex;
	}
}
