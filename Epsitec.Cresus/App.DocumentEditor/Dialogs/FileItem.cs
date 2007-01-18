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
				if (this.cachedFileName == null)
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						this.cachedFileName = Common.Document.Settings.GlobalSettings.NewEmptyDocument;
					}
					else
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
				}
				
				return this.cachedFileName;
			}
		}

		public string ShortFileName
		{
			//	Nom du fichier court, sans le chemin d'accès ni l'extension.
			get
			{
				if (this.cachedShortFileName == null)
				{
					if (this.isNewEmptyDocument)  // nouveau document vide ?
					{
						this.cachedShortFileName = "—";
					}
					else
					{
						if (this.IsDirectory)
						{
							this.cachedShortFileName = TextLayout.ConvertToTaggedText (this.folderItem.DisplayName);
						}
						else if (this.IsShortcut)
						{
							FolderItem item = FileManager.ResolveShortcut (this.folderItem, FolderQueryMode.NoIcons);
							this.cachedShortFileName = TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (item.FullPath));
						}
						else
						{
							this.cachedShortFileName = TextLayout.ConvertToTaggedText (System.IO.Path.GetFileNameWithoutExtension (this.folderItem.FullPath));
						}
					}
				}

				return this.cachedShortFileName;
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
				else
				{
					return this.folderItem.IsFolder;
				}
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
				if (this.isNewEmptyDocument)
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
				if (this.isNewEmptyDocument)
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
				if (this.isNewEmptyDocument)
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
				if (this.isNewEmptyDocument)
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
				if (this.isNewEmptyDocument || this.IsDirectoryOrShortcut)
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

		private List<FileItem> GetAncestorList()
		{
			//	Retourne la liste des ancêtres dans l'ordre feuille->racine
			//	(en dernier dans la liste, on trouve toujours le bureau).
			List<FileItem> list = new List<FileItem> ();
			FileItem current = this;
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
					FileItem current = this;
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
			if (FileItem.type == null)
			{
				FileItem.type = new Epsitec.Common.Types.StructuredType ();

				//	TODO: remplacer les Druid.Empty par des caption ID décrivant
				//	les diverses colonnes; cela implique qu'il faudra commencer
				//	par rajouter des captions, puis utiliser Res.Captions.Xyz.Id
				//	à la place des Druid.Empty ci-après :

				FileItem.type.Fields.Add ("icon", Epsitec.Common.Types.StringType.Default, Druid.Empty);
				FileItem.type.Fields.Add ("name", Epsitec.Common.Types.StringType.Default, Druid.Empty);
				FileItem.type.Fields.Add ("info", Epsitec.Common.Types.StringType.Default, Druid.Empty);
				FileItem.type.Fields.Add ("date", Epsitec.Common.Types.DateTimeType.Default, Druid.Empty);
				FileItem.type.Fields.Add ("size", Epsitec.Common.Types.LongIntegerType.Default, Druid.Empty);
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
				List<FileItem> path1 = this.GetAncestorList ();
				List<FileItem> path2 = that.GetAncestorList ();

				int index1 = path1.Count;
				int index2 = path2.Count;

				while ((index1 > 0) || (index2 > 0))
				{
					FileItem p1 = null;
					FileItem p2 = null;
					
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

			string f1 = this.ShortFileName.ToLowerInvariant ();
			string f2 = that.ShortFileName.ToLowerInvariant ();
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
			//	Retourne la liste des champs définis pour cette structure; voir
			//	aussi la méthode GetStructuredType.

			yield return "icon";
			yield return "name";
			yield return "info";
			yield return "date";
			yield return "size";
		}

		object Epsitec.Common.Types.IStructuredData.GetValue(string id)
		{
			//	Retourne la valeur du champ spécifié.
			switch (id)
			{
				case "icon":
					return this.FixIcon;
				
				case "name":
					return this.ShortFileName;
				
				case "info":
					return this.Description;
				
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
			return FileItem.GetStructuredType ();
		}

		#endregion

		private static Epsitec.Common.Types.StructuredType type;

		private string cachedFileName;
		private string cachedShortFileName;
		private System.DateTime cachedDateTime;
		private long cachedFileSize = -2;
		private int cachedDepth = -1;
		
		protected FolderItem folderItem;
		protected FolderItemIcon smallIcon;
		protected FileItem parent;
		protected bool isModel;
		protected bool isNewEmptyDocument;
		protected bool sortAccordingToLevel = false;
	}
}
