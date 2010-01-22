//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.UI;

using System;
using System.Xml;


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
			this.BenefeciaryIbanTextField.TextChanged += sender =>
			{
				this.BvrWidget.BeneficiaryIban = this.BenefeciaryIbanTextField.Text;
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
			this.BeneficiaryAddressTextField.TextChanged += sender =>
			{
				string text = this.BeneficiaryAddressTextField.Text.Replace ("<br/>", "\n");
				this.BvrWidget.BeneficiaryAddress = text;
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
			this.ReasonTextField.TextChanged += sender =>
			{
				string text = this.ReasonTextField.Text.Replace ("<br/>", "\n");
				this.BvrWidget.Reason = text;
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
			this.BvrWidget = new BvrWidget (Application.bvrDefinition)
			{
				Anchor = AnchorStyles.TopLeft,
				Margins = new Margins (240, 10, 10, 10),
				Parent = this.Window.Root,
				PreferredHeight = 318,
				PreferredWidth = 630,
			};

			try
			{
				XmlDocument bvrDefinitionXml = new XmlDocument ();
				bvrDefinitionXml.Load (Application.bvrValues);

				XmlNodeList values = bvrDefinitionXml.GetElementsByTagName ("value");

				int nbValues = 0;

				foreach (XmlNode value in values)
				{
					string name = value.SelectSingleNode ("name").InnerText.Trim();
					string text = value.SelectSingleNode ("text").InnerText.Trim();
					
					if (text.Contains ("\\n"))
					{
						string[] lines = text.Replace("\\n", "\n").Split ('\n');
						
						text = lines[0];

						for (int i = 1; i < lines.Length; i++)
						{
							text += String.Format ("\n{0}", lines[i].Trim());
						}
					}

					switch (name)
					{
						case "BankAddress":
							this.BvrWidget.BankAddress = text;
							break;
						case "BankAccount":
							this.BvrWidget.BankAccount = text;
							break;
						case "LayoutCode":
							this.BvrWidget.LayoutCode = text;
							break;
						case "ReferenceClientNumber":
							this.BvrWidget.ReferenceClientNumber = text;
							break;
						case "ClearingConstant":
							this.BvrWidget.ClearingConstant = text;
							break;
						case "ClearingBank":
							this.BvrWidget.ClearingBank = text;
							break;
						case "ClearingBankKey":
							this.BvrWidget.ClearingBankKey = text;
							break;
						case "CcpNumber":
							this.BvrWidget.CcpNumber = text;
							break;
						default:
							throw new Exception (String.Format ("Unknown value: {0}", name));
					}

					nbValues++;

				}

				if (nbValues < 8)
				{
					throw new Exception ("Some bvr values are missing.");
				}


			}
			catch (Exception e)
			{
				throw new Exception ("An error occured while loading the bvr values.", e);
			}
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

		private static string bvrDefinition = String.Format ("{0}\\Data\\BvrDefinition.xml", Epsitec.Common.Support.Globals.Directories.ExecutableRoot);

		private static string bvrValues = String.Format ("{0}\\Data\\BvrValues.xml", Epsitec.Common.Support.Globals.Directories.ExecutableRoot);

	}

}
