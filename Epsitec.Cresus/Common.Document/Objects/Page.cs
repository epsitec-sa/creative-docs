using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
	public enum MasterType
	{
		Slave    = 0,	// page normale
		All      = 1,	// page modèle appliquée à toutes les pages
		Even     = 2,	// page modèle appliquée aux pages paires
		Odd      = 3,	// page modèle appliquée aux page impaires
		None     = 4,	// page modèle appliquée à la demande
	}

	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plantée lors de la désérialisation.
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
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Page(document, model);
		}

		// Rang du calque courant.
		public int CurrentLayer
		{
			get
			{
				return this.currentLayer;
			}
			
			set
			{
				this.currentLayer = value;
			}
		}

		// Rang de la page (0..n). Les pages normales et les pages maîtres
		// ont chacune un rang indépendant commençant à zéro.
		public int Rank
		{
			get
			{
				return this.rank;
			}

			set
			{
				this.rank = value;
			}
		}

		// Nom court automatique de la page ("Pn" ou "Mn").
		public string ShortName
		{
			get
			{
				return this.shortName;
			}

			set
			{
				this.shortName = value;
			}
		}

		// Type de la page maître. Le type Slave indique qu'il s'agit d'une
		// page normale.
		public MasterType MasterType
		{
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
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Pour une page normale, indique quelle page maître il faut utiliser.
		public MasterUse MasterUse
		{
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
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Si MasterUse = Specific, donne directement la page maître à utiliser.
		public Page MasterPageToUse
		{
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
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Indique s'il faut utiliser les guides des pages maîtres.
		public bool MasterGuides
		{
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
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Indique si l'application s'arrête à la prochaine page modèle.
		public bool MasterAutoStop
		{
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
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Indique si la page modèle utilise elle-même une page modèle.
		public bool MasterSpecific
		{
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
					this.document.IsDirtySerialize = true;
				}
			}
		}

		// Pour une page maître, retourne le rang du premier calque qui sera
		// à l'avant.
		// - S'il n'y a qu'un calque, il vient derrière.
		// - S'il y a 2 calques, le premier vient derrière et le dernier devant.
		// - S'il y a 3 calques, 1-2 derrière et 3 devant
		// - S'il y a 4 calques, 1-2 derrière et 3-4 devant
		public int MasterFirstFrontLayer
		{
			get
			{
				int totalLayer = this.objects.Count;
				return (totalLayer+1)/2;
			}
		}

		// Liste de repères pour cette page.
		public UndoableList Guides
		{
			get
			{
				return this.guides;
			}

			set
			{
				this.guides = value;
			}
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
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

			this.guides.Clear();
			foreach ( Settings.Guide guide in page.guides )
			{
				this.guides.Add(guide);
			}
		}


		#region Menu
		// Construit le menu pour choisir une page.
		public static VMenu CreateMenu(UndoableList pages, int currentPage, MessageEventHandler message)
		{
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

				string name = string.Format("{0}: {1}", page.ShortName, page.Name);

				string icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveNo.icon";
				if ( i == currentPage )
				{
					icon = "manifest:Epsitec.App.DocumentEditor.Images.ActiveYes.icon";
					name = Misc.Bold(name);
				}

				MenuItem item = new MenuItem("PageSelect(this.Name)", icon, name, "", i.ToString());

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
		#endregion

		
		#region OpletType
		// Ajoute un oplet pour mémoriser les types de la page.
		protected void InsertOpletType()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletType oplet = new OpletType(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		// Mémorise le nom de l'objet.
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
		}
		#endregion

		
		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			if ( this.document.Type != DocumentType.Pictogram )
			{
				info.AddValue("MasterType", this.masterType);
				info.AddValue("MasterUse", this.masterUse);
				info.AddValue("MasterPageToUse", this.masterPageToUse);
				info.AddValue("MasterAutoStop", this.masterAutoStop);
				info.AddValue("MasterSpecific", this.masterSpecific);
				info.AddValue("MasterGuides", this.masterGuides);
				info.AddValue("GuidesList", this.guides);
			}
		}

		// Constructeur qui désérialise l'objet.
		protected Page(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			bool master = false;
			this.masterAutoStop = false;
			this.masterSpecific = false;
			this.masterGuides = true;

			if ( this.document.Type != DocumentType.Pictogram )
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
	}
}
