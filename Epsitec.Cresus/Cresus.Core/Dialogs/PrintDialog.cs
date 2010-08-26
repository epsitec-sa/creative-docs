//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{

	/// <summary>
	/// The <c>PrintDialog</c> class is a dialog which lets the user choose on which printer
	/// he wants to print the bv and how much copies does he want to print.
	/// </summary>
	class PrintDialog : AbstractDialog
	{
		public PrintDialog(CoreApplication application, Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities, List<Printer> printers)
		{
			this.application   = application;
			this.entityPrinter = entityPrinter;
			this.entities      = entities;
			this.printers      = printers;
		}


		/// <summary>
		/// Creates a <see cref="Window"/> for the current dialog.
		/// </summary>
		/// <returns>The created <see cref="Window"/>.</returns>
		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow (window);
			this.SetupWidgets (window);
			this.SetupEvents (window);

			window.AdjustWindowSize ();

			return window;
		}

		/// <summary>
		/// Sets up the properties of the <see cref="PrintDialog.window"/> property of this
		/// instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.application.Window;
			window.Icon = this.application.Window.Icon;
			window.Text = "Imprimer";
			window.WindowSize = new Size (100, 100);
			window.MakeFixedSizeWindow ();
		}

		/// <summary>
		/// Sets up the <see cref="Widget"/>s of the <see cref="PrintDialog"/>, such as the
		/// <see cref="Button"/>s and <see cref="TexfField"/>s.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWidgets(Window window)
		{
			FrameBox frameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Top,
				Parent = window.Root
			};

			GroupBox printerGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 10, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = frameBox,
				Text = "Imprimante",
				TabIndex = 1,
			};

			this.printerTextField = new TextFieldCombo ()
			{
				Dock = DockStyle.Stacked,
				Parent = printerGroupBox,
				PreferredWidth = 200,
				TabIndex = 1,
				IsReadOnly = true,
			};

			this.printers.ForEach (printer => this.printerTextField.Items.Add (printer.PhysicalName));

			string preferredPrinter;
			bool preferredPrinterDefined = Settings.Load ().TryGetValue ("preferredPrinter", out preferredPrinter);

			if (preferredPrinterDefined && this.printers.Any (printer => printer.PhysicalName == preferredPrinter))
			{
				this.printerTextField.Text = FormattedText.Escape (preferredPrinter);
			}
			else
			{
				this.printerTextField.Text = FormattedText.Escape (this.printers[0].PhysicalName);
			}

			GroupBox nbPagesGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = frameBox,
				Text = "Nombre de copies",
				TabIndex = 2,
			};

			this.nbCopiesTextField = new TextFieldUpDown ()
			{
				Dock = DockStyle.Stacked,
				Parent = nbPagesGroupBox,
				PreferredWidth = 200,
				Resolution = 1,
				Step = 1,
				TabIndex = 1,
				Text = "1",
			};

			this.UpdateNbPagesRange ();

			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Stacked,
				Parent = frameBox,
				TabIndex = 3,
			};

			this.printButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (10, 2, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Imprimer",
				TabIndex = 1,
			};

			this.previewButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (0, 12, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Aperçu",
				TabIndex = 2,
			};

			this.cancelButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (0, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Annuler",
				TabIndex = 3,
			};
		}

		/// <summary>
		/// Sets up the <see cref="Event"/>s of this instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupEvents(Window window)
		{
			this.printerTextField.TextChanged += (sender) => this.UpdateNbPagesRange ();
			this.printerTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.nbCopiesTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.cancelButton.Clicked += (sender, e) => this.CloseDialog ();
			this.printButton.Clicked += (sender, e) => this.Print ();
			this.previewButton.Clicked += (sender, e) => this.Preview ();
		}

		/// <summary>
		/// Updates the range of <see cref="PrintDialog.NbCopiesTextField"/> based on the value
		/// of <see cref="PrintDialog.PrinterTextField"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="PrintDialog.PrinterTextField"/> changes.
		/// </remarks>
		protected void UpdateNbPagesRange()
		{
			PrinterSettings printer = PrinterSettings.FindPrinter(FormattedText.Unescape (this.printerTextField.Text));

			this.nbCopiesTextField.MinValue = 1;
			this.nbCopiesTextField.MaxValue = printer.MaximumCopies;
		}

		/// <summary>
		/// Enables or disables <see cref="PrintDialog.PrintButton"/> according to the validity of
		/// <see cref="PrintDialog.PrinterTextField"/> and <see cref="PrintDialog.NbCopiesTextField"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="PrintDialog.PrinterTextField"/>
		/// and <see cref="PrintDialog.NbCopiesTextField"/> changes.
		/// </remarks>
		protected void CheckPrintEnabled()
		{
			this.printButton.Enable = this.nbCopiesTextField.IsValid;	
		}

		protected void Print()
		{
			Printer printer = this.printers.Find (p => p.PhysicalName == FormattedText.Unescape (this.printerTextField.Text));
			PrinterSettings printerSettings = PrinterSettings.FindPrinter (printer.PhysicalName);
			
			bool checkTray = printerSettings.PaperSources.Any (tray => (tray.Name == printer.Tray));

			if (checkTray)
			{
				try
				{
					foreach (var entity in this.entities)
					{
						this.PrintEntities (this.entityPrinter, entity);
					}

					this.LogPrint ();
				}
				catch (System.Exception e)
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une erreur s'est produite lors de l'impression").OpenDialog ();
					ErrorLogger.LogException (new System.Exception ("An error occured while printing.", e));
				}
				finally
				{
					Settings settings = Settings.Load ();
					settings["preferredPrinter"] = FormattedText.Unescape (this.printerTextField.Text);
					settings.Save ();
					this.CloseDialog ();
				}
			}
			else
			{
				string message = string.Format ("Le bac ({0}) de l'imprimante séléctionnée ({1}) n'existe pas.", printer.Tray, printer.PhysicalName);
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
			}
		}

		protected void PrintEntities(Printers.AbstractEntityPrinter entityPrinter, AbstractEntity entity)
		{
			Printer printer = this.printers.Find (p => p.PhysicalName == FormattedText.Unescape (this.printerTextField.Text));
			PrintDocument printDocument = new PrintDocument();

			printDocument.DocumentName = entityPrinter.JobName;
			printDocument.SelectPrinter(printer.PhysicalName);
			printDocument.PrinterSettings.Copies = int.Parse (FormattedText.Unescape (this.nbCopiesTextField.Text), CultureInfo.InvariantCulture);
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);
			printDocument.DefaultPageSettings.PaperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, paperSource => paperSource.Name == printer.Tray);

			Size size = entityPrinter.PageSize;
			double height = size.Height;
			double width  = size.Width;

			double xOffset = printer.XOffset;
			double yOffset = printer.YOffset;

			entityPrinter.BuildSections (null);

			Transform transform;

			if (entityPrinter.PageSize.Width < entityPrinter.PageSize.Height)  // portrait ?
			{
				transform = Transform.Identity;
			}
			else  // paysage ?
			{
				transform = Transform.CreateRotationDegTransform (90, entityPrinter.PageSize.Height/2, entityPrinter.PageSize.Height/2);
			}

			var engine = new MultiPagePrintEngine (entityPrinter, transform);
			printDocument.Print (engine);
		}

	
		protected void Preview()
		{
			this.CloseDialog ();

			var dialog = new Dialogs.PreviewDialog (CoreProgram.Application, this.entityPrinter, this.entities);
			dialog.IsModal = false;
			dialog.OpenDialog ();
		}


		protected void LogPrint()
		{
		}


		private readonly CoreApplication application;
		private readonly IEnumerable<AbstractEntity> entities;
		private readonly Printers.AbstractEntityPrinter entityPrinter;
		private readonly List<Printer> printers;

		private Button printButton;
		private Button previewButton;
		private Button cancelButton;
		private TextFieldUpDown nbCopiesTextField;
		private TextFieldCombo printerTextField;
	}
}
