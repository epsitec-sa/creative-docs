using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for DocumentDialogs.
	/// </summary>
	public class DocumentDialogs
	{
		public DocumentDialogs(Document document)
		{
			this.document = document;
			this.widgetsTable = new System.Collections.Hashtable();
		}

		public void Dispose()
		{
		}


		public void UpdateAllSettings()
		{
			//	Met � jour tous les r�glages dans les diff�rents dialogues.
			this.UpdateSettings(false);
			this.UpdatePrint(false);
			this.UpdateExport(false);
            this.UpdateExportPDF(false);
            this.UpdateExportICO(false);
		}


		#region Infos
		public void BuildInfos(Window window)
		{
			//	Peuple le dialogue des informations.
			if ( this.windowInfos == null )
			{
				this.windowInfos = window;
			}

			this.UpdateInfos();
		}

		public void UpdateInfos()
		{
			//	Met � jour le dialogue des informations.
			if ( this.windowInfos == null || !this.windowInfos.IsVisible )  return;

			TextFieldMulti multi = this.windowInfos.Root.FindChild("Infos") as TextFieldMulti;
			if ( multi != null )
			{
				multi.Text = this.document.Modifier.Statistic(true, true);
			}
		}
		#endregion

		#region Settings
		public void BuildSettings(Window window)
		{
			//	Peuple le dialogue des r�glages.
			if ( this.windowSettings == null )
			{
				this.windowSettings = window;

				Widget parent, container;
				Widget book = this.windowSettings.Root.FindChild("BookDocument");

				//	Onglet Format:
				parent = book.FindChild("Format");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				if ( this.document.Type == DocumentType.Pictogram )
				{
					DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.SizePic);
					this.CreatePoint(container, "PageSize");
				}
				else
				{
					DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.SizeDoc);
					this.CreatePaper(container);
					this.CreatePoint(container, "PageSize");
					this.CreateDouble(container, "OutsideArea");
					DocumentDialogs.CreateSeparator(container);
					this.CreateCombo(container, "DefaultUnit");
				}
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Grid:
				parent = book.FindChild("Grid");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Grid);
				this.CreateBool(container, "GridActive");
				this.CreateBool(container, "GridShow");
				this.CreatePoint(container, "GridStep");
				this.CreatePoint(container, "GridSubdiv");
				this.CreatePoint(container, "GridOffset");

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.TextGrid);
				this.CreateBool(container, "TextGridShow");
				this.CreateDouble(container, "TextGridStep");
				this.CreateDouble(container, "TextGridOffset");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Guides:
				parent = book.FindChild("Guides");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Magnet);
				this.CreateBool(container, "GuidesActive");
				this.CreateBool(container, "GuidesShow");
				this.CreateBool(container, "GuidesMouse");
				DocumentDialogs.CreateSeparator(container);

				if ( this.containerGuides == null )
				{
					this.containerGuides = new Containers.Guides(this.document);
					this.containerGuides.Dock = DockStyle.Fill;
					this.containerGuides.Margins = new Margins(10, 10, 4, 10);
					this.containerGuides.TabIndex = this.tabIndex++;
					this.containerGuides.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
				}
				this.containerGuides.SetParent(container);

				//	Onglet Move:
				parent = book.FindChild("Move");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.DuplicateMove);
				this.CreatePoint(container, "DuplicateMove");
				this.CreateBool(container, "RepeatDuplicateMove");

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.ArrowMove);
				this.CreatePoint(container, "ArrowMove");
				this.CreateDouble(container, "ArrowMoveMul");
				this.CreateDouble(container, "ArrowMoveDiv");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Misc:
				parent = book.FindChild("Misc");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Dimension);
				this.CreateDouble(container, "DimensionScale");
				this.CreateDouble(container, "DimensionDecimal");

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.ToLinePrecision);
				this.CreateDouble(container, "ToLinePrecision");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Fonts:
				parent = book.FindChild("Fonts");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Settings.Fonts.Title);

				if ( this.containerFonts == null )
				{
					this.containerFonts = new Containers.Fonts(this.document);
					this.containerFonts.Dock = DockStyle.Fill;
					this.containerFonts.Margins = new Margins(10, 10, 4, 10);
					this.containerFonts.TabIndex = this.tabIndex++;
					this.containerFonts.TabNavigationMode = TabNavigationMode.ForwardTabPassive;
				}
				this.containerFonts.SetParent(container);
			}

			this.UpdateSettings(true);
		}

		protected void UpdateSettings(bool force)
		{
			//	Appel� lorsque les r�glages ont chang�.
			if ( !force )
			{
				if ( this.windowSettings == null || !this.windowSettings.IsVisible )  return;
			}

			this.UpdateDialogSettings("Settings");
		}

		public void UpdateGuides()
		{
			//	Appel� lorsque les rep�res ont chang�.
			if ( this.containerGuides == null )  return;
			this.containerGuides.SetDirtyContent();
		}

		public void SelectGuide(int rank)
		{
			//	S�lectionne un rep�re.
			if ( this.containerGuides == null )  return;
			this.containerGuides.SelectGuide = rank;
		}

		public void UpdateFontsAdded()
		{
			//	Appel� lorsque la liste de polices s'est allong�e.
			//	Ceci peut arriver apr�s l'ouverture d'un document qui contenait des polices
			//	non install�es.
			if ( this.containerFonts == null )  return;
			this.containerFonts.UpdateListAdded();
		}

		public void UpdateFonts()
		{
			//	Appel� lorsque la liste de polices rapides a chang�.
			if ( this.containerFonts == null )  return;
			this.containerFonts.UpdateList();
		}
		#endregion

		#region Print
		public void BuildPrint(Window window)
		{
			//	Peuple le dialogue d'impression.
			if ( this.windowPrint == null )
			{
				this.windowPrint = window;

				Widget parent, container;
				Widget book = this.windowPrint.Root.FindChild("Book");

				//	Onglet Imprimante:
				parent = book.FindChild("Printer");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Printer);
				this.CreatePrinter(container, "PrintName");

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Area);
				this.CreateRange(container, "PrintRange");
				this.CreateCombo(container, "PrintArea");

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Copies);
				this.CreateDouble(container, "PrintCopies");
				this.CreateBool(container, "PrintCollate");
				this.CreateBool(container, "PrintReverse");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Param�tres:
				parent = book.FindChild("Param");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Param);
				this.CreateBool(container, "PrintAutoLandscape");
				this.CreateBool(container, "PrintDraft");
				this.CreateBool(container, "PrintPerfectJoin");
#if DEBUG
				this.CreateBool(container, "PrintDebugArea");
