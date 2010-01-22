//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;
using System;


namespace Epsitec.App.BanquePiguet
{

	class BvrWidget : Widget
	{

		protected enum Fields {
			BankAddress1,
			BankAddress2,
			BeneficiaryIban1,
			BeneficiaryIban2,
			BeneficiaryAddress1,
			BeneficiaryAddress2,
			BankAccount1,
			BankAccount2,
			LayoutCode,
			Reason,
			ReferenceLine,
			ClearingLine,
			CcpNumber
		}

		public BvrWidget()
		{

			this.BvrFields = new Dictionary<Fields, BvrField> ();

			this.BvrFields.Add (
				Fields.BankAccount1,
				new BvrFieldMultiLine ()
				{
					Text = "Banque Piguet & Cie\n1400 Yverdon-les-Bains",
					TextFont = Font.DefaultFont,
					TextRelativeHeight = 1.0 / 25.0,
					VerticalSpace = 1.0 / 25.0,
					XRelativePosition = 1.0 / 83.0,
					YRelativePosition =  22.2 / 25.0,
				}
			);

			this.BvrFields.Add (
				Fields.BeneficiaryIban1,
				new BvrField ()
				{
					Text = "CH38 0888 8123 4567 8901 2",
					TextFont = Font.DefaultFont,
					TextRelativeHeight = 1.0 / 25.0,
					XRelativePosition = 1.0 / 83.0,
					YRelativePosition =  19.2 / 25.0,
				}
			);

			this.BvrFields.Add (
				Fields.Reason,
				new BvrFieldMultiLineColumn ()
				{
					HorizontalSpace = 2.0 / 83.0,
					Text = "0123456789\n0123456789\n0123456789",
					TextFont = Font.DefaultFont,
					TextRelativeHeight = 1.0 / 25.0,
					VerticalSpace = 1.6 / 25.0,
					XRelativePosition = 49.2 / 83.0,
					YRelativePosition =  22.0 / 25.0,
				}
			);


			/*this.BankAddress = "Banque Piguet & Cie\n1400 Yverdon-les-Bains";
			this.BeneficiaryIban = "CH38 0888 8123 4567 8901 2";
			this.BeneficiaryAddress = "Muster AG\nBahnofstrasse 5\n8001 ZURICH";
			this.BankAccount = "10-664-6";
			this.LayoutCode = "303";
			this.Reason = "0123456789\n0123456789\n0123456789";
			this.ReferenceClientNumber = "0000000000";
			this.ClearingConstant = "07";
			this.ClearingBank = "08777";
			this.ClearingBankKey = "7";
			this.CcpNumber = "100006646>";
			 */

			/*this.BankAddressPosition1 = new Point (1.0/83.0, 22.2/25.0);
			this.BankAddressPosition2 = new Point (25.0/83.0, 22.2/25.0);
			this.BeneficiaryIbanPosition1 = new Point (1.0/83.0, 19.2/25);
			this.BeneficiaryIbanPosition2 = new Point (25.0/83.0, 19.2/25);
			this.BeneficiaryAddressPosition1 = new Point (1.0/83.0, 18.2/25);
			this.BeneficiaryAddressPosition2 = new Point (25.0/83.0, 18.2/25);
			this.BankAccountPosition1 = new Point (11.5/83.0, 14.2/25.0);
			this.BankAccountPosition2 = new Point (35.5/83.0, 14.2/25.0);
			this.LayoutCodePosition = new Point (28.0/83.0, 7.0/25.0);
			this.ReasonPosition = new Point (49.2/83.0, 22.0/25.0);
			this.ReferenceLinePosition = new Point (41.0/83.0, 4.2/25.0);
			this.ClearingLinePosition = new Point (70.3/83.0, 4.2/25.0);
			this.CcpNumberPosition = new Point (70.3/83.0, 2.2/25.0);*/

			/*this.AddressVerticalSpace = (1.0/25.0);
			this.ReasonVerticalSpace = (1.6/25.0);
			this.ReasonHorizontalSpace = (2.0/83.0);
			this.TextHeight = (1.0/25.0);*/

		}


		protected Dictionary<Fields, BvrField> BvrFields
		{
			get;
			set;
		}


		public string BankAddress
		{
			get;
			set;
		}
		
		public string BeneficiaryIban
		{
			get;
			set;
		}

		public string BeneficiaryAddress
		{
			get;
			set;
		}

		public string BankAccount
		{
			get;
			set;
		}

		public string LayoutCode
		{
			get;
			set;
		}

		public string Reason
		{
			get;
			set;
		}

		public string ReferenceClientNumber
		{
			get;
			set;
		}

		protected string ReferenceKey
		{
			get
			{
				return "0";
			}
		}

		protected string ReferenceLine
		{
			get
			{
				return "0000000000000000000000+";
			}
		}

		public string ClearingConstant
		{
			get;
			set;
		}

		public string ClearingBank
		{
			get;
			set;
		}

		public string ClearingBankKey
		{
			get;
			set;
		}


		protected string ClearingKey
		{
			get
			{
				return "0";
			}
		}


		protected string ClearingLine
		{
			get
			{
				return "000000000>";
			}
		}

		public string CcpNumber
		{
			get;
			set;
		}


		public void Print(IPaintPort paintPort, Rectangle bounds)
		{
			this.PaintBvrWidgets (paintPort, bounds);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle bounds = this.Client.Bounds;

			this.PaintBackground (graphics, bounds);
			this.PaintBvrWidgets (graphics, bounds);
		}

		private void PaintBackground(IPaintPort port, Rectangle bounds)
		{
			using (Path path = Path.FromRectangle (bounds))
			{
				port.Color = Color.FromRgb (1.0, 0.8, 0.8);
				port.PaintSurface (path);
			}
		}

		protected void PaintBvrWidgets(IPaintPort paintPort, Rectangle bounds)
		{

			foreach (BvrField field in this.BvrFields.Values)
			{
				field.Paint (paintPort, bounds);
			}

		}

	}

}
