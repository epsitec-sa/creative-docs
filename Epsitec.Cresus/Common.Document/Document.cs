using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Common.Document
{
	public enum DocumentType
	{
		Graphic,		// document graphique
		Pictogram,		// icône
		Text,			// document texte
	}

	public enum DocumentMode
	{
		ReadOnly,		// document uniquement affichable
		Modify,			// document modifiable
		Clipboard,		// document servant uniquement de bloc-notes
	}

	/// <summary>
	/// Summary description for Document.
	/// </summary>
	[System.Serializable()]
	public class Document : ISerializable
	{
		// Crée un nouveau document vide.
		public Document(DocumentType type, DocumentMode mode)
		{
			this.type = type;
			this.mode = mode;

			if ( this.type == DocumentType.Pictogram )
			{
				this.size = new Size(20, 20);
			}
			else
			{
				this.size = new Size(210, 297);  // A4 vertical
			}

			this.hotSpot = new Point(0, 0);
			this.objects = new UndoableList(this, UndoableListType.ObjectsInsideDocument);
			this.propertiesAuto = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.propertiesSel = new UndoableList(this, UndoableListType.PropertiesInsideDocument);
			this.propertiesStyle = new UndoableList(this, UndoableListType.PropertiesInsideDocument);

			if ( this.mode == DocumentMode.Modify    ||
				 this.mode == DocumentMode.Clipboard )
			{
				this.modifier = new Modifier(this);
				this.notifier = new Notifier(this);
			}

			if ( this.mode == DocumentMode.Clipboard )
			{
				Viewer clipboardViewer = new Viewer(this);
				this.Modifier.ActiveViewer = clipboardViewer;
				this.Modifier.AttachViewer(clipboardViewer);
				this.Modifier.New();
			}
		}

		// Type de ce document.
		public DocumentType Type
		{
			get { return this.type; }
		}

		// Mode de travail pour ce document.
		public DocumentMode Mode
		{
			get { return this.mode; }
		}

		// Nom du document.
		public string Name
		{
			get { return this.name; }
			set { this.name = value; }
		}

		// Bloc-notes associé.
		public Document Clipboard
		{
			get { return this.clipboard; }
			set { this.clipboard = value; }
		}

		// Liste des objets de ce document.
		public UndoableList Objects
		{
			get { return this.objects; }
		}


		// Liste des propriétés automatiques de ce document.
		public UndoableList PropertiesAuto
		{
			get { return this.propertiesAuto; }
		}

		// Liste des propriétés sélectionnées de ce document.
		public UndoableList PropertiesSel
		{
			get { return this.propertiesSel; }
		}

		// Liste des styles de ce document.
		public UndoableList PropertiesStyle
		{
			get { return this.propertiesStyle; }
		}


		// Modificateur éventuel pour ce document.
		public Modifier Modifier
		{
			get { return this.modifier; }
		}

		// Notificateur éventuel pour ce document.
		public Notifier Notifier
		{
			get { return this.notifier; }
		}


		// Taille du document.
		public Size Size
		{
			get
			{
				return this.size;
			}
			
			set
			{
				if ( this.size != value )
				{
					this.size = value;
					this.IsDirtySerialize = true;
				}
			}
		}

		// Point chaud du document.
		public Point HotSpot
		{
			get
			{
				return this.hotSpot;
			}
			
			set
			{
				if ( this.hotSpot != value )
				{
					this.hotSpot = value;
					this.IsDirtySerialize = true;
				}
			}
		}

		// Nom du fichier associé.
		public string Filename
		{
			get
			{
				return this.filename;
			}

			set
			{
				if ( this.filename != value )
				{
					this.filename = value;

					if ( this.Notifier != null )
					{
						this.Notifier.NotifyDocumentChanged();
					}
				}
			}
		}

		// Indique si la sérialisation est nécessaire.
		public bool IsDirtySerialize
		{
			get
			{
				return this.isDirtySerialize;
			}

			set
			{
				if ( this.isDirtySerialize != value )
				{
					this.isDirtySerialize = value;

					if ( this.Notifier != null )
					{
						this.Notifier.NotifySaveChanged();
					}
				}
			}
		}

		// Ouvre un document existant sur disque.
		public string Read(string filename)
		{
			if ( this.Modifier != null )
			{
				this.Modifier.New();
			}

			try
			{
				using ( Stream stream = File.OpenRead(filename) )
				{
					string err = this.Read(stream);
					if ( err == "" )
					{
						this.Filename = filename;
						this.IsDirtySerialize = false;
					}
					else
					{
						this.IsDirtySerialize = false;
					}
					return err;
				}
			}
			catch ( System.Exception e )
			{
				this.IsDirtySerialize = false;
				return e.Message;
			}
		}

		// Ouvre un document sérialisé, soit parce que l'utilisateur veut ouvrir
		// explicitement un fichier, soit par Engine.
		public string Read(Stream stream)
		{
			if ( !this.ReadIdentifier(stream) )
			{
				return "Type de fichier incorrect.";
			}

			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Binder = new VersionDeserializationBinder();

			// Initialise la variable statique utilisée par VersionDeserializationBinder.
			// Exemple de contenu:
			// "Common.Document, Version=1.0.1777.18519, Culture=neutral, PublicKeyToken=7344997cc606b490"
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetAssembly(this.GetType());
			Document.AssemblyFullName = ass.FullName;

			// Initialise la variable statique permettant à tous les constructeurs
			// de connaître le pointeur au document.
			Document.ReadDocument = this;

			Document doc = null;
			try
			{
				string compressorName;
				using ( Stream compressor = IO.Decompression.CreateStream(stream, out compressorName) )
				{
					doc = (Document) formatter.Deserialize(compressor);
				}
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			this.objects = doc.objects;
			this.propertiesAuto = doc.propertiesAuto;
			this.propertiesStyle = doc.propertiesStyle;
			this.size = doc.size;
			this.hotSpot = doc.hotSpot;
			this.ReadFinalize();

			if ( this.Notifier != null )
			{
				this.Notifier.NotifyAllChanged();
			}
			return "";
		}

		sealed class VersionDeserializationBinder : SerializationBinder 
		{
			// Retourne un type correspondant à l'application courante, afin
			// d'accepter de désérialiser un fichier généré par une application
			// ayant un autre numéro de révision.
			// Application courante: Version=1.0.1777.18519
			// Version dans le fichier: Version=1.0.1777.11504
			public override System.Type BindToType(string assemblyName, string typeName) 
			{
				System.Type typeToDeserialize;
				typeToDeserialize = System.Type.GetType(string.Format("{0}, {1}", typeName, Document.AssemblyFullName));
				return typeToDeserialize;
			}
		}

		// Utilisé par les constructeurs de désérialisation du genre:
		// protected Toto(SerializationInfo info, StreamingContext context)
		public static Document ReadDocument = null;
		protected static string AssemblyFullName = "";

		// Adapte tous les objets après une désérialisation.
		protected void ReadFinalize()
		{
			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = false;
			}

			foreach ( AbstractObject obj in this.Deep(null) )
			{
				obj.ReadFinalize();
			}

			if ( this.Modifier != null )
			{
				this.Modifier.OpletQueueEnable = true;
			}
		}

		// Enregistre le document sur disque.
		public string Write(string filename)
		{
			System.Diagnostics.Debug.Assert(this.mode == DocumentMode.Modify);
			this.Modifier.DeselectAll();

			try
			{
				using ( Stream stream = File.OpenWrite(filename) )
				{
					this.WriteIdentifier(stream);
					Stream compressor = IO.Compression.CreateBZip2Stream(stream);
					BinaryFormatter formatter = new BinaryFormatter();
					formatter.Serialize(compressor, this);
					compressor.Close();
				}
			}
			catch ( System.Exception e )
			{
				return e.Message;
			}

			this.Filename = filename;
			this.IsDirtySerialize = false;
			return "";
		}

		// Lit les 8 bytes d'en-tête et vérifie qu'ils contiennent bien "<?icon?>".
		protected bool ReadIdentifier(Stream stream)
		{
			byte[] buffer = new byte[8];
			stream.Read(buffer, 0, 8);
			if ( buffer[0] != (byte) '<' )  return false;
			if ( buffer[1] != (byte) '?' )  return false;
			if ( buffer[2] != (byte) 'i' )  return false;
			if ( buffer[3] != (byte) 'c' )  return false;
			if ( buffer[4] != (byte) 'o' )  return false;
			if ( buffer[5] != (byte) 'n' )  return false;
			if ( buffer[6] != (byte) '?' )  return false;
			if ( buffer[7] != (byte) '>' )  return false;
			return true;
		}

		// Ecrit les 8 bytes d'en-tête "<?icon?>".
		protected void WriteIdentifier(Stream stream)
		{
			byte[] buffer = new byte[8];
			buffer[0] = (byte) '<';
			buffer[1] = (byte) '?';
			buffer[2] = (byte) 'i';
			buffer[3] = (byte) 'c';
			buffer[4] = (byte) 'o';
			buffer[5] = (byte) 'n';
			buffer[6] = (byte) '?';
			buffer[7] = (byte) '>';
			stream.Write(buffer, 0, 8);
		}

		#region Serialization
		// Sérialise le document.
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			int revision = 1;
			int version  = 0;
			info.AddValue("Revision", revision);
			info.AddValue("Version", version);
			info.AddValue("Type", this.type);
			info.AddValue("Name", this.name);
			info.AddValue("Size", this.size);
			info.AddValue("HotSpot", this.hotSpot);
			info.AddValue("UniqueObjectId", Modifier.UniqueObjectId);
			info.AddValue("UniqueStyleId", Modifier.UniqueStyleId);
			info.AddValue("Objects", this.objects);
			info.AddValue("Properties", this.propertiesAuto);
			info.AddValue("Styles", this.propertiesStyle);
		}

		// Constructeur qui désérialise le document.
		protected Document(SerializationInfo info, StreamingContext context)
		{
			this.readRevision = info.GetInt32("Revision");
			this.readVersion  = info.GetInt32("Version");
			this.type = (DocumentType) info.GetValue("Type", typeof(DocumentType));
			this.name = info.GetString("Name");
			this.size = (Size) info.GetValue("Size", typeof(Size));
			this.hotSpot = (Point) info.GetValue("HotSpot", typeof(Point));
			Modifier.UniqueObjectId = info.GetInt32("UniqueObjectId");
			Modifier.UniqueStyleId = info.GetInt32("UniqueStyleId");
			this.objects = (UndoableList) info.GetValue("Objects", typeof(UndoableList));
			this.propertiesAuto = (UndoableList) info.GetValue("Properties", typeof(UndoableList));
			this.propertiesStyle = (UndoableList) info.GetValue("Styles", typeof(UndoableList));
		}

		// Retourne le numéro de révision du fichier en cours de lecture.
		public int ReadRevision
		{
			get { return this.readRevision; }
		}

		// Retourne le numéro de version du fichier en cours de lecture.
		public int ReadVersion
		{
			get { return this.readVersion; }
		}
		#endregion

		
		// Dessine le document.
		public void Paint(Graphics graphics, DrawingContext drawingContext, Rectangle clipRect)
		{
			if ( drawingContext.RootStackIsEmpty )  return;

			if ( !clipRect.IsInfinite )
			{
				clipRect = drawingContext.Viewer.ScreenToInternal(clipRect);
			}

			AbstractObject branch = drawingContext.RootObject();
			AbstractObject activLayer = drawingContext.RootObject(2);
			AbstractObject page = drawingContext.RootObject(1);
			foreach ( ObjectLayer layer in this.Flat(page) )
			{
				bool dimmedLayer = false;
				if ( layer != activLayer )  // calque passif ?
				{
					if ( layer.Type == LayerType.Hide ||
						 drawingContext.LayerDrawingMode == LayerDrawingMode.HideInactive )
					{
						continue;
					}

					if ( layer.Type == LayerType.Dimmed &&
						 drawingContext.LayerDrawingMode == LayerDrawingMode.DimmedInactive )
					{
						dimmedLayer = true;
					}
				}

				PropertyModColor modColor = layer.PropertyModColor;
				drawingContext.modifyColor = new DrawingContext.ModifyColor(modColor.ModifyColor);

				foreach ( DeepBranchEntry entry in this.DeepBranch(layer, branch) )
				{
					AbstractObject obj = entry.Object;
					if ( !obj.BoundingBox.IntersectsWith(clipRect) )  continue;

					if ( obj.IsHide )  // objet caché ?
					{
						if ( !drawingContext.HideHalfActive )  continue;
						drawingContext.IsDimmed = true;
					}
					else
					{
						drawingContext.IsDimmed = dimmedLayer;
					}

					if ( !entry.IsInsideBranch && layer.Type != LayerType.Show )
					{
						drawingContext.IsDimmed = true;
					}

					obj.DrawGeometry(graphics, drawingContext);
				}
			}
		}

		// Imprime le document.
		public void Print(Common.Dialogs.Print dp)
		{
		}


		#region Flat
		public System.Collections.IEnumerable Flat(AbstractObject root)
		{
			return new FlatEnumerable(this, root, false);
		}

		public System.Collections.IEnumerable Flat(AbstractObject root, bool onlySelected)
		{
			return new FlatEnumerable(this, root, onlySelected);
		}

		// Enumérateur permettant de parcourir à plat l'arbre des objets.
		protected class FlatEnumerable : System.Collections.IEnumerable,
										 System.Collections.IEnumerator
		{
			public FlatEnumerable(Document document, AbstractObject root, bool onlySelected)
			{
				this.document = document;
				this.onlySelected = onlySelected;

				if ( root == null )
				{
					this.list = this.document.Objects;
				}
				else
				{
					this.list = root.Objects;
				}

				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.index = -1;
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets sélectionnés ?
				{
					while ( true )
					{
						this.index ++;
						if ( this.index >= this.list.Count )  return false;

						AbstractObject obj = this.list[this.index] as AbstractObject;
						if ( obj.IsSelected )  return true;
					}
				}
				else	// tous les objets ?
				{
					this.index ++;
					return ( this.index < this.list.Count );
				}
			}

			public object Current
			{
				get
				{
					if ( this.index >= 0 && this.index < this.list.Count )
					{
						return this.list[this.index] as AbstractObject;
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected bool						onlySelected;
			protected UndoableList				list;
			protected int						index;
		}
		#endregion

		#region FlatReverse
		public System.Collections.IEnumerable FlatReverse(AbstractObject root)
		{
			return new FlatReverseEnumerable(this, root, false);
		}

		public System.Collections.IEnumerable FlatReverse(AbstractObject root, bool onlySelected)
		{
			return new FlatReverseEnumerable(this, root, onlySelected);
		}

		// Enumérateur permettant de parcourir à plat depuis la fin l'arbre des objets.
		protected class FlatReverseEnumerable : System.Collections.IEnumerable,
												System.Collections.IEnumerator
		{
			public FlatReverseEnumerable(Document document, AbstractObject root, bool onlySelected)
			{
				this.document = document;
				this.onlySelected = onlySelected;

				if ( root == null )
				{
					this.list = this.document.Objects;
				}
				else
				{
					this.list = root.Objects;
				}

				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.index = this.list.Count;
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets sélectionnés ?
				{
					while ( true )
					{
						this.index --;
						if ( this.index < 0 )  return false;

						AbstractObject obj = this.list[this.index] as AbstractObject;
						if ( obj.IsSelected )  return true;
					}
				}
				else	// tous les objets ?
				{
					this.index --;
					return ( this.index >= 0 );
				}
			}

			public object Current
			{
				get
				{
					if ( this.index >= 0 && this.index < this.list.Count )
					{
						return this.list[this.index] as AbstractObject;
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected bool						onlySelected;
			protected UndoableList				list;
			protected int						index;
		}
		#endregion

		#region Deep
		public System.Collections.IEnumerable Deep(AbstractObject root)
		{
			return new DeepEnumerable(this, root, false);
		}

		public System.Collections.IEnumerable Deep(AbstractObject root, bool onlySelected)
		{
			return new DeepEnumerable(this, root, onlySelected);
		}

		// Enumérateur permettant de parcourir en profondeur l'arbre des objets.
		// En mode onlySelected, seuls les objets sélectionnés du premier niveau
		// sont concernés. Un objet fils d'un objet sélectionné du premier niveau
		// est toujours considéré comme sélectionné, bien qu'il ne le soit pas
		// physiquement !
		protected class DeepEnumerable : System.Collections.IEnumerable,
										 System.Collections.IEnumerator
		{
			public DeepEnumerable(Document document, AbstractObject root, bool onlySelected)
			{
				this.document = document;
				this.onlySelected = onlySelected;
				this.root = root;
				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.stack = new System.Collections.Stack();

				UndoableList list = this.document.Objects;
				if ( this.root != null )
				{
					list = this.root.Objects;
				}
				this.stack.Push(new TreeInfo(list, -1));

				this.first = true;
				this.index = 0;
			}

			protected bool InternalMoveNext()
			{
				TreeInfo ti;

				if ( this.first )
				{
					this.first = false;
					ti = this.stack.Peek() as TreeInfo;
					return ( this.index < ti.List.Count );
				}

				if ( this.index == -1 )  return false;

				ti = this.stack.Peek() as TreeInfo;
				AbstractObject obj = ti.List[this.index] as AbstractObject;
				if ( obj.Objects != null && obj.Objects.Count > 0 )  // objet avec fils ?
				{
					if ( this.onlySelected )  // seulement les objets sélectionnés ?
					{
						if ( this.stack.Count > 1 || obj.IsSelected )
						{
							this.stack.Push(new TreeInfo(obj.Objects, this.index));
							this.index = 0;
							return true;
						}
					}
					else	// tous les objets ?
					{
						this.stack.Push(new TreeInfo(obj.Objects, this.index));
						this.index = 0;
						return true;
					}
				}

				while ( true )
				{
					this.index ++;
					if ( this.index < ti.List.Count )  return true;

					ti = this.stack.Pop() as TreeInfo;
					this.index = ti.Parent;
					if ( this.index == -1 )  return false;

					ti = this.stack.Peek() as TreeInfo;
				}
			}

			public bool MoveNext()
			{
				if ( this.onlySelected )  // seulement les objets sélectionnés ?
				{
					while ( true )
					{
						if ( !this.InternalMoveNext() )  return false;
						if ( this.stack.Count > 1 )  return true;
						AbstractObject obj = this.Current as AbstractObject;
						if ( obj.IsSelected )  return true;
					}
				}
				else	// tous les objets ?
				{
					return this.InternalMoveNext();
				}
			}

			public object Current
			{
				get
				{
					TreeInfo ti = this.stack.Peek() as TreeInfo;

					if ( this.index < ti.List.Count )
					{
						return ti.List[this.index];
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected bool						onlySelected;
			protected AbstractObject			root;
			protected bool						first;
			protected int						index;
			protected System.Collections.Stack	stack;
		}
		#endregion

		#region DeepBranch
		public System.Collections.IEnumerable DeepBranch(AbstractObject root, AbstractObject branch)
		{
			return new DeepBranchEnumerable(this, root, branch);
		}

		// Enumérateur permettant de parcourir en profondeur l'arbre des objets.
		// L'objet rendu est de type DeepBranchEntry. Ceci permet de savoir si
		// l'on est ou non à l'intérieur d'une branche quelconque.
		protected class DeepBranchEnumerable : System.Collections.IEnumerable,
											   System.Collections.IEnumerator
		{
			public DeepBranchEnumerable(Document document, AbstractObject root, AbstractObject branch)
			{
				this.document = document;
				this.root = root;
				this.branch = branch;
				this.Reset();
			}

			// Implémentation de IEnumerable:
			public System.Collections.IEnumerator GetEnumerator()
			{
				return this;
			}

			// Implémentation de IEnumerator:
			public void Reset()
			{
				this.stack = new System.Collections.Stack();

				UndoableList list = this.document.Objects;
				if ( this.root != null )
				{
					list = this.root.Objects;
				}
				this.stack.Push(new TreeInfo(list, -1));

				this.first = true;
				this.index = 0;
				this.isInsideBranch = (this.root == this.branch);
			}

			public bool MoveNext()
			{
				TreeInfo ti;
				AbstractObject obj;

				if ( this.first )
				{
					this.first = false;
					ti = this.stack.Peek() as TreeInfo;
					return ( this.index < ti.List.Count );
				}

				if ( this.index == -1 )  return false;

				ti = this.stack.Peek() as TreeInfo;
				obj = ti.List[this.index] as AbstractObject;
				if ( obj.Objects != null && obj.Objects.Count > 0 )  // objet avec fils ?
				{
					if ( !this.isInsideBranch )
					{
						if ( obj == this.branch )  this.isInsideBranch = true;
					}
					this.stack.Push(new TreeInfo(obj.Objects, this.index));
					this.index = 0;
					return true;
				}

				while ( true )
				{
					this.index ++;
					if ( this.index < ti.List.Count )  return true;

					ti = this.stack.Pop() as TreeInfo;
					this.index = ti.Parent;
					if ( this.index == -1 )  return false;

					ti = this.stack.Peek() as TreeInfo;

					if ( this.isInsideBranch )
					{
						obj = ti.List[this.index] as AbstractObject;
						if ( obj == this.branch )  this.isInsideBranch = false;
					}
				}
			}

			public object Current
			{
				get
				{
					TreeInfo ti = this.stack.Peek() as TreeInfo;

					if ( this.index < ti.List.Count )
					{
						AbstractObject obj = ti.List[this.index] as AbstractObject;
						return new DeepBranchEntry(obj, this.isInsideBranch);
					}
					else
					{
						return null;
					}
				}
			}

			protected Document					document;
			protected AbstractObject			root;
			protected AbstractObject			branch;
			protected bool						isInsideBranch;
			protected bool						first;
			protected int						index;
			protected System.Collections.Stack	stack;
		}
		#endregion

		#region DeepBranchEntry
		public class DeepBranchEntry
		{
			public DeepBranchEntry(AbstractObject obj, bool isInsideBranch)
			{
				this.obj = obj;
				this.isInsideBranch = isInsideBranch;
			}

			public AbstractObject Object
			{
				get { return this.obj; }
			}

			public bool IsInsideBranch
			{
				get { return this.isInsideBranch; }
			}

			protected AbstractObject	obj;
			protected bool				isInsideBranch;
		}
		#endregion

		#region TreeInfo
		protected class TreeInfo
		{
			public TreeInfo(UndoableList list, int parent)
			{
				this.list = list;
				this.parent = parent;
			}

			public UndoableList List
			{
				get { return this.list; }
			}

			public int Parent
			{
				get { return this.parent; }
			}

			protected UndoableList		list;
			protected int				parent;
		}
		#endregion


		protected DocumentType					type;
		protected DocumentMode					mode;
		protected string						name;
		protected Document						clipboard;
		protected Size							size;
		protected Point							hotSpot;
		protected string						filename;
		protected bool							isDirtySerialize;
		protected UndoableList					objects;
		protected UndoableList					propertiesAuto;
		protected UndoableList					propertiesSel;
		protected UndoableList					propertiesStyle;
		protected Modifier						modifier;
		protected Notifier						notifier;
		protected int							readRevision;
		protected int							readVersion;
	}
}
