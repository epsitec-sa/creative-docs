//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Epsitec.Common.Support.Implementation
{	
	/// <summary>
	/// La classe FileProvider donne accès aux ressources stockées dans des
	/// fichiers.
	/// </summary>
	public class FileProvider : AbstractResourceProvider
	{
		public FileProvider()
		{
			this.id_regex = RegexFactory.FileName;
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
					case ResourceLevel.Localized:
					case ResourceLevel.Customized:
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
				case ResourceLevel.Localized:	return this.file_local;
				case ResourceLevel.Customized:	return this.file_custom;
				case ResourceLevel.All:			return this.file_all;
			}
			
			return null;
		}
		
		
		public override string			Prefix
		{
			get { return "file"; }
		}
		
		
		public override void Setup(ResourceManager resource_manager)
		{
			base.Setup (resource_manager);
			
			string dir_1 = resource_manager.DefaultPath;
			string dir_2 = System.IO.Directory.GetCurrentDirectory ();
			string dir_3 = System.IO.Path.GetDirectoryName (typeof (ResourceManager).Assembly.Location);
			
			if (! this.SelectPath (dir_1))
			{
				if (! this.SelectPath (dir_2))
				{
					if (! this.SelectPath (dir_3))
					{
						throw new System.IO.FileNotFoundException ("Cannot find resources directory.");
					}
				}
			}
		}
		
		private bool SelectPath(string path)
		{
			if (! path.EndsWith (System.IO.Path.DirectorySeparatorChar.ToString ()))
			{
				path = path + System.IO.Path.DirectorySeparatorChar;
			}
			
			//	Pas très propre, mais ça suffit maintenant: on supprime le chemin \bin\... pour remonter au niveau
			//	plus intéressant (celui des sources).
			
			if (path.ToLower ().EndsWith (@"\bin\debug\"))
			{
				path = path.Substring (0, path.Length - 10);
			}
			else if (path.ToLower ().EndsWith (@"\bin\release\"))
			{
				path = path.Substring (0, path.Length - 12);
			}
			
			path = path + "resources" + System.IO.Path.DirectorySeparatorChar;
			
			if (System.IO.Directory.Exists (path))
			{
//-				System.Diagnostics.Debug.WriteLine ("Path prefix for resource files: " + path);
				
				this.path_prefix = path;
				this.path_prefix_base = path;
				return true;
			}
			
			return false;
		}
		
		public override bool SetupApplication(string application)
		{
			this.application = application;
			
			if ((this.application != null) &&
				(this.application.Length > 0))
			{
				string path = System.IO.Path.Combine (this.path_prefix_base, this.application);
				
				if (System.IO.Directory.Exists (path))
				{
					this.path_prefix = path + System.IO.Path.DirectorySeparatorChar;
				}
			}
			
			return true;
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
				
				string path = this.GetPathFromId (id, ResourceLevel.Default);
				
				if (System.IO.File.Exists (path))
				{
					return true;
				}
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

		public override string[] GetModules()
		{
			string path  = this.path_prefix_base;
			int    start = path.Length;
			
			string[] files = System.IO.Directory.GetDirectories (path);

			List<string> modules = new List<string> ();

			foreach (string file in files)
			{
				string module = file.Substring (start);

				if (this.ValidateId (module))
				{
					modules.Add (module);
				}
			}
			
			return modules.ToArray ();
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
				
				if (! RegexFactory.ResourceBundleName.IsMatch (bundle_name))
				{
					continue;
				}
				
				if ((type_filter != null) &&
					(type_filter != "*"))
				{
					System.Text.RegularExpressions.Regex type_regex = Support.RegexFactory.FromSimpleJoker (type_filter);
					ResourceBundle bundle = ResourceBundle.Create (this.manager, bundle_name);
					
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
					
					if (! type_regex.IsMatch (bundle.Type))
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
			//	TODO:  Add FileProvider.Remove implementation
			throw new ResourceException ("Not implemented");
		}
		
		
		protected string					path_prefix;
		protected string					path_prefix_base;
		protected Regex						id_regex;
		
		protected string					file_default;
		protected string					file_local;
		protected string					file_custom;
		protected string					file_all;
		
		protected string					application;
	}
}
