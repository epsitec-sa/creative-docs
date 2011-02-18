//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX & Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.IO;

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;

namespace Epsitec.Common.Dialogs
{
	//	Cette classe représente une 'ligne' dans la liste, qui peut représenter
	//	un fichier, un dossier ou la commande 'nouveau document vide'.
	public class FileListItem : System.IComparable<FileListItem>, System.IEquatable<FileListItem>, Epsitec.Common.Types.IStructuredData, Epsitec.Common.Types.IStructuredTypeProvider
	{
		public FileListItem(string iconUri, string fileName, string shortFileName, string description)
		{
			//	Crée un item pour 'Nouveau document vide'.
			this.isSynthetic = true;
			this.smallIconUri= iconUri;
			this.largeIconUri = iconUri;
			this.cachedFileName = fileName;
			this.cachedShortFileName = shortFileName;
			this.cachedDescription = description;
		}

		public FileListItem(FolderItem folderItem)
		{
			//	Crée un item pour un fichier ou un dossier.
			this.FolderItem = folderItem;
		}

		public string DefaultDescription
		{
			get
			{
				return this.defaultDescription;
			}
			set
			{
				this.defaultDescription = value;
			}
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
				if (this.folderItem.IsEmpty)
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
					this.cachedTarget   = item;
				}
				else
				{
					this.cachedFileName = this.folderItem.FullPath;
				}
			}
			
