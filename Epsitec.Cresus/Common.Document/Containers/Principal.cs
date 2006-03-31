using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Principal contient tous les panneaux des propri�t�s.
	/// </summary>
	[SuppressBundleSupport]
	public class Principal : Abstract
	{
		public Principal(Document document) : base(document)
		{
			StaticText help = new StaticText(this);
			help.Text = Res.Strings.Container.Help.Principal;
			help.Dock = DockStyle.Top;
			help.DockMargins = new Margins(0, 0, -2, 7);

			this.CreateSelectorToolBar();
			this.CreateAggregateToolBar();
			this.CreateTextToolBar();
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
			this.scrollable.SetParent(this);

			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Dock = DockStyle.Bottom;
			this.colorSelector.DockMargins = new Margins(0, 0, 10, 0);
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
			this.colorSelector.SetParent(this);
			this.colorSelector.Visibility = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.aggregateCombo.TextChanged -= new EventHandler(this.HandleAggregateComboChanged);
				this.aggregateCombo.ComboClosed -= new EventHandler(this.HandleAggregateComboClosed);
				this.selectorName.TextChanged -= new EventHandler(this.HandleSelectorNameChanged);
				this.selectorName.ComboOpening -= new CancelEventHandler(this.HandleSelectorNameComboOpening);
				this.selectorName.ComboClosed -= new EventHandler(this.HandleSelectorNameComboClosed);
				this.selectorGo.Pressed -= new MessageEventHandler(this.HandleSelectorGo);
			}

			base.Dispose(disposing);
		}

		public HToolBar SelectorToolBar
		{
			//	Donne la toolbar pour les s�lections.
			get
			{
				return this.selectorToolBar;
			}
		}

		public void UpdateSelectorStretch()
		{
			//	Met � jour le bouton de stretch.
			IconButton button = this.selectorToolBar.FindChild("SelectorStretch") as IconButton;
			if ( button == null )  return;

			SelectorTypeStretch type = this.document.Modifier.ActiveViewer.SelectorTypeStretch;
			button.IconName = Principal.GetSelectorTypeStretchIcon(type);
		}

		protected void CreateSelectorToolBar()
		{
			//	Cr�e la toolbar pour les s�lections.
			this.selectorToolBar = new HToolBar(this);
			this.selectorToolBar.Dock = DockStyle.Top;
			this.selectorToolBar.DockMargins = new Margins(0, 0, 0, 5);

//-			System.Diagnostics.Debug.Assert(this.selectorToolBar.CommandDispatcher != null);
			
			this.selectorAuto = new IconButton(Misc.Icon("SelectorAuto"));
			this.selectorAuto.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorAuto);
			this.selectorAuto.Command = "SelectorAuto";
			ToolTip.Default.SetToolTip(this.selectorAuto, Res.Strings.Container.Principal.Button.Auto);
			
			this.selectorIndividual = new IconButton(Misc.Icon("SelectorIndividual"));
			this.selectorIndividual.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorIndividual);
			this.selectorIndividual.Command = "SelectorIndividual";
			ToolTip.Default.SetToolTip(this.selectorIndividual, Res.Strings.Container.Principal.Button.Individual);
			
			this.selectorScaler = new IconButton(Misc.Icon("SelectorScaler"));
			this.selectorScaler.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorScaler);
			this.selectorScaler.Command = "SelectorScaler";
			ToolTip.Default.SetToolTip(this.selectorScaler, Res.Strings.Container.Principal.Button.Scaler);
			
			this.selectorStretch = new IconButton(Misc.Icon("SelectorStretch"));
			this.selectorStretch.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorStretch);
			this.selectorStretch.Command = "SelectorStretch";
			this.selectorStretch.Name = "SelectorStretch";
			ToolTip.Default.SetToolTip(this.selectorStretch, Res.Strings.Container.Principal.Button.Stretch);

			GlyphButton selectorStretchType = new GlyphButton("SelectorStretchType");
			selectorStretchType.AutoFocus = false;
			selectorStretchType.Name = "SelectorStretchType";
			selectorStretchType.GlyphShape = GlyphShape.ArrowDown;
			selectorStretchType.ButtonStyle = ButtonStyle.ToolItem;
			selectorStretchType.Width = 14;
			selectorStretchType.DockMargins = new Margins(-1, 0, 0, 0);
			ToolTip.Default.SetToolTip(selectorStretchType, Res.Strings.Container.Principal.Button.StretchType);
			this.selectorToolBar.Items.Add(selectorStretchType);

			this.selectorToolBar.Items.Add(new IconSeparator());
			
			this.selectorTotal = new IconButton(Misc.Icon("SelectTotal"));
			this.selectorTotal.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorTotal);
			this.selectorTotal.Command = "SelectTotal";
			ToolTip.Default.SetToolTip(this.selectorTotal, Res.Strings.Container.Principal.Button.Total);
			
			this.selectorPartial = new IconButton(Misc.Icon("SelectPartial"));
			this.selectorPartial.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorPartial);
			this.selectorPartial.Command = "SelectPartial";
			ToolTip.Default.SetToolTip(this.selectorPartial, Res.Strings.Container.Principal.Button.Partial);

			this.selectorToolBar.Items.Add(new IconSeparator());
			
			this.selectorAdaptLine = new IconButton(Misc.Icon("SelectorAdaptLine"));
			this.selectorAdaptLine.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorAdaptLine);
			this.selectorAdaptLine.Command = "SelectorAdaptLine";
			ToolTip.Default.SetToolTip(this.selectorAdaptLine, Res.Strings.Container.Principal.Button.AdaptLine);
			
			this.selectorAdaptText = new IconButton(Misc.Icon("SelectorAdaptText"));
			this.selectorAdaptText.AutoFocus = false;
			this.selectorToolBar.Items.Add(this.selectorAdaptText);
			this.selectorAdaptText.Command = "SelectorAdaptText";
			ToolTip.Default.SetToolTip(this.selectorAdaptText, Res.Strings.Container.Principal.Button.AdaptText);
		}

		protected void CreateAggregateToolBar()
		{
			//	Cr�e la toolbar pour les agr�gats.
			this.aggregateToolBar = new HToolBar(this);
			this.aggregateToolBar.Dock = DockStyle.Top;
			this.aggregateToolBar.DockMargins = new Margins(0, 0, 0, 5);

//-			System.Diagnostics.Debug.Assert(this.aggregateToolBar.CommandDispatcher != null);

			StaticText st = new StaticText(this.aggregateToolBar);
			st.Width = 30;
			st.Dock = DockStyle.Left;
			st.Text = Res.Strings.Container.Principal.Button.AggregateLabel;

			this.aggregateCombo = new Widgets.StyleCombo(this.aggregateToolBar);
			this.aggregateCombo.Document = this.document;
			this.aggregateCombo.StyleCategory = StyleCategory.Graphic;
			this.aggregateCombo.IsDeep = true;
			this.aggregateCombo.Width = 130;
			this.aggregateCombo.Dock = DockStyle.Left;
			this.aggregateCombo.DockMargins = new Margins(0, 0, 1, 1);
			this.aggregateCombo.TextChanged += new EventHandler(this.HandleAggregateComboChanged);
			this.aggregateCombo.ComboClosed += new EventHandler(this.HandleAggregateComboClosed);
			ToolTip.Default.SetToolTip(this.aggregateCombo, Res.Strings.Container.Principal.Button.AggregateCombo);
			
			this.aggregateNew3 = new IconButton(Misc.Icon("AggregateNew3"));
			this.aggregateNew3.AutoFocus = false;
			this.aggregateNew3.Pressed += new MessageEventHandler(this.HandleAggregateNew3);
			this.aggregateToolBar.Items.Add(this.aggregateNew3);
			ToolTip.Default.SetToolTip(this.aggregateNew3, Res.Strings.Action.AggregateNew3);
			
			this.aggregateNewAll = new IconButton(Misc.Icon("AggregateNewAll"));
			this.aggregateNewAll.AutoFocus = false;
			this.aggregateNewAll.Pressed += new MessageEventHandler(this.HandleAggregateNewAll);
			this.aggregateToolBar.Items.Add(this.aggregateNewAll);
			ToolTip.Default.SetToolTip(this.aggregateNewAll, Res.Strings.Action.AggregateNewAll);
			
			this.aggregateFree = new IconButton(Misc.Icon("AggregateFree"));
			this.aggregateFree.AutoFocus = false;
			this.aggregateFree.Pressed += new MessageEventHandler(this.HandleAggregateFree);
			this.aggregateToolBar.Items.Add(this.aggregateFree);
			ToolTip.Default.SetToolTip(this.aggregateFree, Res.Strings.Action.AggregateFree);
		}

		protected void UpdateAggregate()
		{
			//	Met � jour les boutons des agr�gats.
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
			//	Cr�e la toolbar pour le texte.
			this.textToolBar = new HToolBar(this);
			this.textToolBar.Dock = DockStyle.Top;
			this.textToolBar.DockMargins = new Margins(0, 0, 0, 5);

			StaticText st = new StaticText(this.textToolBar);
			st.Text = Res.Strings.TextPanel.Filter.Title;
			st.Width = 90;
			st.Dock = DockStyle.Left;

			this.textUsual = new IconButton(this.textToolBar);
			this.textUsual.AutoFocus = false;
			this.textUsual.IconName = Misc.Icon("TextFilterUsual");
			this.textUsual.Name = "Usual";
			this.textUsual.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textUsual.Dock = DockStyle.Left;
			this.textUsual.Clicked += new MessageEventHandler(this.HandleFilterTextClicked);
			ToolTip.Default.SetToolTip(this.textUsual, Res.Strings.TextPanel.Filter.Tooltip.Usual);

			this.textFrequently = new IconButton(this.textToolBar);
			this.textFrequently.AutoFocus = false;
			this.textFrequently.IconName = Misc.Icon("TextFilterFrequently");
			this.textFrequently.Name = "Frequently";
			this.textFrequently.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textFrequently.Dock = DockStyle.Left;
			this.textFrequently.Clicked += new MessageEventHandler(this.HandleFilterTextClicked);
			ToolTip.Default.SetToolTip(this.textFrequently, Res.Strings.TextPanel.Filter.Tooltip.Frequently);

			this.textAll = new IconButton(this.textToolBar);
			this.textAll.AutoFocus = false;
			this.textAll.IconName = Misc.Icon("TextFilterAll");
			this.textAll.Name = "All";
			this.textAll.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textAll.Dock = DockStyle.Left;
			this.textAll.Clicked += new MessageEventHandler(this.HandleFilterTextClicked);
			ToolTip.Default.SetToolTip(this.textAll, Res.Strings.TextPanel.Filter.Tooltip.All);

			this.textToolBar.Items.Add(new IconSeparator());

			this.textParagraph = new IconButton(this.textToolBar);
			this.textParagraph.AutoFocus = false;
			this.textParagraph.IconName = Misc.Icon("TextFilterParagraph");
			this.textParagraph.Name = "Paragraph";
			this.textParagraph.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textParagraph.Dock = DockStyle.Left;
			this.textParagraph.Clicked += new MessageEventHandler(this.HandleFilterTextClicked);
			ToolTip.Default.SetToolTip(this.textParagraph, Res.Strings.TextPanel.Filter.Tooltip.Paragraph);

			this.textCharacter = new IconButton(this.textToolBar);
			this.textCharacter.AutoFocus = false;
			this.textCharacter.IconName = Misc.Icon("TextFilterCharacter");
			this.textCharacter.Name = "Character";
			this.textCharacter.ButtonStyle = ButtonStyle.ActivableIcon;
			this.textCharacter.Dock = DockStyle.Left;
			this.textCharacter.Clicked += new MessageEventHandler(this.HandleFilterTextClicked);
			ToolTip.Default.SetToolTip(this.textCharacter, Res.Strings.TextPanel.Filter.Tooltip.Character);

			this.UpdateText();
		}

		protected void UpdateText()
		{
			//	Met � jour les boutons de la toolbar du texte.
			this.textUsual.ActiveState      = (this.textFilter == "Usual"     ) ? ActiveState.Yes : ActiveState.No;
			this.textFrequently.ActiveState = (this.textFilter == "Frequently") ? ActiveState.Yes : ActiveState.No;
			this.textAll.ActiveState        = (this.textFilter == "All"       ) ? ActiveState.Yes : ActiveState.No;
			this.textParagraph.ActiveState  = (this.textFilter == "Paragraph" ) ? ActiveState.Yes : ActiveState.No;
			this.textCharacter.ActiveState  = (this.textFilter == "Character" ) ? ActiveState.Yes : ActiveState.No;
		}

		protected void CreateSelectorPanel()
		{
			//	Cr�e le panneau pour les s�lections.
			this.selectorPanel = new Panel(this);
			this.selectorPanel.Dock = DockStyle.Top;
			this.selectorPanel.DockMargins = new Margins(0, 0, 0, 5);
			this.selectorPanel.Hide();

			this.selectorName = new TextFieldCombo(this.selectorPanel);
			this.selectorName.Width = 150;
			this.selectorName.Dock = DockStyle.Left;
			this.selectorName.TextChanged += new EventHandler(this.HandleSelectorNameChanged);
			this.selectorName.ComboOpening += new CancelEventHandler(this.HandleSelectorNameComboOpening);
			this.selectorName.ComboClosed += new EventHandler(this.HandleSelectorNameComboClosed);
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

		protected void UpdateSelectorGo()
		{
			//	Met � jour le bouton de s�leciton.
			this.selectorGo.Enable = (this.selectorName.Text.Length > 0);
		}


		public override void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en �vidence l'objet survol� par la souris.
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

		
		protected override void DoUpdateContent()
		{
			//	Effectue la mise � jour du contenu.
			//?System.Diagnostics.Debug.WriteLine(string.Format("A: DebugAliveWidgetsCount = {0}", Widget.DebugAliveWidgetsCount));
			Viewer viewer = this.document.Modifier.ActiveViewer;
			DrawingContext context = viewer.DrawingContext;

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

			this.UpdateAggregate();

			this.detailButton.SetParent(null);

			//	Supprime tous les panneaux ou boutons.
			this.scrollable.Panel.SuspendLayout();

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
			{
				if ( widget is Panels.Abstract )
				{
					Panels.Abstract panel = widget as Panels.Abstract;
					panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				}

				if ( widget is TextPanels.Abstract )
				{
					TextPanels.Abstract panel = widget as TextPanels.Abstract;
					panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				}

				widget.Dispose();
			}

			Widget originColorLastPanel = null;
			if ( this.document.Modifier.ActiveViewer.IsCreating )
			{
				//	Cr�e tous les boutons pour l'objet en cours de cr�ation.
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
						button.Height = 40;
						button.Command = cmd;
						button.Name = name;
						button.Text = text;
						button.Alignment = ContentAlignment.MiddleLeft;
						button.Dock = DockStyle.Top;
						button.DockMargins = new Margins(10, 10, topMargin, 0);
						button.SetParent(this.scrollable.Panel);
					}

					topMargin = (cmd == "") ? 20 : 4;
				}
			}
			else if ( this.document.Modifier.IsToolEdit )
			{
				//	Cr�e tous les panneaux des "propri�t�s" de texte (pour les wrappers).
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
							panel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
							panel.Dock = DockStyle.Top;
							panel.DockMargins = new Margins(0, 1, tm, -1);
							panel.IsExtendedSize = this.document.Modifier.IsTextPanelExtended(panel);
							panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
							panel.SetParent(this.scrollable.Panel);
						}
					}
				}
			}
			else
			{
				//	Cr�e tous les panneaux des propri�t�s.
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

					panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);

					panel.TabIndex = index++;
					panel.TabNavigation = Widget.TabNavigationMode.ActivateOnTab | Widget.TabNavigationMode.ForwardToChildren | Widget.TabNavigationMode.ForwardOnly;
					panel.Dock = DockStyle.Top;
					panel.DockMargins = new Margins(0, 1, topMargin, -1);
					panel.SetParent(this.scrollable.Panel);

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

		protected bool IsTextPanelsExtended()
		{
			//	Indique si tous les panneaux pour le texte sont �tendus.
			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
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
			//	Indique si tous les panneaux pour le texte sont r�duits.
			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
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
			//	Etend ou r�duit tous les panneaux pour le texte.
			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
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
			//	Effectue la mise � jour des propri�t�s.
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

		protected override void DoUpdateSelNames()
		{
			//	Effectue la mise � jour de la s�lection par noms.
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
			//	Le bouton des d�tails a �t� cliqu�.
			this.document.Modifier.PropertiesDetail = !this.document.Modifier.PropertiesDetail;
		}

		private void HandleOriginColorChanged(object sender)
		{
			//	Le widget qui d�termine la couleur d'origine a chang�.
			this.HandleOriginColorChanged(sender, false);
		}

		private void HandleOriginColorChanged(object sender, bool lastOrigin)
		{
			this.originColorPanel = null;
			this.originColorTextPanel = null;

			Widget wSender = sender as Widget;
			Color backColor = Color.Empty;

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
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

		private void HandleColorSelectorChanged(object sender)
		{
			//	Couleur chang�e dans la roue.
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

			foreach ( Widget widget in this.scrollable.Panel.Children.Widgets )
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
			//	Texte du nom � s�lectionner chang�.
			this.UpdateSelectorGo();
		}

		private void HandleSelectorNameComboOpening(object sender, CancelEventArgs e)
		{
			//	Combo du texte du nom � s�lectionner ouvert.
			this.selectorName.Items.Clear();
			System.Collections.ArrayList list = this.document.Modifier.SelectNames();
			foreach ( string name in list )
			{
				this.selectorName.Items.Add(name);
			}
		}

		private void HandleSelectorNameComboClosed(object sender)
		{
			//	Combo du texte du nom � s�lectionner ferm�.
			if ( this.ignoreChanged )  return;
			this.document.Modifier.SelectName(this.selectorName.Text);
		}

		private void HandleSelectorGo(object sender, MessageEventArgs e)
		{
			//	Bouton "chercher" actionn�.
			this.document.Modifier.SelectName(this.selectorName.Text);
		}


		private void HandleAggregateComboChanged(object sender)
		{
			//	Texte des agr�gats chang�.
			if ( this.ignoreChanged )  return;
			this.document.Modifier.AggregateChangeName(this.aggregateCombo.Text);
		}

		private void HandleAggregateComboClosed(object sender)
		{
			//	Combo des agr�gats ferm�.
			if ( this.ignoreChanged )  return;
			int sel = this.aggregateCombo.SelectedIndex;
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

			if ( this.textFilter == button.Name )
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
				this.textFilter = button.Name;
				this.UpdateText();
				this.DoUpdateContent();
			}
		}


		#region StretchMenu
		public VMenu CreateStretchTypeMenu(MessageEventHandler message)
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

		protected void CreateStretchTypeMenu(VMenu menu, MessageEventHandler message,
											 SelectorTypeStretch type, string text)
		{
			//	Cr�e une case du menu des actions � refaire/annuler.
			string icon = Principal.GetSelectorTypeStretchIcon(type);
			string name = ((int)type).ToString();
			MenuItem item = new MenuItem("SelectorStretchTypeDo(this.Name)", icon, text, "", name);

			if ( message != null )
			{
				item.Pressed += message;
			}

			menu.Items.Add(item);
		}

		protected static string GetSelectorTypeStretchIcon(SelectorTypeStretch type)
		{
			//	Retourne l'ic�ne � utiliser pour un type de d�formation.
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
		protected IconButton					selectorScaler;
		protected IconButton					selectorStretch;
		protected IconButton					selectorTotal;
		protected IconButton					selectorPartial;
		protected IconButton					selectorAdaptLine;
		protected IconButton					selectorAdaptText;
		protected Panel							selectorPanel;
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
	}
}
