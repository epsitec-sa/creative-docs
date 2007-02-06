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
	public class FileListItem : System.IComparable<FileListItem>, Epsitec.Common.Types.IStructuredData, Epsitec.Common.Types.IStructuredTypeProvider
	{
		public FileListItem(string iconName, string fileName, string shortFileName, string description)
		{
			//	Crée un item pour 'Nouveau document vide'.
			this.isSynthetic = true;
			this.smallIconName = iconName;
			this.largeIconName = iconName;
			this.cachedFileName = fileName;
			this.cachedShortFileName = shortFileName;
			this.cachedDescription = description;
		}

		public FileListItem(FolderItem folderItem, bool isModel)
		{
			//	Crée un item pour un fichier ou un dossier.
			this.FolderItem = folderItem;
			this.isModel = isModel;
		}

		public FolderItem FolderItem
		{
			get
			{
				return this.folderItem;
			}
			set
			{
				System.Diagnostics.Debug.Assert (this.isSynthetic == false);
				
				this.folderItem = value;

				switch (this.folderItem.QueryMode.IconSize)
				{
					case FileInfoIconSize.Small:
						this.smallIcon = this.folderItem.Icon;
						this.largeIcon = null;
						break;
					
					case FileInfoIconSize.Large:
						this.smallIcon = null;
						this.largeIcon = this.folderItem.Icon;
						break;
					
					default:
						this.smallIcon = null;
						this.largeIcon = null;
						break;
				}

				this.cachedDateTime = new System.DateTime (0L);
				this.cachedDepth = -1;
				this.cachedFileSize = -2;
				this.cachedFileName = null;
				this.cachedShortFileName = null;
				this.cachedDescription = null;
			}
		}

		public FolderItemIcon GetSmallIcon()
		{
			if (this.SmallIcon == null)
			{
				this.SmallIcon = FileManager.GetFolderItemIcon (this.FolderItem, FolderQueryMode.SmallIcons);
			}

			return this.SmallIcon;
		}

		public FolderItemIcon GetLargeIcon()
		{
			if (this.LargeIcon == null)
			{
				this.LargeIcon = FileManager.GetFolderItemIcon (this.FolderItem, FolderQueryMode.LargeIcons);
			}

			return this.LargeIcon;
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

		public FolderItemIcon LargeIcon
		{
			get
			{
				return this.largeIcon;
			}
			set
			{
				this.largeIcon = value;
			}
		}

		public string FullPath
		{
			get
			{
				if (this.folderItem == null)
				{
					return this.GetResolvedFileName ();
				}
				else
				{
					return this.folderItem.FullPath;
				}
			}
		}

		public string GetResolvedFileName()
		{
			//	Nom du fichier avec le chemin d'accès complet. Dans le cas d'un raccourci,
			//	retourne le chemin complet de la cible.
			if (this.cachedFileName == null)
			{
				if (this.IsShortcut)
				{
					FolderItem item = FileManager.ResolveShortcut (this.folderItem, FolderQueryMode.NoIcons);
					this.cachedFileName = item.FullPath;
				}
				else
				{
					this.cachedFileName = this.folderItem.FullPath;
				}
			}
			
			return this.cachedFileName;
		}

		public string ShortFileName
		{
			//	Nom du fichier court, sans le chemin d'accès ni l'extension.
			get
			{
				if (this.cachedShortFileName == null)
				{
					if (this.IsDirectory)
					{
						this.cachedShortFileName = TextLayout.ConvertToTaggedText (this.folderItem.DisplayName);
					}
					else if (this.IsShortcut)
					{
						this.cachedShortFileName = TextLayout.ConvertToTaggedText (this.folderItem.DisplayName);
//						FolderItem item = FileManager.ResolveShortcut (this.folderItem, FolderQueryMode.NoIcons);
//						this.cachedShortFileName = TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (item.FullPath));
					}
					else
					{
						this.cachedShortFileName = TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (this.folderItem.FullPath));
					}
				}

				return this.cachedShortFileName;
			}
		}

		public bool IsSynthetic
		{
			//	Retourne true si c'est un 'Nouveau document vide'
			get
			{
				return this.isSynthetic;
			}
		}

		public bool IsDirectory
		{
			get
			{
				if (this.isSynthetic)
				{
					return false;
				}
				else
				{
					return this.folderItem.IsFolder;
				}
			}
		}

		public bool IsDataFile
		{
			get
			{
				if (this.IsDirectory)
				{
					return false;
				}
				if (this.IsShortcut)
				{
					return false;
				}
				if (this.IsDrive)
				{
					return false;
				}
				if (this.IsSynthetic)
				{
					return false;
				}
				if (this.IsVirtual)
				{
					return false;
				}

				return true;
			}
		}

		public bool IsDirectoryOrShortcut
		{
			get
			{
				if (this.isSynthetic)
				{
					return false;
				}
				else
				{
					return this.folderItem.IsFolder || this.folderItem.IsShortcut;
				}
			}
		}

		public bool IsShortcut
		{
			get
			{
				if (this.isSynthetic)
				{
					return false;
				}
				else
				{
					return this.folderItem.IsShortcut;
				}
			}
		}

		public bool IsDrive
		{
			get
			{
				if (this.isSynthetic)
				{
					return false;
				}
				else
				{
					return this.folderItem.IsDrive;
				}
			}
		}

		public bool IsVirtual
		{
			get
			{
				if (this.isSynthetic)
				{
					return false;
				}
				else
				{
					return this.folderItem.IsVirtual;
				}
			}
		}

		public string FileDate
		{
			//	Date de modification du fichier.
			get
			{
				this.InitializeCachedFileDate ();
				
				if (this.cachedDateTime.Ticks == 0)
				{
					return "";
				}
				else
				{
					return string.Concat (this.cachedDateTime.ToShortDateString (), " ", this.cachedDateTime.ToShortTimeString ());
				}
			}
		}

		private void InitializeCachedFileDate()
		{
			if (this.cachedDateTime.Ticks == 0)
			{
				if (this.isSynthetic)
				{
					//	rien à faire
				}
				else if (!string.IsNullOrEmpty (this.folderItem.FullPath))
				{
					if (this.IsDirectoryOrShortcut)
					{
						System.IO.DirectoryInfo info = new System.IO.DirectoryInfo (this.folderItem.FullPath);

						if (info.Exists)
						{
							this.cachedDateTime = info.LastWriteTime;
						}
					}
					else
					{
						System.IO.FileInfo info = new System.IO.FileInfo (this.folderItem.FullPath);

						if (info.Exists)
						{
							this.cachedDateTime = info.LastWriteTime;
						}
					}
				}
			}
		}

		public string FileSize
		{
			//	Taille du fichier en kilo-bytes.
			get
			{
				this.InitializeCachedFileSize ();

				if (this.cachedFileSize < 0)
				{
					return "";
				}
				else
				{
					long size = this.cachedFileSize;
					
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

		private void InitializeCachedFileSize()
		{
			if (this.cachedFileSize == -2)
			{
				if (this.isSynthetic || this.IsDirectoryOrShortcut)
				{
					this.cachedFileSize = -1;
				}
				else
				{
					System.IO.FileInfo info = new System.IO.FileInfo (this.folderItem.FullPath);
					if (!info.Exists)
					{
						this.cachedFileSize = -1;
					}
					else
					{
						this.cachedFileSize = info.Length;
					}
				}
			}
		}

		public string Description
		{
			//	Retourne la description du fichier, basée sur les statistiques si elles existent.
			get
			{
				if (this.cachedDescription == null)
				{
					if (this.IsDirectoryOrShortcut)
					{
						this.cachedDescription = this.folderItem.TypeName;
					}
					else
					{
						IDocumentInfo stat = this.Statistics;
						if (stat == null)
						{
							this.cachedDescription = this.isModel ? Res.Strings.Dialog.File.Model : Res.Strings.Dialog.File.Document;
						}
						else
						{
							this.cachedDescription = stat.GetDescription ();
//-							string.Format (Res.Strings.Dialog.File.Statistics, stat.PageFormat, stat.PageCount.ToString (), stat.LayerCount.ToString (), stat.ObjectCount.ToString (), stat.ComplexObjectCount.ToString (), stat.FontCount.ToString (), stat.ImageCount.ToString ());
						}
					}
				}

				return this.cachedDescription;
			}
		}

		public string GetIconName(FileInfoIconSize size)
		{
			if (size == FileInfoIconSize.Small)
			{
				if (this.smallIconName == null)
				{
					FolderItemIcon icon = this.GetSmallIcon ();
					
					this.smallIconName = "";

					if (icon != null)
					{
						if ((this.IsDirectoryOrShortcut) ||
							(!this.GetAsyncImage ()))
						{
							this.smallIconName = icon.ImageName;
						}
					}
				}
				
				return this.smallIconName.Length == 0 ? null : this.smallIconName;
			}

			if (size == FileInfoIconSize.Large)
			{
				if (this.largeIconName == null)
				{
					FolderItemIcon icon = this.GetLargeIcon ();

					this.largeIconName = "";

					if (icon != null)
					{
						if ((this.IsDirectoryOrShortcut) ||
							(!this.GetAsyncImage ()))
						{
							this.largeIconName = icon.ImageName;
						}
					}
				}

				return this.largeIconName.Length == 0 ? null : this.largeIconName;
			}
			
			return null;
		}

		private bool GetAsyncImage()
		{
			if (this.documentInfo == null)
			{
				DocumentCache.Add (this.folderItem.FullPath);
				this.documentInfo = DocumentCache.FindDocumentInfo (this.folderItem.FullPath);

				if (this.documentInfo != null)
				{
					this.documentInfo.GetAsyncThumbnail (
						delegate (Image image)
						{
							this.cachedImage = image;

							if (this.attachedImagePlaceholder != null)
							{
								this.attachedImagePlaceholder.Image = image;
								this.attachedImagePlaceholder.PaintFrame = true;
								this.attachedImagePlaceholder.DisplayMode = ImageDisplayMode.Stretch;
							}
						});
				}
			}
			
			return this.documentInfo != null;
		}

		public void FillCache()
		{
			string text;
			
			text = this.ShortFileName;
			text = this.FileDate;
			text = this.FileSize;
			text = this.Description;
//-			text = this.GetIconName (FileInfoIconSize.Small);
//-			text = this.GetIconName (FileInfoIconSize.Large);
		}

		public ImagePlaceholder AttachedImagePlaceholder
		{
			get
			{
				return this.attachedImagePlaceholder;
			}
			set
			{
				this.attachedImagePlaceholder = value;
			}
		}

		public void GetImage(out Image image, out bool icon)
		{
			//	Donne l'image miniature associée au fichier.
			if (this.isSynthetic)  // nouveau document vide ?
			{
				image = null;
				icon = false;
			}
			else
			{
				if ((this.IsDirectoryOrShortcut) ||
					(!this.GetAsyncImage ()))
				{
					image = null;
					icon = true;
				}
				else
				{
					image = this.cachedImage;
					
					if (image == null)
					{
						icon = true;
					}
					else
					{
						icon = false;
					}
				}
			}
		}

		protected IDocumentInfo Statistics
		{
			//	Retourne les statistiques associées au fichier.
			get
			{
				if (this.isSynthetic)  // nouveau document vide ?
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
						return DocumentCache.FindDocumentInfo (this.folderItem.FullPath);
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

		public FileListItem Parent
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

		private List<FileListItem> GetAncestorList()
		{
			//	Retourne la liste des ancêtres dans l'ordre feuille->racine
			//	(en dernier dans la liste, on trouve toujours le bureau).
			List<FileListItem> list = new List<FileListItem> ();
			FileListItem current = this;
			while ((current != null) && (list.Count < 100))
			{
				list.Add (current);
				current = current.parent;
			}
			return list;
		}

		public int Depth
		{
			//	Retourne la profondeur d'imbrication du dossier.
			//	Pour un dossier du bureau, la profondeur est 0.
			//	Pour un dossier du poste de travail, la profondeur est 1.
			get
			{
				if (this.cachedDepth < 0)
				{
					int depth = 0;
					FileListItem current = this;
					while (current.parent != null)
					{
						current = current.parent;
						depth++;
					}
					this.cachedDepth = depth;
				}
				
				return this.cachedDepth;
			}
		}

		public static Epsitec.Common.Types.StructuredType GetStructuredType()
		{
			//	Retourne un descripteur de type structuré pour permettre d'accéder
			//	à FileItem comme si c'était une structure de type StructuredData.
			//	Chaque champ qui doit être accessible doit être nommé et décrit
			//	ici; voir aussi l'implémentation de IStructuredData plus loin.
			if (FileListItem.type == null)
			{
				FileListItem.type = new Epsitec.Common.Types.StructuredType ();

				//	TODO: remplacer les Druid.Empty par des caption ID décrivant
				//	les diverses colonnes; cela implique qu'il faudra commencer
				//	par rajouter des captions, puis utiliser Res.Captions.Xyz.Id
				//	à la place des Druid.Empty ci-après :

				FileListItem.type.Fields.Add ("icon", Epsitec.Common.Types.StringType.Default, Druid.Empty);
				FileListItem.type.Fields.Add ("name", Epsitec.Common.Types.StringType.Default, Druid.Empty);
				FileListItem.type.Fields.Add ("type", Epsitec.Common.Types.IntegerType.Default, Druid.Empty);
				FileListItem.type.Fields.Add ("date", Epsitec.Common.Types.DateTimeType.Default, Druid.Empty);
				FileListItem.type.Fields.Add ("size", Epsitec.Common.Types.LongIntegerType.Default, Druid.Empty);
			}

			return FileListItem.type;
		}

		#region IComparable<FileItem> Members
		public int CompareTo(FileListItem that)
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

			if (this == that)
			{
				return 0;
			}

			if (this.sortAccordingToLevel)
			{
				List<FileListItem> path1 = this.GetAncestorList ();
				List<FileListItem> path2 = that.GetAncestorList ();

				int index1 = path1.Count;
				int index2 = path2.Count;

				while ((index1 > 0) || (index2 > 0))
				{
					FileListItem p1 = null;
					FileListItem p2 = null;
					
					if (index1-- > 0)
					{
						p1 = path1[index1];
					}
					if (index2-- > 0)
					{
						p2 = path2[index2];
					}
					
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

		protected int BaseCompareTo(FileListItem that)
		{
			//	Comparaison simple, sans tenir compte du niveau.
			
			if (this == that)
			{
				return 0;
			}

			if (this.isSynthetic || that.isSynthetic)
			{
				if (this.isSynthetic == that.isSynthetic)
				{
					return string.Compare (this.ShortFileName, that.ShortFileName);
				}
				else
				{
					return this.isSynthetic ? -1 : 1;  // 'nouveau document vide' au début
				}
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

			string f1 = this.ShortFileName.ToLowerInvariant ();
			string f2 = that.ShortFileName.ToLowerInvariant ();
			return f1.CompareTo (f2);
		}
		#endregion

		private static int PropertyComparer(object a, object b)
		{
			FileListItem item1 = a as FileListItem;
			FileListItem item2 = b as FileListItem;
			return item1.BaseCompareTo (item2);
		}

		public static PropertyComparer GetDescriptionPropertyComparer()
		{
			return FileListItem.PropertyComparer;
		}

		#region IStructuredData Members

		void Epsitec.Common.Types.IStructuredData.AttachListener(string id, EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs> handler)
		{
		}

		void Epsitec.Common.Types.IStructuredData.DetachListener(string id, EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs> handler)
		{
		}

		IEnumerable<string> Epsitec.Common.Types.IStructuredData.GetValueIds()
		{
			//	Retourne la liste des champs définis pour cette structure; voir
			//	aussi la méthode GetStructuredType.

			yield return "icon";
			yield return "name";
			yield return "type";
			yield return "date";
			yield return "size";
		}

		object Epsitec.Common.Types.IStructuredData.GetValue(string id)
		{
			//	Retourne la valeur du champ spécifié.
			switch (id)
			{
				case "icon":
					return "x";
				
				case "name":
					return this.ShortFileName;
				
				case "type":
					return 0;
				
				case "date":
					this.InitializeCachedFileDate ();
					return this.cachedDateTime;
				
				case "size":
					this.InitializeCachedFileSize ();
					return this.cachedFileSize;
				
				default:
					return Epsitec.Common.Types.UnknownValue.Instance;
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
			return FileListItem.GetStructuredType ();
		}

		#endregion

		private static Epsitec.Common.Types.StructuredType type;

		private string cachedFileName;
		private string cachedShortFileName;
		private string cachedDescription;
		private System.DateTime cachedDateTime;
		private long cachedFileSize = -2;
		private int cachedDepth = -1;
		private IDocumentInfo documentInfo;
		private Image cachedImage;
		
		private string smallIconName;
		private string largeIconName;

		private ImagePlaceholder attachedImagePlaceholder;
		
		protected FolderItem folderItem;
		protected FolderItemIcon smallIcon;
		protected FolderItemIcon largeIcon;
		protected FileListItem parent;
		protected bool isModel;
		protected bool isSynthetic;
		protected bool sortAccordingToLevel = false;
	}
}
