using Epsitec.Common.Widgets;
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

			this.toolBar.Items.Add(new IconButton(@"file:images/new1.icon"));
			this.toolBar.Items[0].Name = "new";
			this.toolBar.Items[0].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			TextField filename = new TextField();
			filename.Width = 100;
			this.toolBar.Items.Add(filename);

			this.toolBar.Items.Add(new IconButton(@"file:images/open1.icon"));
			this.toolBar.Items[2].Name = "open";
			this.toolBar.Items[2].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/save1.icon"));
			this.toolBar.Items[3].Name = "save";
			this.toolBar.Items[3].Clicked += new MessageEventHandler(this.ToolBarClickedAction);

			this.toolBar.Items.Add(new IconSeparator());
			
			this.toolBar.Items.Add(new IconButton(@"file:images/delete1.icon"));
			this.toolBar.Items[5].Name = "delete";
			this.toolBar.Items[5].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/duplicate1.icon"));
			this.toolBar.Items[6].Name = "duplicate";
			this.toolBar.Items[6].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconSeparator());
			
			this.toolBar.Items.Add(new IconButton(@"file:images/undo1.icon"));
			this.toolBar.Items[8].Name = "undo";
			this.toolBar.Items[8].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/redo1.icon"));
			this.toolBar.Items[9].Name = "redo";
			this.toolBar.Items[9].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconSeparator());
			
			this.toolBar.Items.Add(new IconButton(@"file:images/orderup1.icon"));
			this.toolBar.Items[11].Name = "orderup";
			this.toolBar.Items[11].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/orderdown1.icon"));
			this.toolBar.Items[12].Name = "orderdown";
			this.toolBar.Items[12].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconSeparator());
			
			this.toolBar.Items.Add(new IconButton(@"file:images/grid1.icon"));
			this.toolBar.Items[14].Name = "grid";
			this.toolBar.Items[14].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/mode1.icon"));
			this.toolBar.Items[15].Name = "mode";
			this.toolBar.Items[15].Clicked += new MessageEventHandler(this.ToolBarClickedAction);
			
			StaticText label1 = new StaticText();
			label1.Width = 60;
			label1.Text = "<b>Outils:  </b>";
			label1.Alignment = Drawing.ContentAlignment.MiddleRight;
			this.toolBar.Items.Add(label1);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/select1.icon"));
			this.toolBar.Items[17].Name = "select";
			this.toolBar.Items[17].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconSeparator());
			
			this.toolBar.Items.Add(new IconButton(@"file:images/line1.icon"));
			this.toolBar.Items[19].Name = "ObjectLine";
			this.toolBar.Items[19].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/arrow1.icon"));
			this.toolBar.Items[20].Name = "ObjectArrow";
			this.toolBar.Items[20].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/rectangle1.icon"));
			this.toolBar.Items[21].Name = "ObjectRectangle";
			this.toolBar.Items[21].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/circle1.icon"));
			this.toolBar.Items[22].Name = "ObjectCircle";
			this.toolBar.Items[22].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/ellipse1.icon"));
			this.toolBar.Items[23].Name = "ObjectEllipse";
			this.toolBar.Items[23].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/regular1.icon"));
			this.toolBar.Items[24].Name = "ObjectRegular";
			this.toolBar.Items[24].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/poly1.icon"));
			this.toolBar.Items[25].Name = "ObjectPoly";
			this.toolBar.Items[25].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/bezier1.icon"));
			this.toolBar.Items[26].Name = "ObjectBezier";
			this.toolBar.Items[26].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
			this.toolBar.Items.Add(new IconButton(@"file:images/text1.icon"));
			this.toolBar.Items[27].Name = "ObjectText";
			this.toolBar.Items[27].Clicked += new MessageEventHandler(this.ToolBarClicked);
			
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

			if ( this.colorSelector != null )
			{
				rect.Left   = 0;
				rect.Right  = this.leftPane.Width;
				rect.Bottom = 0;
				rect.Top    = System.Math.Min(this.colorSelector.DefaultHeight, this.leftPane.Height-this.leftHeightUsed);
				this.colorSelector.Bounds = rect;
			}

			double dim = System.Math.Min(this.middlePane.Width, this.middlePane.Height)-20;
			this.drawer.Location = new Drawing.Point(10, this.middlePane.Height-10-dim);
			this.drawer.Size = new Drawing.Size(dim, dim);

			this.lister.Location = new Drawing.Point(10, 10);
			this.lister.Size = new Drawing.Size(this.middlePane.Width-20, this.middlePane.Height-20);

			dim = System.Math.Min(this.rightPane.Width, this.rightPane.Height)-20;
			rect.Left   = 10;
			rect.Bottom = this.rightPane.Height-10-dim-3;
			rect.Width  = dim;
			rect.Height = dim;
			rect.Inflate(1, 1);
			this.frame1.Bounds = rect;
			rect.Inflate(-1, -1);

			rect.Offset(0, -dim-10);
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
			TextField tf = this.toolBar.Items[1] as TextField;
			string filename = "..\\..\\images\\" + tf.Text + ".icon";

			IconButton button = sender as IconButton;
			string cmd = button.Name;
			switch ( cmd )
			{
				case "open":       this.drawer.ActionOpen(filename);  break;
				case "save":       this.drawer.ActionSave(filename);  break;
				case "new":        this.drawer.ActionNew();           break;
				case "delete":     this.drawer.ActionDelete();        break;
				case "duplicate":  this.drawer.ActionDuplicate();     break;
				case "undo":       this.drawer.ActionUndo();          break;
				case "redo":       this.drawer.ActionRedo();          break;
				case "orderup":    this.drawer.ActionOrder(1);        break;
				case "orderdown":  this.drawer.ActionOrder(-1);       break;
				case "grid":       this.drawer.ActionGrid();          break;
			}

			if ( cmd == "new" )
			{
				tf.Text = "";
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
	}
}
