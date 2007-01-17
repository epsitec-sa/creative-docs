using System.Collections.Generic;
using System.IO;

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;
using Epsitec.Common.IO;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	//	Cette classe représente une 'ligne' dans la liste, qui peut représenter
	//	un fichier, un dossier ou la commande 'nouveau document vide'.
	public class FileItem : System.IComparable<FileItem>, Epsitec.Common.Types.IStructuredData, Epsitec.Common.Types.IStructuredTypeProvider
	{
		public FileItem()
		{
			//	Crée un item pour 'Nouveau document vide'.
			this.isNewEmptyDocument = true;
		}

		public FileItem(FolderItem folderItem, bool isModel)
		{
			//	Crée un item pour un fichier ou un dossier.
			this.folderItem = folderItem;
			this.isModel = isModel;
			this.isNewEmptyDocument = false;
		}

		public FolderItem FolderItem
		{
			get
			{
				return this.folderItem;
			}
			set
			{
				this.folderItem = value;

				if (this.folderItem.QueryMode.IconSize == FileInfoIconSize.Small)
				{
					this.smallIcon = this.folderItem.Icon;
				}
				else
				{
					this.smallIcon = null;
				}
			}
		}

		public FolderItemIcon SmallIcon
		{
			get
			{
				return this.smallIcon;
			}
			set
			{
				this.smallIcon = value;
			}
		}

		public string FileName
		{
			//	Nom du fichier avec le chemin d'accès complet.
			get
			{
				if (this.isNewEmptyDocument)  // nouveau document vide ?
				{
					return Common.Document.Settings.GlobalSettings.NewEmptyDocument;
				}
				else
				{
					if (this.IsShortcut)
					{
						FolderItem item = FileManager.ResolveShortcut (this.folderItem, FolderQueryMode.NoIcons);
						return item.FullPath;
					}
					else
					{
						return this.folderItem.FullPath;
					}
				}
			}
		}

		public string ShortFileName
		{
			//	Nom du fichier court, sans le chemin d'accès ni l'extension.
			get
			{
				if (this.isNewEmptyDocument)  // nouveau document vide ?
				{
					return "—";
				}
				else
				{
					if (this.IsDirectory)
					{
						return TextLayout.ConvertToTaggedText (this.folderItem.DisplayName);
					}
					else if (this.IsShortcut)
					{
						FolderItem item = FileManager.ResolveShortcut (this.folderItem, FolderQueryMode.NoIcons);
						return TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (item.FullPath));
					}
					else
					{
						return TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (this.folderItem.FullPath));
					}
				}
			}
		}

		public bool IsDirectory
		{
			get
			{
				if (this.isNewEmptyDocument)
				{
					return false;
				}

				return this.folderItem.IsFolder;
			}
		}

		public bool IsDirectoryOrShortcut
		{
			get
			{
				if (this.isNewEmptyDocument)
				{
					return false;
				}

				return this.folderItem.IsFolder || this.folderItem.IsShortcut;
			}
		}

		public bool IsShortcut
		{
			get
			{
				if (this.isNewEmptyDocument)
				{
					return false;
				}

				return this.folderItem.IsShortcut;
			}
		}

		public bool IsDrive
		{
			get
			{
				if (this.isNewEmptyDocument)
				{
					return false;
				}

				return this.folderItem.IsDrive;
			}
		}

		public bool IsVirtual
		{
			get
			{
				if (this.isNewEmptyDocument)
				{
					return false;
				}

				return this.folderItem.IsVirtual;
			}
		}

		public string FileDate
		{
			//	Date de modification du fichier.
			get
			{
				if (this.isNewEmptyDocument)
				{
					return "";
				}
				else
				{
					if (this.IsDirectoryOrShortcut)
					{
						if (string.IsNullOrEmpty (this.folderItem.FullPath))
						{
							return "";
						}

						System.IO.DirectoryInfo info = new System.IO.DirectoryInfo (this.folderItem.FullPath);
						if (!info.Exists)
						{
							return "";
						}

						return string.Concat (info.LastWriteTime.ToShortDateString (), " ", info.LastWriteTime.ToShortTimeString ());
					}
					else
					{
						System.IO.FileInfo info = new System.IO.FileInfo (this.folderItem.FullPath);
						if (!info.Exists)
						{
							return "";
						}

						return string.Concat (info.LastWriteTime.ToShortDateString (), " ", info.LastWriteTime.ToShortTimeString ());
					}
				}
			}
		}

		public string FileSize
		{
			//	Taille du fichier en kilo-bytes.
			get
			{
				if (this.isNewEmptyDocument || this.IsDirectoryOrShortcut)
				{
					return "";
				}
				else
				{
					System.IO.FileInfo info = new System.IO.FileInfo (this.folderItem.FullPath);
					if (!info.Exists)
					{
						return "";
					}

					long size = info.Length;

					size = (size+512)/1024;
					if (size < 1024)
					{
						double s = (double) size;
						s = System.Math.Floor (s*1000/1024);  // 0..999 KB
						return string.Format (Res.Strings.Dialog.File.Size.Kilo, s.ToString ());
					}

					size = (size+512)/1024;
					if (size < 1024)
					{
						double s = (double) size;
						s = System.Math.Floor (s*1000/1024);  // 0..999 MB
						return string.Format (Res.Strings.Dialog.File.Size.Mega, s.ToString ());
					}

					size = (size+512)/1024;
					if (size < 1024)
					{
						double s = (double) size;
						s = System.Math.Floor (s*1000/1024);  // 0..999 GB
						return string.Format (Res.Strings.Dialog.File.Size.Giga, s.ToString ());
					}

					return "?";
				}
			}
		}

		public string Description
		{
			//	Retourne la description du fichier, basée sur les statistiques si elles existent.
			get
			{
				if (this.isNewEmptyDocument)  // nouveau document vide ?
				{
					return Res.Strings.Dialog.New.EmptyDocument;
				}
				else
				{
					if (this.IsDirectoryOrShortcut)
					{
						return this.folderItem.TypeName;
					}
					else
					{
						Document.Statistics stat = this.Statistics;
						if (stat == null)
						{
							return this.isModel ? Res.Strings.Dialog.File.Model : Res.Strings.Dialog.File.Document;
						}
						else
						{
							return string.Format (Res.Strings.Dialog.File.Statistics, stat.PageFormat, stat.PagesCount.ToString (), stat.LayersCount.ToString (), stat.ObjectsCount.ToString (), stat.ComplexesCount.ToString (), stat.FontsCount.ToString (), stat.ImagesCount.ToString ());
						}
					}
				}
			}
		}

		public string FixIcon
		{
			//	Retourne l'éventuelle icône fixe qui remplace l'image miniature.
			get
			{
				if (this.isNewEmptyDocument)  // nouveau document vide ?
				{
					return "New";
				}
				else
				{
					return null;
				}
			}
		}

		public void GetImage(out Image image, out bool icon)
		{
			//	Donne l'image miniature associée au fichier.
			if (this.isNewEmptyDocument)  // nouveau document vide ?
			{
				image = null;
				icon = false;
			}
			else
			{
				if (this.IsDirectoryOrShortcut)
				{
					image = this.folderItem.Icon.Image;
					icon = true;
				}
				else
				{
					DocumentCache.Add (this.folderItem.FullPath);
					image = DocumentCache.Image (this.folderItem.FullPath);
					if (image == null)
					{
						image = this.folderItem.Icon.Image;
						icon = true;
					}
					else
					{
						icon = false;
					}
				}
			}
		}

		protected Document.Statistics Statistics
		{
			//	Retourne les statistiques associées au fichier.
			get
			{
				if (this.isNewEmptyDocument)  // nouveau document vide ?
				{
					return null;
				}
				else
				{
					if (this.IsDirectoryOrShortcut)
					{
						return null;
					}
					else
					{
						DocumentCache.Add (this.folderItem.FullPath);
						return DocumentCache.Statistics (this.folderItem.FullPath);
					}
				}
			}
		}


		public bool SortAccordingToLevel
		{
			//	Indique si le tri doit tenir compte du niveau (lent).
			get
			{
				return this.sortAccordingToLevel;
			}
			set
			{
				this.sortAccordingToLevel = value;
			}
		}

		public FileItem Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		protected FileItem GetParent(int level)
		{
			//	Retourne un parent.
			//	Le niveau 0 correspond au bureau.
			//	Le niveau Deep correspond à l'objet lui-même.
			//	Un niveau supérieur retourne null.
			int deep = this.Deep;
			if (level <= deep)
			{
				FileItem current = this;
				for (int i=0; i<deep-level; i++)
				{
					current = current.parent;
				}
				return current;
			}
			else
			{
				return null;
			}
		}

		public int Deep
		{
			//	Retourne la profondeur d'imbrication du dossier.
			//	Pour un dossier du bureau, la profondeur est 0.
			//	Pour un dossier du poste de travail, la profondeur est 1.
			get
			{
				int deep = 0;
				FileItem current = this;
				while (current.parent != null)
				{
					current = current.parent;
					deep++;
				}
				return deep;
			}
		}

		public static Epsitec.Common.Types.StructuredType GetStructuredType()
		{
			if (FileItem.type == null)
			{
				FileItem.type = new Epsitec.Common.Types.StructuredType ();
				FileItem.type.Fields.Add ("icon", Epsitec.Common.Types.StringType.Default);
				FileItem.type.Fields.Add ("name", Epsitec.Common.Types.StringType.Default);
				FileItem.type.Fields.Add ("info", Epsitec.Common.Types.StringType.Default);
				FileItem.type.Fields.Add ("date", Epsitec.Common.Types.StringType.Default);
				FileItem.type.Fields.Add ("size", Epsitec.Common.Types.StringType.Default);
			}

			return FileItem.type;
		}

		#region IComparable<FileItem> Members
		public int CompareTo(FileItem that)
		{
			//	Comparaison simple ou complexe, selon SortAccordingToLevel.
			//	En mode complexe (SortAccordingToLevel = true), on cherche
			//	à obtenir cet ordre:
			//		A		(deep = 0)
			//		B		(deep = 0)
			//		B/1		(deep = 1)
			//		B/1/a	(deep = 2)
			//		B/1/b	(deep = 2)
			//		B/2		(deep = 1)
			//		C		(deep = 0)
			if (this.sortAccordingToLevel)
			{
				for (int level=0; level<100; level++)
				{
					FileItem p1 = this.GetParent (level);
					FileItem p2 = that.GetParent (level);

					if (p1 == null && p2 == null)
					{
						return this.BaseCompareTo (that);
					}

					if (p1 == null && p2 != null)
					{
						return -1;
					}

					if (p1 != null && p2 == null)
					{
						return 1;
					}

					int c = p1.BaseCompareTo (p2);
					if (c != 0)
					{
						return c;
					}
				}
			}

			return this.BaseCompareTo (that);
		}

		protected int BaseCompareTo(FileItem that)
		{
			//	Comparaison simple, sans tenir compte du niveau.

			if (this.isNewEmptyDocument != that.isNewEmptyDocument)
			{
				return this.isNewEmptyDocument ? -1 : 1;  // 'nouveau document vide' au début
			}

			if (this.IsDrive != that.IsDrive)
			{
				return this.IsDrive ? -1 : 1;  // unités avant les fichiers
			}

			if (this.IsVirtual != that.IsVirtual)
			{
				return this.IsVirtual ? -1 : 1;  // unités virtuelles avant les dossiers
			}

			if (this.IsDirectoryOrShortcut != that.IsDirectoryOrShortcut)
			{
				return this.IsDirectoryOrShortcut ? -1 : 1;  // dossiers avant les fichiers
			}

			if (this.IsDrive)
			{
				int ct = this.folderItem.DriveInfo.Name.CompareTo (that.folderItem.DriveInfo.Name);
				if (ct != 0)
				{
					return ct;
				}
			}
			else
			{
				int ct = this.folderItem.TypeName.CompareTo (that.folderItem.TypeName);
				if (ct != 0)
				{
					return ct;
				}
			}

			string f1 = this.ShortFileName.ToLower ();
			string f2 = that.ShortFileName.ToLower ();
			return f1.CompareTo (f2);
		}
		#endregion

		#region IStructuredData Members

		void Epsitec.Common.Types.IStructuredData.AttachListener(string id, EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs> handler)
		{
		}

		void Epsitec.Common.Types.IStructuredData.DetachListener(string id, EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs> handler)
		{
		}

		IEnumerable<string> Epsitec.Common.Types.IStructuredData.GetValueIds()
		{
			yield return "icon";
			yield return "name";
			yield return "info";
			yield return "date";
			yield return "size";
		}

		object Epsitec.Common.Types.IStructuredData.GetValue(string id)
		{
			switch (id)
			{
				case "icon":
					return this.FixIcon;
				case "name":
					return this.ShortFileName;
				case "info":
					return this.Description;
				case "date":
					return this.FileDate;
				case "size":
					return this.FileSize;
				default:
					throw new System.InvalidOperationException ();
			}
		}

		void Epsitec.Common.Types.IStructuredData.SetValue(string id, object value)
		{
			throw new System.InvalidOperationException ();
		}

		#endregion

		#region IStructuredTypeProvider Members

		Epsitec.Common.Types.IStructuredType Epsitec.Common.Types.IStructuredTypeProvider.GetStructuredType()
		{
			return FileItem.GetStructuredType ();
		}

		#endregion

		private static Epsitec.Common.Types.StructuredType type;

		protected FolderItem folderItem;
		protected FolderItemIcon smallIcon;
		protected FileItem parent;
		protected bool isModel;
		protected bool isNewEmptyDocument;
		protected bool sortAccordingToLevel = false;
	}
}
