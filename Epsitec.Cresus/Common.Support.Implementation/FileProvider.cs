//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Epsitec.Common.Support.Implementation
{	
	/// <summary>
	/// La classe FileProvider donne acc�s aux ressources stock�es dans des
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
			
			this.idRegex = RegexFactory.FileName;
			this.SelectLocale (CultureInfo.CurrentCulture);
		}
		
		public override string			Prefix
		{
			get { return "file"; }
		}
		
		public override bool SelectModule(ref ResourceModuleInfo module)
		{
			string moduleName = null;
			string modulePath = null;
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
							modulePath = item.Path;
							break;
						}
					}
				}
			}
			else
			{
				moduleName = module.Name;
				modulePath = module.Path;
			}
			
			if (moduleName != null)
			{
				//	Search the module based on its module name. 

				if (string.IsNullOrEmpty (modulePath))
				{
					foreach (ResourceModuleInfo item in this.GetModules ())
					{
						if (item.Name == module.Name)
						{
							moduleName = item.Name;
							modulePath = item.Path;
							break;
						}
					}
				}
				
				if ((!string.IsNullOrEmpty (modulePath)) &&
					(System.IO.Directory.Exists (modulePath)))
				{
					moduleId = FileProvider.GetModuleId (modulePath);

					if ((moduleId >= 0) &&
						((module.Id < 0) || (module.Id == moduleId)))
					{
						module = new ResourceModuleInfo (moduleName, modulePath, moduleId);

						this.pathPrefix = string.Concat (modulePath, System.IO.Path.DirectorySeparatorChar);
						this.module = module;

						return true;
					}
				}
			}

			return false;
		}

		protected override void SelectLocale(System.Globalization.CultureInfo culture)
		{
			base.SelectLocale (culture);

			this.genericFileSuffix = ".resource";
			
			this.defaultFileSuffix = string.Concat (".", this.defaultSuffix, this.genericFileSuffix);
			this.localFileSuffix   = string.Concat (".", this.localSuffix,   this.genericFileSuffix);
			this.customFileSuffix  = string.Concat (".", this.customSuffix,  this.genericFileSuffix);
		}
		
		public override bool ValidateId(string id)
		{
			return base.ValidateId (id) && this.idRegex.IsMatch (id);
		}
		
		public override bool Contains(string id)
		{
			if (this.ValidateId (id))
			{
				//	On valide toujours le nom avant, pour �viter des mauvaises surprises si
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
			
			if ((string.IsNullOrEmpty (path) == false) &&
				(System.IO.File.Exists (path)))
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
			List<ResourceModuleInfo> modules = new List<ResourceModuleInfo> ();

			foreach (string file in this.GetModuleProbingDirectories ())
			{
				//	Extract the module name from the full file name by stripping
				//	the directory path prefix.

				string moduleName = System.IO.Path.GetFileName (file);

				if (this.ValidateId (moduleName))
				{
					int moduleId = FileProvider.GetModuleId (file);
					
					if (moduleId >= 0)
					{
						if (modules.FindIndex (delegate (ResourceModuleInfo info) { return info.Id == moduleId; }) == -1)
						{
							modules.Add (new ResourceModuleInfo (moduleName, file, moduleId));
						}
					}
				}
			}

			modules.Sort (
				delegate (ResourceModuleInfo a, ResourceModuleInfo b)
				{
					if (a.Name == b.Name)
					{
						return a.Id.CompareTo (b.Id);
					}
					else
					{
						return string.CompareOrdinal (a.Name, b.Name);
					}
				});
			
			return modules.ToArray ();
		}

		public override string[] GetIds(string nameFilter, string typeFilter, ResourceLevel level, System.Globalization.CultureInfo culture)
		{
			if (this.culture != culture)
			{
				this.SelectLocale (culture);
			}
			
			string fileFilter = nameFilter;
			
			string path   = this.pathPrefix;
			string suffix = this.GetLevelSuffix (level);
			string search;
			
			if (level == ResourceLevel.All)
			{
				search = string.Concat (fileFilter, ".*", suffix);
			}
			else
			{
				search = string.Concat (fileFilter, suffix);
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			string[] files = System.IO.Directory.GetFiles (path, search);
			
			int start = path.Length;
			int strip = suffix.Length + start;
			
			for (int i = 0; i < files.Length; i++)
			{
				string fullName   = files[i];
				string bundleName = fullName.Substring (start, fullName.Length - strip);
				
				if (! RegexFactory.ResourceBundleName.IsMatch (bundleName))
				{
					continue;
				}
				
				if ((typeFilter != null) &&
					(typeFilter != "*"))
				{
					System.Text.RegularExpressions.Regex typeRegex = Support.RegexFactory.FromSimpleJoker (typeFilter);
					ResourceBundle bundle = ResourceBundle.Create (this.manager, bundleName);
					
					bundle.RefInclusionEnabled = false;
					bundle.AutoMergeEnabled    = false;
					
					byte[] data = this.GetData (bundleName, ResourceLevel.Default, culture);
					
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
						//	donn�es binaires. Sautons-le.
						
						continue;
					}
					
					if (! typeRegex.IsMatch (bundle.Type))
					{
						//	Saute ce bundle, car il n'est pas du type ad�quat :
						
						continue;
					}
				}
				
				list.Add (bundleName);
			}
			
			files = new string[list.Count];
			list.CopyTo (files);
			
			return files;
		}

		/// <summary>
		/// Defines the global probing path which is searched before the local
		/// resources folder.
		/// </summary>
		/// <param name="path">The global probing path (several paths can be specified;
		/// they must be separated by <code>";"</code>).</param>
		public static void DefineGlobalProbingPath(string path)
		{
			FileProvider.globalProbingPath = path;
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
				System.IO.FileMode fileMode = System.IO.FileMode.Open;
				
				switch (mode)
				{
					case ResourceSetMode.CreateOnly:
						fileMode = System.IO.FileMode.CreateNew;
						break;
					case ResourceSetMode.UpdateOnly:
						fileMode = System.IO.FileMode.Open;
						break;
					case ResourceSetMode.Write:
						fileMode = System.IO.FileMode.OpenOrCreate;
						break;
					default:
						throw new System.ArgumentException (string.Format ("Mode {0} not supported.", mode), "mode");
				}
				
				using (System.IO.FileStream stream = new System.IO.FileStream (path, fileMode, System.IO.FileAccess.Write))
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


		private string GetPathFromId(string id, ResourceLevel level)
		{
			if (this.ValidateId (id))
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

				buffer.Append (this.pathPrefix);
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

		private string GetLevelSuffix(ResourceLevel level)
		{
			System.Diagnostics.Debug.Assert (level != ResourceLevel.Merged);

			switch (level)
			{
				case ResourceLevel.Default:
					return this.defaultFileSuffix;
				case ResourceLevel.Localized:
					return this.localFileSuffix;
				case ResourceLevel.Customized:
					return this.customFileSuffix;
				case ResourceLevel.All:
					return this.genericFileSuffix;
			}

			return null;
		}

		private bool SelectPath(string path)
		{
			if (!path.EndsWith (System.IO.Path.DirectorySeparatorChar.ToString ()))
			{
				path = path + System.IO.Path.DirectorySeparatorChar;
			}

			//	Pas tr�s propre, mais �a suffit maintenant: on supprime le chemin \bin\... pour remonter au niveau
			//	plus int�ressant (celui des sources).

			if (path.ToLower ().EndsWith (@"\bin\debug\"))
			{
				path = path.Substring (0, path.Length - 10);
			}
			else if (path.ToLower ().EndsWith (@"\bin\release\"))
			{
				path = path.Substring (0, path.Length - 12);
			}

			path = System.IO.Path.Combine (path, "resources");

			if (System.IO.Directory.Exists (path))
			{
				this.pathPrefixRoot = path;
				this.pathPrefix     = path + System.IO.Path.DirectorySeparatorChar;

				return true;
			}
			else
			{
				return false;
			}
		}

		private IEnumerable<string> GetModuleProbingDirectories()
		{
			List<string> paths = new List<string> ();

			if (!string.IsNullOrEmpty (FileProvider.globalProbingPath))
			{
				paths.AddRange (FileProvider.globalProbingPath.Split (';'));
			}
			
			paths.Add (this.pathPrefixRoot);

			foreach (string path in paths)
			{
				if (System.IO.Directory.Exists (path))
				{
					foreach (string file in System.IO.Directory.GetDirectories (path))
					{
						yield return file;
					}
				}
			}
		}

		private static int GetModuleId(string path)
		{
			string fileName = System.IO.Path.Combine (path, "module.info");

			if (!System.IO.File.Exists (fileName))
			{
				return -1;
			}

			int moduleId = -1;
			try
			{
				//	Load the "module.info" file from the resource sub-folder
				//	where all the module bundles are stored, then extract the
				//	identifier :

				System.Xml.XmlDocument xml = new System.Xml.XmlDocument ();
				xml.Load (fileName);
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

		private static string				globalProbingPath;

		private string						pathPrefix;
		private string						pathPrefixRoot;
		
		private Regex						idRegex;

		private string						defaultFileSuffix;
		private string						localFileSuffix;
		private string						customFileSuffix;
		private string						genericFileSuffix;

		private ResourceModuleInfo			module;
	}
}
