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
			this.id_regex    = RegexFactory.FileName;
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
				case ResourceLevel.All:			return this.file_all;
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
			
			this.file_default = string.Concat (".", this.default_suffix, ".resource");
			this.file_local   = string.Concat (".", this.local_suffix,   ".resource");
			this.file_custom  = string.Concat (".", this.custom_suffix,  ".resource");
			this.file_all     = ".resource";
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
		
		
		public override byte[] GetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
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
		
		
		public override string[] GetIds(string name_filter, string type_filter, ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			string file_filter = name_filter;
			
			string path   = this.path_prefix;
			string suffix = this.GetLevelSuffix (level);
			string search;
			
			if (level == ResourceLevel.All)
			{
				search = string.Concat (file_filter, ".*", suffix);
			}
			else
			{
				search = string.Concat (file_filter, suffix);
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			string[] files = System.IO.Directory.GetFiles (path, search);
			
			int start = path.Length;
			int strip = suffix.Length + start;
			
			for (int i = 0; i < files.Length; i++)
			{
				string full_name   = files[i];
				string bundle_name = full_name.Substring (start, full_name.Length - strip);
				
				if ((type_filter != null) &&
					(type_filter != "*"))
				{
					ResourceBundle bundle = ResourceBundle.Create (bundle_name);
					
					bundle.RefInclusionEnabled = false;
					bundle.AutoMergeEnabled    = false;
					
					byte[] data = this.GetData (bundle_name, ResourceLevel.Default, culture);
					
					if (ResourceBundle.CheckBundleHeader (data) == false)
					{
						continue;
					}
					
					try
					{
						bundle.Compile (data);
					}
					catch (System.Xml.XmlException)
					{
						//	Ce n'est pas un bundle compilable, probablement parce qu'il contient des
						//	données binaires. Sautons-le.
						
						continue;
					}
					
					if (type_filter != bundle.Type)
					{
						//	Saute ce bundle, car il n'est pas du type adéquat :
						
						continue;
					}
				}
				
				list.Add (bundle_name);
			}
			
			files = new string[list.Count];
			list.CopyTo (files);
			
			return files;
		}

		
		public override bool SetData(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture, byte[] data, ResourceSetMode mode)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			string path = this.GetPathFromId (id, level);
			
			if (path != null)
			{
				System.IO.FileMode file_mode = System.IO.FileMode.Open;
				
				switch (mode)
				{
					case ResourceSetMode.CreateOnly:
						file_mode = System.IO.FileMode.CreateNew;
						break;
					case ResourceSetMode.UpdateOnly:
						file_mode = System.IO.FileMode.Open;
						break;
					case ResourceSetMode.Write:
						file_mode = System.IO.FileMode.OpenOrCreate;
						break;
					default:
						throw new System.ArgumentException (string.Format ("Mode {0} not supported.", mode), "mode");
				}
				
				using (System.IO.FileStream stream = new System.IO.FileStream (path, file_mode, System.IO.FileAccess.Write))
				{
					stream.Write (data, 0, data.Length);
					stream.SetLength (data.Length);
					stream.Flush ();
					
					return true;
				}
			}
			
			return false;
		}
		
		public override bool Remove(string id, Epsitec.Common.Support.ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			// TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
		}
		
		
		
		protected string				path_prefix;
		protected Regex					id_regex;
		
		protected string				file_default;
		protected string				file_local;
		protected string				file_custom;
		protected string				file_all;
	}
}
