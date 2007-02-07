using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	internal class FileListSettings
	{
		public FileListSettings()
		{
			this.showHiddenFiles    = FolderItem.ShowHiddenFiles;
			this.hideFileExtensions = FolderItem.HideFileExtensions;
		}

		public FolderQueryMode FolderQueryMode
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

		public bool ShowHiddenFiles
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

		public bool HideFileExtensions
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

		public bool HideFolders
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

		public bool HideShortcuts
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

		public void Process(System.Collections.IList list, FolderItem item)
		{
			string path = item.FullPath.ToLowerInvariant ();

			if (item.IsFileSystemNode)
			{
				//	TODO: ...gérer isModel...

				FileListItem fileItem = new FileListItem (item, false);

				if (item.IsShortcut)
				{
					//	Vérifie que le fichier existe plutôt que de montrer des raccourcis cassés
					//	qui ne mènent nulle part:

					FolderItem target = fileItem.GetResolvedTarget ();

					if (target.IsFolder)
					{
						if (this.hideFolders)
						{
							return;
						}
					}
					else if (target.IsEmpty)
					{
						return;
					}
					else
					{
						if ((this.filterRegex == null) ||
							(this.filterRegex.IsMatch (target.FullPath)))
						{
							//	OK: le fichier cible passe le critère de l'extension...
						}
					}
				}

				fileItem.FillCache ();

				lock (list.SyncRoot)
				{
					list.Add (fileItem);
				}
			}
		}

		private bool showHiddenFiles;
		private bool hideFileExtensions;
		private bool hideFolders;
		private bool hideShortcuts;
		private FolderQueryMode folderQueryMode;
		private Regex filterRegex;
	}
}
