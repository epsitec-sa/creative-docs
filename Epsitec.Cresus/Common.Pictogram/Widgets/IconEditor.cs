using Epsitec.Common.Widgets;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe IconEditor représente l'éditeur d'icône complet.
	/// </summary>
	public class IconEditor : Epsitec.Common.Widgets.Widget
	{
		public IconEditor()
		{
			Epsitec.Common.Support.ImageProvider.RegisterAssembly("Epsitec.Common.Pictogram", this.GetType().Assembly);
			
			this.CreateLayout();
			this.UpdatePanels();
		}

		public IconEditor(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.allWidgets = false;

				this.toolBar = null;
				this.root = null;
				this.pane = null;
				this.leftPane = null;
				this.middlePane = null;
				this.rightPane = null;
				this.drawer = null;
				this.lister = null;
				this.frame1 = null;
				this.frame2 = null;
			}
			
			base.Dispose(disposing);
		}


		protected void CreateLayout()
		{
			this.toolBar = new ToolBar();
			int i = 0;

			this.toolBar.Items.Add(new IconButton(@"file:images/new1.icon"));
			this.toolBar.Items[i].Name = "new";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/open1.icon"));
			this.toolBar.Items[i].Name = "open";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/save1.icon"));
			this.toolBar.Items[i].Name = "save";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;

			this.toolBar.Items.Add(new IconSeparator());
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/delete1.icon"));
			this.toolBar.Items[i].Name = "delete";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/duplicate1.icon"));
			this.toolBar.Items[i].Name = "duplicate";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconSeparator());
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/undo1.icon"));
			this.toolBar.Items[i].Name = "undo";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/redo1.icon"));
			this.toolBar.Items[i].Name = "redo";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconSeparator());
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/orderup1.icon"));
			this.toolBar.Items[i].Name = "orderup";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/orderdown1.icon"));
			this.toolBar.Items[i].Name = "orderdown";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconSeparator());
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/grid1.icon"));
			this.toolBar.Items[i].Name = "grid";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/mode1.icon"));
			this.toolBar.Items[i].Name = "mode";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			i ++;
			
			StaticText label1 = new StaticText();
			label1.Width = 60;
			label1.Text = "<b>Outils:  </b>";
			label1.Alignment = Drawing.ContentAlignment.MiddleRight;
			this.toolBar.Items.Add(label1);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/select1.icon"));
			this.toolBar.Items[i].Name = "select";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconSeparator());
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/line1.icon"));
			this.toolBar.Items[i].Name = "ObjectLine";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/arrow1.icon"));
			this.toolBar.Items[i].Name = "ObjectArrow";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/rectangle1.icon"));
			this.toolBar.Items[i].Name = "ObjectRectangle";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/circle1.icon"));
			this.toolBar.Items[i].Name = "ObjectCircle";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/ellipse1.icon"));
			this.toolBar.Items[i].Name = "ObjectEllipse";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/regular1.icon"));
			this.toolBar.Items[i].Name = "ObjectRegular";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/poly1.icon"));
			this.toolBar.Items[i].Name = "ObjectPoly";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/bezier1.icon"));
			this.toolBar.Items[i].Name = "ObjectBezier";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Items.Add(new IconButton(@"file:images/text1.icon"));
			this.toolBar.Items[i].Name = "ObjectText";
			this.toolBar.Items[i].Clicked += new MessageEventHandler(this.ToolBarClicked);
			i ++;
			
			this.toolBar.Parent = this;

			this.root = new Widget();
			this.root.Parent = this;
			
			this.pane = new PaneBook();
			this.pane.PaneBookStyle = PaneBookStyle.LeftRight;
			this.pane.PaneBehaviour = PaneBookBehaviour.FollowMe;
			this.pane.SizeChanged += new EventHandler(this.pane_SizeChanged);
			this.pane.Parent = root;

			this.leftPane = new PanePage();
			this.leftPane.PaneAbsoluteSize = 200;
			this.leftPane.PaneElasticity = 0;
			this.leftPane.PaneMinSize = 200;
			this.leftPane.PaneMaxSize = 200;
			this.pane.Items.Add(this.leftPane);

			this.middlePane = new PanePage();
			this.middlePane.PaneRelativeSize = 10;
			this.middlePane.PaneElasticity = 1;
			this.middlePane.PaneMinSize = 100;
			this.pane.Items.Add(this.middlePane);

			this.rightPane = new PanePage();
			this.rightPane.PaneAbsoluteSize = 40;
			this.rightPane.PaneElasticity = 0;
			this.rightPane.PaneMinSize = 40;
			this.rightPane.PaneMaxSize = 200;
			this.pane.Items.Add(this.rightPane);

			this.drawer = new Drawer();
			this.drawer.IsEditable = true;
			this.drawer.SelectedTool = "select";
			this.drawer.PanelChanged += new EventHandler(this.DrawerPanelChanged);
			this.drawer.ToolChanged += new EventHandler(this.DrawerToolChanged);
			this.drawer.AllChanged += new EventHandler(this.DrawerAllChanged);
			this.drawer.Parent = this.middlePane;

			this.lister = new Lister();
			this.lister.PanelChanged += new EventHandler(this.DrawerAllChanged);
			this.lister.Objects = this.drawer.Objects;
			this.lister.Parent = this.middlePane;
			this.lister.Hide();

			this.frame1 = new SampleButton();
			this.frame1.ButtonStyle = ButtonStyle.ToolItem;
			this.frame1.ActiveState = WidgetState.ActiveYes;
			this.frame1.IconObjects.Objects = this.drawer.Objects;
			this.frame1.Parent = this.rightPane;

			this.frame2 = new SampleButton();
			this.frame2.ButtonStyle = ButtonStyle.ToolItem;
			this.frame2.ActiveState = WidgetState.ActiveYes;
			this.frame2.SetEnabled(false);
			this.frame2.IconObjects.Objects = this.drawer.Objects;
			this.frame2.Parent = this.rightPane;

			this.drawer.AddClone(this.frame1);
			this.drawer.AddClone(this.frame2);
			this.drawer.AddClone(this.lister);

			this.allWidgets = true;
			this.ResizeLayout();
			this.UpdateToolBar();
		}

		private void DrawerPanelChanged(object sender)
		{
			this.UpdatePanels();
		}

		private void DrawerToolChanged(object sender)
		{
			this.UpdateToolBar();
		}

		private void DrawerAllChanged(object sender)
		{
			this.UpdateToolBar();
			this.UpdatePanels();
		}

		// Met à jour les panneaux de gauche en fonction des propriétés de l'objet.
		protected void UpdatePanels()
		{
			this.leftPane.Children.Clear();

			Drawing.Rectangle rect = new Drawing.Rectangle();

			System.Collections.ArrayList list = this.drawer.PropertiesList();
			double posy = this.leftPane.Height;
			Widget originColorLastPanel = null;
			foreach ( AbstractProperty property in list )
			{
				AbstractPanel panel = property.CreatePanel();

				AbstractProperty p = this.drawer.GetProperty(property.Type);
				panel.SetProperty(p);
				panel.Multi = p.Multi;

				rect.Left   = 0;
				rect.Right  = this.leftPane.Width;
				rect.Bottom = posy-panel.DefaultHeight;
				rect.Top    = posy;
				panel.Bounds = rect;
				panel.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Top;
				panel.Changed += new EventHandler(this.PanelChanged);
				panel.ExtendedChanged += new EventHandler(this.ExtendedChanged);
				panel.OriginColorChanged += new EventHandler(this.OriginColorChanged);
				panel.Parent = this.leftPane;

				if ( panel.PropertyType == this.originColorType )
				{
					originColorLastPanel = panel;
				}

				posy -= rect.Height;
			}
			this.leftHeightUsed = this.leftPane.Height-posy;

			if ( this.colorSelector == null )
			{
				this.colorSelector = new ColorSelector();
			}
			rect.Left   = 0;
			rect.Right  = this.leftPane.Width;
			rect.Bottom = 0;
			rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.leftPane.Height-this.leftHeightUsed);
			this.colorSelector.Bounds = rect;
			this.colorSelector.Anchor = AnchorStyles.LeftAndRight|AnchorStyles.Bottom;
			this.colorSelector.Changed += new EventHandler(this.ColorSelectorChanged);
			this.colorSelector.Parent = this.leftPane;

			this.OriginColorChanged(originColorLastPanel, true);
		}

		// Le contenu d'un panneau a été changé.
		private void PanelChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty property = panel.GetProperty();
			drawer.SetProperty(property);
			panel.Multi = false;
		}

		// La hauteur d'un panneau a été changée.
		private void ExtendedChanged(object sender)
		{
			AbstractPanel panel = sender as AbstractPanel;
			AbstractProperty property = panel.GetProperty();
			drawer.SetPropertyExtended(property);
			this.UpdatePanels();
		}

		// Le widget qui détermine la couleur d'origine a changé.
		private void OriginColorChanged(object sender)
		{
			this.OriginColorChanged(sender, false);
		}

		private void OriginColorChanged(object sender, bool lastOrigin)
		{
			this.originColorPanel = null;

			foreach ( Widget widget in this.leftPane.Children )
			{
				AbstractPanel panel = widget as AbstractPanel;
				if ( panel == null )  continue;
				Widget wSender = sender as Widget;
				if ( panel == wSender )
				{
					this.originColorPanel = panel;
					panel.OriginColorSelect( lastOrigin ? this.originColorRank : -1 );
				}
				else
				{
					panel.OriginColorDeselect();
				}
			}

			if ( this.originColorPanel == null )
			{
				this.colorSelector.SetEnabled(false);
			}
			else
			{
				this.colorSelector.SetEnabled(true);
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.originColorType = this.originColorPanel.PropertyType;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
		}

		// Couleur d'origine changée dans la roue.
		private void ColorSelectorChanged(object sender)
		{
			if ( this.originColorPanel == null )  return;
			this.originColorPanel.OriginColorChange(this.colorSelector.Color);

			AbstractProperty property = this.originColorPanel.GetProperty();
			drawer.SetProperty(property);
			this.originColorPanel.Multi = false;
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.ResizeLayout();
		}

		protected void ResizeLayout()
		{
			if ( !this.allWidgets )  return;

			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);

			this.toolBar.Location = new Drawing.Point(0, rect.Height-this.toolBar.DefaultHeight);
			this.toolBar.Size = new Drawing.Size(rect.Width, this.toolBar.DefaultHeight);

			this.root.Location = new Drawing.Point(0, 0);
			this.root.Size = new Drawing.Size(rect.Width, rect.Height-this.toolBar.DefaultHeight);
			//this.root.SetClientAngle(0);
			//this.root.SetClientZoom(1.0);

			this.pane.Location = new Drawing.Point(0, 0);
			this.pane.Size = new Drawing.Size(rect.Width, rect.Height-this.toolBar.DefaultHeight);
			this.rightPane.PaneMinSize = 20+this.drawer.IconObjects.Size.Width;

			if ( this.colorSelector != null )
			{
				rect.Left   = 0;
				rect.Right  = this.leftPane.Width;
				rect.Bottom = 0;
				rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.leftPane.Height-this.leftHeightUsed);
				this.colorSelector.Bounds = rect;
			}

			Drawing.Size iconSize = this.drawer.IconObjects.Size;
			double dimx = this.middlePane.Width-20;
			double dimy = dimx*iconSize.Height/iconSize.Width;
			if ( dimy > this.middlePane.Height-20 )
			{
				dimy = this.middlePane.Height-20;
				dimx = dimy*iconSize.Width/iconSize.Height;
			}
			this.drawer.Location = new Drawing.Point(10, this.middlePane.Height-10-dimy);
			this.drawer.Size = new Drawing.Size(dimx, dimy);

			this.lister.Location = new Drawing.Point(10, 10);
			this.lister.Size = new Drawing.Size(this.middlePane.Width-20, this.middlePane.Height-20);

			dimx = this.rightPane.Width-20;
			dimy = dimx*iconSize.Height/iconSize.Width;
			rect.Left   = 10;
			rect.Bottom = this.rightPane.Height-10-dimy-1;
			rect.Width  = dimx;
			rect.Height = dimy;
			rect.Inflate(1, 1);
			this.frame1.Bounds = rect;
			rect.Inflate(-1, -1);

			rect.Offset(0, -dimy-10);
			rect.Inflate(1, 1);
			this.frame2.Bounds = rect;
			rect.Inflate(-1, -1);
		}

		private void pane_SizeChanged(object sender)
		{
			PaneBook pane = (PaneBook)sender;

			if ( pane == this.pane )
			{
			}
			this.ResizeLayout();
		}


		// Bouton de la toolbar cliqué.
		private void ToolBarClickedAction(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			string cmd = button.Name;

#if true
			if ( cmd == "open" )
			{
				System.Console.Out.WriteLine(System.IO.Directory.GetCurrentDirectory());
				FileOpen dialog = new FileOpen();
			
				dialog.Title = "Ouvrir une icone";
				dialog.FileName = this.filename;
				dialog.Filters.Add ("icon", "Icônes", "*.icon");
				//dialog.InitialDirectory = "..\\..\\images\\";			
				dialog.Show();

				this.filename = dialog.FileName;
				System.Console.Out.WriteLine(System.IO.Directory.GetCurrentDirectory());
			}

			if ( cmd == "save" )
			{
				FileSave dialog = new FileSave();
			
				dialog.Title = "Enregisrter une icone";
				dialog.FileName = this.filename;
				dialog.Filters.Add ("icon", "Icônes", "*.icon");
				//dialog.InitialDirectory = "..\\..\\images\\";			
				dialog.Show();

				this.filename = dialog.FileName;
			}
#endif

			switch ( cmd )
			{
				case "open":       this.drawer.ActionOpen(this.filename);  break;
				case "save":       this.drawer.ActionSave(this.filename);  break;
				case "new":        this.drawer.ActionNew();                break;
				case "delete":     this.drawer.ActionDelete();             break;
				case "duplicate":  this.drawer.ActionDuplicate();          break;
				case "undo":       this.drawer.ActionUndo();               break;
				case "redo":       this.drawer.ActionRedo();               break;
				case "orderup":    this.drawer.ActionOrder(1);             break;
				case "orderdown":  this.drawer.ActionOrder(-1);            break;
				case "grid":       this.drawer.ActionGrid();               break;
			}

			if ( cmd == "new" )
			{
				this.filename = "";
			}

			if ( cmd == "open" )
			{
				this.ResizeLayout();
				this.Invalidate();
			}

			if ( cmd == "mode" )
			{
				if ( this.drawer.IsActive )
				{
					this.drawer.IsActive = false;
					this.drawer.Hide();
					this.lister.Show();

					this.drawer.SelectedTool = "select";
					this.UpdatePanels();
				}
				else
				{
					this.drawer.IsActive = true;
					this.drawer.Show();
					this.lister.Hide();
				}
				this.UpdateToolBar();
			}

			if ( cmd == "grid" )
			{
				this.UpdateToolBar();
			}

			if ( this.lister.IsVisible )
			{
				this.lister.UpdateContent();
			}
		}

		// Bouton de la toolbar cliqué.
		private void ToolBarClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			if ( this.drawer.IsVisible )
			{
				this.drawer.SelectedTool = button.Name;
			}
			else
			{
				this.drawer.SelectedTool = "select";
			}
			this.UpdateToolBar();
			this.UpdatePanels();
		}

		// Met à jour l'outil sélectionné dans la toolbar.
		protected void UpdateToolBar()
		{
			foreach ( Widget widget in this.toolBar.Items )
			{
				IconButton button = widget as IconButton;
				if ( button == null )  continue;

				string cmd = button.Name;
				button.SetEnabled(this.drawer.IsCommandEnable(cmd));
				//?button.SetVisible(this.drawer.IsCommandEnable(cmd));
				button.ActiveState = this.drawer.IsCommandActive(cmd) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			}
		}


		protected bool							allWidgets = false;
		protected ToolBar						toolBar;
		protected Widget						root;
		protected PaneBook						pane;
		protected PanePage						leftPane;
		protected PanePage						middlePane;
		protected PanePage						rightPane;
		protected ColorCircle					circle;
		protected Drawer						drawer;
		protected Lister						lister;
		protected SampleButton					frame1;
		protected SampleButton					frame2;
		protected ColorSelector					colorSelector;
		protected AbstractPanel					originColorPanel = null;
		protected PropertyType					originColorType = PropertyType.None;
		protected int							originColorRank = -1;
		protected double						leftHeightUsed = 0;
		protected string						filename = "";
	}
}
