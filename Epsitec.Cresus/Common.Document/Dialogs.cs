using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Dialogs.
	/// </summary>
	public class Dialogs
	{
		public Dialogs(Document document)
		{
			this.document = document;
		}


		// Affiche le dialogue des réglages.
		public void ShowSettings()
		{
			if ( this.windowSettings == null )
			{
				this.CreateSettings();
			}
			this.windowSettings.Show();
		}

		// Appelé lorsque les réglages ont changé.
		public void UpdateSettings()
		{
			if ( this.windowSettings == null )  return;

			int total = this.document.Settings.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				setting.UpdateValue();
			}
		}

		// Appelé lorsque les repères ont changé.
		public void UpdateGuides()
		{
			if ( this.containerGuides == null )  return;
			this.containerGuides.SetDirtyContent();
		}

		// Crée le dialogue des réglages.
		protected void CreateSettings()
		{
			this.windowSettings = new Window();
			
			this.windowSettings.ClientSize = new Size(300, 320);
			this.windowSettings.Text = "Réglages";
			this.windowSettings.MakeSecondaryWindow();
			this.windowSettings.MakeFixedSizeWindow();
			this.windowSettings.MakeToolWindow();
			this.windowSettings.PreventAutoClose = true;
			this.windowSettings.Owner = this.document.Modifier.ActiveViewer.Window;
			this.windowSettings.WindowCloseClicked += new EventHandler(this.HandleWindowSettingsCloseClicked);

			// Crée les onglets.
			TabBook book = new TabBook();
			book.Arrows = TabBookArrows.Stretch;
			book.Anchor = AnchorStyles.LeftAndRight | AnchorStyles.TopAndBottom;
			book.AnchorMargins = new Margins(6, 6, 6, 34);
			this.windowSettings.Root.Children.Add(book);

			TabPage bookFormat = new TabPage();
			bookFormat.TabTitle = "Format";
			book.Items.Add(bookFormat);

			TabPage bookGrid = new TabPage();
			bookGrid.TabTitle = "Grille";
			book.Items.Add(bookGrid);

			TabPage bookGuides = new TabPage();
			bookGuides.TabTitle = "Repères";
			book.Items.Add(bookGuides);

			TabPage bookDuplicate = new TabPage();
			bookDuplicate.TabTitle = "Dupliquer";
			book.Items.Add(bookDuplicate);

			TabPage bookColors = new TabPage();
			bookColors.TabTitle = "Couleurs";
			book.Items.Add(bookColors);

			book.ActivePage = bookFormat;

			// Onglet bookFormat:
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.CreateTitle(bookFormat, "Dimensions d'un pictogramme");
			}
			else
			{
				this.CreateTitle(bookFormat, "Dimensions d'une page");
			}
			this.CreatePoint(bookFormat, "PageSize");

			// Onglet bookGrid:
			this.CreateTitle(bookGrid, "Grille magnétique");
			this.CreateBool(bookGrid, "GridActive");
			this.CreateBool(bookGrid, "GridShow");
			this.CreatePoint(bookGrid, "GridStep");
			this.CreatePoint(bookGrid, "GridOffset");
			this.CreateSeparator(bookGrid);
			this.CreateBool(bookGrid, "GuidesActive");
			this.CreateBool(bookGrid, "GuidesShow");

			// Onglet bookGuides:
			this.CreateTitle(bookGuides, "Définition des repères");
			this.containerGuides = new Containers.Guides(this.document);
			this.containerGuides.Dock = DockStyle.Fill;
			this.containerGuides.DockMargins = new Margins(10, 10, 4, 10);
			this.containerGuides.Parent = bookGuides;

			// Onglet bookDuplicate:
			this.CreateTitle(bookDuplicate, "Déplacement lorsq'un objet est dupliqué");
			this.CreatePoint(bookDuplicate, "DuplicateMove");

			// Onglet bookColors:
			this.CreateTitle(bookColors, "Choix des couleurs");

			// Bouton de fermeture.
			Button buttonClose = new Button();
			buttonClose.Width = 75;
			buttonClose.Text = "Fermer";
			buttonClose.ButtonStyle = ButtonStyle.DefaultAccept;
			buttonClose.Anchor = AnchorStyles.BottomLeft;
			buttonClose.AnchorMargins = new Margins(6, 0, 0, 6);
			buttonClose.Clicked += new MessageEventHandler(this.HandleButtonCloseClicked);
			this.windowSettings.Root.Children.Add(buttonClose);
			ToolTip.Default.SetToolTip(buttonClose, "Fermer les réglages");
		}

		private void HandleWindowSettingsCloseClicked(object sender)
		{
			this.windowSettings.Hide();
		}

		private void HandleButtonCloseClicked(object sender, MessageEventArgs e)
		{
			this.windowSettings.Hide();
		}


		#region WidgetTitle
		// Crée un widget de titre pour un onglet.
		protected void CreateTitle(Widget parent, string labelText)
		{
			StaticText text = new StaticText(parent);
			text.Text = string.Format("<b>{0}</b>", labelText);
			text.Dock = DockStyle.Top;
			text.DockMargins = new Margins(10, 10, 8, 2);

			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.DockMargins = new Margins(0, 0, 3, 6);
		}

		// Crée un séparateur pour un onglet.
		protected void CreateSeparator(Widget parent)
		{
			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.DockMargins = new Margins(0, 0, 4, 6);
		}
		#endregion


		#region WidgetBool
		// Crée un widget pour éditer un réglage de type Bool.
		protected void CreateBool(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			CheckButton check = new CheckButton(parent);
			check.Text = sBool.Text;
			check.Width = 100;
			check.Name = sBool.Name;
			check.ActiveState = sBool.Value ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			check.Dock = DockStyle.Top;
			check.DockMargins = new Margins(10, 10, 0, 5);
			check.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
			sBool.CheckButton(check);
		}

		private void HandleCheckActiveStateChanged(object sender)
		{
			CheckButton check = sender as CheckButton;
			if ( check == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(check.Name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			sBool.Value = ( check.ActiveState == WidgetState.ActiveYes );
		}
		#endregion


		#region WidgetDouble
		// Crée des widgets pour éditer un réglage de type Double.
		protected void CreateDouble(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			Widget container = new Widget(parent);
			container.Height = 22;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = sDouble.Text;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			TextFieldSlider field = new TextFieldSlider(container);
			field.Width = 60;
			field.Name = sDouble.Name;
			field.MinValue = (decimal) sDouble.MinValue;
			field.MaxValue = (decimal) sDouble.MaxValue;
			field.Step = (decimal) sDouble.Step;
			field.Resolution = (decimal) sDouble.Resolution;
			field.Value = (decimal) sDouble.Value;
			field.ValueChanged += new EventHandler(this.HandleFieldDoubleChanged);
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			string tooltip = string.Format("{0}..{1}", sDouble.MinValue, sDouble.MaxValue);
			ToolTip.Default.SetToolTip(field, tooltip);
			sDouble.TextField(field);
		}

		private void HandleFieldDoubleChanged(object sender)
		{
			TextFieldSlider field = sender as TextFieldSlider;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			sDouble.Value = (double) field.Value;
		}
		#endregion


		#region WidgetPoint
		// Crée des widgets pour éditer un réglage de type Point.
		protected void CreatePoint(Widget parent, string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			StaticText text;
			TextFieldSlider field;
			string tooltip;

			Widget container = new Widget(parent);
			container.Height = 22+2+22;
			container.Dock = DockStyle.Top;
			container.DockMargins = new Margins(10, 10, 0, 5);

			Widget containerXY = new Widget(container);
			containerXY.Width = 120+60;
			containerXY.Height = container.Height;
			containerXY.Dock = DockStyle.Left;
			containerXY.DockMargins = new Margins(0, 0, 0, 0);

			Widget containerX = new Widget(containerXY);
			containerX.Width = containerXY.Width;
			containerX.Height = 22;
			containerX.Dock = DockStyle.Top;
			containerX.DockMargins = new Margins(0, 0, 0, 0);

			text = new StaticText(containerX);
			text.Text = sPoint.TextX;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			field = new TextFieldSlider(containerX);
			field.Width = 60;
			field.Name = sPoint.Name;
			field.MinValue = (decimal) sPoint.MinValue;
			field.MaxValue = (decimal) sPoint.MaxValue;
			field.Step = (decimal) sPoint.Step;
			field.Resolution = (decimal) sPoint.Resolution;
			field.Value = (decimal) sPoint.Value.X;
			field.ValueChanged += new EventHandler(this.HandleFieldPointXChanged);
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			tooltip = string.Format("{0}..{1}", sPoint.MinValue, sPoint.MaxValue);
			ToolTip.Default.SetToolTip(field, tooltip);
			sPoint.TextField(0, field);

			Widget containerY = new Widget(containerXY);
			containerY.Width = containerXY.Width;
			containerY.Height = 22;
			containerY.Dock = DockStyle.Bottom;
			containerY.DockMargins = new Margins(0, 0, 0, 0);

			text = new StaticText(containerY);
			text.Text = sPoint.TextY;
			text.Width = 120;
			text.Dock = DockStyle.Left;
			text.DockMargins = new Margins(0, 0, 0, 0);

			field = new TextFieldSlider(containerY);
			field.Width = 60;
			field.Name = sPoint.Name;
			field.MinValue = (decimal) sPoint.MinValue;
			field.MaxValue = (decimal) sPoint.MaxValue;
			field.Step = (decimal) sPoint.Step;
			field.Resolution = (decimal) sPoint.Resolution;
			field.Value = (decimal) sPoint.Value.Y;
			field.ValueChanged += new EventHandler(this.HandleFieldPointYChanged);
			field.Dock = DockStyle.Left;
			field.DockMargins = new Margins(0, 0, 0, 0);
			tooltip = string.Format("{0}..{1}", sPoint.MinValue, sPoint.MaxValue);
			ToolTip.Default.SetToolTip(field, tooltip);
			sPoint.TextField(1, field);

			Separator sep = new Separator(container);
			sep.Width = 1;
			sep.Height = container.Height;
			sep.Dock = DockStyle.Left;
			sep.DockMargins = new Margins(8, 0, 0, 0);

			CheckButton check = new CheckButton(container);
			check.Width = 50;
			check.Height = container.Height;
			check.Name = sPoint.Name;
			check.Text = "Liés";
			check.ActiveState = sPoint.Link ? WidgetState.ActiveYes : WidgetState.ActiveNo;
			check.Dock = DockStyle.Left;
			check.DockMargins = new Margins(-3, 0, 0, 0);
			check.ActiveStateChanged += new EventHandler(this.HandleCheckPointActiveStateChanged);
		}

		private void HandleFieldPointXChanged(object sender)
		{
			TextFieldSlider field = sender as TextFieldSlider;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			Drawing.Point point = sPoint.Value;
			point.X = (double) field.Value;
			if ( sPoint.Link )
			{
				point.Y = point.X;
			}
			sPoint.Value = point;
		}

		private void HandleFieldPointYChanged(object sender)
		{
			TextFieldSlider field = sender as TextFieldSlider;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			Drawing.Point point = sPoint.Value;
			point.Y = (double) field.Value;
			if ( sPoint.Link )
			{
				point.X = point.Y;
			}
			sPoint.Value = point;
		}

		private void HandleCheckPointActiveStateChanged(object sender)
		{
			CheckButton check = sender as CheckButton;
			if ( check == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(check.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			sPoint.Link = ( check.ActiveState == WidgetState.ActiveYes );
		}
		#endregion


		protected Document						document;
		protected Window						windowSettings;
		protected Containers.Guides				containerGuides;
	}
}
