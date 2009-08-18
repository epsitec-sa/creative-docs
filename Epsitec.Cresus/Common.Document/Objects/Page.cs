using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum MasterType
	{
		Slave    = 0,	// page normale
		All      = 1,	// page modèle appliquée à toutes les pages
		Even     = 2,	// page modèle appliquée aux pages paires
		Odd      = 3,	// page modèle appliquée aux page impaires
		None     = 4,	// page modèle appliquée à la demande
	}

	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum MasterUse
	{
		Never    = 0,	// n'utilise jamais de page modèle
		Default  = 1,	// utilise une page modèle
		Specific = 2,	// utilise une page modèle donnée
	}

	/// <summary>
	/// La classe Page est la classe de l'objet graphique "page".
	/// </summary>
	[System.Serializable()]
	public class Page : Objects.Abstract
	{
		public Page(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.objects = new UndoableList(this.document, UndoableListType.ObjectsInsideDocument);
			this.guides = new UndoableList(this.document, UndoableListType.Guides);
			this.rank = 0;
			this.shortName = "";
			this.masterType = MasterType.Slave;
			this.masterUse = MasterUse.Default;
			this.masterPageToUse = null;
			this.masterAutoStop = false;
			this.masterSpecific = false;
			this.masterGuides = true;
			this.pageSize = new Size(0,0);
			this.hotSpot = Point.Zero;
			this.glyphOrigin = Point.Zero;
			this.glyphSize = new Size(0,0);
			this.language = null;
			this.pageStyle = null;
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Page(document, model);
		}

		public int CurrentLayer
		{
			//	Rang du calque courant.
			get
			{
				return this.currentLayer;
			}
			
			set
			{
				this.currentLayer = value;
			}
		}

		public int Rank
		{
			//	Rang de la page (0..n). Les pages normales et les pages maîtres
			//	ont chacune un rang indépendant commençant à zéro.
			get
			{
				return this.rank;
			}

			set
			{
				this.rank = value;
			}
		}

		public string ShortName
		{
			//	Nom court automatique de la page ("n" ou "Mn").
			get
			{
				return this.shortName;
			}

			set
			{
				this.shortName = value;
			}
		}

		public string LongName
		{
			//	Nom de la page suivi éventuellement du format.
			get
			{
				string text = this.Name;

				if ( this.PageSize.Width != 0 && this.PageSize.Height == 0 )
				{
					text = string.Format("{0} ({1}x...)", text, this.document.Modifier.RealToString(this.PageSize.Width));
				}
				
				if ( this.PageSize.Width == 0 && this.PageSize.Height != 0 )
				{
					text = string.Format("{0} (...x{1})", text, this.document.Modifier.RealToString(this.PageSize.Height));
				}
				
				if ( this.PageSize.Width != 0 && this.PageSize.Height != 0 )
				{
					text = string.Format("{0} ({1}x{2})", text, this.document.Modifier.RealToString(this.PageSize.Width), this.document.Modifier.RealToString(this.PageSize.Height));
				}
				
				return text;
			}
		}

		public string InfoName
		{
			//	Texte d'information sur la page en cours.
			get
			{
				string text = this.Name;
				if ( text == "" )
				{
					text = this.ShortName;
				}

				string width;
				if ( this.PageSize.Width == 0 )
				{
					width = this.document.Modifier.RealToString(this.document.DocumentSize.Width);
				}
				else
				{
					width = Misc.Bold(this.document.Modifier.RealToString(this.PageSize.Width));
				}

				string height;
				if ( this.PageSize.Height == 0 )
				{
					height = this.document.Modifier.RealToString(this.document.DocumentSize.Height);
				}
				else
				{
					height = Misc.Bold(this.document.Modifier.RealToString(this.PageSize.Height));
				}
				
				return string.Format("{0} ({1}x{2})", text, width, height);
			}
		}

		public MasterType MasterType
		{
			//	Type de la page maître. Le type Slave indique qu'il s'agit d'une
			//	page normale.
			get
			{
				return this.masterType;
			}

			set
			{
				if ( this.masterType != value )
				{
					this.InsertOpletType();
					this.masterType = value;
					this.document.Modifier.UpdatePageShortNames();
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public MasterUse MasterUse
		{
			//	Pour une page normale, indique quelle page maître il faut utiliser.
			get
			{
				return this.masterUse;
			}

			set
			{
				if ( this.masterUse != value )
				{
					this.InsertOpletType();
					this.masterUse = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public Page MasterPageToUse
		{
			//	Si MasterUse = Specific, donne directement la page maître à utiliser.
			get
			{
				return this.masterPageToUse;
			}

			set
			{
				if ( this.masterPageToUse != value )
				{
					this.InsertOpletType();
					this.masterPageToUse = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public bool MasterGuides
		{
			//	Indique s'il faut utiliser les guides des pages maîtres.
			get
			{
				return this.masterGuides;
			}

			set
			{
				if ( this.masterGuides != value )
				{
					this.InsertOpletType();
					this.masterGuides = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.None);
				}
			}
		}

		public bool MasterAutoStop
		{
			//	Indique si l'application s'arrête à la prochaine page modèle.
			get
			{
				return this.masterAutoStop;
			}

			set
			{
				if ( this.masterAutoStop != value )
				{
					this.InsertOpletType();
					this.masterAutoStop = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public bool MasterSpecific
		{
			//	Indique si la page modèle utilise elle-même une page modèle.
			get
			{
				return this.masterSpecific;
			}

			set
			{
				if ( this.masterSpecific != value )
				{
					this.InsertOpletType();
					this.masterSpecific = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public int MasterFirstFrontLayer
		{
			//	Pour une page maître, retourne le rang du premier calque qui sera
			//	à l'avant.
			//	- S'il n'y a qu'un calque, il vient derrière.
			//	- S'il y a 2 calques, le premier vient derrière et le dernier devant.
			//	- S'il y a 3 calques, 1-2 derrière et 3 devant
			//	- S'il y a 4 calques, 1-2 derrière et 3-4 devant
			get
			{
				int totalLayer = this.objects.Count;
				return (totalLayer+1)/2;
			}
		}

		public UndoableList Guides
		{
			//	Liste de repères pour cette page.
			get
			{
				return this.guides;
			}

			set
			{
				this.guides = value;
			}
		}


		public Size PageSize
		{
			//	Taille de la page. Si Empty, c'est la taille globale qui est utilisée.
			get
			{
				return this.pageSize;
			}

			set
			{
				if ( this.pageSize != value )
				{
					this.InsertOpletType();
					this.pageSize = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea();
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public Point HotSpot
		{
			//	Position du point chaud de l'icône, uniquement pour Pictogram.
			get
			{
				return this.hotSpot;
			}

			set
			{
				if ( this.hotSpot != value )
				{
					this.InsertOpletType();
					this.hotSpot = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public Point GlyphOrigin
		{
			//	Origine du glyphe lorsqu'il est dans un texte, uniquement pour Pictogram.
			get
			{
				return this.glyphOrigin;
			}

			set
			{
				if ( this.glyphOrigin != value )
				{
					this.InsertOpletType();
					this.glyphOrigin = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public Size GlyphSize
		{
			//	Taille du glyphe lorsqu'il est dans un texte, uniquement pour Pictogram.
			get
			{
				return this.glyphSize;
			}

			set
			{
				if ( this.glyphSize != value )
				{
					this.InsertOpletType();
					this.glyphSize = value;
					this.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public string Language
		{
			//	Langue de la page, uniquement pour Pictogram.
			get
			{
				return this.language;
			}

			set
			{
				if ( this.language != value )
				{
					this.InsertOpletType();
					this.language = value;
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}

		public string PageStyle
		{
			//	Style de la page, uniquement pour Pictogram.
			get
			{
				return this.pageStyle;
			}

			set
			{
				if ( this.pageStyle != value )
				{
					this.InsertOpletType();
					this.pageStyle = value;
					this.document.SetDirtySerialize(CacheBitmapChanging.All);
				}
			}
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			base.CloneObject(src);
			Page page = src as Page;

			this.rank            = page.rank;
			this.shortName       = page.shortName;
			this.masterType      = page.masterType;
			this.masterUse       = page.masterUse;
			this.masterPageToUse = page.masterPageToUse;
			this.masterAutoStop  = page.masterAutoStop;
			this.masterSpecific  = page.masterSpecific;
			this.masterGuides    = page.masterGuides;
			this.pageSize        = page.pageSize;
			this.hotSpot         = page.hotSpot;
			this.glyphOrigin     = page.glyphOrigin;
			this.glyphSize       = page.glyphSize;
			this.language        = page.language;
			this.pageStyle       = page.pageStyle;

			this.guides.Clear();
			foreach ( Settings.Guide guide in page.guides )
			{
				this.guides.Add(guide);
			}
		}


		#region Menu
		public static VMenu CreateMenu(UndoableList pages, int currentPage, string cmd, Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu pour choisir une page exclusivement.
			int total = pages.Count;
			bool slave = true;
			VMenu menu = new VMenu();
			for ( int i=0 ; i<total ; i++ )
			{
				Objects.Page page = pages[i] as Objects.Page;

				if ( i > 0 && slave != (page.MasterType == MasterType.Slave) )
				{
					menu.Items.Add(new MenuSeparator());
				}

				string name = string.Format("{0}: {1}", page.ShortName, page.LongName);

				string icon = Misc.Icon("RadioNo");
				if ( i == currentPage )
				{
					icon = Misc.Icon("RadioYes");
					name = Misc.Bold(name);
				}

				if (!string.IsNullOrEmpty(cmd))
				{
					Misc.CreateStructuredCommandWithName(cmd);
				}

				MenuItem item = new MenuItem(cmd, icon, name, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));

				if ( message != null )
				{
					item.Pressed += message;
				}

				menu.Items.Add(item);

				slave = (page.MasterType == MasterType.Slave);
			}
			menu.AdjustSize();
			return menu;
		}

		public static VMenu CreateMenu(UndoableList pages, List<int> currentsPage, string cmd, Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu pour choisir une page parmi plusieurs.
			int total = pages.Count;
			bool slave = true;
			VMenu menu = new VMenu();
			for (int i=0; i<total; i++)
			{
				Objects.Page page = pages[i] as Objects.Page;

				if (i > 0 && slave != (page.MasterType == MasterType.Slave))
				{
					menu.Items.Add(new MenuSeparator());
				}

				string name = string.Format("{0}: {1}", page.ShortName, page.LongName);

				string icon = Misc.Icon("ActiveNo");
				if (currentsPage.Contains(i))
				{
					icon = Misc.Icon("ActiveYes");
					name = Misc.Bold(name);
				}

				if (!string.IsNullOrEmpty(cmd))
				{
					Misc.CreateStructuredCommandWithName(cmd);
				}

				MenuItem item = new MenuItem(cmd, icon, name, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));

				if (message != null)
				{
					item.Pressed += message;
				}

				menu.Items.Add(item);

				slave = (page.MasterType == MasterType.Slave);
			}
			menu.AdjustSize();
			return menu;
		}
		#endregion


		#region CacheBitmap
		protected override void CacheBitmapCreate()
		{
			//	Crée le bitmap caché.
			//?System.Diagnostics.Debug.WriteLine(string.Format("CacheBitmapCreate page #{0}", this.PageNumber));
			if (this.cacheBitmapSize.IsEmpty)
			{
				this.cacheBitmap = null;
			}
			else
			{
				Size size = this.cacheBitmapSize;
				size -= new Size(2, 2);  // laisse un cadre d'un pixel
				this.cacheBitmap = this.document.Printer.CreateMiniatureBitmap(size, false, this.PageNumber, -1);
			}
		}
		#endregion

	
		#region OpletType
		protected void InsertOpletType()
		{
			//	Ajoute un oplet pour mémoriser les types de la page.
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletType oplet = new OpletType(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise le nom de l'objet.
		protected class OpletType : AbstractOplet
		{
			public OpletType(Page host)
			{
				this.host = host;
				this.masterType = host.masterType;
				this.masterUse = host.masterUse;
				this.masterPageToUse = host.masterPageToUse;
				this.masterAutoStop = host.masterAutoStop;
				this.masterSpecific = host.masterSpecific;
				this.masterGuides = host.masterGuides;
				this.pageSize = host.pageSize;
				this.hotSpot = host.hotSpot;
				this.glyphOrigin = host.glyphOrigin;
				this.glyphSize = host.glyphSize;
				this.language = host.language;
				this.pageStyle = host.pageStyle;
			}

			protected void Swap()
			{
				MasterType type = host.masterType;
				host.masterType = this.masterType;  // host.masterType <-> this.masterType
				this.masterType = type;

				MasterUse use = host.masterUse;
				host.masterUse = this.masterUse;  // host.masterUse <-> this.masterUse
				this.masterUse = use;

				Page page = host.masterPageToUse;
				host.masterPageToUse = this.masterPageToUse;  // host.masterPageToUse <-> this.masterPageToUse
				this.masterPageToUse = page;

				this.host.document.Modifier.UpdatePageShortNames();
				this.host.document.Modifier.ActiveViewer.DrawingContext.UpdateAfterPageChanged();
				this.host.document.Notifier.NotifyPagesChanged();
				this.host.document.Notifier.NotifyArea(this.host.document.Modifier.ActiveViewer);

				Misc.Swap(ref host.masterAutoStop, ref this.masterAutoStop);
				Misc.Swap(ref host.masterSpecific, ref this.masterSpecific);
				Misc.Swap(ref host.masterGuides, ref this.masterGuides);

				Misc.Swap(ref host.pageSize, ref this.pageSize);
				Misc.Swap(ref host.hotSpot, ref this.hotSpot);
				Misc.Swap(ref host.glyphOrigin, ref this.glyphOrigin);
				Misc.Swap(ref host.glyphSize, ref this.glyphSize);

				string l = host.language;
				host.language = this.language;
				this.language = l;

				string s = host.pageStyle;
				host.pageStyle = this.pageStyle;
				this.pageStyle = s;
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Page					host;
			protected MasterType			masterType;
			protected MasterUse				masterUse;
			protected Page					masterPageToUse;
			protected bool					masterAutoStop;
			protected bool					masterSpecific;
			protected bool					masterGuides;
			protected Size					pageSize;
			protected Point					hotSpot;
			protected Point					glyphOrigin;
			protected Size					glyphSize;
			protected string				language;
			protected string				pageStyle;
		}
		#endregion

		
		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);

			if ( this.document.Type == DocumentType.Pictogram )
			{
				info.AddValue("PageSize", this.pageSize);
				info.AddValue("HotSpot", this.hotSpot);
				info.AddValue("GlyphOrigin", this.glyphOrigin);
				info.AddValue("GlyphSize", this.glyphSize);
				info.AddValue("Language", this.language);
				info.AddValue("PageStyle", this.pageStyle);
			}
			else
			{
				info.AddValue("MasterType", this.masterType);
				info.AddValue("MasterUse", this.masterUse);
				info.AddValue("MasterPageToUse", this.masterPageToUse);
				info.AddValue("MasterAutoStop", this.masterAutoStop);
				info.AddValue("MasterSpecific", this.masterSpecific);
				info.AddValue("MasterGuides", this.masterGuides);
				info.AddValue("GuidesList", this.guides);
				info.AddValue("PageSize", this.pageSize);
			}
		}

		protected Page(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
			bool master = false;
			this.masterAutoStop = false;
			this.masterSpecific = false;
			this.masterGuides = true;

			if ( this.document.Type == DocumentType.Pictogram )
			{
				if ( this.document.IsRevisionGreaterOrEqual(1,5,1) )
				{
					this.pageSize = (Size) info.GetValue("PageSize", typeof(Size));
					this.hotSpot = (Point) info.GetValue("HotSpot", typeof(Point));
					this.glyphOrigin = (Point) info.GetValue("GlyphOrigin", typeof(Point));
					this.glyphSize = (Size) info.GetValue("GlyphSize", typeof(Size));
					this.language = info.GetString("Language");
				}
				if ( this.document.IsRevisionGreaterOrEqual(1,5,2) )
				{
					this.pageStyle = info.GetString("PageStyle");
				}
			}
			else
			{
				if ( this.document.IsRevisionGreaterOrEqual(1,0,9) )
				{
					this.masterType = (MasterType) info.GetValue("MasterType", typeof(MasterType));
					this.masterUse = (MasterUse) info.GetValue("MasterUse", typeof(MasterUse));
					this.masterPageToUse = (Page) info.GetValue("MasterPageToUse", typeof(Page));
					this.guides = (UndoableList) info.GetValue("GuidesList", typeof(UndoableList));
					master = true;
				}

				if ( this.document.IsRevisionGreaterOrEqual(1,0,12) )
				{
					this.masterAutoStop = info.GetBoolean("MasterAutoStop");
					this.masterSpecific = info.GetBoolean("MasterSpecific");
				}

				if ( this.document.IsRevisionGreaterOrEqual(1,0,11) )
				{
					this.masterGuides = info.GetBoolean("MasterGuides");
				}

				if ( this.document.IsRevisionGreaterOrEqual(1,5,1) )
				{
					this.pageSize = (Size) info.GetValue("PageSize", typeof(Size));
				}
			}

			if ( !master )
			{
				this.guides = new UndoableList(this.document, UndoableListType.Guides);
				this.masterType = MasterType.Slave;
				this.masterUse = MasterUse.Default;
				this.masterPageToUse = null;
			}
		}
		#endregion

		
		protected int					currentLayer;
		protected int					rank;
		protected string				shortName;
		protected MasterType			masterType;
		protected MasterUse				masterUse;
		protected Page					masterPageToUse;
		protected bool					masterAutoStop;
		protected bool					masterSpecific;
		protected bool					masterGuides;
		protected UndoableList			guides;
		protected Size					pageSize;
		protected Point					hotSpot;
		protected Point					glyphOrigin;
		protected Size					glyphSize;
		protected string				language;
		protected string				pageStyle;
	}
}