			return this.cachedFileName;
		}

		public FolderItem GetResolvedTarget()
		{
			this.GetResolvedFileName ();  //  pour mettre à jour cachedTarget
			return this.cachedTarget;
		}

		public string ShortFileName
		{
			//	Nom du fichier court, sans le chemin d'accès (et ni l'extension, si c'est
			//	défini comme tel par l'utilisateur)
			get
			{
				if (this.cachedShortFileName == null)
				{
					if (this.IsDirectory)
					{
						this.cachedShortFileName = this.folderItem.DisplayName;
					}
					else if (this.IsShortcut)
					{
						this.cachedShortFileName = this.folderItem.DisplayName;
					}
					else
					{
						this.cachedShortFileName = FolderItem.GetShortFileName (this.folderItem.FullPath);
					}
				}

				return this.cachedShortFileName;
			}
			set
			{
				this.cachedShortFileName = value;
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

		public bool IsDefaultItem
		{
			get
			{
				return this.isDefaultItem;
			}
			set
			{
				this.isDefaultItem = value;
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
				
				if (this.cachedDateTime.Ticks < 1000)
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
					try
					{
						if (this.IsDrive)
						{
							this.cachedDateTime = new System.DateTime (1L);
						}
						else if (this.IsDirectory)
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
					catch (System.ArgumentException)
					{
						this.cachedDateTime = new System.DateTime (1L);
					}
					catch (System.IO.IOException)
					{
						this.cachedDateTime = new System.DateTime (1L);
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
					
					size = (size+1023)/1024;
					if (size/1024 < 10)
					{
						double s = (double) size;
						s = System.Math.Ceiling (s*1000/1024);  // 0..999 KB
						return string.Format (Res.Strings.Dialog.File.Size.Kilo.ToSimpleText (), s.ToString ());
					}

					size = (size+1023)/1024;
					if (size/1024 < 10)
					{
						double s = (double) size;
						s = System.Math.Ceiling (s*1000/1024);  // 0..999 MB
						return string.Format (Res.Strings.Dialog.File.Size.Mega.ToSimpleText (), s.ToString ());
					}

					size = (size+1023)/1024;
					if (size/1024 < 10)
					{
						double s = (double) size;
						s = System.Math.Ceiling (s*1000/1024);  // 0..999 GB
						return string.Format (Res.Strings.Dialog.File.Size.Giga.ToSimpleText (), s.ToString ());
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
							this.cachedDescription = this.defaultDescription;
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

		public string GetIconUri(FileInfoIconSize size)
		{
			if (size == FileInfoIconSize.Small)
			{
				if (this.smallIconUri == null)
				{
					FolderItemIcon icon = this.GetSmallIcon ();
					
					this.smallIconUri = "";

					if (icon != null)
					{
						if ((this.IsDirectoryOrShortcut) ||
							(!this.GetAsyncImage ()))
						{
							this.smallIconUri = icon.ImageName;
						}
					}
				}
				
				return this.smallIconUri.Length == 0 ? null : this.smallIconUri;
			}

			if (size == FileInfoIconSize.Large)
			{
				if (this.largeIconUri == null)
				{
					FolderItemIcon icon = this.GetLargeIcon ();

					this.largeIconUri = "";

					if (icon != null)
					{
						if ((this.IsDirectoryOrShortcut) ||
							(!this.GetAsyncImage ()))
						{
							this.largeIconUri = icon.ImageName;
						}
					}
				}

				return this.largeIconUri.Length == 0 ? null : this.largeIconUri;
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
//-			text = this.GetIconUri (FileInfoIconSize.Small);
//-			text = this.GetIconUri (FileInfoIconSize.Large);
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

				FileListItem.type.Fields.Add ("icon", Epsitec.Common.Types.StringType.NativeDefault, Support.Druid.Empty);
				FileListItem.type.Fields.Add ("name", Epsitec.Common.Types.StringType.NativeDefault, Res.Captions.File.Column.Name.Id);
				FileListItem.type.Fields.Add ("info", Epsitec.Common.Types.IntegerType.Default, Res.Captions.File.Column.Info.Id);
				FileListItem.type.Fields.Add ("date", Epsitec.Common.Types.DateTimeType.Default, Res.Captions.File.Column.Date.Id);
				FileListItem.type.Fields.Add ("size", Epsitec.Common.Types.LongIntegerType.Default, Res.Captions.File.Column.Size.Id);
			}

			return FileListItem.type;
		}

		#region IComparable<FileListItem> Members
		
		public int CompareTo(FileListItem other)
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

			if (this == other)
			{
				return 0;
			}

			if (this.sortAccordingToLevel)
			{
				List<FileListItem> path1 = this.GetAncestorList ();
				List<FileListItem> path2 = other.GetAncestorList ();

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
						return this.BaseCompareTo (other);
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

			return this.BaseCompareTo (other);
		}

		protected int BaseCompareTo(FileListItem other)
		{
			//	Comparaison simple, sans tenir compte du niveau.
			
			if (this == other)
			{
				return 0;
			}

			if (this.isSynthetic || other.isSynthetic)
			{
				if (this.isSynthetic == other.isSynthetic)
				{
					return string.Compare (this.ShortFileName, other.ShortFileName);
				}
				else
				{
					return this.isSynthetic ? -1 : 1;  // 'nouveau document vide' au début
				}
			}
			if (this.isDefaultItem || other.isDefaultItem)
			{
				if (this.isDefaultItem == other.isDefaultItem)
				{
					return string.Compare (this.ShortFileName, other.ShortFileName);
				}
				else
				{
					return this.isDefaultItem ? -1 : 1;  // 'document par défaut' au début
				}
			}

			if (this.IsDrive != other.IsDrive)
			{
				return this.IsDrive ? -1 : 1;  // unités avant les fichiers
			}

			if (this.IsVirtual != other.IsVirtual)
			{
				return this.IsVirtual ? -1 : 1;  // unités virtuelles avant les dossiers
			}

			if (this.IsDirectoryOrShortcut != other.IsDirectoryOrShortcut)
			{
				return this.IsDirectoryOrShortcut ? -1 : 1;  // dossiers avant les fichiers
			}

			if (this.IsDrive)
			{
				DriveInfo infoA = FileListItem.GetDriveInfo (this.FullPath);
				DriveInfo infoB = FileListItem.GetDriveInfo (other.FullPath);

				if ((infoA == null) ||
					(infoB == null))
				{
					if (infoA != infoB)
					{
						if (infoA == null)
						{
							return 1;
						}
						else
						{
							return -1;
						}
					}
				}
				else
				{
					int typeA = FileListItem.GetDriveTypeRank (infoA.DriveType);
					int typeB = FileListItem.GetDriveTypeRank (infoB.DriveType);

					if (typeA != typeB)
					{
						if (typeA < typeB)
						{
							return -1;
						}
						else
						{
							return 1;
						}
					}
				}
				
				int ct = this.folderItem.DriveInfo.Name.CompareTo (other.folderItem.DriveInfo.Name);
				if (ct != 0)
				{
					return ct;
				}
			}
			else
			{
				int ct = this.folderItem.TypeName.CompareTo (other.folderItem.TypeName);
				if (ct != 0)
				{
					return ct;
				}
			}

			string f1 = this.ShortFileName.ToLowerInvariant ();
			string f2 = other.ShortFileName.ToLowerInvariant ();
			return f1.CompareTo (f2);
		}

		#endregion

		#region IEquatable<FileItem> Members

		public bool Equals(FileListItem other)
		{
			return this.CompareTo (other) == 0;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return this.Equals (obj as FileListItem);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

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

		void Epsitec.Common.Types.IStructuredData.SetValue(string id, object value)
		{
			throw new System.InvalidOperationException ();
		}

		IEnumerable<string> Epsitec.Common.Types.IStructuredData.GetValueIds()
		{
			//	Retourne la liste des champs définis pour cette structure; voir
			//	aussi la méthode GetStructuredType.

			yield return "icon";
			yield return "name";
			yield return "info";
			yield return "date";
			yield return "size";
		}

		object Epsitec.Common.Types.IValueStore.GetValue(string id)
		{
			//	Retourne la valeur du champ spécifié.
			switch (id)
			{
				case "icon":
					return "x";
				
				case "name":
					if (this.IsDrive)
					{
						return string.Concat (this.FullPath ?? "", this.ShortFileName ?? "");
					}
					else
					{
						return this.ShortFileName;
					}

				case "info":
					if (string.IsNullOrEmpty (this.Description))
					{
						return "";
					}
					else
					{
						return this.Description.Split (new string[] { "<br" }, System.StringSplitOptions.None)[0];
					}
				
				case "date":
					this.InitializeCachedFileDate ();
					return this.cachedDateTime;
				
				case "size":
					this.InitializeCachedFileSize ();
					return this.cachedFileSize;
				
				default:
					return Epsitec.Common.Types.UnknownValue.Value;
			}
		}

		void Epsitec.Common.Types.IValueStore.SetValue(string id, object value, Epsitec.Common.Types.ValueStoreSetMode mode)
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

		private static DriveInfo GetDriveInfo(string fullPath)
		{
			lock (FileListItem.globalExclusion)
			{
				int ticks = System.Environment.TickCount;
				int delta = System.Math.Abs (ticks - FileListItem.driveInfoTicks);

				if ((delta > 1000) ||
					(FileListItem.driveInfos == null))
				{
					FileListItem.driveInfos = DriveInfo.GetDrives ();
					FileListItem.driveInfoTicks = ticks;
				}

				foreach (DriveInfo info in FileListItem.driveInfos)
				{
					if (info.Name == fullPath)
					{
						return info;
					}
				}
			}

			return null;
		}
		
		private static int GetDriveTypeRank(DriveType driveType)
		{
			switch (driveType)
			{
				case DriveType.Fixed:
				case DriveType.Ram:
					return 0;
				case DriveType.Removable:
				case DriveType.CDRom:
					return 1;
				case DriveType.Network:
					return 2;
				default:
					return 9;
			}
		}

		private static object globalExclusion = new object ();
		private static Epsitec.Common.Types.StructuredType type;
		private static int driveInfoTicks;
		private static DriveInfo[] driveInfos;

		private string cachedFileName;
		private string cachedShortFileName;
		private string cachedDescription;
		private System.DateTime cachedDateTime;
		private long cachedFileSize = -2;
		private int cachedDepth = -1;
		private IDocumentInfo documentInfo;
		private Image cachedImage;
		private FolderItem cachedTarget;
		
		private string smallIconUri;
		private string largeIconUri;
		private string defaultDescription;

		private ImagePlaceholder attachedImagePlaceholder;
		
		protected FolderItem folderItem;
		protected FolderItemIcon smallIcon;
		protected FolderItemIcon largeIcon;
		protected FileListItem parent;
		protected bool isSynthetic;
		protected bool isDefaultItem;
		protected bool sortAccordingToLevel = false;
	}
}
