//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

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
		
		
		protected string GetPathFromId(string id, ResourceLevel level)
		{
			if (this.ValidateId (id))
			{
				switch (level)
				{
					case ResourceLevel.Default:
						return id + ".resource";
					case ResourceLevel.Localised:
						return id + this.ext_local;
					case ResourceLevel.Customised:
						return id + ".custom.resource";
					default:
						throw new ResourceException ("Invalid resource level");
				}
			}
			
			return null;
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
			this.culture   = culture;
			this.ext_local = "." + this.culture.TwoLetterISOLanguageName + ".resource";
		}
		
		
		public bool ValidateId(string id)
		{
			return (id != null) && (id != "") && (id.Length < 100) && this.id_regex.IsMatch (id);
		}
		
		public bool Contains(string id)
		{
			if (this.ValidateId (id))
			{
				//	On valide toujours le nom avant, pour �viter des mauvaises surprises si
				//	l'appelant est malicieux.
				
				//	TODO: v�rifie si la ressource existe, en cherchant uniquement le niveau
				//	ResourceLevel.Default.
			}
			
			return false;
		}
		
		public byte[] GetData(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			string path = this.GetPathFromId (id, level);
			
			if (path != null)
			{
				try
				{
					using (System.IO.FileStream stream = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
					{
						int    size = (int) stream.Length;
						byte[] data = new byte[size];
						stream.Read (data, 0, size);
						return data;
					}
				}
				catch
				{
				}
			}
			
			return null;
		}
		
		public System.IO.Stream GetDataStream(string id, Epsitec.Cresus.Support.ResourceLevel level)
		{
			string path = this.GetPathFromId (id, level);
			
			if (path != null)
			{
				try
				{
					System.IO.FileStream stream = new System.IO.FileStream (path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
					return stream;
				}
				catch
				{
				}
			}
			
			return null;
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
		protected string				ext_local;
	}
}
