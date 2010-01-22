//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.UI;


namespace Epsitec.App.BanquePiguet
{

	class Application : Epsitec.Common.Widgets.Application
	{

		public override string ShortWindowTitle
		{
			get
			{
				return "Banque Piguet";
			}
		}

		private TextField BenefeciaryIbanTextField
		{
			get;
			set;
		}

		private TextFieldMulti BeneficiaryAddressTextField
		{
			get;
			set;
		}

		private TextFieldMulti ReasonTextField
		{
			get;
			set;
		}

		private BvrWidget BvrWidget
		{
			get;
			set;
		}

		public void SetupUI()
		{
			this.SetupWindow ();
			this.SetupForm ();
			this.SetupBvrWidget ();
		}

		private void SetupWindow()
		{
			this.Window = new Window ()
			{
				Text = this.ShortWindowTitle,
				WindowSize = new Size (1000, 330),
			};
		}

		private void SetupForm()
		{

			GroupBox beneficiaryIbanGroupBox = new GroupBox ()
			{
				Anchor = AnchorStyles.TopLeft,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (10, 10, 10, 10),
				Padding = new Margins (5, 5, 5, 5),
				Parent = this.Window.Root,
				Text = "N° IBAN du bénéficiaire",
			};

			this.BenefeciaryIbanTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryIbanGroupBox,
				PreferredWidth = 200,
			};
			
			GroupBox beneficiaryAddressGroupBox = new GroupBox ()
			{
				Anchor = AnchorStyles.TopLeft,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (10, 10, 65, 10),
				Padding = new Margins (5, 5, 5, 5),
				Parent = this.Window.Root,
				Text = "Nom et adresse du bénéficiaire",
			};

			this.BeneficiaryAddressTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryAddressGroupBox,
				PreferredHeight = 65,
				PreferredWidth = 200,
				ScrollerVisibility = false,
			};
			

			GroupBox reasonGroupBox = new GroupBox ()
			{
				Anchor = AnchorStyles.TopLeft,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (10, 10, 165, 10),
				Padding = new Margins (5, 5, 5, 5),
				Parent = this.Window.Root,
				Text = "Motif du versement",
			};

			this.ReasonTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Parent = reasonGroupBox,
				PreferredHeight = 52,
				PreferredWidth = 200,
				ScrollerVisibility = false,
			};

			Button printButton = new Button ()
			{
				Anchor = AnchorStyles.TopLeft,
				CommandObject = ApplicationCommands.Print,
				Margins = new Margins (10, 10, 255, 10),
				Parent = this.Window.Root,
			};
		}

		private void SetupBvrWidget()
		{
			this.BvrWidget = new BvrWidget ()
			{
				Anchor = AnchorStyles.TopLeft,
				Margins = new Margins (240, 10, 10, 10),
				Parent = this.Window.Root,
				PreferredHeight = 318,
				PreferredWidth = 630,
			};
		}

		[Command (ApplicationCommands.Id.Print)]
		private void ExecuteCommandPrint()
		{

			Epsitec.Common.Dialogs.PrintDialog dialog = new Epsitec.Common.Dialogs.PrintDialog
			{
				Owner = this.Window,
				AllowFromPageToPage = false,
				AllowSelectedPages = false
			};

			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = 1;
			dialog.OpenDialog ();

			if (dialog.Result == Epsitec.Common.Dialogs.DialogResult.Accept)
			{
				Epsitec.Common.Printing.PrintPort.PrintSinglePage (painter => this.BvrWidget.Print(painter, new Rectangle(0, 0, 21.0, 10.6)), dialog.Document, 25, 25);
			}
		}

	}

}
