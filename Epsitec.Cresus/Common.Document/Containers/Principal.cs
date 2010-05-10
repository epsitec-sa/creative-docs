using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Principal contient tous les panneaux des propriétés.
	/// </summary>
	public class Principal : Abstract
	{
		public Principal(Document document) : base(document)
		{
			this.helpText = new StaticText(this);
			this.helpText.Text = Res.Strings.Container.Help.Principal;
			this.helpText.Dock = DockStyle.Top;
			this.helpText.Margins = new Margins(0, 0, -2, 7);

			this.CreateSelectorToolBar();
			this.CreateAggregateToolBar();
			this.CreateTextToolBar();
			this.CreateSelectorPanel();

			this.detailButton = new CheckButton();
			this.detailButton.Text = Res.Strings.Container.Principal.Button.Detail;
			this.detailButton.Dock = DockStyle.Top;
			this.detailButton.Margins = new Margins(0, 0, 5, 5);
			this.detailButton.TabIndex = 1;
			this.detailButton.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			this.detailButton.Clicked += this.HandleDetailButtonClicked;

			this.scrollable = new Scrollable();
			this.scrollable.Dock = DockStyle.Fill;
			this.scrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.scrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.scrollable.Viewport.IsAutoFitting = true;
			this.scrollable.PaintViewportFrame = true;
			this.scrollable.ViewportFrameMargins = new Margins(0, 1, 0, 0);
			this.scrollable.ViewportPadding = new Margins (-1);
			this.scrollable.SetParent(this);

			this.colorSelector = new ColorSelector();
			Misc.AutoFocus(this.colorSelector, false);
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.CloseButtonVisibility = true;
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.Margins = new Margins(0, 0, 10, 0);
			this.colorSelector.ColorChanged += new EventHandler<DependencyPropertyChangedEventArgs>(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += this.HandleColorSelectorClosed;
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			this.colorSelector.SetParent(this);
			this.colorSelector.Visibility = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.aggregateCombo.TextChanged -= this.HandleAggregateComboChanged;
				this.aggregateCombo.ComboClosed -= this.HandleAggregateComboClosed;
				this.selectorName.TextChanged -= this.HandleSelectorNameChanged;
				this.selectorName.ComboOpening -= new EventHandler<CancelEventArgs> (this.HandleSelectorNameComboOpening);
				this.selectorName.ComboClosed -= this.HandleSelectorNameComboClosed;
				this.selectorGo.Clicked -= this.HandleSelectorGo;
			}

			base.Dispose(disposing);
		}

		public HToolBar SelectorToolBar
		{
			//	Donne la toolbar pour les sélections.
			get
			{
				return this.selectorToolBar;
			}
		}

		public void UpdateSelectorStretch()
		{
			//	Met à jour le bouton de stretch.
			IconButton button = this.selectorToolBar.FindChild("SelectorStretch") as IconButton;
			if ( button == null )  return;

			SelectorTypeStretch type = this.document.Modifier.ActiveViewer.SelectorTypeStretch;
			button.IconUri = Principal.GetSelectorTypeStretchIcon(type);
		}

		protected void CreateSelectorToolBar()
		{
			//	Crée la toolbar pour les sélections.
			this.selectorToolBar = new HToolBar(this);
			this.selectorToolBar.Dock = DockStyle.Top;
			this.selectorToolBar.Margins = new Margins(0, 0, 0, -1);

//-			System.Diagnostics.Debug.Assert(this.selectorToolBar.CommandDispatcher != null);
			
			this.selectorAuto = new IconButton(Misc.Icon("SelectorAuto"));
			this.selectorAuto.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorAuto);
			this.selectorAuto.CommandObject = Command.Get("SelectorAuto");
			ToolTip.Default.SetToolTip(this.selectorAuto, Res.Strings.Container.Principal.Button.Auto);
			
			this.selectorIndividual = new IconButton(Misc.Icon("SelectorIndividual"));
			this.selectorIndividual.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorIndividual);
			this.selectorIndividual.CommandObject = Command.Get("SelectorIndividual");
			ToolTip.Default.SetToolTip(this.selectorIndividual, Res.Strings.Container.Principal.Button.Individual);
			
			this.selectorScaler = new IconButton(Misc.Icon("SelectorScaler"));
			this.selectorScaler.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorScaler);
			this.selectorScaler.CommandObject = Command.Get("SelectorScaler");
			ToolTip.Default.SetToolTip(this.selectorScaler, Res.Strings.Container.Principal.Button.Scaler);
			
			this.selectorStretch = new IconButton(Misc.Icon("SelectorStretch"));
			this.selectorStretch.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorStretch);
			this.selectorStretch.CommandObject = Command.Get("SelectorStretch");
			this.selectorStretch.Name = "SelectorStretch";
			ToolTip.Default.SetToolTip(this.selectorStretch, Res.Strings.Container.Principal.Button.Stretch);

			GlyphButton selectorStretchType = new GlyphButton("SelectorStretchType");
			selectorStretchType.AutoFocus = false;
			selectorStretchType.Name = "SelectorStretchType";
			selectorStretchType.GlyphShape = GlyphShape.Menu;
			selectorStretchType.ButtonStyle = ButtonStyle.ComboItem;
			selectorStretchType.PreferredWidth = 12;
			selectorStretchType.Margins = new Margins(-1, 0, 0, 0);
			selectorStretchType.ExecuteCommandOnPressed = true;
			ToolTip.Default.SetToolTip(selectorStretchType, Res.Strings.Container.Principal.Button.StretchType);
			this.selectorToolBar.Items.Add(selectorStretchType);

			this.selectorToolBar.Items.Add(new IconSeparator());
			
			this.selectorTotal = new IconButton(Misc.Icon("SelectTotal"));
			this.selectorTotal.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorTotal);
			this.selectorTotal.CommandObject = Command.Get("SelectTotal");
			ToolTip.Default.SetToolTip(this.selectorTotal, Res.Strings.Container.Principal.Button.Total);
			
			this.selectorPartial = new IconButton(Misc.Icon("SelectPartial"));
			this.selectorPartial.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorPartial);
			this.selectorPartial.CommandObject = Command.Get("SelectPartial");
			ToolTip.Default.SetToolTip(this.selectorPartial, Res.Strings.Container.Principal.Button.Partial);

			this.selectorToolBar.Items.Add(new IconSeparator());
			
			this.selectorAdaptLine = new IconButton(Misc.Icon("SelectorAdaptLine"));
			this.selectorAdaptLine.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorAdaptLine);
			this.selectorAdaptLine.CommandObject = Command.Get("SelectorAdaptLine");
			ToolTip.Default.SetToolTip(this.selectorAdaptLine, Res.Strings.Container.Principal.Button.AdaptLine);

