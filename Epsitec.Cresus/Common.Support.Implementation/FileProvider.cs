//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Support.Implementation
{
	using System.Globalization;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// La classe FileProvider donne accès aux ressources stockées dans des
	/// fichiers.
	/// </summary>
	public class FileProvider : AbstractResourceProvider
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
			this.path_prefix = System.IO.Directory.GetCurrentDirectory () + System.IO.Path.DirectorySeparatorChar;
			
			//	Pas très propre, mais ça suffit maintenant: on supprime le chemin \bin\... pour remonter au niveau
			//	plus intéressant (celui des sources).
			
			if (this.path_prefix.EndsWith (@"\bin\Debug\"))
			{
				this.path_prefix = this.path_prefix.Substring (0, this.path_prefix.Length - 10);
			}
			else if (this.path_prefix.EndsWith (@"\bin\Release\"))
			{
				this.path_prefix = this.path_prefix.Substring (0, this.path_prefix.Length - 12);
			}
			
			this.path_prefix = this.path_prefix + "resources" + System.IO.Path.DirectorySeparatorChar;
			
			System.Diagnostics.Debug.WriteLine ("Path prefix for files: " + this.path_prefix);
		}
		
		
		protected string GetPathFromId(string id, ResourceLevel level)
		{
			if (this.ValidateId (id))
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append (this.path_prefix);
				buffer.Append (id);
				buffer.Append (this.GetLevelSuffix (level));
				
				switch (level)
				{
					case ResourceLevel.Default:
					case ResourceLevel.Localised:
					case ResourceLevel.Customised:
						break;
					
					default:
						throw new ResourceException (string.Format ("Invalid resource level {0} for resource '{1}'.", level, id));
				}
				
				return buffer.ToString ();
			}
			
			return null;
		}
		
		protected string GetLevelSuffix(ResourceLevel level)
		{
			switch (level)
			{
				case ResourceLevel.Default:		return this.file_default;
				case ResourceLevel.Localised:	return this.file_local;
				case ResourceLevel.Customised:	return this.file_custom;
			}
			
			return null;
		}
		
		
		public override string			Prefix
		{
			get { return "file"; }
		}
		
		
		public override void Setup(string application)
		{
		}

		public override void SelectLocale(System.Globalization.CultureInfo culture)
		{
			base.SelectLocale (culture);
			
			this.file_default = "." + this.default_suffix + ".resource";
			this.file_local   = "." + this.local_suffix   + ".resource";
			this.file_custom  = "." + this.custom_suffix  + ".resource";
		}
		
		public override bool ValidateId(string id)
		{
			return base.ValidateId (id) && this.id_regex.IsMatch (id);
		}
		
		public override bool Contains(string id)
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
		
		
		public override byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level)
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
		
		
		public override string[] GetIds(string filter, ResourceLevel level)
		{
			filter = "*";
			
			string path   = this.path_prefix;
			string search = filter + this.GetLevelSuffix (level);
			
			string[] files = System.IO.Directory.GetFiles (path, search);
			
			int start = path.Length;
			int strip = search.Length - filter.Length + start;
			
			for (int i = 0; i < files.Length; i++)
			{
				files[i] = files[i].Substring (start, files[i].Length - strip);
			}
			
			return files;
		}

		
		public override void Create(string id, Epsitec.Common.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Create implementation
			throw new ResourceException ("Not implemented");
		}
		
		public override void Update(string id, Epsitec.Common.Support.ResourceLevel level, byte[] data)
		{
			// TODO:  Add FileProvider.Update implementation
			throw new ResourceException ("Not implemented");
		}
		
		public override void Remove(string id, Epsitec.Common.Support.ResourceLevel level)
		{
			// TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
		}
		
		
		protected string				path_prefix;
		protected Regex					id_regex;
		
		protected string				file_default;
		protected string				file_local;
		protected string				file_custom;
	}
}
