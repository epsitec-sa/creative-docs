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
		public FileProvider(ResourceManager manager) : base (manager)
		{
			string dir1 = manager.DefaultPath;
			string dir2 = System.IO.Directory.GetCurrentDirectory ();
			string dir3 = System.IO.Path.GetDirectoryName (typeof (ResourceManager).Assembly.Location);

			if (!this.SelectPath (dir1) &&
				!this.SelectPath (dir2) &&
				!this.SelectPath (dir3))
			{
				throw new System.IO.FileNotFoundException ("Cannot find resources directory.");
			}
			
			this.id_regex = RegexFactory.FileName;
			this.SelectLocale (CultureInfo.CurrentCulture);
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
			System.Diagnostics.Debug.Assert (level != ResourceLevel.Merged);
			
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
		
		public override bool SelectModule(ref ResourceModuleInfo module)
		{
			string moduleName = null;
			int    moduleId   = -1;

			if (string.IsNullOrEmpty (module.Name))
			{
				if (module.Id >= 0)
				{
					//	Search for the module based on its module identifier. This
					//	is currently slow, as it requires a walk through all possible
					//	modules, until we find the matching one.

					foreach (ResourceModuleInfo item in this.GetModules ())
					{
						if (item.Id == module.Id)
						{
							moduleName = item.Name;
							break;
						}
					}
				}
			}
			else
			{
				moduleName = module.Name;
			}
			
			if (moduleName != null)
			{
				//	Search the module based on its module name. 
				
				string path = System.IO.Path.Combine (this.path_prefix_base, moduleName);
				
				if (System.IO.Directory.Exists (path))
				{
					moduleId = FileProvider.GetModuleId (path);

					if ((moduleId >= 0) &&
						((module.Id < 0) || (module.Id == moduleId)))
					{
						this.path_prefix = string.Concat (path, System.IO.Path.DirectorySeparatorChar);
						this.module = module;

						module = new ResourceModuleInfo (moduleName, moduleId);
						
						return true;
					}
				}
			}

			return false;
		}

		protected override void SelectLocale(System.Globalization.CultureInfo culture)
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

		public override ResourceModuleInfo[] GetModules()
		{
			string path  = this.path_prefix_base;
			int    start = path.Length;
			
			string[] files = System.IO.Directory.GetDirectories (path);

			List<ResourceModuleInfo> modules = new List<ResourceModuleInfo> ();

			foreach (string file in files)
			{
				//	Extract the module name from the full file name by stripping
				//	the directory path prefix.
				
				string moduleName = file.Substring (start);

				if (this.ValidateId (moduleName))
				{
					int moduleId = FileProvider.GetModuleId (file);
					
					if (moduleId >= 0)
					{
						modules.Add (new ResourceModuleInfo (moduleName, moduleId));
					}
				}
			}
			
			return modules.ToArray ();
		}

		private static int GetModuleId(string path)
		{
			int moduleId = -1;
			try
			{
				//	Load the "module.info" file from the resource sub-folder
				//	where all the module bundles are stored, then extract the
				//	identifier :

				System.Xml.XmlDocument xml = new System.Xml.XmlDocument ();
				xml.Load (System.IO.Path.Combine (path, "module.info"));
				System.Xml.XmlElement root = xml.DocumentElement;

				if (root.Name == "ModuleInfo")
				{
					int idValue;
					string idAttribute = root.GetAttribute ("id");

					if ((string.IsNullOrEmpty (idAttribute) == false) &&
						(int.TryParse (idAttribute, NumberStyles.Integer, CultureInfo.InvariantCulture, out idValue)))
					{
						moduleId = idValue;
					}
				}

				if (moduleId < 0)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Invalid XML file found in '{0}'", path));
				}
			}
			catch (System.IO.FileNotFoundException)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Could not find module.info file in '{0}'", path));
			}
			catch (System.IO.PathTooLongException)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Path to module.info file for '{0}' is too long", path));
			}
			
			return moduleId;
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
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}

			string path = this.GetPathFromId (id, level);

			if (path != null)
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
					return true;
				}
			}

			return false;
		}
		
		
		protected string					path_prefix;
		protected string					path_prefix_base;
		protected Regex						id_regex;
		
		protected string					file_default;
		protected string					file_local;
		protected string					file_custom;
		protected string					file_all;
		
		protected ResourceModuleInfo		module;
	}
}