#if false	//	Les nouveaux objets texte n'y sont plus sensible !
			this.selectorAdaptText = new IconButton(Misc.Icon("SelectorAdaptText"));
			this.selectorAdaptText.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorAdaptText);
			this.selectorAdaptText.CommandObject = Command.Get("SelectorAdaptText");
			ToolTip.Default.SetToolTip(this.selectorAdaptText, Res.Strings.Container.Principal.Button.AdaptText);
#endif
		}

		protected void CreateAggregateToolBar()
		{
			//	Crée la toolbar pour les agrégats.
			this.aggregateToolBar = new HToolBar(this);
			this.aggregateToolBar.Dock = DockStyle.Top;
			this.aggregateToolBar.Margins = new Margins(0, 0, 0, -1);

//-			System.Diagnostics.Debug.Assert(this.aggregateToolBar.CommandDispatcher != null);

			StaticText st = new StaticText(this.aggregateToolBar);
			st.PreferredWidth = 30;
			st.Dock = DockStyle.Left;
			st.Text = Res.Strings.Container.Principal.Button.AggregateLabel;

			this.aggregateCombo = new Widgets.StyleCombo(this.aggregateToolBar);
			this.aggregateCombo.Document = this.document;
			this.aggregateCombo.StyleCategory = StyleCategory.Graphic;
			this.aggregateCombo.IsDeep = true;
			this.aggregateCombo.PreferredWidth = 130;
			this.aggregateCombo.Dock = DockStyle.Left;
			this.aggregateCombo.Margins = new Margins(0, 0, 1, 1);
			this.aggregateCombo.TextChanged += this.HandleAggregateComboChanged;
			this.aggregateCombo.ComboClosed += this.HandleAggregateComboClosed;
			ToolTip.Default.SetToolTip(this.aggregateCombo, Res.Strings.Container.Principal.Button.AggregateCombo);
			
			this.aggregateNew3 = new IconButton(Misc.Icon("AggregateNew3"));
			this.aggregateNew3.AutoFocus = false;
			this.aggregateNew3.Clicked += this.HandleAggregateNew3;
			this.aggregateToolBar.Items.Add(this.aggregateNew3);
			ToolTip.Default.SetToolTip(this.aggregateNew3, Res.Strings.Action.AggregateNew3);
			
			this.aggregateNewAll = new IconButton(Misc.Icon("AggregateNewAll"));
			this.aggregateNewAll.AutoFocus = false;
			this.aggregateNewAll.Clicked += this.HandleAggregateNewAll;
			this.aggregateToolBar.Items.Add(this.aggregateNewAll);
			ToolTip.Default.SetToolTip(this.aggregateNewAll, Res.Strings.Action.AggregateNewAll);
			
			this.aggregateFree = new IconButton(Misc.Icon("AggregateFree"));
			this.aggregateFree.AutoFocus = false;
			this.aggregateFree.Clicked += this.HandleAggregateFree;
			this.aggregateToolBar.Items.Add(this.aggregateFree);
			ToolTip.Default.SetToolTip(this.aggregateFree, Res.Strings.Action.AggregateFree);
		}

		protected void UpdateAggregate()
		{
			//	Met à jour les boutons des agrégats.
			string name = this.document.Modifier.AggregateGetSelectedName();

			this.ignoreChanged = true;
			this.aggregateCombo.Text = name;
			this.aggregateCombo.SelectAll();
			this.ignoreChanged = false;

			this.aggregateNew3.Enable = (name == "");
			this.aggregateNewAll.Enable = (name == "");
			this.aggregateFree.Enable = (name != "");
		}

		protected void CreateTextToolBar()
		{
			//	Crée la toolbar pour le texte.
			this.textToolBar = new HToolBar(this);
			this.textToolBar.Dock = DockStyle.Top;
			this.textToolBar.Margins = new Margins(0, 0, 0, -1);

			StaticText st = new StaticText(this.textToolBar);
			st.Text = Res.Strings.TextPanel.Filter.Title;
			st.PreferredWidth = 90;
			st.Dock = DockStyle.Left;

			this.textUsual = new IconButton(this.textToolBar);
			this.textUsual.AutoFocus = false;
			this.textUsual.IconUri = Misc.Icon("TextFilterUsual");
			this.textUsual.Name = "Usual";
			this.textUsual.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textUsual.Dock = DockStyle.Left;
			this.textUsual.Clicked += this.HandleFilterTextClicked;
			ToolTip.Default.SetToolTip(this.textUsual, Res.Strings.TextPanel.Filter.Tooltip.Usual);

			this.textFrequently = new IconButton(this.textToolBar);
			this.textFrequently.AutoFocus = false;
			this.textFrequently.IconUri = Misc.Icon("TextFilterFrequently");
			this.textFrequently.Name = "Frequently";
			this.textFrequently.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textFrequently.Dock = DockStyle.Left;
			this.textFrequently.Clicked += this.HandleFilterTextClicked;
			ToolTip.Default.SetToolTip(this.textFrequently, Res.Strings.TextPanel.Filter.Tooltip.Frequently);

			this.textAll = new IconButton(this.textToolBar);
			this.textAll.AutoFocus = false;
			this.textAll.IconUri = Misc.Icon("TextFilterAll");
			this.textAll.Name = "All";
			this.textAll.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textAll.Dock = DockStyle.Left;
			this.textAll.Clicked += this.HandleFilterTextClicked;
			ToolTip.Default.SetToolTip(this.textAll, Res.Strings.TextPanel.Filter.Tooltip.All);

			this.textToolBar.Items.Add(new IconSeparator());

			this.textParagraph = new IconButton(this.textToolBar);
			this.textParagraph.AutoFocus = false;
			this.textParagraph.IconUri = Misc.Icon("TextFilterParagraph");
			this.textParagraph.Name = "Paragraph";
			this.textParagraph.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textParagraph.Dock = DockStyle.Left;
			this.textParagraph.Clicked += this.HandleFilterTextClicked;
			ToolTip.Default.SetToolTip(this.textParagraph, Res.Strings.TextPanel.Filter.Tooltip.Paragraph);

			this.textCharacter = new IconButton(this.textToolBar);
			this.textCharacter.AutoFocus = false;
			this.textCharacter.IconUri = Misc.Icon("TextFilterCharacter");
			this.textCharacter.Name = "Character";
			this.textCharacter.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textCharacter.Dock = DockStyle.Left;
			this.textCharacter.Clicked += this.HandleFilterTextClicked;
			ToolTip.Default.SetToolTip(this.textCharacter, Res.Strings.TextPanel.Filter.Tooltip.Character);

			this.UpdateText();
		}

		protected void UpdateText()
		{
			//	Met à jour les boutons de la toolbar du texte.
			this.textUsual.ActiveState      = (this.textFilter == "Usual"     ) ? ActiveState.Yes : ActiveState.No;
			this.textFrequently.ActiveState = (this.textFilter == "Frequently") ? ActiveState.Yes : ActiveState.No;
			this.textAll.ActiveState        = (this.textFilter == "All"       ) ? ActiveState.Yes : ActiveState.No;
			this.textParagraph.ActiveState  = (this.textFilter == "Paragraph" ) ? ActiveState.Yes : ActiveState.No;
			this.textCharacter.ActiveState  = (this.textFilter == "Character" ) ? ActiveState.Yes : ActiveState.No;
		}

		protected void CreateSelectorPanel()
		{
			//	Crée le panneau pour les sélections.
			this.selectorPanel = new Viewport(this);
			this.selectorPanel.Dock = DockStyle.Top;
			this.selectorPanel.Margins = new Margins(0, 0, 0, 5);
			this.selectorPanel.Hide();

			this.selectorName = new TextFieldCombo(this.selectorPanel);
			this.selectorName.PreferredWidth = 150;
			this.selectorName.Dock = DockStyle.Left;
			this.selectorName.TextChanged += this.HandleSelectorNameChanged;
			this.selectorName.ComboOpening += new EventHandler<CancelEventArgs> (this.HandleSelectorNameComboOpening);
			this.selectorName.ComboClosed += this.HandleSelectorNameComboClosed;
			ToolTip.Default.SetToolTip(this.selectorName, Res.Strings.Container.Principal.Button.SelName);

			this.selectorGo = new Button(this.selectorPanel);
			this.selectorGo.Text = Res.Strings.Container.Principal.Button.SelGo;
			this.selectorGo.PreferredWidth = 80;
			this.selectorGo.Dock = DockStyle.Left;
			this.selectorGo.Margins = new Margins(3, 0, 0, 0);
			this.selectorGo.Clicked += this.HandleSelectorGo;
			ToolTip.Default.SetToolTip(this.selectorGo, Res.Strings.Container.Principal.Button.SelGoHelp);

			this.UpdateSelectorGo();
		}

		protected void UpdateSelectorGo()
		{
			//	Met à jour le bouton de séleciton.
			this.selectorGo.Enable = (this.selectorName.Text.Length > 0);
		}

		public bool ShowOnlyPanels
		{
			get
			{
				return this.showOnlyPanels;
			}
			set
			{
				if ( this.showOnlyPanels != value )
				{
					this.showOnlyPanels = value;
					this.DoUpdateContent();
				}
			}
		}

		public override void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en évidence l'objet survolé par la souris.
			if ( !this.IsVisible )  return;

			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
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

		public void UpdateContent()
		{
			this.DoUpdateContent ();
		}

		public void SetTextFilter(string filter)
		{
			this.textFilter = filter;
			this.UpdateText ();
			this.DoUpdateContent ();
		}

		protected override void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
			//?System.Diagnostics.Debug.WriteLine(string.Format("A: DebugAliveWidgetsCount = {0}", Widget.DebugAliveWidgetsCount));
			Viewer viewer = this.document.Modifier.ActiveViewer;
			DrawingContext context = viewer.DrawingContext;

			this.helpText.Visibility = this.document.GlobalSettings.LabelProperties && !this.showOnlyPanels;

			if ( this.showOnlyPanels )
			{
				this.aggregateToolBar.Hide();
				this.selectorToolBar.Hide();
				this.selectorPanel.Hide();
				this.textToolBar.Hide();
			}
			else
			{
				if ( this.document.Modifier.Tool == "ToolSelect" ||
					 this.document.Modifier.Tool == "ToolGlobal" )
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
					this.selectorPanel.Visibility = (this.document.Modifier.NamesExist);
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

				if ( this.document.Modifier.IsToolEdit )
				{
					this.textToolBar.Show();
				}
				else
				{
					this.textToolBar.Hide();
				}
			}
			
			this.UpdateAggregate();

			this.detailButton.SetParent(null);

			//	Supprime tous les panneaux ou boutons.

			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				if ( widget is Panels.Abstract )
				{
					Panels.Abstract panel = widget as Panels.Abstract;
					panel.OriginColorChanged -= this.HandleOriginColorChanged;
				}

				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;
					panel.OriginColorChanged -= this.HandleOriginColorChanged;
				}

				widget.Dispose();
			}

			Widget originColorLastPanel = null;
			if ( this.document.Modifier.ActiveViewer.IsCreating )
			{
				//	Crée tous les boutons pour l'objet en cours de création.
				Objects.Abstract layer = context.RootObject();
				int rank = viewer.CreateRank;
				Objects.Abstract creatingObject = layer.Objects[rank] as Objects.Abstract;
				double topMargin = 50;
				for ( int i=0 ; i<100 ; i++ )
				{
					string cmd, name, text;
					if ( !creatingObject.CreateAction(i, out cmd, out name, out text) )  break;

					if ( cmd != "" )
					{
						Button button = new Button();
						button.PreferredHeight = 40;
						button.CommandObject = Command.Get(cmd);
						button.Name = name;
						button.Text = text;
						button.ContentAlignment = ContentAlignment.MiddleLeft;
						button.Dock = DockStyle.Top;
						button.Margins = new Margins(10, 10, topMargin, 0);
						button.SetParent(this.scrollable.Viewport);
					}

					topMargin = (cmd == "") ? 20 : 4;
				}
			}
			else if ( this.document.Modifier.IsToolEdit )
			{
				//	Crée tous les panneaux des "propriétés" de texte (pour les wrappers).
				Objects.AbstractText editObject = this.document.Modifier.RetEditObject();
				if ( editObject != null )
				{
					TextPanels.Abstract.StaticDocument = this.document;
					System.Collections.ArrayList list = editObject.CreateTextPanels(this.textFilter);
					if ( list != null )
					{
						int index = 1;
						foreach ( TextPanels.Abstract panel in list )
						{
							double tm = (index == 1) ? 0 : panel.TopMargin;
							panel.TabIndex = index++;
							panel.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
							panel.Dock = DockStyle.Top;
							panel.Margins = new Margins(0, 1, tm, -1);
							panel.IsExtendedSize = this.document.Modifier.IsTextPanelExtended(panel);
							panel.OriginColorChanged += this.HandleOriginColorChanged;
							panel.SetParent(this.scrollable.Viewport);
						}
					}
				}
			}
			else
			{
				//	Crée tous les panneaux des propriétés.
				System.Collections.ArrayList list = new System.Collections.ArrayList();
				this.document.Modifier.PropertiesList(list);

				if ( list.Count > 0 && this.document.Modifier.TotalSelected > 1 )
				{
					this.detailButton.ActiveState = this.document.Modifier.PropertiesDetail ? ActiveState.Yes : ActiveState.No;
					this.detailButton.SetParent(this);
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

					panel.OriginColorChanged += this.HandleOriginColorChanged;

					panel.TabIndex = index++;
					panel.TabNavigationMode = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
					panel.Dock = DockStyle.Top;
					panel.Margins = new Margins(0, 1, topMargin, -1);
					panel.SetParent(this.scrollable.Viewport);

					if ( panel.Property.Type == this.originColorType )
					{
						originColorLastPanel = panel;
					}
				}
			}

			this.HandleOriginColorChanged(originColorLastPanel, true);
			//?System.Diagnostics.Debug.WriteLine(string.Format("B: DebugAliveWidgetsCount = {0}", Widget.DebugAliveWidgetsCount));
		}

		protected bool IsTextPanelsExtended()
		{
			//	Indique si tous les panneaux pour le texte sont étendus.
			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;
					if ( !panel.IsExtendedSize )  return false;
				}
			}
			return true;
		}

		protected bool IsTextPanelsReduced()
		{
			//	Indique si tous les panneaux pour le texte sont réduits.
			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;
					if ( panel.IsExtendedSize )  return false;
				}
			}
			return true;
		}

		protected void TextPanelsExtend(bool extend)
		{
			//	Etend ou réduit tous les panneaux pour le texte.
			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;
					panel.IsExtendedSize = extend;
				}
			}
		}

		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise à jour des propriétés.
			Widget originColorLastPanel = null;
			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
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

		protected override void DoUpdateGeometry()
		{
			//	Effectue la mise à jour après un changement de géométrie.
			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				Panels.Abstract panel = widget as Panels.Abstract;
				if ( panel != null )
				{
					panel.UpdateGeometry();
				}
			}
		}

		protected override void DoUpdateSelNames()
		{
			//	Effectue la mise à jour de la sélection par noms.
			if ( this.document.Modifier.Tool == "ToolSelect" ||
				 this.document.Modifier.Tool == "ToolGlobal" )
			{
				this.selectorPanel.Visibility = (this.document.Modifier.NamesExist);
			}
			else
			{
				this.selectorPanel.Hide();
			}

			this.ignoreChanged = true;
			this.selectorName.Text = "";
			this.ignoreChanged = false;
		}


		private void HandleDetailButtonClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton des détails a été cliqué.
			this.document.Modifier.PropertiesDetail = !this.document.Modifier.PropertiesDetail;
		}

		private void HandleOriginColorChanged(object sender)
		{
			//	Le widget qui détermine la couleur d'origine a changé.
			this.HandleOriginColorChanged(sender, false);
		}

		private void HandleOriginColorChanged(object sender, bool lastOrigin)
		{
			this.originColorPanel = null;
			this.originColorTextPanel = null;

			Widget wSender = sender as Widget;
			Color backColor = Color.Empty;

			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				if ( widget is Panels.Abstract )
				{
					Panels.Abstract panel = widget as Panels.Abstract;

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

				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;

					if ( panel == wSender )
					{
						this.originColorTextPanel = panel;
						panel.OriginColorSelect( lastOrigin ? this.originColorRank : -1 );
					}
					else
					{
						panel.OriginColorDeselect();
					}
				}
			}

			if ( this.originColorPanel != null )
			{
				this.colorSelector.Visibility = true;
				this.colorSelector.BackColor = backColor;
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorType = this.originColorPanel.Property.Type;
				this.originColorRank = this.originColorPanel.OriginColorRank();
			}
			else if ( this.originColorTextPanel != null )
			{
				this.colorSelector.Visibility = true;
				this.colorSelector.BackColor = backColor;
				this.ignoreColorChanged = true;
				this.colorSelector.Color = this.originColorTextPanel.OriginColorGet();
				this.ignoreColorChanged = false;
				this.originColorRank = this.originColorTextPanel.OriginColorRank();
			}
			else
			{
				this.colorSelector.Visibility = false;
				this.colorSelector.BackColor = Color.Empty;
			}
		}

		private void HandleColorSelectorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Couleur changée dans la roue.
			if ( this.ignoreColorChanged )  return;

			if ( this.originColorPanel != null )
			{
				this.originColorPanel.OriginColorChange(this.colorSelector.Color);
			}

			if ( this.originColorTextPanel != null )
			{
				this.originColorTextPanel.OriginColorChange(this.colorSelector.Color);
			}
		}

		private void HandleColorSelectorClosed(object sender)
		{
			//	Fermer la roue.
			this.originColorPanel = null;
			this.originColorTextPanel = null;
			this.originColorType = Properties.Type.None;

			foreach ( Widget widget in this.scrollable.Viewport.Children.Widgets )
			{
				if ( widget is Panels.Abstract )
				{
					Panels.Abstract panel = widget as Panels.Abstract;
					panel.OriginColorDeselect();
				}

				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;
					panel.OriginColorDeselect();
				}
			}

			this.colorSelector.Visibility = false;
			this.colorSelector.BackColor = Color.Empty;
		}


		private void HandleSelectorNameChanged(object sender)
		{
			//	Texte du nom à sélectionner changé.
			this.UpdateSelectorGo();
		}

		private void HandleSelectorNameComboOpening(object sender, CancelEventArgs e)
		{
			//	Combo du texte du nom à sélectionner ouvert.
			this.selectorName.Items.Clear();
			System.Collections.ArrayList list = this.document.Modifier.SelectNames();
			foreach ( string name in list )
			{
				this.selectorName.Items.Add(name);
			}
		}

		private void HandleSelectorNameComboClosed(object sender)
		{
			//	Combo du texte du nom à sélectionner fermé.
			if ( this.ignoreChanged )  return;
			this.document.Modifier.SelectName(this.selectorName.Text);
		}

		private void HandleSelectorGo(object sender, MessageEventArgs e)
		{
			//	Bouton "chercher" actionné.
			this.document.Modifier.SelectName(this.selectorName.Text);
		}


		private void HandleAggregateComboChanged(object sender)
		{
			//	Texte des agrégats changé.
			if ( this.ignoreChanged )  return;
			this.document.Modifier.AggregateChangeName(this.aggregateCombo.Text);
		}

		private void HandleAggregateComboClosed(object sender)
		{
			//	Combo des agrégats fermé.
			if ( this.ignoreChanged )  return;
			int sel = this.aggregateCombo.SelectedItemIndex;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;
			this.document.Modifier.AggregateUse(agg);
		}

		private void HandleAggregateNew3(object sender, MessageEventArgs e)
		{
			string name = this.aggregateCombo.Text;
			this.document.Modifier.AggregateNew3(10000, name, true);
		}

		private void HandleAggregateNewAll(object sender, MessageEventArgs e)
		{
			string name = this.aggregateCombo.Text;
			this.document.Modifier.AggregateNewAll(10000, name, true);
		}

		private void HandleAggregateFree(object sender, MessageEventArgs e)
		{
			this.document.Modifier.AggregateFree();
		}


		private void HandleFilterTextClicked(object sender, MessageEventArgs e)
		{
			IconButton button = sender as IconButton;
			string filter = button.Name;

			if ( this.textFilter == filter )
			{
				if ( this.IsTextPanelsReduced() )
				{
					this.TextPanelsExtend(true);
				}
				else
				{
					this.TextPanelsExtend(false);
				}
			}
			else
			{
				this.SetTextFilter (filter);
			}
		}


		#region StretchMenu
		public VMenu CreateStretchTypeMenu(Support.EventHandler<MessageEventArgs> message)
		{
			//	Construit le menu des types de stretch.
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

		protected void CreateStretchTypeMenu(VMenu menu, Support.EventHandler<MessageEventArgs> message,
											 SelectorTypeStretch type, string text)
		{
			//	Crée une case du menu des actions à refaire/annuler.
			string icon = Principal.GetSelectorTypeStretchIcon(type);
			string name = ((int)type).ToString(System.Globalization.CultureInfo.InvariantCulture);
			string cmd  = "SelectorStretchTypeDo";
			
			Misc.CreateStructuredCommandWithName (cmd);

			MenuItem item = new MenuItem(cmd, icon, text, "", name);

			if ( message != null )
			{
				item.Clicked += message;
			}

			menu.Items.Add(item);
		}

		protected static string GetSelectorTypeStretchIcon(SelectorTypeStretch type)
		{
			//	Retourne l'icône à utiliser pour un type de déformation.
			if ( type == SelectorTypeStretch.Free      )  return Misc.Icon("SelectorStretch");
			if ( type == SelectorTypeStretch.ParallelH )  return Misc.Icon("SelectorStretchParallelH");
			if ( type == SelectorTypeStretch.ParallelV )  return Misc.Icon("SelectorStretchParallelV");
			if ( type == SelectorTypeStretch.TrapezeH  )  return Misc.Icon("SelectorStretchTrapezeH");
			if ( type == SelectorTypeStretch.TrapezeV  )  return Misc.Icon("SelectorStretchTrapezeV");
			return "";
		}
		#endregion


		protected StaticText					helpText;

		protected HToolBar						selectorToolBar;
		protected IconButton					selectorAuto;
		protected IconButton					selectorIndividual;
		protected IconButton					selectorScaler;
		protected IconButton					selectorStretch;
		protected IconButton					selectorTotal;
		protected IconButton					selectorPartial;
		protected IconButton					selectorAdaptLine;
		protected IconButton					selectorAdaptText;
		protected Viewport						selectorPanel;
		protected TextFieldCombo				selectorName;
		protected Button						selectorGo;

		protected HToolBar						aggregateToolBar;
		protected Widgets.StyleCombo			aggregateCombo;
		protected IconButton					aggregateNew3;
		protected IconButton					aggregateNewAll;
		protected IconButton					aggregateFree;

		protected HToolBar						textToolBar;
		protected IconButton					textUsual;
		protected IconButton					textFrequently;
		protected IconButton					textAll;
		protected IconButton					textParagraph;
		protected IconButton					textCharacter;

		protected CheckButton					detailButton;
		protected Scrollable					scrollable;
		protected ColorSelector					colorSelector;
		protected Panels.Abstract				originColorPanel = null;
		protected TextPanels.Abstract			originColorTextPanel = null;
		protected Properties.Type				originColorType = Properties.Type.None;
		protected int							originColorRank = -1;
		protected string						textFilter = "Usual";

		protected bool							ignoreColorChanged = false;
		protected bool							ignoreChanged = false;
		protected bool							showOnlyPanels = false;
	}
}
