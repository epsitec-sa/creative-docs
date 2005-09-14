using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Principal contient tous les panneaux des propriétés.
	/// </summary>
	[SuppressBundleSupport]
	public class Principal : Abstract
	{
		public Principal(Document document) : base(document)
		{
			this.CreateSelectorToolBar();
			this.CreateAggregateToolBar();
			this.CreateSelectorPanel();

			this.detailButton = new CheckButton();
			this.detailButton.Text = Res.Strings.Container.Principal.Button.Detail;
			this.detailButton.Dock = DockStyle.Top;
			this.detailButton.DockMargins = new Margins(0, 0, 5, 5);
			this.detailButton.TabIndex = 1;
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
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.DockMargins = new Margins(0, 0, 10, 0);
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.colorSelector.Parent = this;
			this.colorSelector.SetVisible(false);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.aggregateCombo.TextChanged -= new EventHandler(this.HandleAggregateComboChanged);
				this.aggregateCombo.ClosedCombo -= new EventHandler(this.HandleAggregateClosedCombo);
				this.selectorName.TextChanged -= new EventHandler(this.HandleSelectorNameChanged);
				this.selectorName.OpeningCombo -= new CancelEventHandler(this.HandleSelectorNameOpeningCombo);
				this.selectorName.ClosedCombo -= new EventHandler(this.HandleSelectorNameClosedCombo);
				this.selectorGo.Pressed -= new MessageEventHandler(this.HandleSelectorGo);
			}

			base.Dispose(disposing);
		}

		// Donne la toolbar pour les sélections.
		public HToolBar SelectorToolBar
		{
			get
			{
				return this.selectorToolBar;
			}
		}

		// Met à jour le bouton de stretch.
		public void UpdateSelectorStretch()
		{
			IconButton button = this.selectorToolBar.FindChild("SelectorStretch") as IconButton;
			if ( button == null )  return;

			SelectorTypeStretch type = this.document.Modifier.ActiveViewer.SelectorTypeStretch;
			button.IconName = Principal.GetSelectorTypeStretchIcon(type);
		}

		// Crée la toolbar pour les sélections.
		protected void CreateSelectorToolBar()
		{
			this.selectorToolBar = new HToolBar(this);
			this.selectorToolBar.Dock = DockStyle.Top;
			this.selectorToolBar.DockMargins = new Margins(0, 0, 0, 5);

			System.Diagnostics.Debug.Assert(this.selectorToolBar.CommandDispatcher != null);
			
			this.selectorAuto = new IconButton(Misc.Icon("SelectorAuto"));
			this.selectorToolBar.Items.Add(this.selectorAuto);
			this.selectorAuto.Command = "SelectorAuto";
			ToolTip.Default.SetToolTip(this.selectorAuto, Res.Strings.Container.Principal.Button.Auto);
			
			this.selectorIndividual = new IconButton(Misc.Icon("SelectorIndividual"));
			this.selectorToolBar.Items.Add(this.selectorIndividual);
			this.selectorIndividual.Command = "SelectorIndividual";
			ToolTip.Default.SetToolTip(this.selectorIndividual, Res.Strings.Container.Principal.Button.Individual);
			
			this.selectorZoom = new IconButton(Misc.Icon("SelectorZoom"));
			this.selectorToolBar.Items.Add(this.selectorZoom);
			this.selectorZoom.Command = "SelectorZoom";
			ToolTip.Default.SetToolTip(this.selectorZoom, Res.Strings.Container.Principal.Button.Zoom);
			
			this.selectorStretch = new IconButton(Misc.Icon("SelectorStretch"));
			this.selectorToolBar.Items.Add(this.selectorStretch);
			this.selectorStretch.Command = "SelectorStretch";
			this.selectorStretch.Name = "SelectorStretch";
			ToolTip.Default.SetToolTip(this.selectorStretch, Res.Strings.Container.Principal.Button.Stretch);

			GlyphButton selectorStretchType = new GlyphButton("SelectorStretchType");
			selectorStretchType.Name = "SelectorStretchType";
			selectorStretchType.GlyphShape = GlyphShape.ArrowDown;
			selectorStretchType.ButtonStyle = ButtonStyle.ToolItem;
			selectorStretchType.Width = 14;
			selectorStretchType.DockMargins = new Margins(-1, 0, 0, 0);
			ToolTip.Default.SetToolTip(selectorStretchType, Res.Strings.Container.Principal.Button.StretchType);
			this.selectorToolBar.Items.Add(selectorStretchType);

			this.selectorToolBar.Items.Add(new IconSeparator());
			
			this.selectorTotal = new IconButton(Misc.Icon("SelectTotal"));
			this.selectorToolBar.Items.Add(this.selectorTotal);
			this.selectorTotal.Command = "SelectTotal";
			ToolTip.Default.SetToolTip(this.selectorTotal, Res.Strings.Container.Principal.Button.Total);
			
			this.selectorPartial = new IconButton(Misc.Icon("SelectPartial"));
			this.selectorToolBar.Items.Add(this.selectorPartial);
			this.selectorPartial.Command = "SelectPartial";
			ToolTip.Default.SetToolTip(this.selectorPartial, Res.Strings.Container.Principal.Button.Partial);

			this.selectorToolBar.Items.Add(new IconSeparator());
			
			this.selectorAdaptLine = new IconButton(Misc.Icon("SelectorAdaptLine"));
			this.selectorToolBar.Items.Add(this.selectorAdaptLine);
			this.selectorAdaptLine.Command = "SelectorAdaptLine";
			ToolTip.Default.SetToolTip(this.selectorAdaptLine, Res.Strings.Container.Principal.Button.AdaptLine);
			
			this.selectorAdaptText = new IconButton(Misc.Icon("SelectorAdaptText"));
			this.selectorToolBar.Items.Add(this.selectorAdaptText);
			this.selectorAdaptText.Command = "SelectorAdaptText";
			ToolTip.Default.SetToolTip(this.selectorAdaptText, Res.Strings.Container.Principal.Button.AdaptText);
		}

		// Crée la toolbar pour les agrégats.
		protected void CreateAggregateToolBar()
		{
			this.aggregateToolBar = new HToolBar(this);
			this.aggregateToolBar.Dock = DockStyle.Top;
			this.aggregateToolBar.DockMargins = new Margins(0, 0, 0, 5);

			System.Diagnostics.Debug.Assert(this.aggregateToolBar.CommandDispatcher != null);

			StaticText st = new StaticText(this.aggregateToolBar);
			st.Width = 30;
			st.Dock = DockStyle.Left;
			st.Text = Res.Strings.Container.Principal.Button.AggregateLabel;

			this.aggregateCombo = new Widgets.AggregateCombo(this.aggregateToolBar);
			this.aggregateCombo.Document = this.document;
			this.aggregateCombo.IsDeep = true;
			this.aggregateCombo.Width = 135;
			this.aggregateCombo.Dock = DockStyle.Left;
			this.aggregateCombo.DockMargins = new Margins(0, 0, 1, 1);
			this.aggregateCombo.TextChanged += new EventHandler(this.HandleAggregateComboChanged);
			this.aggregateCombo.ClosedCombo += new EventHandler(this.HandleAggregateClosedCombo);
			ToolTip.Default.SetToolTip(this.aggregateCombo, Res.Strings.Container.Principal.Button.AggregateCombo);
			
			this.aggregateNew3 = new IconButton(Misc.Icon("AggregateNew3"));
			this.aggregateNew3.Pressed += new MessageEventHandler(this.HandleAggregateNew3);
			this.aggregateToolBar.Items.Add(this.aggregateNew3);
			ToolTip.Default.SetToolTip(this.aggregateNew3, Res.Strings.Action.AggregateNew3);
			
			this.aggregateNewAll = new IconButton(Misc.Icon("AggregateNewAll"));
			this.aggregateNewAll.Pressed += new MessageEventHandler(this.HandleAggregateNewAll);
			this.aggregateToolBar.Items.Add(this.aggregateNewAll);
			ToolTip.Default.SetToolTip(this.aggregateNewAll, Res.Strings.Action.AggregateNewAll);
			
			this.aggregateFree = new IconButton(Misc.Icon("AggregateFree"));
			this.aggregateFree.Pressed += new MessageEventHandler(this.HandleAggregateFree);
			this.aggregateToolBar.Items.Add(this.aggregateFree);
			ToolTip.Default.SetToolTip(this.aggregateFree, Res.Strings.Action.AggregateFree);
		}

		// Met à jour les boutons des agrégats.
		protected void UpdateAggregate()
		{
			string name = this.document.Modifier.AggregateGetSelectedName();

			this.ignoreChanged = true;
			this.aggregateCombo.Text = name;
			this.aggregateCombo.SelectAll();
			this.ignoreChanged = false;

			this.aggregateNew3.SetEnabled(name == "");
			this.aggregateNewAll.SetEnabled(name == "");
			this.aggregateFree.SetEnabled(name != "");
		}

		// Crée le panneau pour les sélections.
		protected void CreateSelectorPanel()
		{
			this.selectorPanel = new Panel(this);
			this.selectorPanel.Dock = DockStyle.Top;
			this.selectorPanel.DockMargins = new Margins(0, 0, 0, 5);
			this.selectorPanel.Hide();

			this.selectorName = new TextFieldCombo(this.selectorPanel);
			this.selectorName.Width = 150;
			this.selectorName.Dock = DockStyle.Left;
			this.selectorName.TextChanged += new EventHandler(this.HandleSelectorNameChanged);
			this.selectorName.OpeningCombo += new CancelEventHandler(this.HandleSelectorNameOpeningCombo);
			this.selectorName.ClosedCombo += new EventHandler(this.HandleSelectorNameClosedCombo);
			ToolTip.Default.SetToolTip(this.selectorName, Res.Strings.Container.Principal.Button.SelName);

			this.selectorGo = new Button(this.selectorPanel);
			this.selectorGo.Text = Res.Strings.Container.Principal.Button.SelGo;
			this.selectorGo.Width = 80;
			this.selectorGo.Dock = DockStyle.Left;
			this.selectorGo.DockMargins = new Margins(3, 0, 0, 0);
			this.selectorGo.Pressed += new MessageEventHandler(this.HandleSelectorGo);
			ToolTip.Default.SetToolTip(this.selectorGo, Res.Strings.Container.Principal.Button.SelGoHelp);

			this.UpdateSelectorGo();
		}

		// Met à jour le bouton de séleciton.
		protected void UpdateSelectorGo()
		{
			this.selectorGo.SetEnabled(this.selectorName.Text.Length > 0);
		}


		// Met en évidence l'objet survolé par la souris.
		public override void Hilite(Objects.Abstract hiliteObject)
		{
			if ( !this.IsVisible )  return;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				if ( widget is Panels.Abstract )
				{
					Panels.Abstract panel = widget as Panels.Abstract;
					if ( this.document.Modifier.PropertiesDetailMany )
					{
						panel.IsObjectHilite = (hiliteObject != null);
						panel.IsHilite = Abstract.IsObjectUseByProperty(panel.Property, hiliteObject);
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
			//?System.Diagnostics.Debug.WriteLine(string.Format("A: DebugAliveWidgetsCount = {0}", Widget.DebugAliveWidgetsCount));
			Viewer viewer = this.document.Modifier.ActiveViewer;
			DrawingContext context = viewer.DrawingContext;

			if ( this.document.Modifier.Tool == "Select" ||
				 this.document.Modifier.Tool == "Global" ||
				 this.document.Modifier.Tool == "Edit"   )
			{
				if ( this.document.Modifier.TotalSelected == 0 )
				{
					this.aggregateToolBar.Hide();
				}
				else
				{
					this.aggregateToolBar.Show();
				}

				this.selectorToolBar.Show();
				this.selectorPanel.SetVisible(this.document.Modifier.NamesExist);
			}
			else
			{
				if ( this.document.Modifier.IsTool )
				{
					this.aggregateToolBar.Hide();
				}
				else
				{
					this.aggregateToolBar.Show();
				}

				this.selectorToolBar.Hide();
				this.selectorPanel.Hide();
			}

			this.UpdateAggregate();

			this.detailButton.Parent = null;

			// Supprime tous les panneaux ou boutons.
			this.scrollable.Panel.SuspendLayout();

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				if ( widget is Panels.Abstract )
				{
					Panels.Abstract panel = widget as Panels.Abstract;
					panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				}

				widget.Dispose();
			}

			Widget originColorLastPanel = null;
			if ( this.document.Modifier.ActiveViewer.IsCreating )
			{
				// Crée tous les boutons pour l'objet en cours de création.
				Objects.Abstract layer = context.RootObject();
				int rank = viewer.CreateRank();
				Objects.Abstract creatingObject = layer.Objects[rank] as Objects.Abstract;
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
				foreach ( Properties.Abstract property in list )
				{
					double topMargin = 0;
					if ( lastBack != -1 && lastBack != Properties.Abstract.BackgroundIntensity(property.Type) )
					{
						topMargin = 5;
					}
					lastBack = Properties.Abstract.BackgroundIntensity(property.Type);

					Panels.Abstract panel = property.CreatePanel(this.document);
					if ( panel == null )  continue;
					panel.Property = property;

					panel.IsExtendedSize = this.document.Modifier.IsPropertiesExtended(property.Type);
					panel.IsLayoutDirect = (property.Type == Properties.Type.Name);

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
			//?System.Diagnostics.Debug.WriteLine(string.Format("B: DebugAliveWidgetsCount = {0}", Widget.DebugAliveWidgetsCount));
		}

		// Effectue la mise à jour des propriétés.
		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			Widget originColorLastPanel = null;
			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				Panels.Abstract panel = widget as Panels.Abstract;
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

		// Effectue la mise à jour de la sélection par noms.
		protected override void DoUpdateSelNames()
		{
			if ( this.document.Modifier.Tool == "Select" ||
				 this.document.Modifier.Tool == "Global" )
			{
				this.selectorPanel.SetVisible(this.document.Modifier.NamesExist);
			}
			else
			{
				this.selectorPanel.Hide();
			}

			this.ignoreChanged = true;
			this.selectorName.Text = "";
			this.ignoreChanged = false;
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
			this.OriginColorRulerDeselect();

			Widget wSender = sender as Widget;
			Color backColor = Color.Empty;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				Panels.Abstract panel = widget as Panels.Abstract;
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
				this.colorSelector.SetVisible(false);
				this.colorSelector.BackColor = Color.Empty;
			}
			else
			{
				this.colorSelector.SetVisible(true);
				this.colorSelector.BackColor = backColor;
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorType = this.originColorPanel.Property.Type;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
		}

		// La couleur dans la règle d'un texte a été cliquée.
		public void TextRulerColorClicked(Common.Widgets.TextRuler ruler)
		{
			this.originColorPanel = null;
			this.originColorRuler = ruler;
			this.originColorRuler.ColorSample.ActiveState = WidgetState.ActiveYes;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				Panels.Abstract panel = widget as Panels.Abstract;
				if ( panel == null )  continue;

				panel.OriginColorDeselect();
			}

			this.colorSelector.SetVisible(true);
			this.colorSelector.BackColor = Color.Empty;
			this.ignoreColorChanged = true;
			this.colorSelector.Color = this.originColorRuler.FontRichColor;
			this.ignoreColorChanged = false;
		}

		// La couleur dans la règle d'un texte a changé.
		public void TextRulerColorChanged(Common.Widgets.TextRuler ruler)
		{
			if ( this.originColorRuler == null )  return;

			this.ignoreColorChanged = true;
			this.colorSelector.Color = this.originColorRuler.FontRichColor;
			this.ignoreColorChanged = false;
		}

		// Couleur changée dans la roue.
		private void HandleColorSelectorChanged(object sender)
		{
			if ( this.ignoreColorChanged )  return;

			if ( this.originColorPanel != null )
			{
				this.originColorPanel.OriginColorChange(this.colorSelector.Color);
			}

			if ( this.originColorRuler != null )
			{
				this.originColorRuler.FontRichColor = this.colorSelector.Color;

				Objects.Abstract edit = this.document.Modifier.RetEditObject();
				if ( edit != null )
				{
					this.document.Notifier.NotifyArea(edit.EditSelectBox);
				}
			}
		}

		// Fermer la roue.
		private void HandleColorSelectorClosed(object sender)
		{
			this.originColorPanel = null;
			this.OriginColorRulerDeselect();
			this.originColorType = Properties.Type.None;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				Panels.Abstract panel = widget as Panels.Abstract;
				if ( panel == null )  continue;
				panel.OriginColorDeselect();
			}

			this.colorSelector.SetVisible(false);
			this.colorSelector.BackColor = Color.Empty;
		}

		// Désélectionne l'origine de couleurs possibles dans la règle.
		protected void OriginColorRulerDeselect()
		{
			if ( this.originColorRuler == null )  return;

			this.originColorRuler.ColorSample.ActiveState = WidgetState.ActiveNo;
			this.originColorRuler = null;
		}


		// Texte du nom à sélectionner changé.
		private void HandleSelectorNameChanged(object sender)
		{
			this.UpdateSelectorGo();
		}

		// Combo du texte du nom à sélectionner ouvert.
		private void HandleSelectorNameOpeningCombo(object sender, CancelEventArgs e)
		{
			this.selectorName.Items.Clear();
			System.Collections.ArrayList list = this.document.Modifier.SelectNames();
			foreach ( string name in list )
			{
				this.selectorName.Items.Add(name);
			}
		}

		// Combo du texte du nom à sélectionner fermé.
		private void HandleSelectorNameClosedCombo(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.document.Modifier.SelectName(this.selectorName.Text);
		}

		// Bouton "chercher" actionné.
		private void HandleSelectorGo(object sender, MessageEventArgs e)
		{
			this.document.Modifier.SelectName(this.selectorName.Text);
		}


		// Texte des agrégats changé.
		private void HandleAggregateComboChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			this.document.Modifier.AggregateChangeName(this.aggregateCombo.Text);
		}

		// Combo des agrégats fermé.
		private void HandleAggregateClosedCombo(object sender)
		{
			if ( this.ignoreChanged )  return;
			int sel = this.aggregateCombo.SelectedIndex;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;
			this.document.Modifier.AggregateUse(agg);
		}

		private void HandleAggregateNew3(object sender, MessageEventArgs e)
		{
			string name = this.aggregateCombo.Text;
			this.document.Modifier.AggregateNew3(10000, name, false);
		}

		private void HandleAggregateNewAll(object sender, MessageEventArgs e)
		{
			string name = this.aggregateCombo.Text;
			this.document.Modifier.AggregateNewAll(10000, name, false);
		}

		private void HandleAggregateFree(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AggregateFree();
		}


		#region StretchMenu
		// Construit le menu des types de stretch.
		public VMenu CreateStretchTypeMenu(MessageEventHandler message)
		{
			VMenu menu = new VMenu();

			this.CreateStretchTypeMenu(menu, message, SelectorTypeStretch.Free, Res.Strings.Container.Principal.Menu.Stretch.Free);
			menu.Items.Add(new MenuSeparator());
			this.CreateStretchTypeMenu(menu, message, SelectorTypeStretch.TrapezeH, Res.Strings.Container.Principal.Menu.Stretch.TrapezeH);
			this.CreateStretchTypeMenu(menu, message, SelectorTypeStretch.TrapezeV, Res.Strings.Container.Principal.Menu.Stretch.TrapezeV);
			menu.Items.Add(new MenuSeparator());
			this.CreateStretchTypeMenu(menu, message, SelectorTypeStretch.ParallelH, Res.Strings.Container.Principal.Menu.Stretch.ParallelH);
			this.CreateStretchTypeMenu(menu, message, SelectorTypeStretch.ParallelV, Res.Strings.Container.Principal.Menu.Stretch.ParallelV);

			menu.AdjustSize();
			return menu;
		}

		// Crée une case du menu des actions à refaire/annuler.
		protected void CreateStretchTypeMenu(VMenu menu, MessageEventHandler message,
											 SelectorTypeStretch type, string text)
		{
			string icon = Principal.GetSelectorTypeStretchIcon(type);
			string name = ((int)type).ToString();
			MenuItem item = new MenuItem("SelectorStretchTypeDo(this.Name)", icon, text, "", name);

			if ( message != null )
			{
				item.Pressed += message;
			}

			menu.Items.Add(item);
		}

		// Retourne l'icône à utiliser pour un type de déformation.
		protected static string GetSelectorTypeStretchIcon(SelectorTypeStretch type)
		{
			if ( type == SelectorTypeStretch.Free      )  return Misc.Icon("SelectorStretch");
			if ( type == SelectorTypeStretch.ParallelH )  return Misc.Icon("SelectorStretchParallelH");
			if ( type == SelectorTypeStretch.ParallelV )  return Misc.Icon("SelectorStretchParallelV");
			if ( type == SelectorTypeStretch.TrapezeH  )  return Misc.Icon("SelectorStretchTrapezeH");
			if ( type == SelectorTypeStretch.TrapezeV  )  return Misc.Icon("SelectorStretchTrapezeV");
			return "";
		}
		#endregion


		protected HToolBar						selectorToolBar;
		protected IconButton					selectorAuto;
		protected IconButton					selectorIndividual;
		protected IconButton					selectorZoom;
		protected IconButton					selectorStretch;
		protected IconButton					selectorTotal;
		protected IconButton					selectorPartial;
		protected IconButton					selectorAdaptLine;
		protected IconButton					selectorAdaptText;
		protected Panel							selectorPanel;
		protected TextFieldCombo				selectorName;
		protected Button						selectorGo;

		protected HToolBar						aggregateToolBar;
		protected Widgets.AggregateCombo		aggregateCombo;
		protected IconButton					aggregateNew3;
		protected IconButton					aggregateNewAll;
		protected IconButton					aggregateFree;

		protected CheckButton					detailButton;
		protected Scrollable					scrollable;
		protected ColorSelector					colorSelector;
		protected Panels.Abstract				originColorPanel = null;
		protected Properties.Type				originColorType = Properties.Type.None;
		protected int							originColorRank = -1;
		protected Common.Widgets.TextRuler		originColorRuler = null;

		protected bool							ignoreColorChanged = false;
		protected bool							ignoreChanged = false;
	}
}
