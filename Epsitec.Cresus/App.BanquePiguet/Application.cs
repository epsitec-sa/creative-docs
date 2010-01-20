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


		public Application()
		{
			this.BeneficiaryAddressTextFields = new TextField[4];
		}


		public override string ShortWindowTitle
		{
			get { return "Banque Piguet"; }
		}


		private TextField BenefeciaryIbanTextField { get; set; }


		private TextField[] BeneficiaryAddressTextFields { get; set; }


		private TextField ReasonTextField { get; set; }


		private BvrWidget BvrWidget { get; set; }


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
				WindowSize = new Size (815, 330),
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
			this.Window.MakeFixedSizeWindow ();


			this.BenefeciaryIbanTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				MaxLength = 21,
				Parent = beneficiaryIbanGroupBox,
				PreferredWidth = 200,
			};

			GroupBox beneficiaryAddressGroupBox = new GroupBox ()
			{
				Anchor = AnchorStyles.TopLeft,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (10, 10, 65, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = this.Window.Root,
				Text = "Nom et adresse du bénéficiaire",
			};


			for (int i = 0; i < (4); i++)
			{
				this.BeneficiaryAddressTextFields[i] = new TextField ()
				{
					Dock = DockStyle.Stacked,
					MaxLength = 27,
					Margins = new Margins (0, 0, 5, 0),
					Parent = beneficiaryAddressGroupBox,
					PreferredWidth = 200,
				};
			}


			GroupBox reasonGroupBox = new GroupBox ()
			{
				Anchor = AnchorStyles.TopLeft,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Margins = new Margins (10, 10, 200, 10),
				Padding = new Margins (5, 5, 5, 5),
				Parent = this.Window.Root,
				Text = "Motif du versement",
			};


			this.ReasonTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				MaxLength = 30,
				Parent = reasonGroupBox,
				PreferredWidth = 200,
			};


			Button printButton = new Button ()
			{
				Anchor = AnchorStyles.TopLeft,
				CommandObject = ApplicationCommands.Print,
				Margins = new Margins (10, 10, 260, 10),
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
				PreferredHeight = 275,
				PreferredWidth = 545,
			};
		}


		[Command (ApplicationCommands.Id.Print)]
		private void ExecuteCommandPrint()
		{
			throw new System.NotImplementedException ();
		}

	}

}
