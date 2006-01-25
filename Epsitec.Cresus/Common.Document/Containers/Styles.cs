using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Containers
{
	/// <summary>
	/// La classe Containers.Styles contient tous les panneaux des styles.
	/// </summary>
	[SuppressBundleSupport]
	public class Styles : Abstract
	{
		public Styles(Document document) : base(document)
		{
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
			this.graphicList.DockMargins = new Margins(0, 0, 0, 0);
			this.graphicList.FinalSelectionChanged += new EventHandler(this.HandleAggregatesTableSelectionChanged);
			this.graphicList.FlyOverChanged += new EventHandler(this.HandleAggregatesTableFlyOverChanged);
			this.graphicList.DoubleClicked += new MessageEventHandler(this.HandleAggregatesTableDoubleClicked);
			this.graphicList.TabIndex = 2;
			this.graphicList.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Table des styles de paragraphe.
			this.paragraphList = new Widgets.TextStylesList();
			this.paragraphList.Document = this.document;
			this.paragraphList.Category = StyleCategory.Paragraph;
			this.paragraphList.HScroller = true;
			this.paragraphList.VScroller = true;
			this.paragraphList.SetParent(this.topPage);
			this.paragraphList.MinSize = new Size(10, 87);
			this.paragraphList.Dock = DockStyle.Fill;
			this.paragraphList.DockMargins = new Margins(0, 0, 0, 0);
			this.paragraphList.FinalSelectionChanged += new EventHandler(this.HandleStylesTableSelectionChanged);
			this.paragraphList.DoubleClicked += new MessageEventHandler(this.HandleStylesTableDoubleClicked);
			this.paragraphList.TabIndex = 2;
			this.paragraphList.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Table des styles de caractère.
			this.characterList = new Widgets.TextStylesList();
			this.characterList.Document = this.document;
			this.characterList.Category = StyleCategory.Character;
			this.characterList.HScroller = true;
			this.characterList.VScroller = true;
			this.characterList.SetParent(this.topPage);
			this.characterList.MinSize = new Size(10, 87);
			this.characterList.Dock = DockStyle.Fill;
			this.characterList.DockMargins = new Margins(0, 0, 0, 0);
			this.characterList.FinalSelectionChanged += new EventHandler(this.HandleStylesTableSelectionChanged);
			this.characterList.DoubleClicked += new MessageEventHandler(this.HandleStylesTableDoubleClicked);
			this.characterList.TabIndex = 2;
			this.characterList.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.CreateNameToolBar();
			this.CreateChildrensToolBar();

			//	Enfants de l'agrégat.
			this.childrensGraphicList = new Widgets.AggregateList();
			this.childrensGraphicList.Document = this.document;
			this.childrensGraphicList.HScroller = true;
			this.childrensGraphicList.VScroller = true;
			this.childrensGraphicList.IsHiliteColumn = false;
			this.childrensGraphicList.IsOrderColumn = true;
			this.childrensGraphicList.IsChildrensColumn = false;
			this.childrensGraphicList.SetParent(this.bottomPage);
			this.childrensGraphicList.Height = 103;
			this.childrensGraphicList.Dock = DockStyle.Top;
			this.childrensGraphicList.DockMargins = new Margins(0, 0, 0, 0);
			this.childrensGraphicList.FinalSelectionChanged += new EventHandler(this.HandleAggregatesChildrensSelectionChanged);
			this.childrensGraphicList.TabIndex = 96;
			this.childrensGraphicList.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.childrensParagraphList = new Widgets.TextStylesList();
			this.childrensParagraphList.Document = this.document;
			this.childrensParagraphList.Category = StyleCategory.Paragraph;
			this.childrensParagraphList.HScroller = true;
			this.childrensParagraphList.VScroller = true;
			this.childrensParagraphList.IsHiliteColumn = false;
			this.childrensParagraphList.IsOrderColumn = true;
			this.childrensParagraphList.IsChildrensColumn = false;
			this.childrensParagraphList.SetParent(this.bottomPage);
			this.childrensParagraphList.Height = 103;
			this.childrensParagraphList.Dock = DockStyle.Top;
			this.childrensParagraphList.DockMargins = new Margins(0, 0, 0, 0);
			this.childrensParagraphList.FinalSelectionChanged += new EventHandler(this.HandleAggregatesChildrensSelectionChanged);
			this.childrensParagraphList.TabIndex = 96;
			this.childrensParagraphList.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			this.childrensCharacterList = new Widgets.TextStylesList();
			this.childrensCharacterList.Document = this.document;
			this.childrensCharacterList.Category = StyleCategory.Paragraph;
			this.childrensCharacterList.HScroller = true;
			this.childrensCharacterList.VScroller = true;
			this.childrensCharacterList.IsHiliteColumn = false;
			this.childrensCharacterList.IsOrderColumn = true;
			this.childrensCharacterList.IsChildrensColumn = false;
			this.childrensCharacterList.SetParent(this.bottomPage);
			this.childrensCharacterList.Height = 103;
			this.childrensCharacterList.Dock = DockStyle.Top;
			this.childrensCharacterList.DockMargins = new Margins(0, 0, 0, 0);
			this.childrensCharacterList.FinalSelectionChanged += new EventHandler(this.HandleAggregatesChildrensSelectionChanged);
			this.childrensCharacterList.TabIndex = 96;
			this.childrensCharacterList.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	Sélectionneur.
			this.CreateSelectorToolBar();

			this.bottomScrollable = new Scrollable();
			this.bottomScrollable.Dock = DockStyle.Fill;
			this.bottomScrollable.HorizontalScrollerMode = ScrollableScrollerMode.HideAlways;
			this.bottomScrollable.VerticalScrollerMode = ScrollableScrollerMode.ShowAlways;
			this.bottomScrollable.Panel.IsAutoFitting = true;
			this.bottomScrollable.IsForegroundFrame = true;
			this.bottomScrollable.ForegroundFrameMargins = new Margins(0, 1, 0, 0);
			this.bottomScrollable.SetParent(this.bottomPage);

			//	Conteneur du panneau.
			this.panelContainer = new Widget(this.bottomScrollable.Panel);
			this.panelContainer.Height = 0.0;
			this.panelContainer.Dock = DockStyle.Top;
			this.panelContainer.DockMargins = new Margins(0, 1, 0, 0);
			this.panelContainer.TabIndex = 99;
			this.panelContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			//	Roue des couleurs.
			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.SetParent(this.bottomScrollable.Panel);
			this.colorSelector.Dock = DockStyle.Top;
			this.colorSelector.DockMargins = new Margins(1, 3, 5, 2);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.colorSelector.Visibility = false;

			this.UpdateChildrensExtend();

			this.category = StyleCategory.Graphic;
			this.UpdateCategory();

			this.panelContainer.Height = 1;  // nécessaire pour mettre à jour la première fois !
			this.panelContainer.ForceLayout();
		}

		protected void CreateCategoryGroup()
		{
			//	Crée les boutons radio pour le choix de la catégorie.
			this.categoryContainer = new Widget(this.topPage);
			this.categoryContainer.Height = 20;
			this.categoryContainer.Dock = DockStyle.Top;
			this.categoryContainer.DockMargins = new Margins(0, 0, 0, 5);
			this.categoryContainer.TabIndex = 1;
			this.categoryContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.index = 0;

			this.categoryGraphic = new Button(this.categoryContainer);
			this.categoryGraphic.Name = "Graphic";
			this.categoryGraphic.ButtonStyle = ButtonStyle.ActivableIcon;
			this.categoryGraphic.AutoFocus = false;
			this.categoryGraphic.ActiveState = ActiveState.Yes;
			this.categoryGraphic.Text = Res.Strings.Panel.AggregateCategory.Graphic;
			this.categoryGraphic.Width = 80;
			this.categoryGraphic.Dock = DockStyle.Left;
			this.categoryGraphic.DockMargins = new Margins(0, 0, 0, 0);
			this.categoryGraphic.TabIndex = this.index++;
			this.categoryGraphic.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.categoryGraphic.Clicked += new MessageEventHandler(this.HandleCategoryChanged);

			this.categoryParagraph = new Button(this.categoryContainer);
			this.categoryParagraph.Name = "Paragraph";
			this.categoryParagraph.ButtonStyle = ButtonStyle.ActivableIcon;
			this.categoryParagraph.AutoFocus = false;
			this.categoryParagraph.Text = Res.Strings.Panel.AggregateCategory.Paragraph;
			this.categoryParagraph.Width = 80;
			this.categoryParagraph.Dock = DockStyle.Left;
			this.categoryParagraph.DockMargins = new Margins(0, 0, 0, 0);
			this.categoryParagraph.TabIndex = this.index++;
			this.categoryParagraph.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.categoryParagraph.Clicked += new MessageEventHandler(this.HandleCategoryChanged);

			this.categoryCharacter = new Button(this.categoryContainer);
			this.categoryCharacter.Name = "Character";
			this.categoryCharacter.ButtonStyle = ButtonStyle.ActivableIcon;
			this.categoryCharacter.AutoFocus = false;
			this.categoryCharacter.Text = Res.Strings.Panel.AggregateCategory.Character;
			this.categoryCharacter.Width = 80-1;
			this.categoryCharacter.Dock = DockStyle.Left;
			this.categoryCharacter.DockMargins = new Margins(0, 0, 0, 0);
			this.categoryCharacter.TabIndex = this.index++;
			this.categoryCharacter.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.categoryCharacter.Clicked += new MessageEventHandler(this.HandleCategoryChanged);
		}

		protected void CreateAggregateToolBar()
		{
			//	Crée la toolbar principale.
			this.aggregateToolBar = new HToolBar(this.topPage);
			this.aggregateToolBar.Dock = DockStyle.Top;
			this.aggregateToolBar.DockMargins = new Margins(0, 0, 0, -1);
			this.aggregateToolBar.TabIndex = 1;
			this.aggregateToolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.index = 0;

			this.buttonAggregateNewEmpty = new IconButton(Misc.Icon("AggregateNewEmpty"));
			this.buttonAggregateNewEmpty.Clicked += new MessageEventHandler(this.HandleButtonAggregateNewEmpty);
			this.buttonAggregateNewEmpty.TabIndex = this.index++;
			this.buttonAggregateNewEmpty.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateNewEmpty);
			ToolTip.Default.SetToolTip(this.buttonAggregateNewEmpty, Res.Strings.Action.AggregateNewEmpty);

			this.buttonAggregateNew3 = new IconButton(Misc.Icon("AggregateNew3"));
			this.buttonAggregateNew3.Clicked += new MessageEventHandler(this.HandleButtonAggregateNew3);
			this.buttonAggregateNew3.TabIndex = this.index++;
			this.buttonAggregateNew3.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateNew3);
			ToolTip.Default.SetToolTip(this.buttonAggregateNew3, Res.Strings.Action.AggregateNew3);

			this.buttonAggregateNewAll = new IconButton(Misc.Icon("AggregateNewAll"));
			this.buttonAggregateNewAll.Clicked += new MessageEventHandler(this.HandleButtonAggregateNewAll);
			this.buttonAggregateNewAll.TabIndex = this.index++;
			this.buttonAggregateNewAll.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateNewAll);
			ToolTip.Default.SetToolTip(this.buttonAggregateNewAll, Res.Strings.Action.AggregateNewAll);

			this.buttonAggregateDuplicate = new IconButton(Misc.Icon("AggregateDuplicate"));
			this.buttonAggregateDuplicate.Clicked += new MessageEventHandler(this.HandleButtonAggregateDuplicate);
			this.buttonAggregateDuplicate.TabIndex = this.index++;
			this.buttonAggregateDuplicate.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateDuplicate);
			ToolTip.Default.SetToolTip(this.buttonAggregateDuplicate, Res.Strings.Action.AggregateDuplicate);

			this.aggregateToolBar.Items.Add(new IconSeparator());

			this.buttonAggregateUp = new IconButton(Misc.Icon("AggregateUp"));
			this.buttonAggregateUp.Clicked += new MessageEventHandler(this.HandleButtonAggregateUp);
			this.buttonAggregateUp.TabIndex = this.index++;
			this.buttonAggregateUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateUp);
			ToolTip.Default.SetToolTip(this.buttonAggregateUp, Res.Strings.Action.AggregateUp);

			this.buttonAggregateDown = new IconButton(Misc.Icon("AggregateDown"));
			this.buttonAggregateDown.Clicked += new MessageEventHandler(this.HandleButtonAggregateDown);
			this.buttonAggregateDown.TabIndex = this.index++;
			this.buttonAggregateDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateDown);
			ToolTip.Default.SetToolTip(this.buttonAggregateDown, Res.Strings.Action.AggregateDown);

			this.aggregateToolBar.Items.Add(new IconSeparator());

			this.buttonAggregateDelete = new IconButton(Misc.Icon("AggregateDelete"));
			this.buttonAggregateDelete.Clicked += new MessageEventHandler(this.HandleButtonAggregateDelete);
			this.buttonAggregateDelete.TabIndex = this.index++;
			this.buttonAggregateDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonAggregateDelete);
			ToolTip.Default.SetToolTip(this.buttonAggregateDelete, Res.Strings.Action.AggregateDelete);
		}

		protected void CreateChildrensToolBar()
		{
			//	Crée la toolbar pour le choix des enfants.
			this.childrensToolBar = new HToolBar(this.bottomPage);
			this.childrensToolBar.Dock = DockStyle.Top;
			this.childrensToolBar.DockMargins = new Margins(0, 0, 0, 0);
			this.childrensToolBar.TabIndex = 95;
			this.childrensToolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.AggregateChildrens.Label.Name;
			this.childrensToolBar.Items.Add(st);

			this.index = 0;

			this.buttonChildrensNew = new IconButton(Misc.Icon("AggregateChildrensNew"));
			this.buttonChildrensNew.Clicked += new MessageEventHandler(this.HandleButtonChildrensNew);
			this.buttonChildrensNew.TabIndex = this.index++;
			this.buttonChildrensNew.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.childrensToolBar.Items.Add(this.buttonChildrensNew);
			ToolTip.Default.SetToolTip(this.buttonChildrensNew, Res.Strings.Action.AggregateChildrensNew);

			this.childrensToolBar.Items.Add(new IconSeparator());

			this.buttonChildrensUp = new IconButton(Misc.Icon("Up"));
			this.buttonChildrensUp.Clicked += new MessageEventHandler(this.HandleButtonChildrensUp);
			this.buttonChildrensUp.TabIndex = this.index++;
			this.buttonChildrensUp.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.childrensToolBar.Items.Add(this.buttonChildrensUp);
			ToolTip.Default.SetToolTip(this.buttonChildrensUp, Res.Strings.Action.AggregateChildrensUp);

			this.buttonChildrensDown = new IconButton(Misc.Icon("Down"));
			this.buttonChildrensDown.Clicked += new MessageEventHandler(this.HandleButtonChildrensDown);
			this.buttonChildrensDown.TabIndex = this.index++;
			this.buttonChildrensDown.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.childrensToolBar.Items.Add(this.buttonChildrensDown);
			ToolTip.Default.SetToolTip(this.buttonChildrensDown, Res.Strings.Action.AggregateChildrensDown);

			this.childrensToolBar.Items.Add(new IconSeparator());

			this.buttonChildrensDelete = new IconButton(Misc.Icon("DeleteItem"));
			this.buttonChildrensDelete.Clicked += new MessageEventHandler(this.HandleButtonChildrensDelete);
			this.buttonChildrensDelete.TabIndex = this.index++;
			this.buttonChildrensDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.childrensToolBar.Items.Add(this.buttonChildrensDelete);
			ToolTip.Default.SetToolTip(this.buttonChildrensDelete, Res.Strings.Action.AggregateChildrensDelete);
		}

		protected void CreateNameToolBar()
		{
			//	Crée la toolbar pour le nom de l'agrégat.
			this.nameToolBar = new HToolBar(this.bottomPage);
			this.nameToolBar.Dock = DockStyle.Top;
			this.nameToolBar.DockMargins = new Margins(0, 0, 0, 0);
			this.nameToolBar.TabIndex = 94;
			this.nameToolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			StaticText st = new StaticText();
			st.Width = 80;
			st.Text = Res.Strings.Panel.AggregateName.Label.Name;
			this.nameToolBar.Items.Add(st);

			this.name = new TextField();
			this.name.Width = 135;
			this.name.DockMargins = new Margins(0, 0, 1, 1);
			this.name.TextChanged += new EventHandler(this.HandleNameTextChanged);
			this.name.TabIndex = 1;
			this.name.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.nameToolBar.Items.Add(this.name);
			ToolTip.Default.SetToolTip(this.name, Res.Strings.Panel.AggregateName.Tooltip.Name);

			this.buttonChildrensExtend = new GlyphButton(this.nameToolBar);
			this.buttonChildrensExtend.ButtonStyle = ButtonStyle.Icon;
			this.buttonChildrensExtend.GlyphShape = GlyphShape.ArrowDown;
			this.buttonChildrensExtend.Width = 12;
			this.buttonChildrensExtend.Dock = DockStyle.Right;
			this.buttonChildrensExtend.DockMargins = new Margins(0, 0, 5, 5);
			this.buttonChildrensExtend.Clicked += new MessageEventHandler(this.HandleButtonChildrensExtend);
			this.buttonChildrensExtend.TabIndex = 2;
			this.buttonChildrensExtend.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonChildrensExtend, Res.Strings.Panel.Abstract.Extend);
		}

		protected void CreateSelectorToolBar()
		{
			//	Crée la toolbar pour le sélectionneur de panneaux.
			this.selectorContainer = new Widget(this.bottomPage);
			this.selectorContainer.Height = Styles.selectorSize+8;
			this.selectorContainer.Dock = DockStyle.Top;
			this.selectorContainer.DockMargins = new Margins(0, 0, 5, 0);
			this.selectorContainer.TabIndex = 97;
			this.selectorContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.selectorToolBar = new Widget(this.selectorContainer);
			this.selectorToolBar.Height = Styles.selectorSize+8;
			this.selectorToolBar.Dock = DockStyle.Fill;
			this.selectorToolBar.DockMargins = new Margins(0, 0, 0, 0);
			this.selectorToolBar.TabIndex = 1;
			this.selectorToolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.buttonStyleDelete = new IconButton(this.selectorContainer);
			this.buttonStyleDelete.IconName = Misc.Icon("AggregateStyleDelete");
			this.buttonStyleDelete.AutoFocus = false;
			this.buttonStyleDelete.Clicked += new MessageEventHandler(this.HandleButtonStyleDelete);
			this.buttonStyleDelete.Dock = DockStyle.Right;
			this.buttonStyleDelete.DockMargins = new Margins(0, 0, 0, 6);
			this.buttonStyleDelete.TabIndex = 3;
			this.buttonStyleDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonStyleDelete, Res.Strings.Action.AggregateStyleDelete);

			this.buttonStyleNew = new IconButton(this.selectorContainer);
			this.buttonStyleNew.IconName = Misc.Icon("AggregateStyleNew");
			this.buttonStyleNew.AutoFocus = false;
			this.buttonStyleNew.Clicked += new MessageEventHandler(this.HandleButtonStyleNew);
			this.buttonStyleNew.Dock = DockStyle.Right;
			this.buttonStyleNew.DockMargins = new Margins(0, 0, 0, 6);
			this.buttonStyleNew.TabIndex = 2;
			this.buttonStyleNew.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
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
			this.graphicList.List = this.document.Aggregates;
			this.graphicList.UpdateContent();

			this.paragraphList.List = this.document.TextStyles(StyleCategory.Paragraph);
			this.paragraphList.UpdateContent();
			
			this.characterList.List = this.document.TextStyles(StyleCategory.Character);
			this.characterList.UpdateContent();
			
			this.UpdateAggregateName();
			this.UpdateAggregateChildrens();
			this.UpdateChildrensToolBar();
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
					this.graphicList.UpdateRow(row);
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
					if ( textStyle.TextStyleClass == Common.Text.TextStyleClass.Paragraph )
					{
						this.paragraphList.UpdateRow(row);
					}

					if ( textStyle.TextStyleClass == Common.Text.TextStyleClass.Text )
					{
						this.characterList.UpdateRow(row);
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

					double width = System.Math.Floor(this.selectorToolBar.Width/total);
					width = System.Math.Min(width, Styles.selectorSize);
					double zoom = width/Styles.selectorSize;

					for ( int i=0 ; i<100 ; i++ )
					{
						if ( table[i] != 0 )
						{
							string name = Properties.Abstract.TypeName(table[i]);
							string icon = Properties.Abstract.IconText(table[i]);
							string text = Properties.Abstract.Text(table[i]);

							Widgets.IconMarkButton button = this.UpdateSelectorAdd(width, true, name, icon, text);
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
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Justif",   "TextJustif",   Res.Strings.TextPanel.Justif.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Leading",  "TextLeading",  Res.Strings.TextPanel.Leading.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Margins",  "TextMargins",  Res.Strings.TextPanel.Margins.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Spaces",   "TextSpaces",   Res.Strings.TextPanel.Spaces.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Keep",     "TextKeep",     Res.Strings.TextPanel.Keep.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Font",     "TextFont",     Res.Strings.TextPanel.Font.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Xline",    "TextXline",    Res.Strings.TextPanel.Xline.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, true, "Language", "TextLanguage", Res.Strings.TextPanel.Language.Title);
				}
			}

			if ( this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel != -1 )
				{
					bool enable = (sel != 0);  // le premier style est forcément le style de base !
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Font",     "TextFont",     Res.Strings.TextPanel.Font.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Xline",    "TextXline",    Res.Strings.TextPanel.Xline.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Xscript",  "TextXscript",  Res.Strings.TextPanel.Xscript.Title);
					this.UpdateSelectorAdd(Styles.selectorSize, enable, "Language", "TextLanguage", Res.Strings.TextPanel.Language.Title);
				}
			}
		}

		protected Widgets.IconMarkButton UpdateSelectorAdd(double width, bool enable, string name, string icon, string text)
		{
			Widgets.IconMarkButton button = new Widgets.IconMarkButton(this.selectorToolBar);
			button.Name = name;
			button.IconName = Misc.Icon(icon);
			button.Width = width;
			button.Height = Styles.selectorSize+8;
			button.Enable = enable;
			button.AutoFocus = false;
			button.ButtonStyle = ButtonStyle.ActivableIcon;
			button.Dock = DockStyle.Left;
			button.ActiveState = (name == this.SelectorName) ? ActiveState.Yes : ActiveState.No;
			button.Clicked += new MessageEventHandler(this.HandleSelectorClicked);
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
					text = this.document.TextContext.StyleList.StyleMap.GetCaption(style);
				}
			}

			this.ignoreChanged = true;
			this.name.Text = text;
			this.ignoreChanged = false;
		}

		protected void UpdateChildrensExtend()
		{
			//	Met à jour les panneaux des enfants selon le mode réduit/étendu.
			this.buttonChildrensExtend.GlyphShape = this.isChildrensExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			this.childrensToolBar.Visibility = this.isChildrensExtended;
			this.childrensGraphicList.Visibility = (this.isChildrensExtended && this.category == StyleCategory.Graphic);
			this.childrensParagraphList.Visibility = (this.isChildrensExtended && this.category == StyleCategory.Paragraph);
			this.childrensCharacterList.Visibility = (this.isChildrensExtended && this.category == StyleCategory.Character);
		}

		protected void UpdateChildrensToolBar()
		{
			//	Met à jour les boutons de la toolbar des enfants.
			if ( this.category == StyleCategory.Graphic )
			{
				int aggSel = this.graphicList.SelectedRow;
				int total = this.childrensGraphicList.Rows;
				int sel = this.childrensGraphicList.SelectedRow;

				this.buttonChildrensNew.Enable = (aggSel != -1);
				this.buttonChildrensUp.Enable = (sel != -1 && sel > 0);
				this.buttonChildrensDown.Enable = (sel != -1 && sel < total-1);
				this.buttonChildrensDelete.Enable = (sel != -1);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int styleSel = this.TextStyleList.SelectedRow;
				int total = this.TextChildrensList.Rows;
				int sel = this.TextChildrensList.SelectedRow;

				this.buttonChildrensNew.Enable = (styleSel != -1);
				this.buttonChildrensUp.Enable = (sel != -1 && sel > 0);
				this.buttonChildrensDown.Enable = (sel != -1 && sel < total-1);
				this.buttonChildrensDelete.Enable = (sel != -1);
			}
		}

		protected void UpdateAggregateChildrens()
		{
			//	Met à jour le panneau pour éditer les enfants de l'agrégat sélectionné.
			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate agg = this.GetAggregate();

				if ( agg == null )
				{
					this.childrensGraphicList.List = null;
				}
				else
				{
					this.childrensGraphicList.List = agg.Childrens;
					this.childrensGraphicList.SelectRow(agg.Childrens.Selected, true);
				}

				this.childrensGraphicList.UpdateContent();
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);

				if ( sel == -1 )
				{
					this.TextChildrensList.List = null;
				}
				else
				{
					Common.Text.TextStyle style = this.TextStyleList.List[sel];
					this.TextChildrensList.List = style.ParentStyles;
				}

				this.TextChildrensList.UpdateContent();
			}
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
					return;
				}

				if ( this.panel != null && this.panel.Property.Type == property.Type )
				{
					this.panel.Property = property;
					this.panelContainer.Height = this.panel.DefaultHeight;
					this.panelContainer.ForceLayout();

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
				if ( this.panel == null )  return;

				this.panel.Property = property;
				this.panel.IsExtendedSize = true;
				this.panel.IsLayoutDirect = true;
				this.panel.Changed += new EventHandler(this.HandlePanelChanged);
				this.panel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
				this.panel.SetParent(this.panelContainer);
				this.panel.Dock = DockStyle.Fill;
				this.panel.TabIndex = 1;
				this.panel.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

				this.panelContainer.Height = this.panel.DefaultHeight;
				this.panelContainer.ForceLayout();
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
				if ( panel == null )  return;

				this.textPanel = panel;
				this.textPanel.IsExtendedSize = true;
				this.textPanel.OriginColorChanged += new EventHandler(this.HandleOriginColorChanged);
				this.textPanel.SetParent(this.panelContainer);
				this.textPanel.Dock = DockStyle.Fill;
				this.textPanel.TabIndex = 1;
				this.textPanel.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
				this.textPanel.UpdateAfterAttach();

				this.panelContainer.Height = this.textPanel.DefaultHeight;
				this.panelContainer.ForceLayout();
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
				this.panel.Changed -= new EventHandler(this.HandlePanelChanged);
				this.panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				this.panel.Dispose();
				this.panel = null;
			}

			if ( this.textPanel != null )
			{
				this.document.Wrappers.StyleTextWrapper.Detach();
				this.document.Wrappers.StyleParagraphWrapper.Detach();

				this.textPanel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				this.textPanel.Dispose();
				this.textPanel = null;
			}
				
			this.panelContainer.Height = 0.0;
			this.panelContainer.ForceLayout();
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
			this.categoryGraphic  .ActiveState = (name == "Graphic"  ) ? ActiveState.Yes : ActiveState.No;
			this.categoryParagraph.ActiveState = (name == "Paragraph") ? ActiveState.Yes : ActiveState.No;
			this.categoryCharacter.ActiveState = (name == "Character") ? ActiveState.Yes : ActiveState.No;

			if ( this.categoryGraphic  .ActiveState == ActiveState.Yes )  this.Category = StyleCategory.Graphic;
			if ( this.categoryParagraph.ActiveState == ActiveState.Yes )  this.Category = StyleCategory.Paragraph;
			if ( this.categoryCharacter.ActiveState == ActiveState.Yes )  this.Category = StyleCategory.Character;
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
					this.UpdateChildrensExtend();
					this.UpdateAggregateChildrens();
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
			Button button = sender as Button;
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
				this.document.IsDirtySerialize = true;

				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateNew3(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel agrégat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
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
				
				Text.TextStyle style = this.document.TextContext.StyleList.NewTextStyle(this.document.Modifier.OpletQueue, null, type, properties, parents);
				this.document.TextContext.StyleList.SetNextStyle(this.document.Modifier.OpletQueue, style, initialStyle.NextStyle);

				rank ++;
				this.StylesShiftRanks(rank);
				this.document.TextContext.StyleList.StyleMap.SetCaption(this.document.Modifier.OpletQueue, style, Misc.CopyName(initialName));
				this.document.TextContext.StyleList.StyleMap.SetRank(this.document.Modifier.OpletQueue, style, rank);
				this.document.SetSelectedTextStyle(this.category, rank);
				
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;

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

				this.document.SetSelectedTextStyle(this.category, sel-1);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;
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

				this.document.SetSelectedTextStyle(this.category, sel+1);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;
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
				
				if ( sel >= this.TextStyleList.List.Length )
				{
					sel = this.TextStyleList.List.Length-1;
				}
				this.document.SetSelectedTextStyle(this.category, sel);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;
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
			this.UpdateAggregateChildrens();
			this.UpdateChildrensToolBar();
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
			this.UpdateAggregateChildrens();
			this.UpdateChildrensToolBar();
			this.ShowSelection();
		}

		private void HandleStylesTableDoubleClicked(object sender, MessageEventArgs e)
		{
			//	Liste double-cliquée.
			System.Diagnostics.Debug.Assert(this.category != StyleCategory.Graphic);
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleButtonChildrensNew(object sender, MessageEventArgs e)
		{
			//	Crée un nouvel enfant.
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenuChildrens(pos);
			if ( menu == null )  return;
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // dépasse en bas ?
			{
				pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
				pos.Y += menu.Height;  // déroule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // dépasse à droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonChildrensUp(object sender, MessageEventArgs e)
		{
			//	Enfant en haut.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.childrensGraphicList.SelectedRow;
				if ( sel == -1 )  return;
				Properties.Aggregate agg = this.GetAggregate();
				this.document.Modifier.AggregateChildrensSwap(agg, sel, sel-1);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
			}
		}

		private void HandleButtonChildrensDown(object sender, MessageEventArgs e)
		{
			//	Enfant en bas.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.childrensGraphicList.SelectedRow;
				if ( sel == -1 )  return;
				Properties.Aggregate agg = this.GetAggregate();
				this.document.Modifier.AggregateChildrensSwap(agg, sel, sel+1);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
			}
		}

		private void HandleButtonChildrensDelete(object sender, MessageEventArgs e)
		{
			//	Supprime l'enfant.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.childrensGraphicList.SelectedRow;
				if ( sel == -1 )  return;
				Properties.Aggregate agg = this.GetAggregate();
				Properties.Aggregate delAgg = agg.Childrens[sel] as Properties.Aggregate;
				this.document.Modifier.AggregateChildrensDelete(agg, delAgg);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return;
				Text.TextStyle[] styles = this.TextStyleList.List;
				Text.TextStyle currentStyle = styles[sel];

				System.Collections.ArrayList parents = new System.Collections.ArrayList();
				Text.TextStyle[] currentParents = currentStyle.ParentStyles;
				foreach ( Text.TextStyle style in currentParents )
				{
					parents.Add(style);
				}

				int parentSel = this.TextChildrensList.SelectedRow;
				if ( parentSel == -1 )  return;
				parents.RemoveAt(parentSel);

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChildrensDelete);
				this.document.TextContext.StyleList.RedefineTextStyle(this.document.Modifier.OpletQueue, currentStyle, currentStyle.StyleProperties, parents);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;
				this.SetDirtyContent();
			}
		}

		private void HandleButtonChildrensExtend(object sender, MessageEventArgs e)
		{
			//	Etend/réduit le panneau des enfants.
			this.isChildrensExtended = !this.isChildrensExtended;
			this.UpdateChildrensExtend();
			this.ForceLayout();
		}

		private void HandleAggregatesChildrensSelectionChanged(object sender)
		{
			//	Sélection changée dans la liste des enfants.
			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate agg = this.GetAggregate();
				this.document.Modifier.OpletQueueEnable = false;
				agg.Childrens.Selected = this.childrensGraphicList.SelectedRow;
				this.document.Modifier.OpletQueueEnable = true;

				this.UpdateChildrensToolBar();
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				this.UpdateChildrensToolBar();
			}
		}


		private void HandleButtonStyleNew(object sender, MessageEventArgs e)
		{
			//	Crée une nouvelle propriété.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenuTypes(pos);
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // dépasse en bas ?
			{
				pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
				pos.Y += menu.Height;  // déroule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // dépasse à droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
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

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
				agg.AggregateName = this.name.Text;
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;

				this.document.Notifier.NotifyAggregateChanged(agg);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return;

				Common.Text.TextStyle style = this.TextStyleList.List[sel];
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChange, "ChangeAggregateName", sel);
				this.document.TextContext.StyleList.StyleMap.SetCaption(this.document.Modifier.OpletQueue, style, this.name.Text);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;

				this.document.Notifier.NotifyTextStyleChanged(style);
			}
		}

		private void HandlePanelChanged(object sender)
		{
			//	Le contenu du panneau a changé.
			int sel = this.graphicList.SelectedRow;
			if ( sel != -1 )
			{
				this.graphicList.UpdateRow(sel);

				double h = this.panel.DefaultHeight;
				if ( h != this.panelContainer.Height )
				{
					this.panel.Height = h;
					this.panelContainer.Height = h;
					this.panelContainer.ForceLayout();
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

		private void HandleColorSelectorChanged(object sender)
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
		protected VMenu CreateMenuTypes(Point pos)
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
				item.Pressed += new MessageEventHandler(this.HandleMenuTypesPressed);
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

		
		#region MenuChildrens
		protected VMenu CreateMenuChildrens(Point pos)
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
					if ( currentAgg.Childrens.Contains(agg) )  continue;

					string line = agg.AggregateName;
					MenuItem item = new MenuItem("ChildrensNew", "", line, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
					item.Pressed += new MessageEventHandler(this.HandleMenuChildrensPressed);
					menu.Items.Add(item);
					used ++;
				}
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				if ( sel == -1 )  return null;

				Text.TextStyle[] styles = this.TextStyleList.List;
				Text.TextStyle currentStyle = styles[sel];

				for ( int i=0 ; i<styles.Length ; i++ )
				{
					Text.TextStyle style = styles[i];
					if ( style == currentStyle )  continue;
					if ( Styles.ContainsStyle(currentStyle.ParentStyles, style) )  continue;

					string line = this.document.TextContext.StyleList.StyleMap.GetCaption(style);
					MenuItem item = new MenuItem("ChildrensNew", "", line, "", i.ToString(System.Globalization.CultureInfo.InvariantCulture));
					item.Pressed += new MessageEventHandler(this.HandleMenuChildrensPressed);
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

		private void HandleMenuChildrensPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			int i = System.Int32.Parse(item.Name, System.Globalization.CultureInfo.InvariantCulture);

			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate newAgg = this.document.Aggregates[i] as Properties.Aggregate;
				Properties.Aggregate agg = this.GetAggregate();
				this.document.Modifier.AggregateChildrensNew(agg, newAgg);
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int sel = this.document.GetSelectedTextStyle(this.category);
				Text.TextStyle[] styles = this.TextStyleList.List;
				Text.TextStyle currentStyle = styles[sel];
				Text.TextStyle newStyle = styles[i];

				System.Collections.ArrayList parents = new System.Collections.ArrayList();
				Text.TextStyle[] currentParents = currentStyle.ParentStyles;
				foreach ( Text.TextStyle style in currentParents )
				{
					parents.Add(style);
				}
				parents.Insert(0, newStyle);

				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.AggregateChildrensNew);
				this.document.TextContext.StyleList.RedefineTextStyle(this.document.Modifier.OpletQueue, currentStyle, currentStyle.StyleProperties, parents);
				this.document.Modifier.OpletQueueValidateAction();
				this.document.IsDirtySerialize = true;
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

		protected Widgets.TextStylesList TextChildrensList
		{
			//	Donne le widget pour la liste des enfants selon la catégorie actuelle.
			get
			{
				if ( this.category == StyleCategory.Paragraph )  return this.childrensParagraphList;
				if ( this.category == StyleCategory.Character )  return this.childrensCharacterList;
				throw new System.ArgumentException("TextChildrensList(" + this.category.ToString() + ")");
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

		protected PaneBook					mainBook;
		protected PanePage					topPage;
		protected PanePage					bottomPage;
		protected Scrollable				bottomScrollable;

		protected Widget					categoryContainer;
		protected Button					categoryGraphic;
		protected Button					categoryParagraph;
		protected Button					categoryCharacter;
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

		protected Widget					selectorContainer;
		protected Widget					selectorToolBar;
		protected string[]					selectorName = new string[(int) StyleCategory.Count];
		protected IconSeparator				separatorStyle;
		protected IconButton				buttonStyleNew;
		protected IconButton				buttonStyleDelete;

		protected HToolBar					childrensToolBar;
		protected IconButton				buttonChildrensNew;
		protected IconButton				buttonChildrensUp;
		protected IconButton				buttonChildrensDown;
		protected IconButton				buttonChildrensDelete;
		protected GlyphButton				buttonChildrensExtend;

		protected Widgets.AggregateList		childrensGraphicList;
		protected Widgets.TextStylesList	childrensParagraphList;
		protected Widgets.TextStylesList	childrensCharacterList;

		protected Widget					panelContainer;
		protected Panels.Abstract			panel;
		protected TextPanels.Abstract		textPanel;
		protected ColorSelector				colorSelector;

		protected int						index;
		protected bool						isChildrensExtended = false;
		protected bool						ignoreChanged = false;
	}
}
