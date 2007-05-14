using System.Collections.Generic;
using System.Text.RegularExpressions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Viewers
{
	/// <summary>
	/// Permet de représenter les ressources d'un module.
	/// </summary>
	public class Strings2 : Abstract
	{
		public Strings2(Module module, PanelsContext context, ResourceAccess access, MainWindow mainWindow) : base(module, context, access, mainWindow)
		{
			this.accessor = new Support.ResourceAccessors.StringResourceAccessor();
			this.accessor.Load(access.ResourceManager);

			StructuredType cultureMapType = new StructuredType();
			cultureMapType.Fields.Add("Name", StringType.Default);
			cultureMapType.Fields.Add("Primary", StringType.Default);
			cultureMapType.Fields.Add("Secondary", StringType.Default);

			this.collectionView = new CollectionView(this.accessor.Collection);
			this.itemViewFactory = new ItemViewFactory(this);
			
			//	Crée les deux volets gauche/droite séparés d'un splitter.
			this.left = new Widget(this);
			this.left.Name = "Left";
			this.left.MinWidth = 80;
			this.left.MaxWidth = 400;
			this.left.PreferredWidth = Abstract.leftArrayWidth;
			this.left.Dock = DockStyle.Left;
			this.left.Padding = new Margins(10, 10, 10, 10);
			this.left.TabIndex = this.tabIndex++;
			this.left.TabNavigationMode = TabNavigationMode.ForwardTabPassive;

			this.splitter = new VSplitter(this);
			this.splitter.Dock = DockStyle.Left;
			this.splitter.SplitterDragged += new EventHandler(this.HandleSplitterDragged);
			VSplitter.SetAutoCollapseEnable(this.left, true);

			this.right = new Widget(this);
			this.right.Name = "Right";
			this.right.MinWidth = 200;
			this.right.Dock = DockStyle.Fill;
			this.right.Padding = new Margins(1, 1, 1, 1);
			this.right.TabIndex = this.tabIndex++;
			this.right.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			//	Crée la partie gauche.			
			this.labelEdit = new MyWidgets.TextFieldExName(this.left);
			this.labelEdit.Name = "LabelEdit";
			this.labelEdit.Margins = new Margins(0, 0, 10, 0);
			this.labelEdit.Dock = DockStyle.Bottom;
			this.labelEdit.ButtonShowCondition = ShowCondition.WhenModified;
			this.labelEdit.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.labelEdit.EditionAccepted += new EventHandler(this.HandleTextChanged);
			this.labelEdit.KeyboardFocusChanged += new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);
			this.labelEdit.TabIndex = this.tabIndex++;
			this.labelEdit.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.labelEdit.Visibility = (this.module.Mode == DesignerMode.Build);
			this.currentTextField = this.labelEdit;

			this.table = new UI.ItemTable(this.left);
			this.table.ItemPanel.CustomItemViewFactoryGetter = this.ItemViewFactoryGetter;
			this.table.SourceType = cultureMapType;
			this.table.Items = this.collectionView;

			this.table.Columns.Add(new UI.ItemTableColumn("Name", new Widgets.Layouts.GridLength(200, Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Primary", new Widgets.Layouts.GridLength(100, Widgets.Layouts.GridUnitType.Proportional)));
			this.table.Columns.Add(new UI.ItemTableColumn("Secondary", new Widgets.Layouts.GridLength(100, Widgets.Layouts.GridUnitType.Proportional)));
			
			this.table.HorizontalScrollMode = UI.ItemTableScrollMode.Linear;
			this.table.VerticalScrollMode = UI.ItemTableScrollMode.ItemBased;
			this.table.HeaderVisibility = false;
			this.table.FrameVisibility = false;
			this.table.ItemPanel.Layout = UI.ItemPanelLayout.VerticalList;
			this.table.ItemPanel.ItemSelectionMode = UI.ItemPanelSelectionMode.ExactlyOne;
			this.table.Dock = Widgets.DockStyle.Fill;
			this.table.Margins = new Drawing.Margins(0, 0, 0, 0);
			this.table.SizeChanged += this.HandleTableSizeChanged;
			this.table.ColumnHeader.ColumnWidthChanged += this.HandleColumnHeaderColumnWidthChanged;

			//	Crée la partie droite, bande supérieure pour les boutons des cultures.
			Widget sup = new Widget(this.right);
			sup.Name = "Sup";
			sup.PreferredHeight = 35;
			sup.Padding = new Margins(1, 1, 10, 0);
			sup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			sup.Dock = DockStyle.Top;
			sup.TabIndex = this.tabIndex++;
			sup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
			
			this.primaryCulture = new IconButtonMark(sup);
			this.primaryCulture.ButtonStyle = ButtonStyle.ActivableIcon;
			this.primaryCulture.SiteMark = ButtonMarkDisposition.Below;
			this.primaryCulture.MarkDimension = 5;
			this.primaryCulture.PreferredHeight = 25;
			this.primaryCulture.ActiveState = ActiveState.Yes;
			this.primaryCulture.AutoFocus = false;
			this.primaryCulture.Margins = new Margins(0, 10, 0, 0);
			this.primaryCulture.Dock = DockStyle.StackFill;

			this.secondaryCultureGroup = new Widget(sup);
			this.secondaryCultureGroup.Name = "SecondaryCultureGroup";
			this.secondaryCultureGroup.Margins = new Margins(10, 0, 0, 0);
			this.secondaryCultureGroup.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
			this.secondaryCultureGroup.Dock = DockStyle.StackFill;
			this.secondaryCultureGroup.TabIndex = this.tabIndex++;
			this.secondaryCultureGroup.TabNavigationMode = TabNavigationMode.ForwardTabPassive;


			this.UpdateCultures();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.splitter.SplitterDragged -= new EventHandler(this.HandleSplitterDragged);

				this.labelEdit.EditionAccepted -= new EventHandler(this.HandleTextChanged);
				this.labelEdit.KeyboardFocusChanged -= new EventHandler<Epsitec.Common.Types.DependencyPropertyChangedEventArgs>(this.HandleLabelKeyboardFocusChanged);

				this.table.SizeChanged -= this.HandleTableSizeChanged;
				this.table.ColumnHeader.ColumnWidthChanged -= this.HandleColumnHeaderColumnWidthChanged;
			}

			base.Dispose(disposing);
		}


		public override ResourceAccess.Type ResourceType
		{
			get
			{
				return ResourceAccess.Type.Strings2;
			}
		}

		
		public override void Update()
		{
			//	Met à jour le contenu du Viewer.
			this.UpdateEdit();
			this.UpdateModificationsCulture();
			this.UpdateCommands();
		}

		
		protected override Widget CultureParentWidget
		{
			//	Retourne le parent à utiliser pour les boutons des cultures.
			get
			{
				return this.secondaryCultureGroup;
			}
		}


#if false
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.primaryCulture == null )  return;

			Rectangle box = this.Client.Bounds;
			box.Deflate(10);
			Rectangle rect, r;

			int lines = System.Math.Max((int)box.Height/50, 4);
			int editLines = lines*2/3;
			int aboutLines = lines-editLines;
			double cultureHeight = 20;
			double editHeight = editLines*13+8;
			double aboutHeight = aboutLines*13+8;

			//	Il faut obligatoirement s'occuper d'abord de this.array, puisque les autres
			//	widgets dépendent des largeurs relatives de ses colonnes.
			rect = box;
			rect.Top -= cultureHeight+5;
			rect.Bottom += editHeight+5+aboutHeight+5;
			this.table.SetManualBounds(rect);

			rect = box;
			rect.Bottom = rect.Top-cultureHeight-5;
			rect.Left += this.GetColomnWidth(0);
			rect.Width = this.GetColomnWidth(1);
			this.primaryCulture.SetManualBounds(rect);

			if (this.secondaryCultures != null)
			{
				rect.Left = rect.Right+2;
				rect.Width = this.GetColomnWidth(2)-2;
				double w = System.Math.Floor(rect.Width/this.secondaryCultures.Length);
				for (int i=0; i<this.secondaryCultures.Length; i++)
				{
					r = rect;
					r.Left += w*i;
					r.Width = w;
					if (i == this.secondaryCultures.Length-1)
					{
						r.Right = rect.Right;
					}
					this.secondaryCultures[i].SetManualBounds(r);
				}
			}

#if false
			rect = box;
			rect.Top = rect.Bottom+editHeight+aboutHeight+5;
			rect.Bottom = rect.Top-editHeight;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelStatic.SetManualBounds(rect);
			rect.Width += 5+1;
			r = rect;
			r.Bottom = r.Top-21;
			this.labelEdit.SetManualBounds(r);
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryEdit.SetManualBounds(rect);
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryEdit.SetManualBounds(rect);

			rect = box;
			rect.Top = rect.Bottom+aboutHeight;
			rect.Bottom = rect.Top-aboutHeight;
			rect.Width = this.array.GetColumnsAbsoluteWidth(0)-5;
			this.labelAbout.SetManualBounds(rect);
			rect.Left += this.array.GetColumnsAbsoluteWidth(0);
			rect.Width = this.array.GetColumnsAbsoluteWidth(1)+1;
			this.primaryAbout.SetManualBounds(rect);
			rect.Left = rect.Right-1;
			rect.Width = this.array.GetColumnsAbsoluteWidth(2);
			this.secondaryAbout.SetManualBounds(rect);
#endif
		}
#endif

		protected double GetColomnWidth(int column)
		{
			//	Retourne la largeur d'une colonne.
			return this.table.Columns[column].Width.Value;
		}

		
		private void HandleSplitterDragged(object sender)
		{
			//	Le splitter a été bougé.
			Abstract.leftArrayWidth = this.left.ActualWidth;
		}

		private void HandleTableSizeChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			UI.ItemTable table = (UI.ItemTable) sender;
			Drawing.Size size = (Drawing.Size) e.NewValue;

			double width = size.Width - table.GetPanelPadding().Width;
			//?table.ColumnHeader.SetColumnWidth(0, width);

			table.ItemPanel.ItemViewDefaultSize = new Size(width, 20);
		}

		private void HandleColumnHeaderColumnWidthChanged(object sender, UI.ColumnWidthChangeEventArgs e)
		{
		}

		private void HandleTextChanged(object sender)
		{
			//	Un texte éditable a changé.
			if (this.ignoreChange)
			{
				return;
			}

			AbstractTextField edit = sender as AbstractTextField;
			string text = edit.Text;
		}

		private void HandleLabelKeyboardFocusChanged(object sender, Epsitec.Common.Types.DependencyPropertyChangedEventArgs e)
		{
			//	Appelé lorsque la ligne éditable pour le label voit son focus changer.
			TextFieldEx field = sender as TextFieldEx;
			field.AcceptEdition();
			this.HandleEditKeyboardFocusChanged(sender, e);
		}


		protected UI.IItemViewFactory ItemViewFactoryGetter(UI.ItemView itemView)
		{
			//	Retourne le "factory" a utiliser pour les éléments représentés dans cet ItemTable/ItemPanel.
			if (itemView.Item == null || itemView.Item.GetType() != typeof(CultureMap))
			{
				return null;
			}
			else
			{
				return this.itemViewFactory;
			}
		}


		private class ItemViewFactory : UI.AbstractItemViewFactory
		{
			public ItemViewFactory(Strings2 owner)
			{
				this.owner = owner;
			}

			protected override Widget CreateElement(string name, UI.ItemPanel panel, UI.ItemView view, UI.ItemViewShape shape)
			{
				CultureMap item = view.Item as CultureMap;

				switch (name)
				{
					case "Name":
						return this.CreateName(item);
					case "Primary":
						return this.CreatePrimary(item);
					case "Secondary":
						return this.CreateSecondary(item);
				}

				return null;
			}

			private Widget CreateName(CultureMap item)
			{
				StaticText widget = new StaticText();

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(item.Name);
				widget.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;

				return widget;
			}

			private Widget CreatePrimary(CultureMap item)
			{
				StaticText widget = new StaticText();
				StructuredData data = item.GetCultureData(Resources.DefaultTwoLetterISOLanguageName);
				string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);

				return widget;
			}

			private Widget CreateSecondary(CultureMap item)
			{
				StaticText widget = new StaticText();
				StructuredData data = item.GetCultureData("en"); // TODO: choisir ici la culture secondaire qui a été sélectionnée
				string text = data.GetValue(Support.Res.Fields.ResourceString.Text) as string;

				widget.Margins = new Margins(5, 5, 0, 0);
				widget.Text = TextLayout.ConvertToTaggedText(text);

				return widget;
			}
			

			Strings2 owner;
		}


		protected Support.ResourceAccessors.StringResourceAccessor accessor;
		protected UI.ItemTable					table;
		protected CollectionView				collectionView;
		private ItemViewFactory					itemViewFactory;

		protected Widget						left;
		protected Widget						right;
		protected VSplitter						splitter;
		protected Widget						secondaryCultureGroup;
		protected MyWidgets.TextFieldExName		labelEdit;
	}
}
