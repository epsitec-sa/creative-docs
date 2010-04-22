using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Styles contient tous les panneaux des styles.
	/// </summary>
	public class Styles : Abstract
	{
		public Styles(Document document) : base(document)
		{
			this.helpText = new StaticText(this);
			this.helpText.Text = Res.Strings.Container.Help.Styles;
			this.helpText.Dock = DockStyle.Top;
			this.helpText.Margins = new Margins(0, 0, -2, 7);

			this.mainBook = new PaneBook(this);
			this.mainBook.PaneBookStyle = PaneBookStyle.BottomTop;
			this.mainBook.PaneBehaviour = PaneBookBehaviour.Draft;
			this.mainBook.Dock = DockStyle.Fill;

			this.topPage = new PanePage();
			this.topPage.PaneRelativeSize = 50;
			this.topPage.PaneMinSize = 155;  // minimun pour avoir 2 styles dans la liste
			this.topPage.PaneElasticity = 0.5;
			this.mainBook.Items.Add(this.topPage);

			this.bottomPage = new PanePage();
			this.bottomPage.PaneRelativeSize = 50;
			this.bottomPage.PaneMinSize = 150;
			this.bottomPage.PaneElasticity = 0.5;
			this.mainBook.Items.Add(this.bottomPage);

			this.CreateCategoryGroup();
			this.CreateAggregateToolBar();

			//	Table des agrégats (styles graphiques).
			this.graphicList = new Widgets.AggregateList();
			this.graphicList.Document = this.document;
			this.graphicList.List = this.document.Aggregates;
			this.graphicList.HScroller = true;
			this.graphicList.VScroller = true;
			this.graphicList.SetParent(this.topPage);
			this.graphicList.MinSize = new Size(10, 87);
			this.graphicList.Dock = DockStyle.Fill;
			this.graphicList.Margins = new Margins(0, 0, 0, 0);
			this.graphicList.FinalSelectionChanged += this.HandleAggregatesTableSelectionChanged;
			this.graphicList.FlyOverChanged += this.HandleAggregatesTableFlyOverChanged;
			this.graphicList.DoubleClicked += this.HandleAggregatesTableDoubleClicked;
			this.graphicList.TabIndex = 2;
			this.graphicList.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Table des styles de paragraphe.
			this.paragraphList = new Widgets.TextStylesList();
			this.paragraphList.Document = this.document;
			this.paragraphList.Category = StyleCategory.Paragraph;
			this.paragraphList.HScroller = true;
			this.paragraphList.VScroller = true;
			this.paragraphList.SetParent(this.topPage);
			this.paragraphList.MinSize = new Size(10, 87);
			this.paragraphList.Dock = DockStyle.Fill;
			this.paragraphList.Margins = new Margins(0, 0, 0, 0);
			this.paragraphList.FinalSelectionChanged += this.HandleStylesTableSelectionChanged;
			this.paragraphList.DoubleClicked += this.HandleStylesTableDoubleClicked;
			this.paragraphList.TabIndex = 2;
			this.paragraphList.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			//	Table des styles de caractère.
			this.characterList = new Widgets.TextStylesList();
			this.characterList.Document = this.document;
			this.characterList.Category = StyleCategory.Character;
			this.characterList.HScroller = true;
			this.characterList.VScroller = true;
			this.characterList.SetParent(this.topPage);
			this.characterList.MinSize = new Size(10, 87);
			this.characterList.Dock = DockStyle.Fill;
			this.characterList.Margins = new Margins(0, 0, 0, 0);
			this.characterList.FinalSelectionChanged += this.HandleStylesTableSelectionChanged;
			this.characterList.DoubleClicked += this.HandleStylesTableDoubleClicked;
			this.characterList.TabIndex = 2;
			this.characterList.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.CreateNameToolBar();
			this.CreateChildrenToolBar();

			//	Sélectionneur.
			this.CreateSelectorToolBar();

			this.bottomScrollable = new Scrollable();
			this.bottomScrollable.Dock = DockStyle.Fill;
			this.bottomScrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.bottomScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.bottomScrollable.Viewport.IsAutoFitting = true;
			this.bottomScrollable.PaintViewportFrame = true;
			this.bottomScrollable.ViewportFrameMargins = new Margins(0, 1, 0, 0);
			this.bottomScrollable.ViewportPadding = new Margins (-1);
			this.bottomScrollable.SetParent(this.bottomPage);

			//	Conteneur du panneau.
			this.panelContainer = new Widget(this.bottomScrollable.Viewport);
			this.panelContainer.PreferredHeight = 0.0;
			this.panelContainer.Dock = DockStyle.Top;
			this.panelContainer.Margins = new Margins(0, 1, 0, 0);
			this.panelContainer.TabIndex = 99;
			this.panelContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			//	Roue des couleurs.
			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.CloseButtonVisibility = true;
			this.colorSelector.ColorChanged += new EventHandler<DependencyPropertyChangedEventArgs>(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += this.HandleColorSelectorClosed;
			this.colorSelector.SetParent(this.bottomScrollable.Viewport);
			this.colorSelector.Dock = DockStyle.Top;
			this.colorSelector.Margins = new Margins(1, 3, 5, 2);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			this.colorSelector.Visibility = false;

			this.category = StyleCategory.Graphic;
			this.UpdateCategory();

			this.panelContainer.PreferredHeight = 1;  // nécessaire pour mettre à jour la première fois !
//-			this.panelContainer.ForceLayout();
		}

		protected void CreateCategoryGroup()
		{
			//	Crée les boutons radio pour le choix de la catégorie.
			this.categoryContainer = new Widget(this.topPage);
			this.categoryContainer.PreferredHeight = 20+8;
			this.categoryContainer.Dock = DockStyle.Top;
			this.categoryContainer.Margins = new Margins(0, 0, 0, 0);
			this.categoryContainer.TabIndex = 1;
			this.categoryContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.index = 0;

			this.categoryGraphic = new IconButtonMark(this.categoryContainer);
			this.categoryGraphic.Name = "Graphic";
			this.categoryGraphic.ButtonStyle = ButtonStyle.ActivableIcon;
			this.categoryGraphic.AutoFocus = false;
			this.categoryGraphic.PreferredWidth = 80;
			this.categoryGraphic.Dock = DockStyle.Left;
			this.categoryGraphic.Margins = new Margins(0, 0, 0, 0);
			this.categoryGraphic.TabIndex = this.index++;
			this.categoryGraphic.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			this.categoryGraphic.Clicked += this.HandleCategoryChanged;

			this.categoryParagraph = new IconButtonMark(this.categoryContainer);
			this.categoryParagraph.Name = "Paragraph";
			this.categoryParagraph.ButtonStyle = ButtonStyle.ActivableIcon;
			this.categoryParagraph.AutoFocus = false;
			this.categoryParagraph.PreferredWidth = 80;
			this.categoryParagraph.Dock = DockStyle.Left;
			this.categoryParagraph.Margins = new Margins(0, 0, 0, 0);
			this.categoryParagraph.TabIndex = this.index++;
			this.categoryParagraph.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			this.categoryParagraph.Clicked += this.HandleCategoryChanged;

			this.categoryCharacter = new IconButtonMark(this.categoryContainer);
			this.categoryCharacter.Name = "Character";
			this.categoryCharacter.ButtonStyle = ButtonStyle.ActivableIcon;
			this.categoryCharacter.AutoFocus = false;
			this.categoryCharacter.PreferredWidth = 80-1;
			this.categoryCharacter.Dock = DockStyle.Left;
			this.categoryCharacter.Margins = new Margins(0, 0, 0, 0);
			this.categoryCharacter.TabIndex = this.index++;
			this.categoryCharacter.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			this.categoryCharacter.Clicked += this.HandleCategoryChanged;

			this.SetCategory("Graphic");
		}

		protected void CreateAggregateToolBar()
		{
			//	Crée la toolbar principale.
			this.aggregateToolBar = new HToolBar(this.topPage);
			this.aggregateToolBar.Dock = DockStyle.Top;
			this.aggregateToolBar.Margins = new Margins(0, 0, 0, -1);
			this.aggregateToolBar.TabIndex = 1;
			this.aggregateToolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.index = 0;

			this.buttonAggregateNewEmpty = new IconButton(Misc.Icon("AggregateNewEmpty"));
			this.buttonAggregateNewEmpty.Clicked += this.HandleButtonAggregateNewEmpty;
			this.buttonAggregateNewEmpty.TabIndex = this.index++;
			this.buttonAggregateNewEmpty.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateNewEmpty);
			ToolTip.Default.SetToolTip(this.buttonAggregateNewEmpty, Res.Strings.Action.AggregateNewEmpty);

			this.buttonAggregateNew3 = new IconButton(Misc.Icon("AggregateNew3"));
			this.buttonAggregateNew3.Clicked += this.HandleButtonAggregateNew3;
			this.buttonAggregateNew3.TabIndex = this.index++;
			this.buttonAggregateNew3.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateNew3);
			ToolTip.Default.SetToolTip(this.buttonAggregateNew3, Res.Strings.Action.AggregateNew3);

			this.buttonAggregateNewAll = new IconButton(Misc.Icon("AggregateNewAll"));
			this.buttonAggregateNewAll.Clicked += this.HandleButtonAggregateNewAll;
			this.buttonAggregateNewAll.TabIndex = this.index++;
			this.buttonAggregateNewAll.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateNewAll);
			ToolTip.Default.SetToolTip(this.buttonAggregateNewAll, Res.Strings.Action.AggregateNewAll);

			this.buttonAggregateDuplicate = new IconButton(Misc.Icon("AggregateDuplicate"));
			this.buttonAggregateDuplicate.Clicked += this.HandleButtonAggregateDuplicate;
			this.buttonAggregateDuplicate.TabIndex = this.index++;
			this.buttonAggregateDuplicate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateDuplicate);
			ToolTip.Default.SetToolTip(this.buttonAggregateDuplicate, Res.Strings.Action.AggregateDuplicate);

			this.aggregateToolBar.Items.Add(new IconSeparator());

			this.buttonAggregateUp = new IconButton(Misc.Icon("AggregateUp"));
			this.buttonAggregateUp.Clicked += this.HandleButtonAggregateUp;
			this.buttonAggregateUp.TabIndex = this.index++;
			this.buttonAggregateUp.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateUp);
			ToolTip.Default.SetToolTip(this.buttonAggregateUp, Res.Strings.Action.AggregateUp);

			this.buttonAggregateDown = new IconButton(Misc.Icon("AggregateDown"));
			this.buttonAggregateDown.Clicked += this.HandleButtonAggregateDown;
			this.buttonAggregateDown.TabIndex = this.index++;
			this.buttonAggregateDown.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateDown);
			ToolTip.Default.SetToolTip(this.buttonAggregateDown, Res.Strings.Action.AggregateDown);

			this.aggregateToolBar.Items.Add(new IconSeparator());

			this.buttonAggregateDelete = new IconButton(Misc.Icon("AggregateDelete"));
			this.buttonAggregateDelete.Clicked += this.HandleButtonAggregateDelete;
			this.buttonAggregateDelete.TabIndex = this.index++;
			this.buttonAggregateDelete.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateDelete);
			ToolTip.Default.SetToolTip(this.buttonAggregateDelete, Res.Strings.Action.AggregateDelete);
		}

		protected void CreateChildrenToolBar()
		{
			//	Crée la toolbar pour le choix des enfants.
			this.childrenToolBar = new HToolBar(this.bottomPage);
			this.childrenToolBar.Dock = DockStyle.Top;
			this.childrenToolBar.Margins = new Margins(0, 0, 0, 0);
			this.childrenToolBar.TabIndex = 95;
			this.childrenToolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.PreferredWidth = 45;
			st.Margins = new Margins(0, 5, 0, 0);
			st.Text = Res.Strings.Panel.AggregateChildren.Label.Name;
			st.ContentAlignment = ContentAlignment.MiddleRight;
			this.childrenToolBar.Items.Add(st);

			this.dummyChildren = new Widgets.DummyTextFieldCombo();
			this.dummyChildren.IsReadOnly = true;
			this.dummyChildren.PreferredWidth = 185;
			this.dummyChildren.Margins = new Margins(0, 0, 1, 1);
			this.dummyChildren.ComboOpenPressed += this.HandleMenuChildrenOpenPressed;
			this.childrenToolBar.Items.Add(this.dummyChildren);
			ToolTip.Default.SetToolTip(this.dummyChildren, Res.Strings.Panel.AggregateChildren.Tooltip.Name);

			this.nameChildren = new StaticText(this.dummyChildren);
			this.nameChildren.Dock = DockStyle.Fill;
			this.nameChildren.Margins = new Margins(2, 2, 0, 0);
			this.nameChildren.TabIndex = 1;
			this.nameChildren.TabNavigationMode = TabNavigationMode.ActivateOnTab;
		}

		protected void CreateNameToolBar()
		{
			//	Crée la toolbar pour le nom de l'agrégat.
			this.nameToolBar = new HToolBar(this.bottomPage);
			this.nameToolBar.Dock = DockStyle.Top;
			this.nameToolBar.Margins = new Margins(0, 0, 0, 0);
			this.nameToolBar.TabIndex = 94;
			this.nameToolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.PreferredWidth = 45;
			st.Margins = new Margins(0, 5, 0, 0);
			st.Text = Res.Strings.Panel.AggregateName.Label.Name;
			st.ContentAlignment = ContentAlignment.MiddleRight;
			this.nameToolBar.Items.Add(st);

			this.name = new TextField();
			this.name.PreferredWidth = 110;
			this.name.Margins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += this.HandleNameTextChanged;
			this.name.AutoSelectOnFocus = true;
			this.name.TabIndex = 1;
			this.name.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.nameToolBar.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.AggregateName.Tooltip.Name);
		}

		protected void CreateSelectorToolBar()
		{
			//	Crée la toolbar pour le sélectionneur de panneaux.
			this.selectorContainer = new Widget(this.bottomPage);
			this.selectorContainer.PreferredHeight = Styles.selectorSize+8;
			this.selectorContainer.Dock = DockStyle.Top;
			this.selectorContainer.Margins = new Margins(0, 0, 5, 0);
			this.selectorContainer.TabIndex = 97;
			this.selectorContainer.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.selectorToolBar = new Widget(this.selectorContainer);
			this.selectorToolBar.PreferredHeight = Styles.selectorSize+8;
			this.selectorToolBar.Dock = DockStyle.Fill;
			this.selectorToolBar.Margins = new Margins(0, 0, 0, 0);
			this.selectorToolBar.TabIndex = 1;
			this.selectorToolBar.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.buttonStyleDelete = new IconButton(this.selectorContainer);
			this.buttonStyleDelete.IconUri = Misc.Icon("AggregateStyleDelete");
			this.buttonStyleDelete.AutoFocus = false;
			this.buttonStyleDelete.Clicked += this.HandleButtonStyleDelete;
			this.buttonStyleDelete.Dock = DockStyle.Right;
			this.buttonStyleDelete.Margins = new Margins(0, 0, 0, 6);
			this.buttonStyleDelete.TabIndex = 3;
			this.buttonStyleDelete.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonStyleDelete, Res.Strings.Action.AggregateStyleDelete);

			this.buttonStyleNew = new IconButton(this.selectorContainer);
			this.buttonStyleNew.IconUri = Misc.Icon("AggregateStyleNew");
			this.buttonStyleNew.AutoFocus = false;
			this.buttonStyleNew.Pressed += this.HandleButtonStyleNew;
			this.buttonStyleNew.Dock = DockStyle.Right;
			this.buttonStyleNew.Margins = new Margins(0, 0, 0, 6);
			this.buttonStyleNew.TabIndex = 2;
			this.buttonStyleNew.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonStyleNew, Res.Strings.Action.AggregateStyleNew);

			this.separatorStyle = new IconSeparator(this.selectorContainer);
			this.separatorStyle.Dock = DockStyle.Right;
		}


		public override void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en évidence l'objet survolé par la souris.
			if ( !this.IsVisible )  return;

			if ( this.graphicList.Rows != this.document.Aggregates.Count )
			{
				this.SetDirtyContent();
				this.Update();
			}

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			this.graphicList.HiliteColor = context.HiliteSurfaceColor;

			for ( int i=0 ; i<this.document.Aggregates.Count ; i++ )
			{
				Properties.Aggregate agg = this.document.Aggregates[i] as Properties.Aggregate;
				bool hilite = (hiliteObject != null && hiliteObject.Aggregates.Contains(agg));
				this.graphicList.HiliteRow(i, hilite);
			}
		}

		
		protected override void DoUpdateContent()
		{
			//	Effectue la mise à jour du contenu.
			this.helpText.Visibility = this.document.GlobalSettings.LabelProperties;

			this.graphicList.List = this.document.Aggregates;
			this.graphicList.SelectedRank = this.document.Aggregates.Selected;
			this.graphicList.UpdateContents();

			this.paragraphList.List = this.document.TextStyles(StyleCategory.Paragraph);
			this.paragraphList.SelectedRank = this.document.GetSelectedTextStyle(StyleCategory.Paragraph);
			this.paragraphList.UpdateContents();
			
			this.characterList.List = this.document.TextStyles(StyleCategory.Character);
			this.characterList.SelectedRank = this.document.GetSelectedTextStyle(StyleCategory.Character);
			this.characterList.UpdateContents();
			
			this.UpdateAggregateName();
			this.UpdateAggregateChildren();
			this.UpdateToolBar();
			this.UpdateSelector();
			this.UpdatePanel();
			this.ShowSelection();
		}

		protected override void DoUpdateAggregates(System.Collections.ArrayList aggregateList)
		{
			//	Effectue la mise à jour des agrégats.
			foreach ( Properties.Aggregate agg in aggregateList )
			{
				int row = this.document.Aggregates.IndexOf(agg);
				if ( row != -1 )
				{
					this.graphicList.UpdateRow(row, false);
				}
			}
		}

		protected override void DoUpdateTextStyles(System.Collections.ArrayList textStyleList)
		{
			//	Effectue la mise à jour des styles de texte.
			foreach ( Text.TextStyle textStyle in textStyleList )
			{
				int row = this.document.TextContext.StyleList.StyleMap.GetRank(textStyle);
				if ( row != -1 )
				{
					if ( textStyle.TextStyleClass == Common.Text.TextStyleClass.Paragraph &&
						 row < this.paragraphList.List.Length )
					{
						this.paragraphList.UpdateRow(row, false);
					}

					if ( textStyle.TextStyleClass == Common.Text.TextStyleClass.Text &&
						 row < this.characterList.List.Length )
					{
						this.characterList.UpdateRow(row, false);
					}
				}
			}
		}

		protected override void DoUpdateProperties(System.Collections.ArrayList propertyList)
		{
			//	Effectue la mise à jour des propriétés.
			if ( this.panel != null )
			{
				if ( propertyList.Contains(panel.Property) )
				{
					this.panel.UpdateValues();
				}
			}
		}

		protected void UpdateCategory()
		{
			//	Met à jour la catégorie.
			this.graphicList.Visibility = (this.category == StyleCategory.Graphic);
			this.paragraphList.Visibility = (this.category == StyleCategory.Paragraph);
			this.characterList.Visibility = (this.category == StyleCategory.Character);

			this.buttonAggregateNew3.Visibility = (this.category == StyleCategory.Graphic);
			this.buttonAggregateNewAll.Visibility = (this.category == StyleCategory.Graphic);
			this.separatorStyle.Visibility = (this.category == StyleCategory.Graphic);
			this.buttonStyleNew.Visibility = (this.category == StyleCategory.Graphic);
			this.buttonStyleDelete.Visibility = (this.category == StyleCategory.Graphic);

			this.UpdateAggregateName();
			this.UpdateSelector();
			this.UpdateToolBar();
		}

		protected void UpdateToolBar()
		{
			//	Met à jour les boutons de la toolbar.
			if ( this.category == StyleCategory.Graphic )
			{
				int total = this.graphicList.Rows;
				int sel = this.document.Aggregates.Selected;

				this.buttonAggregateNewAll.Enable = (!this.document.Modifier.IsTool || this.document.Modifier.TotalSelected > 0);
				this.buttonAggregateUp.Enable = (sel != -1 && sel > 0);
				this.buttonAggregateDuplicate.Enable = (sel != -1);
				this.buttonAggregateDown.Enable = (sel != -1 && sel < total-1);
				this.buttonAggregateDelete.Enable = (sel != -1);

				Properties.Type type = Properties.Type.None;
				bool enableDelete = false;
				Properties.Aggregate agg = this.GetAggregate();
				if ( agg != null )
				{
					type = Properties.Abstract.TypeName(this.SelectorName);
					if ( type != Properties.Type.None )
					{
						if ( agg.Property(type) != null )
						{
							enableDelete = true;
						}
					}
				}
				this.buttonStyleNew.Enable = (sel != -1);
				this.buttonStyleDelete.Enable = enableDelete;
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int total = this.TextStyleList.Rows;
				int sel = this.document.GetSelectedTextStyle(this.category);

				bool enableDelete = false;
				if ( sel != -1 )
				{
					Common.Text.TextStyle style = this.TextStyleList.List[sel];
					enableDelete = !this.document.TextContext.StyleList.IsDefaultParagraphTextStyle(style) && !this.document.TextContext.StyleList.IsDefaultTextTextStyle(style);
				}

				this.buttonAggregateUp.Enable = (sel != -1 && sel > 1);
				this.buttonAggregateDuplicate.Enable = (sel != -1);
				this.buttonAggregateDown.Enable = (sel != -1 && sel != 0 && sel < total-1);
				this.buttonAggregateDelete.Enable = enableDelete;
			}
		}


		protected void UpdateSelector()
		{
			//	Met à jour le sélectionneur du panneau.
			foreach ( Widget widget in this.selectorToolBar.Children.Widgets )
			{
				widget.Dispose();  // supprime tous les boutons existants
			}

			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate agg = this.GetAggregate();
				if ( agg != null )
				{
					Properties.Type[] table = new Properties.Type[100];
					int total = 0;
					foreach ( Properties.Abstract property in agg.Styles )
					{
						int order = Properties.Abstract.SortOrder(property.Type);
						if ( table[order] == 0 )
						{
							table[order] = property.Type;
							total ++;
						}
					}

					//	Ne demande la largeur actuelle de selectorToolBar qu'une seule fois,
					//	lorsqu'elle est à jour et ne provoque pas de 'dirty layout' !
					if (this.selectorToolBarWidth == 0)
					{
						this.selectorToolBarWidth = this.selectorToolBar.ActualWidth;
					}
					double width = System.Math.Floor(this.selectorToolBarWidth/total);
					width = System.Math.Min(width, Styles.selectorSize);
					double zoom = width/Styles.selectorSize;

					for ( int i=0 ; i<100 ; i++ )
					{
						if ( table[i] != 0 )
						{
							string name = Properties.Abstract.TypeName(table[i]);
							string icon = Properties.Abstract.IconText(table[i]);
							string text = Properties.Abstract.Text(table[i]);

							IconButtonMark button = this.UpdateSelectorAdd(width, true, name, icon, text);
							button.InnerZoom = zoom;
						}
					}
				}
			}

			if ( this.category == StyleCategory.Paragraph )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel != -1 )
				{
					bool enable = (sel != 0);  // le premier style est forcément le style de base !

					if ( !enable && this.SelectorName == "Generator" )
					{
						this.SelectorName = null;  // ferme un éventuel panneau ouvert
					}

					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Justif",    "TextJustif",    Res.Strings.TextPanel.Justif.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Leading",   "TextLeading",   Res.Strings.TextPanel.Leading.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Margins",   "TextMargins",   Res.Strings.TextPanel.Margins.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Spaces",    "TextSpaces",    Res.Strings.TextPanel.Spaces.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Keep",      "TextKeep",      Res.Strings.TextPanel.Keep.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Tabs",      "TextTabs",      Res.Strings.TextPanel.Tabs.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Generator", "TextGenerator", Res.Strings.TextPanel.Generator.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Font",      "TextFont",      Res.Strings.TextPanel.Font.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Xline",     "TextXline",     Res.Strings.TextPanel.Xline.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true,   "Language",  "TextLanguage",  Res.Strings.TextPanel.Language.Title);
				}
			}

			if ( this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel != -1 )
				{
					bool enable = (sel != 0);  // le premier style est forcément le style de base !

					if ( !enable )
					{
						this.SelectorName = null;  // ferme un éventuel panneau ouvert
					}

					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Font",     "TextFont",     Res.Strings.TextPanel.Font.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Xline",    "TextXline",    Res.Strings.TextPanel.Xline.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Xscript",  "TextXscript",  Res.Strings.TextPanel.Xscript.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Language", "TextLanguage", Res.Strings.TextPanel.Language.Title);
				}
			}
		}

		protected IconButtonMark UpdateSelectorAdd(double width, bool enable, string name, string icon, string text)
		{
			IconButtonMark button = new IconButtonMark(this.selectorToolBar);
			button.Name = name;
			button.IconUri = Misc.Icon(icon);
			button.PreferredWidth = width;
			button.PreferredHeight = Styles.selectorSize+8;
			button.Enable = enable;
			button.AutoFocus = false;
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Dock = DockStyle.Left;
			button.ActiveState = (name == this.SelectorName) ? ActiveState.Yes : ActiveState.No;
			button.Clicked += this.HandleSelectorClicked;
			ToolTip.Default.SetToolTip(button, text);
			return button;
		}

		protected void UpdateAggregateName()
		{
			//	Met à jour le panneau pour éditer le nom de l'agrégat sélectionné.
			string text = "";

			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate agg = this.GetAggregate();
				if ( agg != null )
				{
					text = agg.AggregateName;
				}
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel != -1 )
				{
					Common.Text.TextStyle style = this.TextStyleList.List[sel];
					text = Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style));
				}
			}

			this.ignoreChanged = true;

			this.name.Text = text;
			this.name.SelectAll();

			if ( this.oneShootSelectName )  // vient-on de créer/dupliquer un style ?
			{
				this.oneShootSelectName = false;
				this.name.SelectAll();
				this.name.Focus();
			}

			this.ignoreChanged = false;
		}

		protected void UpdateAggregateChildren()
		{
			//	Met à jour le panneau pour éditer les enfants de l'agrégat sélectionné.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate agg = this.GetAggregate();

				if ( agg != null )
				{
					foreach ( Properties.Aggregate property in agg.Children )
					{
						if ( builder.Length != 0 )  builder.Append(", ");
						builder.Append(property.AggregateName);
					}
				}

				this.dummyChildren.Enable = true;
				this.nameChildren.Enable = true;
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);

				if ( sel != -1 )
				{
					Common.Text.TextStyle style = this.TextStyleList.List[sel];
					Text.TextStyle[] parents = style.ParentStyles;
					//?for ( int i=parents.Length-1 ; i>=0 ; i-- )
					for ( int i=0 ; i<parents.Length ; i++ )
					{
						Text.TextStyle parent = parents[i] as Text.TextStyle;
						if ( builder.Length != 0 )  builder.Append(", ");
						builder.Append(Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(parent)));
					}
				}

				this.dummyChildren.Enable = (sel != 0);
				this.nameChildren.Enable = (sel != 0);
			}

			int overflow = builder.Length-30;  // nb de caractères en "trop"
			if ( overflow > 0 )
			{
				double size = System.Math.Max((1-((double)overflow/(30*2)))*100, 60);  // zoom 60..100
				builder.Insert(0, string.Format("<font size=\"{0}%\">", size.ToString(System.Globalization.CultureInfo.InvariantCulture)));
				builder.Append("</font>");
			}

			this.nameChildren.Text = builder.ToString();
		}

		protected void UpdatePanel()
		{
			//	Met à jour le panneau pour éditer la propriété sélectionnée.
			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Abstract property = this.PropertyPanel();
				if ( property == null )
				{
					this.ClosePanel();

					int sel = this.document.Aggregates.Selected;
					if ( sel == -1 )  return;
					Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;

					string brief = agg.GetStyleBrief();

					this.styleBriefPanel = new StaticText(this.panelContainer);
					this.styleBriefPanel.ContentAlignment = ContentAlignment.TopLeft;
					this.styleBriefPanel.Dock = DockStyle.Fill;
					this.styleBriefPanel.Margins = new Margins(5, -1000, 5, 5);  // dépasse largement à droite
					this.styleBriefPanel.Text = brief;
					this.styleBriefPanel.PreferredHeight = this.styleBriefPanel.GetBestFitSize().Height;

					this.panelContainer.PreferredHeight = this.styleBriefPanel.PreferredHeight;
//-					this.panelContainer.ForceLayout();
					return;
				}

				if ( this.panel != null && this.panel.Property.Type == property.Type )
				{
					this.panel.Property = property;
					this.panelContainer.PreferredHeight = this.panel.DefaultHeight;
//-					this.panelContainer.ForceLayout();

					if ( this.colorSelector.Visibility )
					{
						this.ignoreChanged = true;
						this.colorSelector.Color = this.panel.OriginColorGet();
						this.ignoreChanged = false;
						this.panel.OriginColorSelect(this.panel.OriginColorRank());
					}

					return;
				}

				this.ClosePanel();

				this.panel = property.CreatePanel(this.document);
				if ( this.panel != null )
				{
					this.panel.Property = property;
					this.panel.IsExtendedSize = true;
					this.panel.IsLayoutDirect = true;
					this.panel.Changed += this.HandlePanelChanged;
					this.panel.OriginColorChanged += this.HandleOriginColorChanged;
					this.panel.SetParent(this.panelContainer);
					this.panel.Dock = DockStyle.Fill;
					this.panel.TabIndex = 1;
					this.panel.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

					this.panelContainer.PreferredHeight = this.panel.DefaultHeight;
//-					this.panelContainer.ForceLayout();
				}
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				this.ClosePanel();

				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return;
				Common.Text.TextStyle style = this.TextStyleList.List[sel];
				this.document.Wrappers.StyleTextWrapper.Attach(style, this.document.TextContext, this.document.Modifier.OpletQueue);
				this.document.Wrappers.StyleParagraphWrapper.Attach(style, this.document.TextContext, this.document.Modifier.OpletQueue);

				TextPanels.Abstract.StaticDocument = this.document;
				TextPanels.Abstract panel = TextPanels.Abstract.Create(this.SelectorName, this.document, true, this.category);
				if ( panel == null )
				{
					string brief;
					int lines;
					this.document.Wrappers.GetStyleBrief(style, out brief, out lines);
					double h = 5 + lines*14 + 5 + 1;

					this.styleBriefPanel = new StaticText(this.panelContainer);
					this.styleBriefPanel.ContentAlignment = ContentAlignment.TopLeft;
					this.styleBriefPanel.PreferredHeight = h;
					this.styleBriefPanel.Dock = DockStyle.Fill;
					this.styleBriefPanel.Margins = new Margins(5, -1000, 5, 5);  // dépasse largement à droite
					this.styleBriefPanel.Text = brief;

					this.panelContainer.PreferredHeight = h;
//-					this.panelContainer.ForceLayout();
				}
				else
				{
					this.textPanel = panel;
					this.textPanel.IsExtendedSize = true;
					this.textPanel.OriginColorChanged += this.HandleOriginColorChanged;
					this.textPanel.OriginColorClosed += this.HandleOriginColorClosed;
					this.textPanel.SetParent(this.panelContainer);
					this.textPanel.Dock = DockStyle.Fill;
					this.textPanel.TabIndex = 1;
					this.textPanel.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
					this.textPanel.UpdateAfterAttach();

					this.panelContainer.PreferredHeight = this.textPanel.DefaultHeight;
//-					this.panelContainer.ForceLayout();
				}
			}
		}

		protected Properties.Abstract PropertyPanel()
		{
			//	Cherche la propriété pour le panneau.
			Properties.Aggregate agg = this.GetAggregate();
			if ( agg == null )  return null;

			Properties.Type type = Properties.Abstract.TypeName(this.SelectorName);
			if ( type == Properties.Type.None )  return null;

			return agg.Property(type);
		}

		protected void ClosePanel()
		{
			//	Ferme le panneau pour la propriété et la roue des couleurs.
			this.colorSelector.Visibility = false;
			this.colorSelector.BackColor = Color.Empty;

			if ( this.panel != null )
			{
				this.panel.Changed -= this.HandlePanelChanged;
				this.panel.OriginColorChanged -= this.HandleOriginColorChanged;
				this.panel.Dispose();
				this.panel = null;
			}

			if ( this.textPanel != null )
			{
				this.document.Wrappers.StyleTextWrapper.Detach();
				this.document.Wrappers.StyleParagraphWrapper.Detach();

				this.textPanel.OriginColorChanged -= this.HandleOriginColorChanged;
				this.textPanel.OriginColorClosed -= this.HandleOriginColorClosed;
				this.textPanel.Dispose();
				this.textPanel = null;
			}

			if ( this.styleBriefPanel != null )
			{
				this.styleBriefPanel.Dispose();
				this.styleBriefPanel = null;
			}

			this.panelContainer.PreferredHeight = 0.0;
//-			this.panelContainer.ForceLayout();
		}

		protected void ShowSelection()
		{
			//	Montre la ligne sélectionnée dans la liste.
			if ( this.category == StyleCategory.Graphic )
			{
				this.graphicList.ShowSelect();
			}

			if ( this.category == StyleCategory.Paragraph )
			{
				this.paragraphList.ShowSelect();
			}

			if ( this.category == StyleCategory.Character )
			{
				this.characterList.ShowSelect();
			}
		}


		public void SetCategory(string name)
		{
			//	Choix d'une catégorie à partir d'une string, depuis le monde extérieur.
			if ( name == "Graphic"   )  this.Category = StyleCategory.Graphic;
			if ( name == "Paragraph" )  this.Category = StyleCategory.Paragraph;
			if ( name == "Character" )  this.Category = StyleCategory.Character;

			this.SetCategory(this.categoryGraphic,   Res.Strings.Panel.AggregateCategory.Graphic,   (this.category == StyleCategory.Graphic)  );
			this.SetCategory(this.categoryParagraph, Res.Strings.Panel.AggregateCategory.Paragraph, (this.category == StyleCategory.Paragraph));
			this.SetCategory(this.categoryCharacter, Res.Strings.Panel.AggregateCategory.Character, (this.category == StyleCategory.Character));
		}

		protected void SetCategory(IconButtonMark button, string text, bool state)
		{
			button.Text = text;
			button.ActiveState = state ? ActiveState.Yes : ActiveState.No;
		}

		protected StyleCategory Category
		{
			//	Catégorie sélectionnée (Graphic, Paragraph ou Character).
			get
			{
				return this.category;
			}

			set
			{
				if ( this.category != value )
				{
					this.category = value;
					this.UpdateCategory();
					this.UpdatePanel();
					this.UpdateAggregateChildren();
				}
			}
		}

		protected void StylesShiftRanks(int startRank)
		{
			//	Décale le rank de tous les styles plus grands ou égaux à startRank.
			Text.TextStyle[] styles = this.TextStyleList.List;
			foreach ( Text.TextStyle style in styles )
			{
				int rank = this.document.TextContext.StyleList.StyleMap.GetRank(style);
				if ( rank >= startRank )
				{
					this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style, rank+1);
				}
			}
		}

		private void HandleCategoryChanged(object sender, MessageEventArgs e)
		{
			Widget button = sender as Widget;
			this.SetCategory(button.Name);
		}

		private void HandleSelectorClicked(object sender, MessageEventArgs e)
		{
			this.SelectorName = null;
			foreach ( Widget widget in this.selectorToolBar.Children.Widgets )
			{
				if ( widget == sender )
				{
					if ( widget.ActiveState == ActiveState.Yes )
					{
						widget.ActiveState = ActiveState.No;
					}
					else
					{
						widget.ActiveState = ActiveState.Yes;
						this.SelectorName = widget.Name;
					}
				}
				else
				{
					widget.ActiveState = ActiveState.No;
				}
			}

			this.UpdateToolBar();
			this.UpdatePanel();
		}

		private void HandleButtonAggregateNewEmpty(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.oneShootSelectName = true;
				this.document.Modifier.AggregateNewEmpty(sel, "", true);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				Common.Text.TextStyleClass type = (this.category == StyleCategory.Paragraph) ? Common.Text.TextStyleClass.Paragraph : Common.Text.TextStyleClass.Text;

				System.Collections.ArrayList properties = new System.Collections.ArrayList();

				System.Collections.ArrayList parents = new System.Collections.ArrayList();
				if ( this.category == StyleCategory.Paragraph )
				{
					parents.Add(this.document.TextContext.DefaultParagraphStyle);
				}

				this.document.Modifier.OpletQueueBeginAction((this.category == StyleCategory.Paragraph) ? Res.Strings.Action.AggregateNewParagraph : Res.Strings.Action.AggregateNewCharacter);
				
				Text.TextStyle style = this.document.TextContext.StyleList.NewTextStyle(this.document.Modifier.OpletQueue, null, type, properties, parents);

				int rank = this.document.GetSelectedTextStyle(this.category)+1;
				this.StylesShiftRanks(rank);
				this.document.TextContext.StyleList.StyleMap.SetCaption(this.document.Modifier.OpletQueue, style, this.document.Modifier.GetNextTextStyleName(this.category));
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style, rank);
				this.document.SetSelectedTextStyle(this.category, rank);
				
				this.document.Modifier.OpletQueueValidateAction();
				this.document.Notifier.NotifyTextStyleListChanged();
				this.document.SetDirtySerialize(CacheBitmapChanging.All);

				this.oneShootSelectName = true;

				// plus nécessaire: c'est maintenant fait dans NotifyTextStyleListChanged();
				// this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateNew3(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.oneShootSelectName = true;
				this.document.Modifier.AggregateNew3(sel, "", true);
			}
		}

		private void HandleButtonAggregateNewAll(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.oneShootSelectName = true;
				this.document.Modifier.AggregateNewAll(sel, "", true);
			}
		}

		private void HandleButtonAggregateDuplicate(object sender, MessageEventArgs e)
		{
			//	Duplique un agrégat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.oneShootSelectName = true;
				this.document.Modifier.AggregateDuplicate(sel);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int rank = this.document.GetSelectedTextStyle(this.category);
				Common.Text.TextStyle initialStyle = this.TextStyleList.List[rank];
				string initialName = this.document.TextContext.StyleList.StyleMap.GetCaption(initialStyle);

				Common.Text.TextStyleClass type = initialStyle.TextStyleClass;
				Text.Property[] properties = initialStyle.StyleProperties;
				Text.TextStyle[] parents = initialStyle.ParentStyles;

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateDuplicate);
				this.document.TextContext.TabList.CloneTabs(properties);
				this.document.TextContext.GeneratorList.CloneGenerators(properties);
				
				Text.TextStyle style = this.document.TextContext.StyleList.NewTextStyle(this.document.Modifier.OpletQueue, null, type, properties, parents);
				this.document.TextContext.StyleList.SetNextStyle(this.document.Modifier.OpletQueue, style, initialStyle.NextStyle);

				rank ++;
				this.StylesShiftRanks(rank);
				this.document.TextContext.StyleList.StyleMap.SetCaption(this.document.Modifier.OpletQueue, style, Misc.CopyName(initialName));
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style, rank);
				this.document.SetSelectedTextStyle(this.category, rank);
				
				this.document.Modifier.OpletQueueValidateAction();
				this.document.Notifier.NotifyTextStyleListChanged();
				this.document.SetDirtySerialize(CacheBitmapChanging.All);

				this.oneShootSelectName = true;
				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateUp(object sender, MessageEventArgs e)
		{
			//	Monte d'une ligne l'agrégat sélectionné.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				this.document.Modifier.AggregateSwap(sel, sel-1);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);

				Common.Text.TextStyle style1 = this.TextStyleList.List[sel];
				Common.Text.TextStyle style2 = this.TextStyleList.List[sel-1];

				int rank1 = this.document.TextContext.StyleList.StyleMap.GetRank(style1);
				int rank2 = this.document.TextContext.StyleList.StyleMap.GetRank(style2);

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateUp);
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style1, rank2);
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style2, rank1);
				this.document.Wrappers.StyleArrangeAll();

				this.document.SetSelectedTextStyle(this.category, sel-1);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.Notifier.NotifyTextStyleListChanged();
				this.document.SetDirtySerialize(CacheBitmapChanging.All);
				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateDown(object sender, MessageEventArgs e)
		{
			//	Descend d'une ligne l'agrégat sélectionné.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				this.document.Modifier.AggregateSwap(sel, sel+1);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);

				Common.Text.TextStyle style1 = this.TextStyleList.List[sel];
				Common.Text.TextStyle style2 = this.TextStyleList.List[sel+1];

				int rank1 = this.document.TextContext.StyleList.StyleMap.GetRank(style1);
				int rank2 = this.document.TextContext.StyleList.StyleMap.GetRank(style2);

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateDown);
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style1, rank2);
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style2, rank1);
				this.document.Wrappers.StyleArrangeAll();

				this.document.SetSelectedTextStyle(this.category, sel+1);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.Notifier.NotifyTextStyleListChanged();
				this.document.SetDirtySerialize(CacheBitmapChanging.All);
				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateDelete(object sender, MessageEventArgs e)
		{
			//	Supprime l'agrégat sélectionné.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				this.document.Modifier.AggregateDelete(sel);
			}
			
			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);

				Common.Text.TextStyle style = this.TextStyleList.List[sel];

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateDelete);
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style, -1);
				this.document.TextContext.StyleList.StyleMap.SetCaption(this.document.Modifier.OpletQueue, style, null);
				this.document.TextContext.StyleList.DeleteTextStyle(this.document.Modifier.OpletQueue, style);
				this.document.Wrappers.StyleCheckAllDefaultParent();
				
				if ( sel >= this.TextStyleList.List.Length )
				{
					sel = this.TextStyleList.List.Length-1;
				}
				this.document.SetSelectedTextStyle(this.category, sel);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.Notifier.NotifyTextStyleListChanged();
				this.document.SetDirtySerialize(CacheBitmapChanging.All);
				this.SetDirtyContent();
			}
		}

		private void HandleAggregatesTableSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);

			if ( this.document.Aggregates.Selected != this.graphicList.SelectedRow )
			{
				this.document.Modifier.OpletQueueEnable = false;
				this.document.Aggregates.Selected = this.graphicList.SelectedRow;
				this.document.Modifier.OpletQueueEnable = true;
			}

			Properties.Aggregate agg = this.GetAggregate();
			if ( agg != null )
			{
				Properties.Type type = Properties.Abstract.TypeName(this.SelectorName);
				Properties.Abstract property = agg.Property(type);
				this.document.Modifier.OpletQueueEnable = false;
				agg.Styles.Selected = agg.Styles.IndexOf(property);
				this.document.Modifier.OpletQueueEnable = true;
			}

			this.UpdateToolBar();
			this.UpdateSelector();
			this.UpdatePanel();
			this.UpdateAggregateName();
			this.UpdateAggregateChildren();
			this.ShowSelection();
		}

		private void HandleAggregatesTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Liste double-cliquée.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleAggregatesTableFlyOverChanged(object sender)
		{
			//	La cellule survolée a changé.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			int rank = this.graphicList.FlyOverRow;

			Properties.Aggregate agg = null;
			if ( rank != -1 )
			{
				agg = this.document.Aggregates[rank] as Properties.Aggregate;
			}

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Objects.Abstract layer = context.RootObject();
			foreach ( Objects.Abstract obj in this.document.Deep(layer) )
			{
				obj.IsHilite = (agg != null && obj.Aggregates.Contains(agg));
			}

			this.graphicList.HiliteColor = context.HiliteSurfaceColor;
			int total = this.document.Aggregates.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				this.graphicList.HiliteRow(i, i==rank);
			}
		}

		private void HandleStylesTableSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste.
			System.Diagnostics.Debug.Assert(this.category != StyleCategory.Graphic);
			this.document.SetSelectedTextStyle(this.category, this.TextStyleList.SelectedRow);

			this.UpdateToolBar();
			this.UpdateSelector();
			this.UpdatePanel();
			this.UpdateAggregateName();
			this.UpdateAggregateChildren();
			this.ShowSelection();
		}

		private void HandleStylesTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Liste double-cliquée.
			System.Diagnostics.Debug.Assert(this.category != StyleCategory.Graphic);
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleMenuChildrenOpenPressed(object sender)
		{
			//	Crée un nouveau parent.
			Widget widget = this.dummyChildren;
			VMenu menu = this.CreateMenuChildren();
			if ( menu == null )  return;
			menu.Host = this;
			menu.MinWidth = widget.ActualWidth;
			TextFieldCombo.AdjustComboSize(widget, menu, false);
			menu.ShowAsComboList(widget, Point.Zero, this.dummyChildren.Button);
		}

		private void HandleButtonStyleNew(object sender, MessageEventArgs e)
		{
			//	Crée une nouvelle propriété.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			IconButton button = sender as IconButton;
			VMenu menu = this.CreateMenuTypes();
			menu.Host = this;
			menu.MinWidth = button.ActualWidth;
			TextFieldCombo.AdjustComboSize(button, menu, false);
			menu.ShowAsComboList(button, Point.Zero, button);
		}

		private void HandleButtonStyleDelete(object sender, MessageEventArgs e)
		{
			//	Supprime la propriété sélectionnée.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateStyleDelete(agg, Properties.Abstract.TypeName(this.SelectorName));
			this.UpdateSelector();
			this.UpdatePanel();
		}

		private void HandleNameTextChanged(object sender)
		{
			//	Le nom de l'agrégat a changé.
			if ( this.ignoreChanged )  return;

			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  return;

				Properties.Aggregate agg = this.document.Aggregates[sel] as Properties.Aggregate;
				string name = this.name.Text;
				int attempt = 0;
				while ( !this.document.Modifier.AggregateIsFreeName(agg, name) )
				{
					attempt ++;
					name = string.Format("{0} ({1})", this.name.Text, attempt);
				}

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
				agg.AggregateName = name;
				this.document.Modifier.OpletQueueValidateAction();
				this.document.SetDirtySerialize(CacheBitmapChanging.None);
				this.document.Notifier.NotifyAggregateChanged(agg);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return;

				Common.Text.TextStyle style = this.TextStyleList.List[sel];
				string name = this.name.Text;
				if ( !Misc.IsTextStyleName(name) )
				{
					name = ((this.category == StyleCategory.Paragraph) ? "P." : "C.") + name;
				}
				int attempt = 0;
				while ( !this.document.Wrappers.IsFreeName(style, name) )
				{
					attempt ++;
					name = string.Format("{0} ({1})", this.name.Text, attempt);
				}
				
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
				this.document.TextContext.StyleList.StyleMap.SetCaption(this.document.Modifier.OpletQueue, style, name);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.SetDirtySerialize(CacheBitmapChanging.None);
				this.document.Notifier.NotifyTextStyleChanged(style);
			}
		}

		private void HandlePanelChanged(object sender)
		{
			//	Le contenu du panneau a changé.
			int sel = this.graphicList.SelectedRow;
			if ( sel != -1 )
			{
				this.graphicList.UpdateRow(sel, true);

				double h = this.panel.DefaultHeight;
				if ( h != this.panelContainer.ActualHeight )
				{
					this.panel.PreferredHeight = h;
					this.panelContainer.PreferredHeight = h;
//-					this.panelContainer.ForceLayout();
				}
			}
		}

		private void HandleOriginColorChanged(object sender)
		{
			//	Le widget qui détermine la couleur d'origine a changé.
			this.colorSelector.Visibility = true;

			if ( this.panel != null )
			{
				this.ignoreChanged = true;
				this.colorSelector.Color = this.panel.OriginColorGet();
				this.ignoreChanged = false;
				this.panel.OriginColorSelect(this.panel.OriginColorRank());
			}

			if ( this.textPanel != null )
			{
				this.ignoreChanged = true;
				this.colorSelector.Color = this.textPanel.OriginColorGet();
				this.ignoreChanged = false;
				this.textPanel.OriginColorSelect(this.textPanel.OriginColorRank());
			}
		}

		private void HandleOriginColorClosed(object sender)
		{
			//	Le widget qui détermine la couleur d'origine doit être fermé.
			this.HandleColorSelectorClosed(sender);
		}

		private void HandleColorSelectorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			//	Couleur changée dans la roue.
			if ( this.ignoreChanged )  return;

			if ( this.panel != null )
			{
				this.panel.OriginColorChange(this.colorSelector.Color);
			}

			if ( this.textPanel != null )
			{
				this.textPanel.OriginColorChange(this.colorSelector.Color);
			}
		}

		private void HandleColorSelectorClosed(object sender)
		{
			//	Fermer la roue.
			if ( this.panel != null )
			{
				this.panel.OriginColorDeselect();
			}

			if ( this.textPanel != null )
			{
				this.textPanel.OriginColorDeselect();
			}

			this.colorSelector.Visibility = false;
			this.colorSelector.BackColor = Color.Empty;
		}


		#region MenuTypes
		protected VMenu CreateMenuTypes()
		{
			//	Construit le menu pour choisir le style.
			Properties.Aggregate agg = this.GetAggregate();
			VMenu menu = new VMenu();
			double back = -1;
			for ( int i=0 ; i<100 ; i++ )
			{
				Properties.Type type = Properties.Abstract.SortOrder(i);
				if ( !Properties.Abstract.StyleAbility(type) )  continue;

				if ( back != -1 && back != Properties.Abstract.BackgroundIntensity(type) )
				{
					menu.Items.Add(new MenuSeparator());
				}
				back = Properties.Abstract.BackgroundIntensity(type);

				bool enable = (!this.MenuTypesExist(agg.Styles, type));
				string icon = Misc.Image(Properties.Abstract.IconText(type));
				string text = Properties.Abstract.Text(type);
				string line = string.Format("{0}   {1}",icon, text);
				MenuItem item = new MenuItem("", "", line, "", Properties.Abstract.TypeName(type));
				item.Enable = enable;
				item.Pressed += this.HandleMenuTypesPressed;
				menu.Items.Add(item);
			}
			menu.AdjustSize();
			return menu;
		}

		protected bool MenuTypesExist(UndoableList styles, Properties.Type type)
		{
			foreach ( Properties.Abstract property in styles )
			{
				if ( property.Type == type )  return true;
			}
			return false;
		}

		private void HandleMenuTypesPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			Properties.Aggregate agg = this.GetAggregate();
			Properties.Type type = Properties.Abstract.TypeName(item.Name);
			this.document.Modifier.AggregateStyleNew(agg, type);
			this.SelectorName = Properties.Abstract.TypeName(type);
			this.UpdateSelector();
			this.UpdatePanel();
		}
		#endregion

		
		#region MenuChildren
		protected VMenu CreateMenuChildren()
		{
			//	Construit le menu pour choisir un enfant à ajouter.
			VMenu menu = new VMenu();
			int used = 0;

			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate currentAgg = this.GetAggregate();
				for ( int i=0 ; i<this.document.Aggregates.Count ; i++ )
				{
					Properties.Aggregate agg = this.document.Aggregates[i] as Properties.Aggregate;
					if ( agg == currentAgg )  continue;

					bool active = currentAgg.Children.Contains(agg);
					string icon = Misc.Icon(active ? "ActiveYes" : "ActiveNo");
					string line = agg.AggregateName;
					MenuItem item = new MenuItem("ChildrenNew", icon, line, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
					item.Pressed += this.HandleMenuChildrenPressed;
					menu.Items.Add(item);
					used ++;
				}
			}

			if ( this.category == StyleCategory.Paragraph )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return null;

				Text.TextStyle[] styles = this.TextStyleList.List;
				Text.TextStyle currentStyle = styles[sel];

				//	Met les styles de paragraphe, avec des icônes "radio".
				for ( int i=0 ; i<styles.Length ; i++ )
				{
					Text.TextStyle style = styles[i];
					if ( style == currentStyle )  continue;
					if ( this.document.Wrappers.IsStyleAsCircularRef(currentStyle, style) )  continue;

					bool active = Styles.ContainsStyle(currentStyle.ParentStyles, style);
					string icon = Misc.Icon(active ? "RadioYes" : "RadioNo");
					string line = Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style));
					MenuItem item = new MenuItem("ChildrenNew", icon, line, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
					item.Pressed += this.HandleMenuChildrenPressed;
					menu.Items.Add(item);
					used ++;
				}

				//	Met les styles de caractère, avec des icônes "check".
				bool firstCharacter = true;
				styles = this.characterList.List;
				for ( int i=1 ; i<styles.Length ; i++ )  // saute le style de base (toujours le premier)
				{
					Text.TextStyle style = styles[i];
					if ( style == currentStyle )  continue;

					if ( firstCharacter )
					{
						firstCharacter = false;
						menu.Items.Add(new MenuSeparator());
					}

					bool active = Styles.ContainsStyle(currentStyle.ParentStyles, style);
					string icon = Misc.Icon(active ? "ActiveYes" : "ActiveNo");
					string line = Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style));
					MenuItem item = new MenuItem("ChildrenNew", icon, line, "", (i+10000).ToString(System.Globalization.CultureInfo.InvariantCulture));
					item.Pressed += this.HandleMenuChildrenPressed;
					menu.Items.Add(item);
					used ++;
				}
			}

			if ( this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return null;

				Text.TextStyle[] styles = this.TextStyleList.List;
				Text.TextStyle currentStyle = styles[sel];

				for ( int i=1 ; i<styles.Length ; i++ )  // saute le style de base (toujours le premier)
				{
					Text.TextStyle style = styles[i];
					if ( currentStyle == style )  continue;
					if ( this.document.Wrappers.IsStyleAsCircularRef(currentStyle, style) )  continue;

					bool active = Styles.ContainsStyle(currentStyle.ParentStyles, style);
					string icon = Misc.Icon(active ? "ActiveYes" : "ActiveNo");
					string line = Misc.UserTextStyleName(this.document.TextContext.StyleList.StyleMap.GetCaption(style));
					MenuItem item = new MenuItem("ChildrenNew", icon, line, "", (i+10000).ToString(System.Globalization.CultureInfo.InvariantCulture));
					item.Pressed += this.HandleMenuChildrenPressed;
					menu.Items.Add(item);
					used ++;
				}
			}

			if ( used == 0 )  return null;
			menu.AdjustSize();
			return menu;
		}

		protected static bool ContainsStyle(Text.TextStyle[] styles, Text.TextStyle search)
		{
			foreach ( Text.TextStyle style in styles )
			{
				if ( style == search )  return true;
			}
			return false;
		}

		private void HandleMenuChildrenPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			int i = System.Int32.Parse(item.Name, System.Globalization.CultureInfo.InvariantCulture);

			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate menuAgg = this.document.Aggregates[i] as Properties.Aggregate;
				Properties.Aggregate currentAgg = this.GetAggregate();
				if ( currentAgg.Children.Contains(menuAgg) )
				{
					this.document.Modifier.AggregateChildrenDelete(currentAgg, menuAgg);
				}
				else
				{
					this.document.Modifier.AggregateChildrenNew(currentAgg, menuAgg);
				}
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				Text.TextStyle[] styles = this.TextStyleList.List;
				Text.TextStyle currentStyle = styles[sel];

				Text.TextStyle newStyle = null;
				if ( i < 10000 )
				{
					newStyle = this.paragraphList.List[i];
				}
				else
				{
					newStyle = this.characterList.List[i-10000];
				}

				System.Collections.ArrayList parents = new System.Collections.ArrayList();
				parents.AddRange(currentStyle.ParentStyles);

				if ( this.category == StyleCategory.Paragraph && newStyle.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
				{
					int j = 0;
					while ( j < parents.Count )
					{
						Text.TextStyle parent = parents[j] as Text.TextStyle;
						if ( parent.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
						{
							parents.RemoveAt(j);
						}
						else
						{
							j ++;
						}
					}
				}

				string op;
				if ( parents.Contains(newStyle) )
				{
					op = Res.Strings.Action.AggregateChildrenDelete;
					parents.Remove(newStyle);
				}
				else
				{
					op = Res.Strings.Action.AggregateChildrenNew;
					parents.Add(newStyle);
				}

				parents = this.document.Wrappers.ArrangeParentStyles(parents);

				this.document.Modifier.OpletQueueBeginAction(op);
				this.document.TextContext.StyleList.RedefineTextStyle(this.document.Modifier.OpletQueue, currentStyle, currentStyle.StyleProperties, parents);
				this.document.Modifier.OpletQueueValidateAction();

				this.document.SetDirtySerialize(CacheBitmapChanging.All);
				this.SetDirtyContent();
			}
		}
		#endregion

		
		protected Properties.Aggregate GetAggregate()
		{
			//	Donne l'agrégat sélectionné.
			int sel = this.document.Aggregates.Selected;

			if ( sel == -1 )  return null;
			if ( sel >= this.document.Aggregates.Count )  return null;

			return this.document.Aggregates[sel] as Properties.Aggregate;
		}

		protected Widgets.TextStylesList TextStyleList
		{
			//	Donne le widget pour la liste des styles selon la catégorie actuelle.
			get
			{
				if ( this.category == StyleCategory.Paragraph )  return this.paragraphList;
				if ( this.category == StyleCategory.Character )  return this.characterList;
				throw new System.ArgumentException("TextStyleList(" + this.category.ToString() + ")");
			}
		}

		protected string SelectorName
		{
			//	Nom du panneau sélectionné selon la catégorie actuelle.
			get
			{
				int i = (int) this.category;
				System.Diagnostics.Debug.Assert(i >= 0 && i < this.selectorName.Length);
				return this.selectorName[i];
			}

			set
			{
				int i = (int) this.category;
				System.Diagnostics.Debug.Assert(i >= 0 && i < this.selectorName.Length);
				this.selectorName[i] = value;
			}
		}


		protected static readonly double	selectorSize = 20;

		protected StaticText				helpText;

		protected PaneBook					mainBook;
		protected PanePage					topPage;
		protected PanePage					bottomPage;
		protected Scrollable				bottomScrollable;

		protected Widget					categoryContainer;
		protected IconButtonMark			categoryGraphic;
		protected IconButtonMark			categoryParagraph;
		protected IconButtonMark			categoryCharacter;
		protected StyleCategory				category;

		protected HToolBar					aggregateToolBar;
		protected IconButton				buttonAggregateNewEmpty;
		protected IconButton				buttonAggregateNew3;
		protected IconButton				buttonAggregateNewAll;
		protected IconButton				buttonAggregateDuplicate;
		protected IconButton				buttonAggregateUp;
		protected IconButton				buttonAggregateDown;
		protected IconButton				buttonAggregateDelete;

		protected Widgets.AggregateList		graphicList;
		protected Widgets.TextStylesList	paragraphList;
		protected Widgets.TextStylesList	characterList;

		protected HToolBar					nameToolBar;
		protected TextField					name;

		protected HToolBar					childrenToolBar;
		protected Widgets.DummyTextFieldCombo dummyChildren;
		protected StaticText				nameChildren;

		protected Widget					selectorContainer;
		protected Widget					selectorToolBar;
		protected double					selectorToolBarWidth = 0;
		protected string[]					selectorName = new string[(int) StyleCategory.Count];
		protected IconSeparator				separatorStyle;
		protected IconButton				buttonStyleNew;
		protected IconButton				buttonStyleDelete;

		protected Widget					panelContainer;
		protected Panels.Abstract			panel;
		protected TextPanels.Abstract		textPanel;
		protected StaticText				styleBriefPanel;
		protected ColorSelector				colorSelector;

		protected int						index;
		protected bool						isChildrenExtended = false;
		protected bool						oneShootSelectName = false;
		protected bool						ignoreChanged = false;
	}
}
