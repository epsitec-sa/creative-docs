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
			this.bottomPage.PaneMinSize = 100;
			this.bottomPage.PaneElasticity = 0.5;
			this.mainBook.Items.Add(this.bottomPage);

			this.CreateCategoryGroup();
			this.CreateAggregateToolBar();

			//	Table des agr�gats (styles graphiques).
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

			//	Table des styles de caract�re.
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

			//	Enfants de l'agr�gat.
			this.childrens = new Widgets.AggregateList();
			this.childrens.Document = this.document;
			this.childrens.HScroller = true;
			this.childrens.VScroller = true;
			this.childrens.IsHiliteColumn = false;
			this.childrens.IsOrderColumn = true;
			this.childrens.IsChildrensColumn = false;
			this.childrens.SetParent(this.bottomPage);
			this.childrens.Height = 103;
			this.childrens.Dock = DockStyle.Top;
			this.childrens.DockMargins = new Margins(0, 0, 0, 0);
			this.childrens.FinalSelectionChanged += new EventHandler(this.HandleAggregatesChildrensSelectionChanged);
			this.childrens.TabIndex = 96;
			this.childrens.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

			//	S�lectionneur.
			this.selectorToolBar = new Widget(this.bottomPage);
			this.selectorToolBar.Height = 16+8;
			this.selectorToolBar.Dock = DockStyle.Top;
			this.selectorToolBar.DockMargins = new Margins(0, 0, 5, 0);
			this.selectorToolBar.TabIndex = 97;
			this.selectorToolBar.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			//	Conteneur du panneau.
			this.panelContainer = new Widget(this.bottomPage);
			this.panelContainer.Height = 0.0;
			this.panelContainer.Dock = DockStyle.Top;
			this.panelContainer.DockMargins = new Margins(0, 0, 0, 0);
			this.panelContainer.TabIndex = 99;
			this.panelContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			
			//	Roue des couleurs.
			this.colorSelector = new ColorSelector();
			this.colorSelector.ColorPalette.ColorCollection = this.document.GlobalSettings.ColorCollection;
			this.colorSelector.HasCloseButton = true;
			this.colorSelector.Changed += new EventHandler(this.HandleColorSelectorChanged);
			this.colorSelector.CloseClicked += new EventHandler(this.HandleColorSelectorClosed);
			this.colorSelector.SetParent(this.bottomPage);
			this.colorSelector.Dock = DockStyle.Top;
			this.colorSelector.DockMargins = new Margins(0, 0, 5, 0);
			this.colorSelector.TabIndex = 100;
			this.colorSelector.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.colorSelector.Visibility = false;

			this.UpdateChildrensExtend();

			this.category = StyleCategory.Graphic;
			this.UpdateCategory();
		}

		protected void CreateCategoryGroup()
		{
			//	Cr�e les boutons radio pour le choix de la cat�gorie.
			this.categoryContainer = new Widget(this.topPage);
			this.categoryContainer.Height = 20;
			this.categoryContainer.Dock = DockStyle.Top;
			this.categoryContainer.DockMargins = new Margins(0, 0, 0, 5);
			this.categoryContainer.TabIndex = 1;
			this.categoryContainer.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;

			this.index = 0;

			this.categoryGraphic = new RadioButton(this.categoryContainer, "Category", 0);
			this.categoryGraphic.ActiveState = ActiveState.Yes;
			this.categoryGraphic.Name = "Graphic";
			this.categoryGraphic.Text = Res.Strings.Panel.AggregateCategory.Graphic;
			this.categoryGraphic.Width = 80;
			this.categoryGraphic.Dock = DockStyle.Left;
			this.categoryGraphic.DockMargins = new Margins(0, 0, 0, 0);
			this.categoryGraphic.TabIndex = this.index++;
			this.categoryGraphic.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.categoryGraphic.ActiveStateChanged += new EventHandler(this.HandleCategoryChanged);

			this.categoryParagraph = new RadioButton(this.categoryContainer, "Category", 1);
			this.categoryParagraph.Name = "Paragraph";
			this.categoryParagraph.Text = Res.Strings.Panel.AggregateCategory.Paragraph;
			this.categoryParagraph.Width = 80;
			this.categoryParagraph.Dock = DockStyle.Left;
			this.categoryParagraph.DockMargins = new Margins(0, 0, 0, 0);
			this.categoryParagraph.TabIndex = this.index++;
			this.categoryParagraph.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.categoryParagraph.ActiveStateChanged += new EventHandler(this.HandleCategoryChanged);

			this.categoryCharacter = new RadioButton(this.categoryContainer, "Category", 2);
			this.categoryCharacter.Name = "Character";
			this.categoryCharacter.Text = Res.Strings.Panel.AggregateCategory.Character;
			this.categoryCharacter.Width = 80;
			this.categoryCharacter.Dock = DockStyle.Left;
			this.categoryCharacter.DockMargins = new Margins(0, 0, 0, 0);
			this.categoryCharacter.TabIndex = this.index++;
			this.categoryCharacter.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			this.categoryCharacter.ActiveStateChanged += new EventHandler(this.HandleCategoryChanged);
		}

		protected void CreateAggregateToolBar()
		{
			//	Cr�e la toolbar principale.
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

			this.aggregateToolBar.Items.Add(new IconSeparator());

			this.buttonStyleNew = new IconButton(Misc.Icon("AggregateStyleNew"));
			this.buttonStyleNew.Clicked += new MessageEventHandler(this.HandleButtonStyleNew);
			this.buttonStyleNew.TabIndex = this.index++;
			this.buttonStyleNew.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonStyleNew);
			ToolTip.Default.SetToolTip(this.buttonStyleNew, Res.Strings.Action.AggregateStyleNew);

			this.buttonStyleDelete = new IconButton(Misc.Icon("AggregateStyleDelete"));
			this.buttonStyleDelete.Clicked += new MessageEventHandler(this.HandleButtonStyleDelete);
			this.buttonStyleDelete.TabIndex = this.index++;
			this.buttonStyleDelete.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.aggregateToolBar.Items.Add(this.buttonStyleDelete);
			ToolTip.Default.SetToolTip(this.buttonStyleDelete, Res.Strings.Action.AggregateStyleDelete);
		}

		protected void CreateChildrensToolBar()
		{
			//	Cr�e la toolbar pour le choix des enfants.
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
			//	Cr�e la toolbar pour le nom de l'agr�gat.
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
			this.buttonChildrensExtend.GlyphShape = GlyphShape.ArrowUp;
			this.buttonChildrensExtend.Width = 12;
			this.buttonChildrensExtend.Dock = DockStyle.Right;
			this.buttonChildrensExtend.DockMargins = new Margins(0, 0, 5, 5);
			this.buttonChildrensExtend.Clicked += new MessageEventHandler(this.HandleButtonChildrensExtend);
			this.buttonChildrensExtend.TabIndex = 2;
			this.buttonChildrensExtend.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.buttonChildrensExtend, Res.Strings.Panel.Abstract.Extend);
		}


		public override void Hilite(Objects.Abstract hiliteObject)
		{
			//	Met en �vidence l'objet survol� par la souris.
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
			//	Effectue la mise � jour du contenu.
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
			//	Effectue la mise � jour des agr�gats.
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
			//	Effectue la mise � jour des styles de texte.
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
			//	Effectue la mise � jour des propri�t�s.
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
			//	Met � jour la cat�gorie.
			this.graphicList.Visibility = (this.category == StyleCategory.Graphic);
			this.paragraphList.Visibility = (this.category == StyleCategory.Paragraph);
			this.characterList.Visibility = (this.category == StyleCategory.Character);

			this.buttonAggregateNew3.Visibility = (this.category == StyleCategory.Graphic);
			this.buttonAggregateNewAll.Visibility = (this.category == StyleCategory.Graphic);
			this.buttonStyleNew.Visibility = (this.category == StyleCategory.Graphic);
			this.buttonStyleDelete.Visibility = (this.category == StyleCategory.Graphic);

			this.UpdateAggregateName();
			this.UpdateSelector();
			this.UpdateToolBar();
		}

		protected void UpdateToolBar()
		{
			//	Met � jour les boutons de la toolbar.
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
					type = Properties.Abstract.TypeName(this.selectorName);
					if ( type != Properties.Type.None )
					{
						if ( agg.Property(type) != null )
						{
							enableDelete = true;
						}
					}
				}
				this.buttonStyleNew.Enable = true;
				this.buttonStyleDelete.Enable = enableDelete;
			}

			if ( this.category == StyleCategory.Paragraph || this.category == StyleCategory.Character )
			{
				int total = this.TextStyleList.Rows;
				int sel = this.document.GetSelectedTextStyle(this.category);

				this.buttonAggregateUp.Enable = (sel != -1 && sel > 0);
				this.buttonAggregateDuplicate.Enable = (sel != -1);
				this.buttonAggregateDown.Enable = (sel != -1 && sel < total-1);
				this.buttonAggregateDelete.Enable = (sel != -1);
			}
		}


		protected void UpdateSelector()
		{
			//	Met � jour le s�lectionneur du panneau.
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
					width = System.Math.Min(width, 16);

					for ( int i=0 ; i<100 ; i++ )
					{
						if ( table[i] != 0 )
						{
							string name = Properties.Abstract.TypeName(table[i]);
							string icon = Properties.Abstract.IconText(table[i]);
							string text = Properties.Abstract.Text(table[i]);

							this.UpdateSelectorAdd(width, name, icon, text);
						}
					}
				}
			}

			if ( this.category == StyleCategory.Paragraph )
			{
				this.UpdateSelectorAdd(16, "TextJustif", "TextJustif", Res.Strings.TextPanel.Justif.Title);
				this.UpdateSelectorAdd(16, "TextLeading", "TextLeading", Res.Strings.TextPanel.Leading.Title);
				this.UpdateSelectorAdd(16, "TextMargins", "TextMargins", Res.Strings.TextPanel.Margins.Title);
				this.UpdateSelectorAdd(16, "TextSpaces", "TextSpaces", Res.Strings.TextPanel.Spaces.Title);
				this.UpdateSelectorAdd(16, "TextKeep", "TextKeep", Res.Strings.TextPanel.Keep.Title);
				this.UpdateSelectorAdd(16, "TextFont", "TextFont", Res.Strings.TextPanel.Font.Title);
				this.UpdateSelectorAdd(16, "TextXline", "TextXline", Res.Strings.TextPanel.Xline.Title);
				this.UpdateSelectorAdd(16, "TextLanguage", "TextLanguage", Res.Strings.TextPanel.Language.Title);
			}

			if ( this.category == StyleCategory.Character )
			{
				this.UpdateSelectorAdd(16, "TextFont", "TextFont", Res.Strings.TextPanel.Font.Title);
				this.UpdateSelectorAdd(16, "TextXline", "TextXline", Res.Strings.TextPanel.Xline.Title);
				this.UpdateSelectorAdd(16, "TextXscript", "TextXscript", Res.Strings.TextPanel.Xscript.Title);
				this.UpdateSelectorAdd(16, "TextLanguage", "TextLanguage", Res.Strings.TextPanel.Language.Title);
			}
		}

		protected void UpdateSelectorAdd(double width, string name, string icon, string text)
		{
			Widgets.IconMarkButton button = new Widgets.IconMarkButton(this.selectorToolBar);
			button.Name = name;
			button.IconName = Misc.Icon(icon);
			button.Width = width;
			button.Height = 16+8;
			button.AutoFocus = false;
			button.Dock = DockStyle.Left;
			button.ActiveState = (name == this.selectorName) ? ActiveState.Yes : ActiveState.No;
			button.Clicked += new MessageEventHandler(this.HandleSelectorClicked);
			ToolTip.Default.SetToolTip(button, text);
		}

		protected void UpdateAggregateName()
		{
			//	Met � jour le panneau pour �diter le nom de l'agr�gat s�lectionn�.
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
			//	Met � jour les panneaux des enfants selon le mode r�duit/�tendu.
			this.buttonChildrensExtend.GlyphShape = this.isChildrensExtended ? GlyphShape.ArrowDown : GlyphShape.ArrowUp;
			this.childrensToolBar.Visibility = (this.isChildrensExtended);
			this.childrens.Visibility = (this.isChildrensExtended);
		}

		protected void UpdateChildrensToolBar()
		{
			//	Met � jour les boutons de la toolbar des enfants.
			if ( this.category == StyleCategory.Graphic )
			{
				int aggSel = this.graphicList.SelectedRow;
				int total = this.childrens.Rows;
				int sel = this.childrens.SelectedRow;

				this.buttonChildrensNew.Enable = (aggSel != -1);
				this.buttonChildrensUp.Enable = (sel != -1 && sel > 0);
				this.buttonChildrensDown.Enable = (sel != -1 && sel < total-1);
				this.buttonChildrensDelete.Enable = (sel != -1);
			}

			if ( this.category == StyleCategory.Paragraph )
			{
			}

			if ( this.category == StyleCategory.Character )
			{
			}
		}

		protected void UpdateAggregateChildrens()
		{
			//	Met � jour le panneau pour �diter les enfants de l'agr�gat s�lectionn�.
			if ( this.category == StyleCategory.Graphic )
			{
				Properties.Aggregate agg = this.GetAggregate();

				if ( agg == null )
				{
					this.childrens.List = null;
				}
				else
				{
					this.childrens.List = agg.Childrens;
					this.childrens.SelectRow(agg.Childrens.Selected, true);
				}

				this.childrens.UpdateContent();
			}

			if ( this.category == StyleCategory.Paragraph )
			{
			}

			if ( this.category == StyleCategory.Character )
			{
			}
		}

		protected void UpdatePanel()
		{
			//	Met � jour le panneau pour �diter la propri�t� s�lectionn�e.
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

			if ( this.category == StyleCategory.Paragraph )
			{
				this.ClosePanel();
			}

			if ( this.category == StyleCategory.Character )
			{
				this.ClosePanel();
			}
		}

		protected Properties.Abstract PropertyPanel()
		{
			//	Cherche la propri�t� pour le panneau.
			Properties.Aggregate agg = this.GetAggregate();
			if ( agg == null )  return null;

			Properties.Type type = Properties.Abstract.TypeName(this.selectorName);
			if ( type == Properties.Type.None )  return null;

			return agg.Property(type);
		}

		protected void ClosePanel()
		{
			//	Ferme le panneau pour la propri�t� et la roue des couleurs.
			this.colorSelector.Visibility = false;
			this.colorSelector.BackColor = Color.Empty;

			if ( this.panel != null )
			{
				this.panel.Changed -= new EventHandler(this.HandlePanelChanged);
				this.panel.OriginColorChanged -= new EventHandler(this.HandleOriginColorChanged);
				this.panel.Dispose();
				this.panel = null;
				this.panelContainer.Height = 0.0;
				this.panelContainer.ForceLayout();
			}
		}

		protected void ShowSelection()
		{
			//	Montre la ligne s�lectionn�e dans la liste.
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


		protected StyleCategory Category
		{
			//	Cat�gorie s�lectionn�e (Graphic, Paragraph ou Character).
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
				}
			}
		}

		private void HandleCategoryChanged(object sender)
		{
			if ( this.categoryGraphic  .ActiveState == ActiveState.Yes )  this.Category = StyleCategory.Graphic;
			if ( this.categoryParagraph.ActiveState == ActiveState.Yes )  this.Category = StyleCategory.Paragraph;
			if ( this.categoryCharacter.ActiveState == ActiveState.Yes )  this.Category = StyleCategory.Character;
		}

		private void HandleSelectorClicked(object sender, MessageEventArgs e)
		{
			this.selectorName = null;
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
						this.selectorName = widget.Name;
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
			//	Cr�e un nouvel agr�gat.
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
				parents.Add(this.document.TextContext.DefaultStyle);

				Text.TextStyle style = this.document.TextContext.StyleList.NewTextStyle(null, type, properties, parents);

				int rank = this.document.GetSelectedTextStyle(this.category)+1;
				this.document.TextContext.StyleList.StyleMap.SetCaption(style, this.document.Modifier.GetNextTextStyleName(this.category));
				this.document.TextContext.StyleList.StyleMap.SetRank(style, rank);
				this.document.SetSelectedTextStyle(this.category, rank);

				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateNew3(object sender, MessageEventArgs e)
		{
			//	Cr�e un nouvel agr�gat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.document.Modifier.AggregateNew3(sel, "", true);
			}
		}

		private void HandleButtonAggregateNewAll(object sender, MessageEventArgs e)
		{
			//	Cr�e un nouvel agr�gat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.document.Modifier.AggregateNewAll(sel, "", true);
			}
		}

		private void HandleButtonAggregateDuplicate(object sender, MessageEventArgs e)
		{
			//	Duplique un agr�gat.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				if ( sel == -1 )  sel = 10000;
				this.document.Modifier.AggregateDuplicate(sel);
			}

			if ( this.category == StyleCategory.Paragraph )
			{
			}

			if ( this.category == StyleCategory.Character )
			{
			}
		}

		private void HandleButtonAggregateUp(object sender, MessageEventArgs e)
		{
			//	Monte d'une ligne l'agr�gat s�lectionn�.
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

				this.document.TextContext.StyleList.StyleMap.SetRank(style1, rank2);
				this.document.TextContext.StyleList.StyleMap.SetRank(style2, rank1);

				this.document.SetSelectedTextStyle(this.category, sel-1);
				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateDown(object sender, MessageEventArgs e)
		{
			//	Descend d'une ligne l'agr�gat s�lectionn�.
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

				this.document.TextContext.StyleList.StyleMap.SetRank(style1, rank2);
				this.document.TextContext.StyleList.StyleMap.SetRank(style2, rank1);

				this.document.SetSelectedTextStyle(this.category, sel+1);
				this.SetDirtyContent();
			}
		}

		private void HandleButtonAggregateDelete(object sender, MessageEventArgs e)
		{
			//	Supprime l'agr�gat s�lectionn�.
			if ( this.category == StyleCategory.Graphic )
			{
				int sel = this.document.Aggregates.Selected;
				this.document.Modifier.AggregateDelete(sel);
			}
		}

		private void HandleAggregatesTableSelectionChanged(object sender)
		{
			//	S�lection chang�e dans la liste.
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
				Properties.Type type = Properties.Abstract.TypeName(this.selectorName);
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
			//	Liste double-cliqu�e.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleAggregatesTableFlyOverChanged(object sender)
		{
			//	La cellule survol�e a chang�.
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
			//	S�lection chang�e dans la liste.
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
			//	Liste double-cliqu�e.
			System.Diagnostics.Debug.Assert(this.category != StyleCategory.Graphic);
			this.name.SelectAll();
			this.name.Focus();
		}

		private void HandleButtonChildrensNew(object sender, MessageEventArgs e)
		{
			//	Cr�e un nouvel enfant.
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenuChildrens(pos);
			if ( menu == null )  return;
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // d�passe en bas ?
			{
				pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
				pos.Y += menu.Height;  // d�roule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // d�passe � droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonChildrensUp(object sender, MessageEventArgs e)
		{
			//	Enfant en haut.
			int sel = this.childrens.SelectedRow;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateChildrensSwap(agg, sel, sel-1);
		}

		private void HandleButtonChildrensDown(object sender, MessageEventArgs e)
		{
			//	Enfant en bas.
			int sel = this.childrens.SelectedRow;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateChildrensSwap(agg, sel, sel+1);
		}

		private void HandleButtonChildrensDelete(object sender, MessageEventArgs e)
		{
			//	Supprime l'enfant.
			int sel = this.childrens.SelectedRow;
			if ( sel == -1 )  return;
			Properties.Aggregate agg = this.GetAggregate();
			Properties.Aggregate delAgg = agg.Childrens[sel] as Properties.Aggregate;
			this.document.Modifier.AggregateChildrensDelete(agg, delAgg);
		}

		private void HandleButtonChildrensExtend(object sender, MessageEventArgs e)
		{
			//	Etend/r�duit le panneau des enfants.
			this.isChildrensExtended = !this.isChildrensExtended;
			this.UpdateChildrensExtend();
			this.ForceLayout();
		}

		private void HandleAggregatesChildrensSelectionChanged(object sender)
		{
			//	S�lection chang�e dans la liste des enfants.
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.OpletQueueEnable = false;
			agg.Childrens.Selected = this.childrens.SelectedRow;
			this.document.Modifier.OpletQueueEnable = true;

			this.UpdateChildrensToolBar();
		}


		private void HandleButtonStyleNew(object sender, MessageEventArgs e)
		{
			//	Cr�e une nouvelle propri�t�.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			IconButton button = sender as IconButton;
			Point pos = button.MapClientToScreen(new Point(0,0));
			VMenu menu = this.CreateMenuTypes(pos);
			menu.Host = this;

			ScreenInfo info = ScreenInfo.Find(pos);
			Drawing.Rectangle area = info.WorkingArea;

			if ( pos.Y-menu.Height < area.Bottom )  // d�passe en bas ?
			{
				pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
				pos.Y += menu.Height;  // d�roule contre le haut ?
			}

			if ( pos.X+menu.Width > area.Right )  // d�passe � droite ?
			{
				pos.X -= pos.X+menu.Width-area.Right;
			}

			menu.ShowAsContextMenu(this.Window, pos);
		}

		private void HandleButtonStyleDelete(object sender, MessageEventArgs e)
		{
			//	Supprime la propri�t� s�lectionn�e.
			System.Diagnostics.Debug.Assert(this.category == StyleCategory.Graphic);
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateStyleDelete(agg, Properties.Abstract.TypeName(this.selectorName));
			this.UpdateSelector();
			this.UpdatePanel();
		}

		private void HandleNameTextChanged(object sender)
		{
			//	Le nom de l'agr�gat a chang�.
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
				this.document.TextContext.StyleList.StyleMap.SetCaption(style, this.name.Text);

				this.document.Notifier.NotifyTextStyleChanged(style);
			}
		}

		private void HandlePanelChanged(object sender)
		{
			//	Le contenu du panneau a chang�.
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
			//	Le widget qui d�termine la couleur d'origine a chang�.
			this.colorSelector.Visibility = true;
			this.ignoreChanged = true;
			this.colorSelector.Color = this.panel.OriginColorGet();
			this.ignoreChanged = false;
			this.panel.OriginColorSelect(this.panel.OriginColorRank());
		}

		private void HandleColorSelectorChanged(object sender)
		{
			//	Couleur chang�e dans la roue.
			if ( this.ignoreChanged )  return;
			this.panel.OriginColorChange(this.colorSelector.Color);
		}

		private void HandleColorSelectorClosed(object sender)
		{
			//	Fermer la roue.
			this.panel.OriginColorDeselect();

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
				string active = Misc.Image(enable ? "ActiveNo" : "ActiveYes");
				string icon = Misc.Image(Properties.Abstract.IconText(type));
				string text = Properties.Abstract.Text(type);
				string line = string.Format("{0} {1}   {2}", active, icon, text);
				MenuItem item = new MenuItem("StyleNew", "", line, "", Properties.Abstract.TypeName(type));
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
			this.selectorName = Properties.Abstract.TypeName(type);
			this.UpdateSelector();
			this.UpdatePanel();
		}
		#endregion

		
		#region MenuChildrens
		protected VMenu CreateMenuChildrens(Point pos)
		{
			//	Construit le menu pour choisir un enfant � ajouter.
			VMenu menu = new VMenu();
			Properties.Aggregate currentAgg = this.GetAggregate();
			int used = 0;
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
			if ( used == 0 )  return null;
			menu.AdjustSize();
			return menu;
		}

		protected bool MenuChildrensExist(UndoableList styles, Properties.Type type)
		{
			foreach ( Properties.Abstract property in styles )
			{
				if ( property.Type == type )  return true;
			}
			return false;
		}

		private void HandleMenuChildrensPressed(object sender, MessageEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			int i = System.Int32.Parse(item.Name, System.Globalization.CultureInfo.InvariantCulture);
			Properties.Aggregate newAgg = this.document.Aggregates[i] as Properties.Aggregate;
			Properties.Aggregate agg = this.GetAggregate();
			this.document.Modifier.AggregateChildrensNew(agg, newAgg);
		}
		#endregion

		
		protected Properties.Aggregate GetAggregate()
		{
			//	Donne l'agr�gat s�lectionn�.
			int sel = this.document.Aggregates.Selected;

			if ( sel == -1 )  return null;
			if ( sel >= this.document.Aggregates.Count )  return null;

			return this.document.Aggregates[sel] as Properties.Aggregate;
		}

		protected Widgets.TextStylesList TextStyleList
		{
			get
			{
				if ( this.category == StyleCategory.Paragraph )  return this.paragraphList;
				if ( this.category == StyleCategory.Character )  return this.characterList;
				throw new System.ArgumentException("TextStyleList("+this.category+")");
			}
		}


		protected PaneBook					mainBook;
		protected PanePage					topPage;
		protected PanePage					bottomPage;

		protected Widget					categoryContainer;
		protected RadioButton				categoryGraphic;
		protected RadioButton				categoryParagraph;
		protected RadioButton				categoryCharacter;
		protected StyleCategory				category;

		protected HToolBar					aggregateToolBar;
		protected IconButton				buttonAggregateNewEmpty;
		protected IconButton				buttonAggregateNew3;
		protected IconButton				buttonAggregateNewAll;
		protected IconButton				buttonAggregateDuplicate;
		protected IconButton				buttonAggregateUp;
		protected IconButton				buttonAggregateDown;
		protected IconButton				buttonAggregateDelete;
		protected IconButton				buttonStyleNew;
		protected IconButton				buttonStyleDelete;

		protected Widgets.AggregateList		graphicList;
		protected Widgets.TextStylesList	paragraphList;
		protected Widgets.TextStylesList	characterList;

		protected HToolBar					nameToolBar;
		protected TextField					name;

		protected Widget					selectorToolBar;
		protected string					selectorName;

		protected HToolBar					childrensToolBar;
		protected IconButton				buttonChildrensNew;
		protected IconButton				buttonChildrensUp;
		protected IconButton				buttonChildrensDown;
		protected IconButton				buttonChildrensDelete;
		protected GlyphButton				buttonChildrensExtend;

		protected Widgets.AggregateList		childrens;
		protected Widget					panelContainer;
		protected Panels.Abstract			panel;
		protected ColorSelector				colorSelector;

		protected int						index;
		protected bool						isChildrensExtended = false;
		protected bool						ignoreChanged = false;
	}
}
