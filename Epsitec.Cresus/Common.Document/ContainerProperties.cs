using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ContainerProperties contient tous les panneaux des propriétés.
	/// </summary>
	[SuppressBundleSupport]
	public class ContainerProperties : AbstractContainer
	{
		public ContainerProperties(Document document) : base(document)
		{
			this.detailButton = new CheckButton();
			this.detailButton.Text = "Détails";
			this.detailButton.Dock = DockStyle.Top;
			this.detailButton.DockMargins = new Margins(0, 0, 0, 10);
			this.detailButton.TabIndex = 100;
			this.detailButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.detailButton.Clicked +=new MessageEventHandler(this.HandleDetailButtonClicked);

			this.scrollable = new Scrollable();
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Panel.IsAutoFitting = true;
			this.scrollable.IsForegroundFrame = true;
			this.scrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 0);
			this.scrollable.Parent = this;

			this.colorSelector = new ColorSelector();
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.DockMargins = new Margins(0, 0, 10, 0);
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.colorSelector.Parent = this;

			int total = 0;
			foreach ( int value in System.Enum.GetValues(typeof(PropertyType)) )
			{
				PropertyType type = (PropertyType)value;
				total ++;
			}
			this.isPropertiesExtended = new bool[total];
			for ( int i=0 ; i<total ; i++ )
			{
				this.isPropertiesExtended[i] = false;
			}
		}
		

		// Met en évidence l'objet survolé par la souris.
		public override void Hilite(AbstractObject hiliteObject)
		{
			if ( !this.IsVisible )  return;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				if ( widget is AbstractPanel )
				{
					AbstractPanel panel = widget as AbstractPanel;
					if ( this.document.Modifier.PropertiesDetailMany )
					{
						panel.IsObjectHilite = (hiliteObject != null);
						panel.IsHilite = AbstractContainer.IsObjectUseByProperty(panel.Property, hiliteObject);
					}
					else
					{
						panel.IsObjectHilite = false;
						panel.IsHilite = false;
					}
				}
			}
		}

		
		// Effectue la mise à jour du contenu.
		protected override void DoUpdateContent()
		{
			Viewer viewer = this.document.Modifier.ActiveViewer;
			DrawingContext context = viewer.DrawingContext;

			this.detailButton.Parent = null;

			// Supprime tous les panneaux ou boutons.
			this.scrollable.Panel.SuspendLayout();

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				if ( widget is AbstractPanel )
				{
					AbstractPanel panel = widget as AbstractPanel;
					panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				}

				widget.Parent = null; // retire de son parent
			}

			Widget originColorLastPanel = null;
			if ( this.document.Modifier.ActiveViewer.IsCreating )
			{
				// Crée tous les boutons pour l'objet en cours de création.
				AbstractObject layer = context.RootObject();
				int rank = viewer.CreateRank();
				AbstractObject creatingObject = layer.Objects[rank] as AbstractObject;
				double topMargin = 50;
				for ( int i=0 ; i<100 ; i++ )
				{
					string cmd, name, text;
					if ( !creatingObject.CreateAction(i, out cmd, out name, out text) )  break;
					Button button = new Button();
					button.Command = cmd;
					button.Name = name;
					button.Text = text;
					button.Dock = DockStyle.Top;
					button.DockMargins = new Margins(10, 10, topMargin, 0);
					button.Parent = this.scrollable.Panel;

					if ( topMargin == 50 )  topMargin = 10;
				}
			}
			else
			{
				// Crée tous les panneaux des propriétés.
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				this.document.Modifier.PropertiesList(list);

				if ( list.Count > 0 && this.document.Modifier.TotalSelected > 1 )
				{
					this.detailButton.ActiveState = this.document.Modifier.PropertiesDetail ? WidgetState.ActiveYes : WidgetState.ActiveNo;
					this.detailButton.Parent = this;
				}

				int index = 1;
				double lastBack = -1;
				foreach ( AbstractProperty property in list )
				{
					double topMargin = 0;
					if ( lastBack != -1 && lastBack != AbstractProperty.BackgroundIntensity(property.Type) )
					{
						topMargin = 5;
					}
					lastBack = AbstractProperty.BackgroundIntensity(property.Type);

					AbstractPanel panel = property.CreatePanel(this.document);
					if ( panel == null )  continue;
					panel.Property = property;
					panel.Container = this;

					panel.IsExtendedSize = this.IsPropertiesExtended(property.Type);
					panel.IsLayoutDirect = (property.Type == PropertyType.Name);

					panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);

					panel.TabIndex = index++;
					panel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
					panel.Dock = DockStyle.Top;
					panel.DockMargins = new Margins(0, 1, topMargin, -1);
					panel.Parent = this.scrollable.Panel;

					if ( panel.Property.Type == this.originColorType )
					{
						originColorLastPanel = panel;
					}
				}
			}

			this.scrollable.Panel.ResumeLayout();
			this.HandleOriginColorChanged(originColorLastPanel, true);
		}

		// Effectue la mise à jour des propriétés.
		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			Widget originColorLastPanel = null;
			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				AbstractPanel panel = widget as AbstractPanel;
				if ( panel != null )
				{
					if ( propertyList.Contains(panel.Property) )
					{
						panel.UpdateValues();
					}

					if ( panel.Property.Type == this.originColorType )
					{
						originColorLastPanel = panel;
					}
				}
			}

			this.HandleOriginColorChanged(originColorLastPanel, true);
		}


		// Le bouton des détails a été cliqué.
		private void HandleDetailButtonClicked(object sender, MessageEventArgs e)
		{
			this.document.Modifier.PropertiesDetail = !this.document.Modifier.PropertiesDetail;
		}

		// Le widget qui détermine la couleur d'origine a changé.
		private void HandleOriginColorChanged(object sender)
		{
			this.HandleOriginColorChanged(sender, false);
		}

		private void HandleOriginColorChanged(object sender, bool lastOrigin)
		{
			this.originColorPanel = null;
			Widget wSender = sender as Widget;
			Color backColor = Color.Empty;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				AbstractPanel panel = widget as AbstractPanel;
				if ( panel == null )  continue;

				if ( panel == wSender )
				{
					this.originColorPanel = panel;
					panel.OriginColorSelect( lastOrigin ? this.originColorRank : -1 );
					if ( panel.Property.IsStyle )
					{
						backColor = DrawingContext.ColorStyleBack;
					}
				}
				else
				{
					panel.OriginColorDeselect();
				}
			}

			if ( this.originColorPanel == null )
			{
				//?this.colorSelector.SetEnabled(false);
				this.colorSelector.SetVisible(false);
				this.colorSelector.BackColor = Color.Empty;
			}
			else
			{
				//?this.colorSelector.SetEnabled(true);
				this.colorSelector.SetVisible(true);
				this.colorSelector.BackColor = backColor;
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorType = this.originColorPanel.Property.Type;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
		}

		// Couleur changée dans la roue.
		private void HandleColorSelectorChanged(object sender)
		{
			if ( this.ignoreColorChanged || this.originColorPanel == null )  return;
			this.originColorPanel.OriginColorChange(this.colorSelector.Color);
		}


		// Indique si le panneau d'une propriété doit être étendu.
		public bool IsPropertiesExtended(PropertyType type)
		{
			int i=0;
			foreach ( int value in System.Enum.GetValues(typeof(PropertyType)) )
			{
				if ( type == (PropertyType)value )
				{
					return this.isPropertiesExtended[i];
				}
				i++;
			}
			return false;
		}

		// Indique si le panneau d'une propriété doit être étendu.
		public void IsPropertiesExtended(PropertyType type, bool extended)
		{
			int i=0;
			foreach ( int value in System.Enum.GetValues(typeof(PropertyType)) )
			{
				if ( type == (PropertyType)value )
				{
					this.isPropertiesExtended[i] = extended;
					break;
				}
				i++;
			}
		}


		protected CheckButton			detailButton;
		protected Scrollable			scrollable;
		protected ColorSelector			colorSelector;
		protected AbstractPanel			originColorPanel = null;
		protected PropertyType			originColorType = PropertyType.None;
		protected int					originColorRank = -1;
		protected bool					ignoreColorChanged = false;
		protected bool[]				isPropertiesExtended;
	}
}
