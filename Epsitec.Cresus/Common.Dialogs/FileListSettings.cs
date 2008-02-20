//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Dialogs
{
	internal class FileListSettings : IFileExtensionDescription
	{
		public FileListSettings(AbstractFileDialog dialog)
		{
			this.dialog = dialog;
			this.path   = this.dialog.InitialDirectory;
			
			this.showHiddenFiles    = FolderItem.ShowHiddenFiles;
			this.hideFileExtensions = FolderItem.HideFileExtensions;
		}

		public string							Path
		{
			get
			{
				return this.path;
			}
		}
		
		public FolderQueryMode					FolderQueryMode
		{
			get
			{
				return this.folderQueryMode;
			}
			set
			{
				this.folderQueryMode = value;
			}
		}

		public bool								ShowHiddenFiles
		{
			get
			{
				return this.showHiddenFiles;
			}
			set
			{
				this.showHiddenFiles = value;
			}
		}

		public bool								HideFileExtensions
		{
			get
			{
				return this.hideFileExtensions;
			}
			set
			{
				this.hideFileExtensions = value;
			}
		}

		public bool								HideFolders
		{
			get
			{
				return this.hideFolders;
			}
			set
			{
				this.hideFolders = value;
			}
		}

		public bool								HideShortcuts
		{
			get
			{
				return this.hideShortcuts;
			}
			set
			{
				this.hideShortcuts = value;
			}
		}

		
		public void DefineFilterPattern(string pattern)
		{
			if ((pattern == null) ||
					(pattern.Trim ().Length == 0))
			{
				this.filterRegex = null;
			}
			else
			{
				this.filterRegex = RegexFactory.FromSimpleJoker (pattern, RegexFactory.Options.IgnoreCase);
			}
		}

		public void AddDefaultDescription(string extension, string description)
		{
			if (extension.StartsWith ("."))
			{
				this.defaultDescriptions[extension.ToLowerInvariant ()] = description;
			}
			else
			{
				throw new System.FormatException ("Incorrect file extension: " + extension);
			}
		}

		public bool Filter(FileFilterInfo file)
		{
			if ((file.Attributes & System.IO.FileAttributes.Hidden) != 0)
			{
				if (!this.showHiddenFiles)
				{
					return false;
				}
			}

			if ((file.Attributes & System.IO.FileAttributes.Directory) != 0)
			{
				if (this.hideFolders)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else if (file.LowerCaseExtension == ".lnk")
			{
				if (this.hideShortcuts)
				{
					return false;
				}
				else
				{
					if (this.hideFolders)
					{
						string path = file.Path;
						string name = path.Substring (0, path.Length-4);
						
						if ((this.filterRegex == null) ||
							(this.filterRegex.IsMatch (name)))
						{
							return true;
						}
						else
						{
							return false;
						}
					}

					return true;
				}
			}
			else
			{
				if ((this.filterRegex == null) ||
					(this.filterRegex.IsMatch (file.Path)))
				{
					return true;
				}
			}
			
			return false;
		}

		public void Process(System.Collections.IList list, string path)
		{
			FolderItem item = FileManager.GetFolderItem (path, this.FolderQueryMode);
			this.Process (list, item);
		}

		public void Process(System.Collections.IList list, FolderItem item)
		{
			FileListItem fileItem = this.Process (item);
			
			if (fileItem != null)
			{
				lock (list.SyncRoot)
				{
					if (this.dialog.InitialDirectory == this.path)
					{
						list.Add (fileItem);
					}
				}
			}
		}

		public FileListItem Process(string path)
		{
			FolderItem item = FileManager.GetFolderItem (path, this.FolderQueryMode);
			return this.Process (item);
		}

		public FileListItem Process(FolderItem item)
		{
			if (item.IsEmpty)
			{
				return null;
			}

			string path = item.FullPath.ToLowerInvariant ();

			if (item.IsFileSystemNode)
			{
				//	TODO: ...gérer isModel...

				FileListItem fileItem = new FileListItem (item);

				if (item.IsShortcut)
				{
					//	Vérifie que le fichier existe plutôt que de montrer des raccourcis cassés
					//	qui ne mènent nulle part:

					FolderItem target = fileItem.GetResolvedTarget ();

					if (target.IsFolder)
					{
						if (this.hideFolders)
						{
							return null;
						}
					}
					else if (target.IsEmpty)
					{
						return null;
					}
					else
					{
						if ((this.filterRegex == null) ||
							(this.filterRegex.IsMatch (target.FullPath)))
						{
							//	OK: le fichier cible passe le critère de l'extension...
						}
						else
						{
							return null;
						}
					}
				}
				else
				{
					if (item.IsFolder)
					{
						if (this.hideFolders)
						{
							return null;
						}
					}
					else if ((this.filterRegex == null) || (this.filterRegex.IsMatch (path)))
					{
						//	OK: ...
					}
					else
					{
						return null;
					}
				}

				fileItem.FillCache ();

				string extension = System.IO.Path.GetExtension (path);
				string description;

				if (this.defaultDescriptions.TryGetValue (extension, out description))
				{
					fileItem.DefaultDescription = description;
				}

				return fileItem;
			}
			else
			{
				return null;
			}
		}

		#region IFileExtensionDescription Members

		void IFileExtensionDescription.Add(string extension, string description)
		{
			this.AddDefaultDescription (extension, description);
		}

		string IFileExtensionDescription.FindDescription(string extension)
		{
			string description;
			this.defaultDescriptions.TryGetValue (extension.ToLowerInvariant (), out description);
			return description;
		}

		#endregion

		
		private bool							showHiddenFiles;
		private bool							hideFileExtensions;
		private bool							hideFolders;
		private bool							hideShortcuts;
		private FolderQueryMode					folderQueryMode;
		private Regex							filterRegex;
		
		private AbstractFileDialog				dialog;
		private string							path;
		
		private readonly Dictionary<string, string> defaultDescriptions = new Dictionary<string, string> ();
	}
}