#endif
				this.CreateDouble(container, "PrintDpi");
				this.CreateBool(container, "PrintAA");

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.File);
				this.CreateBool(container, "PrintToFile");
				this.CreateFilename(container, "PrintFilename");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Images:
				parent = book.FindChild("Image");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Image);
				this.CreateCombo(container, "PrintImageFilterA");
				this.CreateCombo(container, "PrintImageFilterB");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Pr�-presse:
				parent = book.FindChild("Publisher");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Publisher);
				this.CreateBool(container, "PrintAutoZoom");
				this.CreateCombo(container, "PrintCentring");
				this.CreateDouble(container, "PrintMargins");
				this.CreateDouble(container, "PrintDebord");
				this.CreateBool(container, "PrintTarget");
				DocumentDialogs.CreateSeparator(container);
			}

			this.UpdatePrint(true);
		}

		protected void UpdatePrint(bool force)
		{
			//	Appel� lorsque les r�glages ont chang�.
			if ( !force )
			{
				if ( this.windowPrint == null || !this.windowPrint.IsVisible )  return;
			}

			this.UpdatePrinter("PrintName");
			this.UpdateRangeField("PrintRange");
			this.UpdateDialogSettings("Print");
			this.UpdateCombo("PrintImageFilterA");
			this.UpdateCombo("PrintImageFilterB");
		}

		public void UpdatePrintPages()
		{
			//	Appel� lorsque les pages ont chang�.
			if ( this.windowPrint == null || !this.windowPrint.IsVisible )  return;

			this.UpdateRangeField("PrintRange");
		}
		#endregion

		#region Export
		public void BuildExport(Window window)
		{
			//	Peuple le dialogue des exportations.
			if ( this.windowExport == null )
			{
				this.windowExport = window;

				Widget parent = this.windowExport.Root.FindChild("Panel");
				Panel container = new Panel(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

				this.tabIndex = 0;

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Export.Param);
				this.CreateBool(container, "ImageOnlySelected");
				this.CreateDouble(container, "ImageDpi");
				this.CreateCombo(container, "ImageDepth");
				this.CreateCombo(container, "ImageCompression");
				this.CreateDouble(container, "ImageQuality");
				this.CreateDouble(container, "ImageAA");
				this.CreateCombo(container, "ImageFilterA");
				this.CreateCombo(container, "ImageFilterB");
				DocumentDialogs.CreateSeparator(container);
			}

			this.UpdateExport(true);
		}

		protected void UpdateExport(bool force)
		{
			//	Appel� lorsque les r�glages ont chang�.
			if ( !force )
			{
				if ( this.windowExport == null || !this.windowExport.IsVisible )  return;
			}

			this.UpdateDialogSettings("Export");
			this.UpdateBool("ImageOnlySelected");
			this.UpdateCombo("ImageDepth");
			this.UpdateCombo("ImageCompression");
			this.UpdateDouble("ImageQuality");
			this.UpdateCombo("ImageFilterA");
			this.UpdateCombo("ImageFilterB");
		}
		#endregion

		#region ExportPDF
		public void BuildExportPDF(Window window)
		{
			//	Peuple le dialogue des exportations en PDF.
			if ( this.windowExportPDF == null )
			{
				this.windowExportPDF = window;

				Widget parent, container;
				Widget book = this.windowExportPDF.Root.FindChild("Book");

				//	Onglet G�n�ral:
				parent = book.FindChild("Generic");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.Print.Area);
				this.CreateRange(container, "ExportPDFRange");

				DocumentDialogs.CreateSeparator(container);
				this.CreateBool(container, "ExportPDFTextCurve");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Couleurs:
				parent = book.FindChild("Color");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				DocumentDialogs.CreateSeparator(container);
				this.CreateCombo(container, "ExportPDFColorConversion");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Images:
				parent = book.FindChild("Image");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				DocumentDialogs.CreateSeparator(container);
				this.CreateCombo(container, "ExportPDFImageCompression");
				this.CreateDouble(container, "ExportPDFJpegQuality");
				this.CreateDouble(container, "ExportPDFImageMinDpi");
				this.CreateDouble(container, "ExportPDFImageMaxDpi");
				this.CreateCombo(container, "ExportPDFImageFilterA");
				this.CreateCombo(container, "ExportPDFImageFilterB");
				DocumentDialogs.CreateSeparator(container);

				//	Onglet Pr�-presse:
				parent = book.FindChild("Publisher");
				container = new Widget(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;
				
				this.tabIndex = 0;
				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.ExportPDF.Publisher);
				this.CreateDouble(container, "ExportPDFDebord");
				this.CreateBool(container, "ExportPDFTarget");
				DocumentDialogs.CreateTitle (container, Res.Strings.Dialog.ExportPDF.BleedEven);
				this.CreateDouble (container, "ExportPDFBleedEvenTop");
				this.CreateDouble(container, "ExportPDFBleedEvenBottom");
				this.CreateDouble(container, "ExportPDFBleedEvenLeft");
				this.CreateDouble(container, "ExportPDFBleedEvenRight");
				DocumentDialogs.CreateTitle (container, Res.Strings.Dialog.ExportPDF.BleedOdd);
				this.CreateDouble (container, "ExportPDFBleedOddTop");
				this.CreateDouble(container, "ExportPDFBleedOddBottom");
				this.CreateDouble(container, "ExportPDFBleedOddLeft");
				this.CreateDouble(container, "ExportPDFBleedOddRight");
			}

			this.UpdateExportPDF(true);
		}

		protected void UpdateExportPDF(bool force)
		{
			//	Appel� lorsque les r�glages ont chang�.
			if ( !force )
			{
				if ( this.windowExportPDF == null || !this.windowExportPDF.IsVisible )  return;
			}

			this.UpdateDialogSettings("ExportPDF");
			this.UpdateRangeField("ExportPDFRange");
			this.UpdateCombo("ExportPDFColorConversion");
			this.UpdateCombo("ExportPDFImageCompression");
			this.UpdateDouble("ExportPDFJpegQuality");
			this.UpdateDouble("ExportPDFImageMinDpi");
			this.UpdateDouble("ExportPDFImageMaxDpi");
			this.UpdateCombo("ExportPDFImageFilterA");
			this.UpdateCombo("ExportPDFImageFilterB");

			//TODO: ajouter this.UpdateBool("ExportPDFTarget");
			
			this.UpdateDouble("ExportPDFDebord");
			this.UpdateDouble("ExportPDFBleedEvenTop");
			this.UpdateDouble("ExportPDFBleedEvenBottom");
			this.UpdateDouble("ExportPDFBleedEvenLeft");
			this.UpdateDouble("ExportPDFBleedEvenRight");
			this.UpdateDouble("ExportPDFBleedOddTop");
			this.UpdateDouble("ExportPDFBleedOddBottom");
			this.UpdateDouble("ExportPDFBleedOddLeft");
			this.UpdateDouble("ExportPDFBleedOddRight");
		}

		public void UpdateExportPDFPages()
		{
			//	Appel� lorsque les pages ont chang�.
			if ( this.windowExportPDF == null || !this.windowExportPDF.IsVisible )  return;

			this.UpdateRangeField("ExportPDFRange");
		}
		#endregion

        #region ExportICO
        public void BuildExportICO(Window window)
        {
            //	Peuple le dialogue des exportations en ICO.
            if (this.windowExportICO == null)
            {
                this.windowExportICO = window;

				Widget parent = this.windowExportICO.Root.FindChild("Panel");
				Panel container = new Panel(parent);
				container.Name = "Container";
				container.Dock = DockStyle.Fill;

                this.tabIndex = 0;

				DocumentDialogs.CreateTitle(container, Res.Strings.Dialog.ExportICO.Param);
				this.CreateCombo(container, "ExportICOFormat");
                DocumentDialogs.CreateSeparator(container);
            }

            this.UpdateExportICO(true);
        }

        protected void UpdateExportICO(bool force)
        {
            //	Appel� lorsque les r�glages ont chang�.
            if (!force)
            {
                if (this.windowExportICO == null || !this.windowExportICO.IsVisible)  return;
            }

            this.UpdateDialogSettings("ExportICO");
            this.UpdateCombo("ExportICOFormat");
        }

        public void UpdateExportICOPages()
        {
            //	Appel� lorsque les pages ont chang�.
            if (this.windowExportICO == null || !this.windowExportICO.IsVisible)  return;
        }
        #endregion

        #region Glyphs
		public void BuildGlyphs(Window window)
		{
			//	Peuple le dialogue des informations.
			if ( this.windowGlyphs == null )
			{
				this.windowGlyphs = window;
			}

			this.UpdateGlyphs();
		}

		public void UpdateGlyphs()
		{
			//	Met � jour le dialogue des informations.
			if ( this.windowGlyphs == null || !this.windowGlyphs.IsVisible )  return;
		}
		#endregion


		#region WidgetTitle
		public static void CreateTitle(Widget parent, string labelText)
		{
			//	Cr�e un widget de titre pour un onglet.
#if false
			StaticText text = new StaticText(parent);
			text.Text = Misc.Bold(labelText);
			text.Dock = DockStyle.Top;
			text.Margins = new Margins(10, 10, 8, 2);

			Separator sep = new Separator(parent);
			sep.Width = parent.Width;
			sep.Height = 1;
			sep.Dock = DockStyle.Top;
			sep.Margins = new Margins(0, 0, 3, 6);
#else
			Separator sep = new Separator(parent);
//@			sep.PreferredWidth = parent.PreferredWidth;  // fen�tre trop grande
//@			//?sep.PreferredWidth = parent.ActualWidth;  // beaucoup d'asserts � l'ouverture
			sep.PreferredHeight = 1;
			sep.Dock = DockStyle.Top;
			sep.Margins = new Margins(0, 0, 6, 3);

			StaticText text = new StaticText(parent);
			text.Text = Misc.Bold(labelText);
			text.PreferredHeight = text.PreferredHeight + 2;
			text.Dock = DockStyle.Top;
			text.Margins = new Margins(10, 10, 2, 8);
#endif
		}

		public static void CreateSeparator(Widget parent)
		{
			//	Cr�e un s�parateur pour un onglet.
			Separator sep = new Separator(parent);
//@			sep.PreferredWidth = parent.PreferredWidth;  // fen�tre trop grande
//@			//?sep.PreferredWidth = parent.ActualWidth;  // beaucoup d'asserts � l'ouverture
			sep.PreferredHeight = 1;
			sep.Dock = DockStyle.Top;
			sep.Margins = new Margins(0, 0, 4, 6);
		}
		#endregion

		#region WidgetLabel
		public static void CreateLabel(Widget parent, string label, string info)
		{
			//	Cr�e des widgets pour afficher un texte fixe.
			Panel container = new Panel(parent);
			container.PreferredHeight = 18;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 0);

			StaticText text = new StaticText(container);
			text.Text = label;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			text = new StaticText(container);
			text.Text = info;
			text.PreferredWidth = 150;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);
		}
		#endregion

		#region WidgetBool
		protected void CreateBool(Widget parent, string name)
		{
			//	Cr�e un widget pour �diter un r�glage de type Bool.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			CheckButton check = new CheckButton(parent);
			check.Text = sBool.Text;
			check.PreferredWidth = 100;
			check.Name = sBool.Name;
			check.ActiveState = sBool.Value ? ActiveState.Yes : ActiveState.No;
			check.TabIndex = this.tabIndex++;
			check.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			check.Dock = DockStyle.Top;
			check.Margins = new Margins(10, 10, 0, 5);
			check.ActiveStateChanged += new EventHandler(this.HandleCheckActiveStateChanged);
			this.WidgetsTableAdd(check, "");
		}

		private void HandleCheckActiveStateChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			CheckButton check = sender as CheckButton;
			if ( check == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(check.Name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			sBool.Value = ( check.ActiveState == ActiveState.Yes );

			int total = this.document.Settings.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( setting.ConditionName == "" )  continue;

				if ( setting.ConditionName == sBool.Name )
				{
					this.EnableWidget(setting.Name, sBool.Value^setting.ConditionState);
				}
			}
		}

		protected void UpdateBool(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Bool sBool = settings as Settings.Bool;
			if ( sBool == null )  return;

			CheckButton button = this.WidgetsTableSearch(name, "") as CheckButton;
			if ( button != null )
			{
				button.Enable = (sBool.IsEnabled);
			}
		}
		#endregion

		#region WidgetDouble
		protected void CreateDouble(Widget parent, string name)
		{
			//	Cr�e des widgets pour �diter un r�glage de type Double.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			Panel container = new Panel(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, sDouble.Info?0:5);

			StaticText text = new StaticText(container);
			text.Text = sDouble.Text;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			TextFieldReal field = new TextFieldReal(container);
			field.PreferredWidth = 60;
			field.Name = sDouble.Name;
			field.TextSuffix = sDouble.Suffix;
			if ( sDouble.Integer )
			{
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.MinValue = (decimal) sDouble.FactorMinValue;
				field.MaxValue = (decimal) sDouble.FactorMaxValue;
				field.Resolution = (decimal) sDouble.FactorResolution;
				field.Step = (decimal) sDouble.FactorStep;
			}
			else
			{
				field.FactorMinRange = (decimal) sDouble.FactorMinValue;
				field.FactorMaxRange = (decimal) sDouble.FactorMaxValue;
				field.FactorStep = (decimal) sDouble.FactorStep;
				this.document.Modifier.AdaptTextFieldRealDimension(field);
			}
			field.InternalValue = (decimal) sDouble.Value;
			field.ValueChanged += new EventHandler(this.HandleFieldDoubleChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, "");

			if ( sDouble.Info )
			{
				container = new Panel(parent);
				container.PreferredHeight = 18;
				container.TabIndex = this.tabIndex++;
				container.Dock = DockStyle.Top;
				container.Margins = new Margins(10, 10, 0, 5);

				text = new StaticText(container);
				text.Name = sDouble.Name;
				text.Text = "<font size=\"80%\">" + sDouble.GetInfo() + "</font>";
				text.PreferredWidth = 150;
				text.Dock = DockStyle.Left;
				text.Margins = new Margins(120, 0, 0, 0);
				this.WidgetsTableAdd(text, ".Info");
			}
		}

		private void HandleFieldDoubleChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			sDouble.Value = (double) field.InternalValue;

			if ( sDouble.Info )
			{
				StaticText info = this.WidgetsTableSearch(sDouble.Name, ".Info") as StaticText;
				info.Text = "<font size=\"80%\">" + sDouble.GetInfo() + "</font>";
			}
		}

		protected void UpdateDouble(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Double sDouble = settings as Settings.Double;
			if ( sDouble == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(name, "") as TextFieldSlider;
			if ( field != null )
			{
				field.Enable = (sDouble.IsEnabled);
			}

			if ( sDouble.Info )
			{
				StaticText info = this.WidgetsTableSearch(name, ".Info") as StaticText;
				if ( info != null )
				{
					info.Visibility = (sDouble.IsEnabled);
				}
			}
		}
		#endregion

		#region WidgetPoint
		protected void CreatePoint(Widget parent, string name)
		{
			//	Cr�e des widgets pour �diter un r�glage de type Point.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			StaticText text;
			TextFieldReal field;

			Panel container = new Panel(parent);
			container.PreferredHeight = 22+2+22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 5);

			Panel containerXY = new Panel(container);
			containerXY.PreferredWidth = 120+60;
			containerXY.PreferredHeight = container.PreferredHeight;
			containerXY.TabIndex = this.tabIndex++;
			containerXY.Dock = DockStyle.Left;
			containerXY.Margins = new Margins(0, 0, 0, 0);

			Panel containerX = new Panel(containerXY);
			containerX.PreferredWidth = containerXY.PreferredWidth;
			containerX.PreferredHeight = 22;
			containerX.TabIndex = this.tabIndex++;
			containerX.Dock = DockStyle.Top;
			containerX.Margins = new Margins(0, 0, 0, 0);

			text = new StaticText(containerX);
			text.Text = sPoint.TextX;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			field = new TextFieldReal(containerX);
			field.PreferredWidth = 60;
			field.Name = sPoint.Name;
			if ( sPoint.Integer )
			{
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.MinValue = (decimal) sPoint.FactorMinValue;
				field.MaxValue = (decimal) sPoint.FactorMaxValue;
				field.Step = (decimal) sPoint.FactorStep;
			}
			else
			{
				field.FactorMinRange = (decimal) sPoint.FactorMinValue;
				field.FactorMaxRange = (decimal) sPoint.FactorMaxValue;
				field.FactorStep = (decimal) sPoint.FactorStep;
				this.document.Modifier.AdaptTextFieldRealDimension(field);
			}
			field.InternalValue = (decimal) sPoint.Value.X;
			field.ValueChanged += new EventHandler(this.HandleFieldPointXChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".X");

			Panel containerY = new Panel(containerXY);
			containerY.PreferredWidth = containerXY.PreferredWidth;
			containerY.PreferredHeight = 22;
			containerY.TabIndex = this.tabIndex++;
			containerY.Dock = DockStyle.Bottom;
			containerY.Margins = new Margins(0, 0, 0, 0);

			text = new StaticText(containerY);
			text.Text = sPoint.TextY;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			field = new TextFieldReal(containerY);
			field.PreferredWidth = 60;
			field.Name = sPoint.Name;
			if ( sPoint.Integer )
			{
				this.document.Modifier.AdaptTextFieldRealScalar(field);
				field.MinValue = (decimal) sPoint.FactorMinValue;
				field.MaxValue = (decimal) sPoint.FactorMaxValue;
				field.Step = (decimal) sPoint.FactorStep;
			}
			else
			{
				field.FactorMinRange = (decimal) sPoint.FactorMinValue;
				field.FactorMaxRange = (decimal) sPoint.FactorMaxValue;
				field.FactorStep = (decimal) sPoint.FactorStep;
				this.document.Modifier.AdaptTextFieldRealDimension(field);
			}
			field.InternalValue = (decimal) sPoint.Value.Y;
			field.ValueChanged += new EventHandler(this.HandleFieldPointYChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".Y");

			Separator sep = new Separator(container);
			sep.Name = sPoint.Name;
			sep.PreferredWidth = 1;
			sep.PreferredHeight = container.PreferredHeight;
			sep.Dock = DockStyle.Left;
			sep.Margins = new Margins(2, 0, 0, 0);
			this.WidgetsTableAdd(sep, ".SepLink");

			IconButton ib = new IconButton(container);
			ib.Name = sPoint.Name;
			DocumentDialogs.UpdateLink(ib, sep, sPoint.Link);
			ib.TabIndex = this.tabIndex++;
			ib.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ib.Dock = DockStyle.Left;
			double m = System.Math.Floor ((container.PreferredHeight-ib.PreferredHeight)/2);
			ib.Margins = new Margins(-1, 8, m, m);
			ib.Clicked += new MessageEventHandler(HandlePointActiveStateChanged);
			ToolTip.Default.SetToolTip(ib, Res.Strings.Dialog.Point.Link);
			this.WidgetsTableAdd(ib, ".Link");

			if ( sPoint.Doubler )
			{
				Button button;

				Panel containerD = new Panel(container);
				containerD.PreferredWidth = 33;
				containerD.TabIndex = this.tabIndex++;
				containerD.Dock = DockStyle.Left;
				containerD.Margins = new Margins(0, 0, 0, 0);

				Panel containerDX = new Panel(containerD);
				containerDX.PreferredWidth = containerD.PreferredWidth;
				containerDX.PreferredHeight = 22;
				containerDX.TabIndex = this.tabIndex++;
				containerDX.Dock = DockStyle.Top;
				containerDX.Margins = new Margins(0, 0, 0, 0);

				button = new Button(containerDX);
				button.ButtonStyle = ButtonStyle.Icon;
				button.PreferredWidth = 17;
				button.Name = sPoint.Name;
				button.Text = @"<font size=""80%"">" + "\u00F72</font>";  // /2
//-				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.Margins = new Margins(0, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointDivXClicked);
				this.WidgetsTableAdd(button, ".DoublerDivX");

				button = new Button(containerDX);
				button.ButtonStyle = ButtonStyle.Icon;
				button.PreferredWidth = 17;
				button.Name = sPoint.Name;
				button.Text = @"<font size=""80%"">" + "\u00D72</font>";  // x2
//-				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.Margins = new Margins(-1, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointMulXClicked);
				this.WidgetsTableAdd(button, ".DoublerMulX");

				Panel containerDY = new Panel(containerD);
				containerDY.PreferredWidth = containerD.PreferredWidth;
				containerDY.PreferredHeight = 22;
				containerDY.TabIndex = this.tabIndex++;
				containerDY.Dock = DockStyle.Top;
				containerDY.Margins = new Margins(0, 0, 0, 0);

				button = new Button(containerDY);
				button.ButtonStyle = ButtonStyle.Icon;
				button.PreferredWidth = 17;
				button.Name = sPoint.Name;
				button.Text = @"<font size=""80%"">" + "\u00F72</font>";  // /2
//-				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.Margins = new Margins(0, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointDivYClicked);
				this.WidgetsTableAdd(button, ".DoublerDivY");

				button = new Button(containerDY);
				button.ButtonStyle = ButtonStyle.Icon;
				button.PreferredWidth = 17;
				button.Name = sPoint.Name;
				button.Text = @"<font size=""80%"">" + "\u00D72</font>";  // x2
//-				button.SetClientZoom(0.8);
				button.TabIndex = this.tabIndex++;
				button.TabNavigationMode = TabNavigationMode.ActivateOnTab;
				button.Dock = DockStyle.Left;
				button.Margins = new Margins(-1, 0, 2, 2);
				button.Clicked += new MessageEventHandler(HandleDoublerPointMulYClicked);
				this.WidgetsTableAdd(button, ".DoublerMulY");
			}
		}

		private void HandleFieldPointXChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			sPoint.ValueX = (double) field.InternalValue;
		}

		private void HandleFieldPointYChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			sPoint.ValueY = (double) field.InternalValue;
	}

		private void HandlePointActiveStateChanged(object sender, MessageEventArgs e)
		{
			IconButton ib = sender as IconButton;
			if ( ib == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(ib.Name);
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;

			sPoint.Link = !sPoint.Link;
			Separator sep = this.WidgetsTableSearch(settings.Name, ".SepLink") as Separator;
			DocumentDialogs.UpdateLink(ib, sep, sPoint.Link);
		}

		protected static void UpdateLink(IconButton ib, Separator sep, bool state)
		{
			if ( state )
			{
				ib.ActiveState = ActiveState.Yes;
				ib.IconName = Misc.Icon("Linked");
				sep.Enable = true;
			}
			else
			{
				ib.ActiveState = ActiveState.No;
				ib.IconName = Misc.Icon("Unlinked");
				sep.Enable = false;
			}
		}

		private void HandleDoublerPointDivXClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".X")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value/2.0M;

		}

		private void HandleDoublerPointMulXClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".X")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value*2.0M;

		}

		private void HandleDoublerPointDivYClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".Y")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value/2.0M;

		}

		private void HandleDoublerPointMulYClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			if ( button == null )  return;

			TextFieldSlider field = this.WidgetsTableSearch(button.Name, ".Y")as TextFieldSlider;
			if ( field == null )  return;
			field.Value = field.Value*2.0M;

		}
		#endregion

		#region WidgetCombo
		protected void CreateCombo(Widget parent, string name)
		{
			//	Cr�e un widget combo pour �diter un r�glage de type Integer.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Integer sInteger = settings as Settings.Integer;
			if ( sInteger == null )  return;

			Panel container = new Panel(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 5);

			StaticText text = new StaticText(container);
			text.Text = sInteger.Text;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.PreferredWidth = 140;
			field.IsReadOnly = true;
			field.Name = sInteger.Name;
			sInteger.InitCombo(field);
			field.SelectedIndexChanged += new EventHandler(this.HandleFieldComboChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, "");
		}

		private void HandleFieldComboChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Integer sInteger = settings as Settings.Integer;
			if ( sInteger == null )  return;

			sInteger.Value = sInteger.RankToType(field.SelectedIndex);

			if ( sInteger.Name == "ExportPDFImageCompression" )
			{
				this.UpdateDouble("ExportPDFJpegQuality");
			}
		}

		protected void UpdateCombo(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Integer sInteger = settings as Settings.Integer;
			if ( sInteger == null )  return;

			TextFieldCombo combo = this.WidgetsTableSearch(name, "") as TextFieldCombo;
			if ( combo == null )  return;

			sInteger.InitCombo(combo);
		}
		#endregion

		#region WidgetFilename
		protected void CreateFilename(Widget parent, string name)
		{
			//	Cr�e des widgets pour choisir un nom de fichier.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			Panel container = new Panel(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 5);

			TextField field = new TextField(container);
			field.Name = sString.Name;
			field.Text = sString.Value;
			field.PreferredWidth = 177;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			field.TextChanged += new EventHandler(this.HandleFilenameTextChanged);
			this.WidgetsTableAdd(field, "");

			Button button = new Button(container);
			button.Name = sString.Name;
			button.Text = Res.Strings.Dialog.Button.Browse;
			button.PreferredWidth = 80;
			button.Dock = DockStyle.Left;
			button.Margins = new Margins(3, 0, 0, 0);
			button.Clicked += new MessageEventHandler(HandleFilenameButtonClicked);
			this.WidgetsTableAdd(button, ".Button");
		}

		private void HandleFilenameTextChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextField field = sender as TextField;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			sString.Value = field.Text;
		}

		private void HandleFilenameButtonClicked(object sender, MessageEventArgs e)
		{
			Common.Dialogs.FileSave dialog = new Common.Dialogs.FileSave();
			dialog.FileName = this.document.Settings.PrintInfo.PrintFilename;
			dialog.Title = Res.Strings.Dialog.Print.ToFile.Title;
			dialog.Filters.Add("prn", TextLayout.ConvertToSimpleText(Res.Strings.Dialog.Print.ToFile.Type), "*.prn");
			dialog.PromptForOverwriting = true;
			dialog.Owner = this.windowPrint;
			dialog.OpenDialog();
			if ( dialog.Result != Common.Dialogs.DialogResult.Accept )  return;

			this.document.Settings.PrintInfo.PrintFilename = dialog.FileName;
			this.document.Settings.PrintInfo.PrintToFile = true;
			this.UpdatePrint(false);
		}
		#endregion

		#region WidgetPaper
		protected void CreatePaper(Widget parent)
		{
			//	Cr�e un widget combo pour �diter le format d'une page.
			Panel container = new Panel(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 0);

			StaticText text = new StaticText(container);
			text.Text = Res.Strings.Dialog.Print.Paper.Direction;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			RadioButton radio = new RadioButton(container);
			radio.PreferredWidth = 65;
			radio.Name = "PaperFormat.Portrait";
			radio.Text = Res.Strings.Dialog.Print.Paper.Portrait;
			radio.Clicked += new MessageEventHandler(this.HandlePaperActiveStateChanged);
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			radio.Margins = new Margins(0, 0, 0, 0);
			radio.Index = 1;
			this.WidgetsTableAdd(radio, "");
			
			radio = new RadioButton(container);
			radio.PreferredWidth = 75;
			radio.Name = "PaperFormat.Landscape";
			radio.Text = Res.Strings.Dialog.Print.Paper.Landscape;
			radio.Clicked += new MessageEventHandler(this.HandlePaperActiveStateChanged);
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			radio.Index = 2;
			this.WidgetsTableAdd(radio, "");

			container = new Panel(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 5);

			text = new StaticText(container);
			text.Text = Res.Strings.Dialog.Print.Paper.PaperList;
			text.PreferredWidth = 120;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			TextFieldCombo field = new TextFieldCombo(container);
			field.PreferredWidth = 140;
			field.IsReadOnly = true;
			field.Name = "PaperFormat";

			field.Items.Add(Res.Strings.Dialog.Print.Format.User);
			foreach (string format in DocumentDialogs.PaperList)
			{
				field.Items.Add(format);
			}

			field.SelectedIndexChanged += new EventHandler(this.HandleFieldPaperChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, "");

			this.UpdatePaper();
		}

		private void HandleFieldPaperChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldCombo field = sender as TextFieldCombo;
			if ( field == null )  return;

			int sel = field.SelectedIndex;
			if ( sel == 0 )  return;  // personnalis� ?

			Settings.Abstract settings = this.document.Settings.Get("PageSize");
			if ( settings == null )  return;
			Settings.Point sPoint = settings as Settings.Point;
			if ( sPoint == null )  return;
			sPoint.Link = false;

			Size size = DocumentDialogs.PaperRankToSize(sel);
			RadioButton radio = this.WidgetsTableSearch("PaperFormat.Landscape", "") as RadioButton;
			if ( radio != null && radio.ActiveState == ActiveState.Yes )
			{
				DocumentDialogs.SwapSize(ref size);
			}
			this.document.DocumentSize = size;
		}

		private void HandlePaperActiveStateChanged(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;

			Size size = this.document.DocumentSize;
			if ( radio.Name == "PaperFormat.Portrait" )
			{
				if ( size.Width > size.Height )
				{
					DocumentDialogs.SwapSize(ref size);
					this.document.DocumentSize = size;
				}
			}
			else
			{
				if ( size.Width < size.Height )
				{
					DocumentDialogs.SwapSize(ref size);
					this.document.DocumentSize = size;
				}
			}
		}

		protected void UpdatePaper()
		{
			bool initial = this.ignoreChanged;
			this.ignoreChanged = true;

			TextFieldCombo combo = this.WidgetsTableSearch("PaperFormat", "") as TextFieldCombo;
			if ( combo != null )
			{
				combo.SelectedIndex = DocumentDialogs.PaperSizeToRank(this.document.DocumentSize);
			}

			RadioButton radio;
			bool portrait = (this.document.DocumentSize.Width <= this.document.DocumentSize.Height);

			radio = this.WidgetsTableSearch("PaperFormat.Portrait", "") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = portrait ? ActiveState.Yes : ActiveState.No;
			}

			radio = this.WidgetsTableSearch("PaperFormat.Landscape", "") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = !portrait ? ActiveState.Yes : ActiveState.No;
			}

			this.ignoreChanged = initial;
		}

		public static string PaperFormat(Size pageSize)
		{
			//	Retourne le format en clair, s'il existe.
			int i = DocumentDialogs.PaperSizeToRank(pageSize);
			if (i == 0)
			{
				return null;
			}
			else
			{
				return DocumentDialogs.PaperList[i-1];
			}
		}

		protected static string[] PaperList =
		{
			"Slide",
			"Letter US",
			"Legal",
			"Tablo�d",
			"Letter - Half",
			"Legal - Half",
			"Executive US",
			"Listing",
			"Poster",
			"A1",
			"A2",
			"A3+",
			"A3",
			"A4",
			"A5",
			"A6",
			"B1 (ISO)",
			"B4 (ISO)",
			"B5 (ISO)",
			"B4 (JIS)",
			"B5 (JIS)",
			"C3",
			"C4",
			"C5",
			"C6",
			"RA2",
			"RA3",
			"RA4",
			"DL",
		};

		protected static Size PaperRankToSize(int rank)
		{
			Size size = new Size(500.0, 500.0);
			switch ( rank )
			{
				case  1:  size = new Size(279.400, 186.182);  break;  // Slide
				case  2:  size = new Size(215.900, 279.400);  break;  // Lettre US
				case  3:  size = new Size(215.900, 355.600);  break;  // Legal
				case  4:  size = new Size(279.400, 431.800);  break;  // Tablo�d
				case  5:  size = new Size(139.700, 215.900);  break;  // Letter - Half
				case  6:  size = new Size(177.800, 215.900);  break;  // Legal - Half
				case  7:  size = new Size(184.150, 266.700);  break;  // Executive US
				case  8:  size = new Size(279.400, 377.952);  break;  // Listing
				case  9:  size = new Size(457.200, 609.600);  break;  // Poster
				case 10:  size = new Size(594.000, 841.000);  break;  // A1
				case 11:  size = new Size(420.000, 594.000);  break;  // A2
				case 12:  size = new Size(329.000, 483.000);  break;  // A3+
				case 13:  size = new Size(297.000, 420.000);  break;  // A3
				case 14:  size = new Size(210.000, 297.000);  break;  // A4
				case 15:  size = new Size(148.000, 210.000);  break;  // A5
				case 16:  size = new Size(105.000, 148.000);  break;  // A6
				case 17:  size = new Size(707.000,1000.000);  break;  // B1 (ISO)
				case 18:  size = new Size(250.000, 353.000);  break;  // B4 (ISO)
				case 19:  size = new Size(176.000, 250.000);  break;  // B5 (ISO)
				case 20:  size = new Size(257.000, 364.000);  break;  // B4 (JIS)
				case 21:  size = new Size(182.000, 257.000);  break;  // B5 (JIS)
				case 22:  size = new Size(324.000, 458.000);  break;  // C3
				case 23:  size = new Size(229.000, 324.000);  break;  // C4
				case 24:  size = new Size(162.000, 229.000);  break;  // C5
				case 25:  size = new Size(114.000, 162.000);  break;  // C6
				case 26:  size = new Size(430.000, 610.000);  break;  // RA2
				case 27:  size = new Size(305.000, 430.000);  break;  // RA3
				case 28:  size = new Size(215.000, 305.000);  break;  // RA4
				case 29:  size = new Size(220.000, 110.000);  break;  // DL
			}
			return size*10.0;
		}

		protected static int PaperSizeToRank(Size size)
		{
			for ( int rank=1 ; rank<=DocumentDialogs.PaperList.Length ; rank++ )
			{
				Size paper = DocumentDialogs.PaperRankToSize(rank);
				if ( size.Width == paper.Width  && size.Height == paper.Height )  return rank;
				if ( size.Width == paper.Height && size.Height == paper.Width  )  return rank;
			}
			return 0;
		}

		protected static void SwapSize(ref Size size)
		{
			double temp = size.Width;
			size.Width = size.Height;
			size.Height = temp;
		}
		#endregion

		#region WidgetPrinter
		protected void CreatePrinter(Widget parent, string name)
		{
			//	Cr�e des widgets pour choisir un nom d'imprimante.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			Panel container = new Panel(parent);
			container.PreferredHeight = 22;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 5);

			TextFieldCombo field = new TextFieldCombo(container);
			field.Name = sString.Name;
			field.IsReadOnly = true;
			field.Text = this.document.Settings.PrintInfo.PrintName;
			field.PreferredWidth = 177;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			field.ComboOpening += new EventHandler<CancelEventArgs> (this.HandlePrinterComboOpening);
			field.ComboClosed += new EventHandler(this.HandlePrinterComboClosed);
			this.WidgetsTableAdd(field, "");

			Button button = new Button(container);
			button.Name = sString.Name;
			button.Text = Res.Strings.Dialog.Print.Properties;
			button.PreferredWidth = 80;
			button.Dock = DockStyle.Left;
			button.Margins = new Margins(3, 0, 0, 0);
			button.Clicked += new MessageEventHandler(HandlePrinterButtonClicked);
			this.WidgetsTableAdd(button, ".Button");
		}

		private void HandlePrinterComboOpening(object sender, CancelEventArgs e)
		{
			TextFieldCombo field = sender as TextFieldCombo;
			field.Items.Clear();

			string[] installed = Common.Printing.PrinterSettings.InstalledPrinters;
			System.Collections.ArrayList list = new System.Collections.ArrayList();
			foreach ( string name in installed )
			{
				list.Add(name);
			}
			list.Sort();

			foreach ( string name in list )
			{
				field.Items.Add(name);
			}
		}

		private void HandlePrinterComboClosed(object sender)
		{
			TextFieldCombo field = sender as TextFieldCombo;
			this.document.Settings.PrintInfo.PrintName = field.Text;
		}

		private void HandlePrinterButtonClicked(object sender, MessageEventArgs e)
		{
			Button button = sender as Button;
			Common.Dialogs.Print dialog = this.document.PrintDialog;
			Settings.PrintInfo pi = this.document.Settings.PrintInfo;
			
			if ( dialog.Document.PrinterSettings.PrinterName != pi.PrintName )
			{
				dialog.Document.SelectPrinter(pi.PrintName);
			}
			
			dialog.Owner = button.Window;
			dialog.OpenDialog();

			if ( dialog.Result == Common.Dialogs.DialogResult.Accept )
			{
				pi.PrintName = dialog.Document.PrinterSettings.PrinterName;
				this.UpdatePrint(false);
			}
		}

		protected void UpdatePrinter(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.String sString = settings as Settings.String;
			if ( sString == null )  return;

			Common.Dialogs.Print dialog = this.document.PrintDialog;
			TextFieldCombo field = this.WidgetsTableSearch(name, "") as TextFieldCombo;
			field.Text = this.document.Settings.PrintInfo.PrintName;
		}
		#endregion

		#region WidgetRange
		protected void CreateRange(Widget parent, string name)
		{
			//	Cr�e des widgets pour choisir les pages � imprimer.
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			RadioButton radio;
			TextFieldReal field;

			radio = new RadioButton(parent);
			radio.Text = Res.Strings.Dialog.Print.Range.All;
			radio.PreferredHeight = 20;
			radio.PreferredWidth = 100;
			radio.Name = sRange.Name;
			radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.All) ? ActiveState.Yes : ActiveState.No;
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Top;
			radio.Margins = new Margins(10, 10, 0, 0);
			radio.ActiveStateChanged += new EventHandler(this.HandleRangeRadioActiveStateChanged);
			radio.Index = 1;
			this.WidgetsTableAdd(radio, ".All");
			
			//	d�but from-to
			Panel container = new Panel(parent);
			container.PreferredHeight = 20;
			container.TabIndex = this.tabIndex++;
			container.Dock = DockStyle.Top;
			container.Margins = new Margins(10, 10, 0, 0);

			radio = new RadioButton(container);
			radio.Text = Res.Strings.Dialog.Print.Range.From;
			radio.PreferredHeight = 20;
			radio.PreferredWidth = 85;
			radio.Name = sRange.Name;
			radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.FromTo) ? ActiveState.Yes : ActiveState.No;
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Left;
			radio.Margins = new Margins(0, 0, 0, 0);
			radio.ActiveStateChanged += new EventHandler(this.HandleRangeRadioActiveStateChanged);
			radio.Index = 2;
			this.WidgetsTableAdd(radio, ".FromTo");
			
			field = new TextFieldReal(container);
			field.PreferredHeight = 20;
			field.PreferredWidth = 50;
			field.Name = sRange.Name;
			this.document.Modifier.AdaptTextFieldRealScalar(field);
			field.MinValue = (decimal) sRange.Min;
			field.MaxValue = (decimal) sRange.Max;
			field.Step = 1M;
			field.InternalValue = (decimal) sRange.From;
			field.ValueChanged += new EventHandler(this.HandleRangeFieldChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".From");

			StaticText text = new StaticText(container);
			text.Text = Res.Strings.Dialog.Print.Range.To;
			text.ContentAlignment = ContentAlignment.MiddleCenter;
			text.PreferredHeight = 20;
			text.PreferredWidth = 30;
			text.Dock = DockStyle.Left;
			text.Margins = new Margins(0, 0, 0, 0);

			field = new TextFieldReal(container);
			field.PreferredHeight = 20;
			field.PreferredWidth = 50;
			field.Name = sRange.Name;
			this.document.Modifier.AdaptTextFieldRealScalar(field);
			field.MinValue = (decimal) sRange.Min;
			field.MaxValue = (decimal) sRange.Max;
			field.Step = 1M;
			field.InternalValue = (decimal) sRange.To;
			field.ValueChanged += new EventHandler(this.HandleRangeFieldChanged);
			field.TabIndex = this.tabIndex++;
			field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			field.Dock = DockStyle.Left;
			field.Margins = new Margins(0, 0, 0, 0);
			this.WidgetsTableAdd(field, ".To");
			//	fin from-to

			radio = new RadioButton(parent);
			radio.Text = Res.Strings.Dialog.Print.Range.Current;
			radio.PreferredHeight = 20;
			radio.PreferredWidth = 100;
			radio.Name = sRange.Name;
			radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.Current) ? ActiveState.Yes : ActiveState.No;
			radio.TabIndex = this.tabIndex++;
			radio.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			radio.Dock = DockStyle.Top;
			radio.Margins = new Margins(10, 10, 0, 5);
			radio.ActiveStateChanged += new EventHandler(this.HandleRangeRadioActiveStateChanged);
			radio.Index = 3;
			this.WidgetsTableAdd(radio, ".Current");

			this.UpdateRangeRadio(name);
		}

		private void HandleRangeRadioActiveStateChanged(object sender)
		{
			RadioButton radio = sender as RadioButton;
			if ( radio == null )  return;
			if ( radio.ActiveState != ActiveState.Yes )  return;

			Settings.Abstract settings = this.document.Settings.Get(radio.Name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			if ( radio == this.WidgetsTableSearch(radio.Name, ".All") )
			{
				sRange.PrintRange = Settings.PrintRange.All;
			}
			if ( radio == this.WidgetsTableSearch(radio.Name, ".FromTo") )
			{
				sRange.PrintRange = Settings.PrintRange.FromTo;
			}
			if ( radio == this.WidgetsTableSearch(radio.Name, ".Current") )
			{
				sRange.PrintRange = Settings.PrintRange.Current;
			}

			this.UpdateRangeRadio(radio.Name);
		}

		private void HandleRangeFieldChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			TextFieldReal field = sender as TextFieldReal;
			if ( field == null )  return;

			Settings.Abstract settings = this.document.Settings.Get(field.Name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			if ( field == this.WidgetsTableSearch(field.Name, ".From") )
			{
				sRange.From = (int) field.InternalValue;
			}
			if ( field == this.WidgetsTableSearch(field.Name, ".To") )
			{
				sRange.To = (int) field.InternalValue;
			}

			sRange.PrintRange = Settings.PrintRange.FromTo;
			this.UpdateRangeRadio(field.Name);
		}

		protected void UpdateRangeRadio(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			RadioButton radio;

			radio = this.WidgetsTableSearch(name, ".All") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.All) ? ActiveState.Yes : ActiveState.No;
			}

			radio = this.WidgetsTableSearch(name, ".FromTo") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.FromTo) ? ActiveState.Yes : ActiveState.No;
			}

			radio = this.WidgetsTableSearch(name, ".Current") as RadioButton;
			if ( radio != null )
			{
				radio.ActiveState = (sRange.PrintRange == Settings.PrintRange.Current) ? ActiveState.Yes : ActiveState.No;
			}
		}

		protected void UpdateRangeField(string name)
		{
			Settings.Abstract settings = this.document.Settings.Get(name);
			if ( settings == null )  return;
			Settings.Range sRange = settings as Settings.Range;
			if ( sRange == null )  return;

			TextFieldReal field;

			field = this.WidgetsTableSearch(name, ".From") as TextFieldReal;
			if ( field != null )
			{
				this.UpdateRangeField(field, sRange, sRange.From);
			}

			field = this.WidgetsTableSearch(name, ".To") as TextFieldReal;
			if ( field != null )
			{
				this.UpdateRangeField(field, sRange, sRange.To);
			}
		}

		protected void UpdateRangeField(TextFieldReal field, Settings.Range sRange, int value)
		{
			field.MinValue = (decimal) sRange.Min;
			field.MaxValue = (decimal) sRange.Max;

			value = System.Math.Max(value, sRange.Min);
			value = System.Math.Min(value, sRange.Max);
			this.ignoreChanged = true;
			field.InternalValue = (decimal) value;
			this.ignoreChanged = false;
		}
		#endregion

		public void FlushAll()
		{
			//	Supprime tous les widgets de tous les dialogues.
			if ( this.windowInfos != null )
			{
				TextFieldMulti multi = this.windowInfos.Root.FindChild("Infos") as TextFieldMulti;
				if ( multi != null )
				{
					multi.Text = "";
				}
				this.windowInfos = null;
			}

			if ( this.windowSettings != null )
			{
				this.containerGuides.SetParent(null);
				this.containerFonts.SetParent(null);  // il ne faut pas d�truire ces widgets

				Widget parent = this.windowSettings.Root.FindChild("BookDocument");
				this.DeletePage(parent, "Format");
				this.DeletePage(parent, "Grid");
				this.DeletePage(parent, "Guides");
				this.DeletePage(parent, "Move");
				this.DeletePage(parent, "Misc");
				this.DeletePage(parent, "Fonts");
				this.windowSettings = null;
			}

			if ( this.windowPrint != null )
			{
				Widget parent = this.windowPrint.Root.FindChild("Book");
				this.DeletePage(parent, "Printer");
				this.DeletePage(parent, "Param");
				this.DeletePage(parent, "Image");
				this.DeletePage(parent, "Publisher");
				this.windowPrint = null;
			}

			if ( this.windowExport != null )
			{
				Widget parent = this.windowExport.Root.FindChild("Panel");
				this.DeleteContainer(parent, "Container");
				this.windowExport = null;
			}

			if ( this.windowExportPDF != null )
			{
				Widget parent = this.windowExportPDF.Root.FindChild("Book");
				this.DeletePage(parent, "Generic");
				this.DeletePage(parent, "Color");
				this.DeletePage(parent, "Image");
				this.DeletePage(parent, "Publisher");
				this.windowExportPDF = null;
			}

			if ( this.windowExportICO != null )
			{
				Widget parent = this.windowExportICO.Root.FindChild("Book");
				this.DeletePage(parent, "Generic");
				this.DeletePage(parent, "Image");
				this.windowExportICO = null;
			}

			this.widgetsTable.Clear();
		}

		protected void DeletePage(Widget parent, string name)
		{
			Widget container = parent.FindChild(name);
			if ( container != null )
			{
				Widget page = container.FindChild("Container");
				if ( page != null )
				{
					page.Dispose();
				}
			}
		}

		protected void DeleteContainer(Widget parent, string name)
		{
			Widget container = parent.FindChild(name);
			if ( container != null )
			{
				container.Dispose();
			}
		}

		protected void UpdateDialogSettings(string dialog)
		{
			//	Met � jour tous les widgets d'un dialogue.
			this.ignoreChanged = true;

			int total = this.document.Settings.Count;
			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( dialog != this.document.Settings.GetOwnerDialog(setting.Name) )  continue;

				if ( setting is Settings.Bool )
				{
					Settings.Bool sBool = setting as Settings.Bool;

					CheckButton check = this.WidgetsTableSearch(setting.Name, "") as CheckButton;
					if ( check != null )
					{
						check.ActiveState = sBool.Value ? ActiveState.Yes : ActiveState.No;
					}
				}

				if ( setting is Settings.Integer )
				{
					Settings.Integer sInteger = setting as Settings.Integer;

					TextFieldCombo combo = this.WidgetsTableSearch(setting.Name, "") as TextFieldCombo;
					if ( combo != null )
					{
						combo.SelectedIndex = sInteger.TypeToRank(sInteger.Value);
					}
				}

				if ( setting is Settings.Double )
				{
					Settings.Double sDouble = setting as Settings.Double;

					TextFieldReal field = this.WidgetsTableSearch(setting.Name, "") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sDouble.Value;
					}
				}

				if ( setting is Settings.Point )
				{
					Settings.Point sPoint = setting as Settings.Point;
					TextFieldReal field;

					field = this.WidgetsTableSearch(setting.Name, ".X") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sPoint.Value.X;
					}

					field = this.WidgetsTableSearch(setting.Name, ".Y") as TextFieldReal;
					if ( field != null )
					{
						field.InternalValue = (decimal) sPoint.Value.Y;
					}

					IconButton ib = this.WidgetsTableSearch(setting.Name, ".Link") as IconButton;
					Separator sep = this.WidgetsTableSearch(setting.Name, ".SepLink") as Separator;
					if ( ib != null && sep != null )
					{
						DocumentDialogs.UpdateLink(ib, sep, sPoint.Link);
						ib.ActiveState = sPoint.Link ? ActiveState.Yes : ActiveState.No;
					}

					if ( setting.Name == "PageSize" )
					{
						this.UpdatePaper();
					}
				}

				if ( setting is Settings.String )
				{
					Settings.String sString = setting as Settings.String;

					TextField field = this.WidgetsTableSearch(setting.Name, "") as TextField;
					if ( field != null )
					{
						field.Text = sString.Value;
					}
				}

				if ( setting is Settings.Range )
				{
					Settings.Range sRange = setting as Settings.Range;

					this.UpdateRangeRadio(sRange.Name);
					this.UpdateRangeField(sRange.Name);
				}
			}

			for ( int i=0 ; i<total ; i++ )
			{
				Settings.Abstract setting = this.document.Settings.Get(i);
				if ( setting.ConditionName == "" )  continue;
				if ( dialog != this.document.Settings.GetOwnerDialog(setting.Name) )  continue;

				Settings.Bool sBool = this.document.Settings.Get(setting.ConditionName) as Settings.Bool;
				if ( sBool == null )  continue;
				this.EnableWidget(setting.Name, sBool.Value^setting.ConditionState);
			}

			this.document.Modifier.AdaptAllTextFieldReal();
			this.ignoreChanged = false;
		}

		protected void EnableWidget(string name, bool enabled)
		{
			//	Modifie l'�tat d'un widget.
			Widget widget = this.WidgetsTableSearch(name, "");
			if ( widget == null )  return;
			widget.Enable = (enabled);
		}

		protected void WidgetsTableAdd(Widget widget, string option)
		{
			//	Ajoute un widget dans la table.
			this.widgetsTable.Add(widget.Name+option, widget);
		}

		protected Widget WidgetsTableSearch(string name, string option)
		{
			//	Cherche un widget dans la table.
			return this.widgetsTable[name+option] as Widget;
		}


		protected Document						document;
		protected Window						windowInfos;
		protected Window						windowSettings;
		protected Window						windowPrint;
		protected Window						windowExport;
		protected Window						windowExportPDF;
		protected Window						windowExportICO;
		protected Window						windowGlyphs;
		protected Containers.Guides				containerGuides;
		protected Containers.Fonts				containerFonts;
		protected System.Collections.Hashtable	widgetsTable;
		protected bool							ignoreChanged = false;
		protected int							tabIndex;
	}
}
