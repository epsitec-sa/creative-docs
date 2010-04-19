//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{
	/// <summary>
	/// The class <c>BvWidget</c> represents a bv and can display it either on the screen or
	/// on paper. See http://www.postfinance.ch/medialib/pf/de/doc/consult/templ/chf/44103_templ.Par.0001.File.dat/44103_templ.pdf
	/// for the full specification.
	/// </summary>
	class BvWidget : Widget
	{

		/// <summary>
		/// The <see cref="BvFieldId"/> enum represents the set of the <see cref="BvField"/>s of a bv.
		/// </summary>
		protected enum BvFieldId {

			/// <summary>
			/// The address of the bank on the left part.
			/// </summary>
			BankAddress1,

			/// <summary>
			/// The address of the bank on the right part.
			/// </summary>
			BankAddress2,

			/// <summary>
			/// The iban of the beneficiary on the left part.
			/// </summary>
			BeneficiaryIban1,

			/// <summary>
			/// The iban of the beneficiary on the right part.
			/// </summary>
			BeneficiaryIban2,

			/// <summary>
			/// The address of the beneficiary on the left part.
			/// </summary>
			BeneficiaryAddress1,

			/// <summary>
			/// The address of the beneficiary on the right part.
			/// </summary>
			BeneficiaryAddress2,

			/// <summary>
			/// The bank account on the left part.
			/// </summary>
			BankAccount1,

			/// <summary>
			/// The bank account on the right part.
			/// </summary>
			BankAccount2,

			/// <summary>
			/// The number of francs on the left part.
			/// </summary>
			AmountFranc1,
			/// <summary>
			/// The number of francs on the right part.
			/// </summary>
			AmountFranc2,

			/// <summary>
			/// The number of cents on the left part.
			/// </summary>
			AmountCent1,

			/// <summary>
			/// The number of cents on the right part.
			/// </summary>
			AmountCent2,

			/// <summary>
			/// The name and address of the person who pays.
			/// </summary>
			PayedBy1,

			/// <summary>
			/// The name and address of the person who pays.
			/// </summary>
			PayedBy2,

			/// <summary>
			/// The layout code of the bv.
			/// </summary>
			LayoutCode,

			/// <summary>
			/// The reason of the transfer.
			/// </summary>
			Reason,

			/// <summary>
			/// The reference line of the bv.
			/// </summary>
			ReferenceLine,

			/// <summary>
			/// The clearing line of the bv.
			/// </summary>
			ClearingLine,

			/// <summary>
			/// The ccp number line of the bv.
			/// </summary>
			CcpNumberLine

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BvWidget"/> class.
		/// </summary>
		public BvWidget() : this (null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BvWidget"/> class.
		/// </summary>
		/// <param name="embedder">The embedder.</param>
		public BvWidget(Widget embedder) : base (embedder)
		{
			this.SetupAttributes ();
			this.SetupBvFields ();
		}

		/// <summary>
		/// Gets or sets the bank address.
		/// </summary>
		/// <value>The bank address.</value>
		public string BankAddress
		{
			get
			{
				return this.BvFields[BvFieldId.BankAddress1].Text;
			}

			set
			{
				if (BvHelper.CheckBankAddress (value))
				{
					this.BvFields[BvFieldId.BankAddress1].Text = value;
					this.BvFields[BvFieldId.BankAddress2].Text = value;
				}
				else
				{
					this.BvFields[BvFieldId.BankAddress1].Text = "";
					this.BvFields[BvFieldId.BankAddress2].Text = "";
				}
				
				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the beneficiary iban.
		/// </summary>
		/// <value>The beneficiary iban.</value>
		public string BeneficiaryIban
		{
			get
			{
				return this.BvFields[BvFieldId.BeneficiaryIban1].Text;
			}

			set
			{
				string iban = BvHelper.BuildNormalizedIban (value);

				if (BvHelper.CheckBeneficiaryIban (iban))
				{
					this.BvFields[BvFieldId.BeneficiaryIban1].Text = iban;
					this.BvFields[BvFieldId.BeneficiaryIban2].Text = iban;
				}
				else
				{
					this.BvFields[BvFieldId.BeneficiaryIban1].Text = "";
					this.BvFields[BvFieldId.BeneficiaryIban2].Text = "";
				}

				this.UpdateReferenceLine ();
				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the beneficiary address.
		/// </summary>
		/// <value>The beneficiary address.</value>
		public string BeneficiaryAddress
		{
			get
			{
				return this.BvFields[BvFieldId.BeneficiaryAddress1].Text;
			}
			set
			{
				if (BvHelper.CheckBeneficiaryAddress (value))
				{
					this.BvFields[BvFieldId.BeneficiaryAddress1].Text = value;
					this.BvFields[BvFieldId.BeneficiaryAddress2].Text = value;
				}
				else
				{
					this.BvFields[BvFieldId.BeneficiaryAddress1].Text = "";
					this.BvFields[BvFieldId.BeneficiaryAddress2].Text = "";
				}

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the bank account.
		/// </summary>
		/// <value>The bank account.</value>
		public string BankAccount
		{
			get
			{
				return this.BvFields[BvFieldId.BankAccount1].Text;
			}
			set
			{
				if (BvHelper.CheckBankAccount (value))
				{
					this.BvFields[BvFieldId.BankAccount1].Text = value;
					this.BvFields[BvFieldId.BankAccount2].Text = value;
				}
				else
				{
					this.BvFields[BvFieldId.BankAccount1].Text = "";
					this.BvFields[BvFieldId.BankAccount2].Text = "";
				}

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the amount of money.
		/// </summary>
		/// <value>The amount of money.</value>
		public string Amount
		{
			get
			{
				return this.amount;
			}
			set
			{
				string normalizedAmount = BvHelper.BuildNormalizedAmount (value);

				this.amount = BvHelper.CheckAmount (normalizedAmount) ? normalizedAmount : "";

				this.UpdateAmount ();
			}
		}

		/// <summary>
		/// Gets or sets the name and address of the person who pays.
		/// </summary>
		/// <value>The name and address of the person who pay.</value>
		public string PayedBy
		{
			get
			{
				return this.BvFields[BvFieldId.PayedBy1].Text;
			}
			set
			{
				if (BvHelper.CheckPayedBy (value))
				{
					this.BvFields[BvFieldId.PayedBy1].Text = value;
					this.BvFields[BvFieldId.PayedBy2].Text = value;
				}
				else
				{
					this.BvFields[BvFieldId.PayedBy1].Text = "";
					this.BvFields[BvFieldId.PayedBy2].Text = "";
				}

				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the layout code.
		/// </summary>
		/// <value>The layout code.</value>
		public string LayoutCode
		{
			/*
			 * The proper version of this property is commented and replaced by a dummy one which will
			 * not touch the empty value of the field and always return "303". This is because the Piguet
			 * Bank has the layout code already printed on their bv, but as I implemented everything to print
			 * it myself, I did not want to erase everything, just in case we might want to print it by
			 * for another project or if they ask us to change the program to print it.
			 * To enable the printing of the layout code, just uncomment the proper (the first one) version
			 * of this property and comment the dummy one.
			 */
			/*
			get
			{
				return this.BvFields[BvFieldId.LayoutCode].Text;
			}
			set
			{
				this.BvFields[BvFieldId.LayoutCode].Text = BvHelper.CheckLayoutCode (value) ? value : "";

				this.Invalidate ();
			}
			*/
			get
			{
				return "303";
			}
			set
            {
            }
		}

		/// <summary>
		/// Gets or sets the reason of the transfer.
		/// </summary>
		/// <value>The reason of the transfer.</value>
		public string Reason
		{
			get
			{
				return this.BvFields[BvFieldId.Reason].Text;
			}
			set
			{
				string reason = BvHelper.BuildNormalizedReason (value);

				this.BvFields[BvFieldId.Reason].Text = BvHelper.CheckReason (reason) ? reason : "";

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the reference client number.
		/// </summary>
		/// <value>The reference client number.</value>
		public string ReferenceClientNumber
		{
			get
			{
				return this.referenceClientNumber;
			}
			set
			{
				this.referenceClientNumber = BvHelper.CheckReferenceClientNumber (value) ? value : "";

				this.UpdateReferenceLine ();
			}
		}

		/// <summary>
		/// Gets or sets the clearing constant.
		/// </summary>
		/// <value>The clearing constant.</value>
		public string ClearingConstant
		{
			get
			{
				return this.clearingConstant;
			}
			set
			{
				this.clearingConstant = BvHelper.CheckClearingConstant (value) ? value : "";

				this.UpdateClearingLine ();
			}
		}

		/// <summary>
		/// Gets or sets the clearing number of the bank.
		/// </summary>
		/// <value>The clearing number of the bank.</value>
		public string ClearingBank
		{
			get
			{
				return this.clearingBank;
			}
			set
			{
				this.clearingBank = BvHelper.CheckClearingBank (value) ? value : "";

				this.UpdateClearingLine ();
			}
		}

		/// <summary>
		/// Gets or sets the control key of the clearing number of the bank.
		/// </summary>
		/// <value>The control key of the clearing number of the bank.</value>
		public string ClearingBankKey
		{
			get
			{
				return this.clearingBankKey;
			}
			set
			{
				this.clearingBankKey = BvHelper.CheckClearingBankKey (value) ? value : "";

				this.UpdateClearingLine ();
			}
		}


		/// <summary>
		/// Gets or sets the CCP number of the bank.
		/// </summary>
		/// <value>The CCP number of the bank.</value>
		public string CcpNumber
		{
			get
			{
				return this.ccpNumber;
			}
			set
			{
				this.ccpNumber = BvHelper.CheckBeneficiaryAddress (value) ? value : "";

				this.UpdateCcpNumberLine ();
			}
		}

		/// <summary>
		/// Gets or sets the size of the bv (in millimeters).
		/// </summary>
		/// <value>The size of the bv.</value>
		public Size BvSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Dictionnary"/> containing the <see cref="BvField"/>s.
		/// </summary>
		/// <value>The <see cref="Dictionnary"/> containing the <see cref="BvField"/>s.</value>
		protected Dictionary<BvFieldId, BvField> BvFields
		{
			get;
			set;
		}

		/// <summary>
		/// Prints the bv on <paramref name="port"/> within <paramref name="bounds"/>. This method
		/// is to use only to print it on paper as it will print the <see cref="BvField"/>s but not
		/// the background.
		/// </summary>
		/// <param name="port">The <see cref="IPaintPort"/> to use.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		public void Print(IPaintPort port, Rectangle bounds)
		{
			this.PaintBvFields (port, bounds);
		}

		/// <summary>
		/// Paints the bv on <paramref name="graphics"/>. This method is to use only to paint it on
		/// screen as it will print the background of the bv in addition to the <see cref="BvField"/>s.
		/// </summary>
		/// <param name="graphics">The <see cref="Graphics"/> to use.</param>
		/// <param name="clipRect">The clip rectangle.</param>
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle bounds = this.Client.Bounds;

			this.PaintBackgroundBv (graphics, bounds);
			this.PaintBvFields (graphics, bounds);
		}

		/// <summary>
		/// Paints the background of the bv: the lines, boxes, circles, texts, and so on.
		/// </summary>
		/// <param name="graphics">The <see cref="Graphics"/> to use.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		protected void PaintBackgroundBv(Graphics graphics, Rectangle bounds)
		{
			Point p1;
			Point p2;

			// Paints the background.
			p1 = this.BuildPoint (0, 0, bounds);
			p2 = this.BuildPoint (1, 1, bounds);
			graphics.AddFilledRectangle (new Rectangle (p1, p2));
			graphics.RenderSolid(BvWidget.colorLightPink);

			p1 = this.BuildPoint (0.289, 0, bounds);
			p2 = this.BuildPoint (1, 0.24, bounds);
			graphics.AddFilledRectangle (new Rectangle (p1, p2));
			graphics.RenderSolid(BvWidget.colorWhite);

			// Paints the black separators.
			using (Path path = new Path ())
			{
				path.MoveTo (this.BuildPoint (0, 1, bounds));
				path.LineTo (this.BuildPoint (1, 1, bounds));
				path.LineTo (this.BuildPoint (1, 0.96, bounds));
				path.LineTo (this.BuildPoint (0, 0.96, bounds));
				path.Close ();

				path.MoveTo (this.BuildPoint (0.289, 0, bounds));
				path.LineTo (this.BuildPoint (0.289, 1, bounds));

				path.MoveTo (this.BuildPoint (0.578, 0.96, bounds));
				path.LineTo (this.BuildPoint (0.578, 0.24, bounds));

				path.MoveTo (this.BuildPoint (0.578, 0.72, bounds));
				path.LineTo (this.BuildPoint (1, 0.72, bounds));

				path.MoveTo (this.BuildPoint (0.843, 0.96, bounds));
				path.LineTo (this.BuildPoint (0.843, 0.72, bounds));

				graphics.Color = BvWidget.colorBlack;
				graphics.PaintOutline (path);
			}

			// Paints the two "L".
			using (Path path = new Path ())
			{
				path.MoveTo (this.BuildPoint (0.325, 0.241, bounds));
				path.LineTo (this.BuildPoint (0.301, 0.241, bounds));
				path.LineTo (this.BuildPoint (0.301, 0.26, bounds));

				path.MoveTo (this.BuildPoint (0.951, 0.241, bounds));
				path.LineTo (this.BuildPoint (0.975, 0.241, bounds));
				path.LineTo (this.BuildPoint (0.975, 0.26, bounds));

				graphics.Color = BvWidget.colorBlack;
				graphics.PaintOutline (path);
			}

			graphics.RenderSolid (BvWidget.colorBlack);

			// Paints the red lines.
			for (int i = 0; i < 3; i++)
			{
				p1 = this.BuildPoint (0.012, (9 - i * 1.5) / 25, bounds);
				p2 = this.BuildPoint (0.277, (9 - i * 1.5) / 25, bounds);
				graphics.AddLine (p1, p2);
			}

			for (int i = 0; i < 4; i++)
			{
				p1 = this.BuildPoint (0.590, (12 - i * 1.5) / 25, bounds);
				p2 = this.BuildPoint (0.975, (12 - i * 1.5) / 25, bounds);
				graphics.AddLine (p1, p2);
			}

			graphics.RenderSolid (BvWidget.colorRed);

			// Paints the two red circles
			double radius = 0.042 * bounds.Width;
			
			p1 = this.BuildPoint (0.922, 0.84, bounds);
			graphics.AddCircle (p1, radius);

			p1 = this.BuildPoint (0.084, 0.12, bounds);
			graphics.AddCircle (p1, radius);

			graphics.RenderSolid (BvWidget.colorRed);
			
			// Paints the red and white boxes.
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					p1 = this.BuildPoint(0.592 + i * 0.024, 0.910 - j * 0.058, bounds);
					p2 = this.BuildPoint(0.610 + i * 0.024, 0.862 - j * 0.058, bounds);
					this.DrawBox (graphics, p1, p2);
				}
			}

			for (int i = 0; i < 11; i++)
			{
				if (i != 8)
				{
					p1 = this.BuildPoint (0.016 + i * 0.024, 0.52, bounds);
					p2 = this.BuildPoint (0.035 + i * 0.024, 0.472, bounds);
					this.DrawBox (graphics, p1, p2);

					p1 = this.BuildPoint (0.306 + i * 0.024, 0.52, bounds);
					p2 = this.BuildPoint (0.325 + i * 0.024, 0.472, bounds);
					this.DrawBox (graphics, p1, p2);
				}
			}

			// Paints the two dots for the amount of money.
			p1 = this.BuildPoint (0.216, 0.47, bounds);
			graphics.AddText (p1.X, p1.Y, ".", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSpecials, bounds));
			
			p1 = this.BuildPoint (0.505, 0.47, bounds);
			graphics.AddText (p1.X, p1.Y, ".", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSpecials, bounds));

			graphics.RenderSolid (BvWidget.colorBlack);

			// Paints the black texts.
			p1 = this.BuildPoint (0.012, 0.967, bounds);
			graphics.AddText (p1.X, p1.Y, "Empfangsschein / Récépicé / Ricevuta", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeBig, bounds));

			p1 = this.BuildPoint (0.325, 0.967, bounds);
			graphics.AddText (p1.X, p1.Y, "Einzahlung Giro", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeBig, bounds));

			p1 = this.BuildPoint (0.554, 0.967, bounds);
			graphics.AddText (p1.X, p1.Y, "Versement Virement", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeBig, bounds));

			p1 = this.BuildPoint (0.819, 0.967, bounds);
			graphics.AddText (p1.X, p1.Y, "Versamento Girata", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeBig, bounds));

			p1 = this.BuildPoint (0.012, 0.54, bounds);
			graphics.AddText (p1.X, p1.Y, "CHF", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeBig, bounds));

			p1 = this.BuildPoint (0.301, 0.54, bounds);
			graphics.AddText (p1.X, p1.Y, "CHF", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeBig, bounds));

			p1 = this.BuildPoint (0.156, 0.168, bounds);
			graphics.AddText (p1.X, p1.Y, "Die Annahmestelle", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.156, 0.148, bounds);
			graphics.AddText (p1.X, p1.Y, "L'office de dépôt", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.156, 0.128, bounds);
			graphics.AddText (p1.X, p1.Y, "L'ufficio d'accettazione", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.342, 0.292, bounds);
			graphics.AddText (p1.X, p1.Y, this.LayoutCode, BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSpecials, bounds));

			graphics.RenderSolid (BvWidget.colorBlack);

			// Paints the red texts.
			p1 = this.BuildPoint (0.012, 0.928, bounds);
			graphics.AddText (p1.X, p1.Y, "Einzahlung für/Versement pour/Versamento per", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.301, 0.928, bounds);
			graphics.AddText (p1.X, p1.Y, "Einzahlung für/Versement pour/Versamento per", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.012, 0.808, bounds);
			graphics.AddText (p1.X, p1.Y, "Zugunsten von/En faveur de/A favore di", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.301, 0.808, bounds);
			graphics.AddText (p1.X, p1.Y, "Zugunsten von/En faveur de/A favore di", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.012, 0.568, bounds);
			graphics.AddText (p1.X, p1.Y, "Konto/Compte/Conto", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.301, 0.568, bounds);
			graphics.AddText (p1.X, p1.Y, "Konto/Compte/Conto", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.012, 0.44, bounds);
			graphics.AddText (p1.X, p1.Y, "Einbezahlt von/Versé par/Versato da", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.590, 0.568, bounds);
			graphics.AddText (p1.X, p1.Y, "Einbezahlt von/Versé par/Versato da", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			p1 = this.BuildPoint (0.590, 0.928, bounds);
			graphics.AddText (p1.X, p1.Y, "Zahlungszweck/Motif versement/Motivo versamento", BvWidget.font, this.BuildTextSize (BvWidget.fontSizeSmall, bounds));

			graphics.RenderSolid (BvWidget.colorRed);

			// Paints the crosses.
			this.DrawCross (graphics, 0.309, 0.965, bounds);
			this.DrawCross (graphics, 0.442, 0.965, bounds);
			this.DrawCross (graphics, 0.542, 0.965, bounds);
			this.DrawCross (graphics, 0.704, 0.965, bounds);
			this.DrawCross (graphics, 0.805, 0.965, bounds);
			this.DrawCross (graphics, 0.957, 0.965, bounds);

			// Paints the border.
			graphics.LineWidth = 2;
			graphics.AddRectangle (bounds);
			graphics.RenderSolid (BvWidget.colorBlack);
		}

		/// <summary>
		/// Computes a <see cref="Point"/> given its relative coordinates and <paramref name="bounds"/>.
		/// </summary>
		/// <param name="relativeX">The relative X coordinate.</param>
		/// <param name="relativeY">The relative Y coordinate.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute <see cref="Point"/> build from relativeX and relativeY.</returns>
		protected Point BuildPoint(double relativeX, double relativeY, Rectangle bounds)
		{
			return new Point (relativeX * bounds.Width, relativeY * bounds.Height);
		}

		/// <summary>
		/// Computes the size of the text given its relative size and <paramref name="bounds"/>.
		/// </summary>
		/// <param name="relativeSize">The relative size of the text.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute size of the text.</returns>
		protected double BuildTextSize(double relativeSize, Rectangle bounds)
		{
			return relativeSize * bounds.Height;
		}

		/// <summary>
		/// Draws a white box with red outline between the given <see cref="Point"/>s.
		/// </summary>
		/// <param name="graphics">The <see cref="Graphics"/> to use.</param>
		/// <param name="p1">The first <see cref="Point"/>.</param>
		/// <param name="p2">The second <see cref="Point"/>.</param>
		protected void DrawBox(Graphics graphics , Point p1, Point p2)
		{
			graphics.AddFilledRectangle (new Rectangle(p1, p2));
			graphics.RenderSolid (BvWidget.colorWhite);

			graphics.AddRectangle (new Rectangle(p1, p2));
			graphics.RenderSolid (BvWidget.colorRed);
		}

		/// <summary>
		/// Draws a cross with black outline at the given relative <see cref="Point"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="Point"/> defining the position of the cross is the lower left corner
		/// of the bottom branch of the cross.
		/// </remarks>
		/// <param name="graphics">The <see cref="Graphics"/> to use.</param>
		/// <param name="relativeX">The relative X coordinate of the cross.</param>
		/// <param name="relativeY">The relative Y coordinate of the cross.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		protected void DrawCross(Graphics graphics, double relativeX, double relativeY, Rectangle bounds)
		{
			double relativeHeight = 0.0094;
			double relativeWidth = 0.0047;

			using (Path path = new Path ())
			{
				path.MoveTo(this.BuildPoint (relativeX, relativeY, bounds));
				path.LineTo(this.BuildPoint (relativeX + relativeWidth, relativeY, bounds));
				path.LineTo(this.BuildPoint (relativeX + relativeWidth, relativeY + relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX + 2 * relativeWidth, relativeY + relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX + 2 * relativeWidth, relativeY + 2 * relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX + relativeWidth, relativeY + 2 * relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX + relativeWidth, relativeY + 3 * relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX, relativeY + 3 * relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX, relativeY + 2 * relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX - relativeWidth, relativeY + 2 * relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX - relativeWidth, relativeY + relativeHeight, bounds));
				path.LineTo(this.BuildPoint (relativeX, relativeY + relativeHeight, bounds));
				path.Close ();
				
				double oldLineWidth = graphics.LineWidth;
				
				graphics.LineWidth = 0.4;
				graphics.Color = BvWidget.colorBlack;
				graphics.PaintOutline (path);

				graphics.LineWidth = oldLineWidth;
			}
		}

		/// <summary>
		/// Paints the <see cref="BvField"/>s using <paramref name="paintPort"/> within
		/// <paramref name="bounds"/>.
		/// </summary>
		/// <param name="paintPort">The <see cref="IPaintPort"/> to use.</param>
		/// <param name="bounds">The bounds.</param>
		protected void PaintBvFields(IPaintPort paintPort, Rectangle bounds)
		{

			foreach (BvFieldId fieldId in this.BvFields.Keys)
			{
				this.BvFields[fieldId].Paint (paintPort, bounds);
			}

		}

		/// <summary>
		/// Sets up the <see cref="BvField"/>s by loading them out of the resource file.
		/// </summary>
		/// <remarks>
		/// This method is called when the <see cref="BvWidget"/> is initialized.
		/// </remarks>
		/// <exception cref="System.Exception">If some <see cref="BvField"/>s are missing.</exception>
		protected void SetupBvFields()
		{
			this.BvFields = new Dictionary<BvFieldId, BvField> ();

			XElement xBvDefinition;

			using (XmlReader xmlReader = XmlReader.Create (Tools.GetResourceStream ("BvDefinition.xml")))
			{
				xBvDefinition = XElement.Load (xmlReader);
			}

			XElement xSize = xBvDefinition.Element ("size");
			this.BvSize = new Size (
				double.Parse (xSize.Element ("width").Value, CultureInfo.InvariantCulture),
				double.Parse (xSize.Element ("height").Value, CultureInfo.InvariantCulture)
			);

			IEnumerable<XElement> xBvfields = xBvDefinition.Element ("fields").Elements ("field");

			foreach (XElement xBvField in xBvfields)
			{
				string name = (string) xBvField.Element ("name");
				BvFieldId id = (BvFieldId) System.Enum.Parse (typeof (BvFieldId), name);
				this.BvFields[id] = BvField.GetInstance (xBvField, this.BvSize);
			}

			if (this.BvFields.Count < System.Enum.GetValues (typeof (BvFieldId)).Length)
			{
				throw new System.Exception ("Some BvFields are missing.");
			}
		}
		
		/// <summary>
		/// Sets up the <see cref="string"/> attributes in order to give them an empty initial value.
		/// </summary>
		/// <remarks>
		/// This method is called when the <see cref="BvWidget"/> is initialized.
		/// </remarks>
		protected void SetupAttributes()
		{
			this.amount = "";
			this.referenceClientNumber = "";
			this.clearingConstant = "";
			this.clearingBank = "";
			this.clearingBankKey = "";
			this.ccpNumber = "";
		}

		/// <summary>
		/// Updates the text for the amount of money.
		/// </summary>
		private void UpdateAmount()
		{
			if (BvHelper.CheckAmount (this.amount))
			{
				string francs = BvHelper.BuildFrancPart (this.amount);
				string cents = BvHelper.BuildCentPart (this.amount);

				this.BvFields[BvFieldId.AmountFranc1].Text = francs;
				this.BvFields[BvFieldId.AmountFranc2].Text = francs;

				this.BvFields[BvFieldId.AmountCent1].Text = cents;
				this.BvFields[BvFieldId.AmountCent2].Text = cents;
			}
			else
			{
				this.BvFields[BvFieldId.AmountFranc1].Text = "";
				this.BvFields[BvFieldId.AmountFranc2].Text = "";

				this.BvFields[BvFieldId.AmountCent1].Text = "";
				this.BvFields[BvFieldId.AmountCent2].Text = "";
			}
			
			this.Invalidate ();
		}

		/// <summary>
		/// Updates the text of the reference line based on the values of <see cref="BvWidget.ReferenceClientNumber"/>
		/// and <see cref="BvWidget.BeneficiaryIban"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="BvWidget.ReferenceClientNumber"/> and
		/// <see cref="BvWidget.BeneficiaryIban"/> change.
		/// </remarks>
		protected void UpdateReferenceLine()
		{
			string iban = this.BeneficiaryIban;
			string reference = this.ReferenceClientNumber;

			this.BvFields[BvFieldId.ReferenceLine].Text = BvHelper.CheckReferenceLine (iban, reference) ? BvHelper.BuildReferenceLine (iban, reference) : "";

			this.Invalidate ();
		}

		/// <summary>
		/// Updates the text of the clearing line based on the values of <see cref="BvWidget.ClearingConstant"/>,
		/// <see cref="BvWidget.ClearingBank"/> and <see cref="BvWidget.ClearingBankKey"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="BvWidget.ClearingConstant"/>,
		/// <see cref="BvWidget.ClearingBank"/> and <see cref="BvWidget.ClearingBankKey"/> change.
		/// </remarks>
		protected void UpdateClearingLine()
		{
			string constant = this.ClearingConstant;
			string clearing = this.ClearingBank;
			string key = this.ClearingBankKey;

			this.BvFields[BvFieldId.ClearingLine].Text = BvHelper.CheckClearingLine (constant, clearing, key) ? BvHelper.BuildClearingLine (constant, clearing, key) : "";

			this.Invalidate ();
		}

		/// <summary>
		/// Updates the text of the CCP number line based on the value of <see cref="BvWidget.CcpNumber"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="BvWidget.CcpNumber"/> changes.
		/// </remarks>
		protected void UpdateCcpNumberLine()
		{
			this.BvFields[BvFieldId.CcpNumberLine].Text = BvHelper.CheckCcpNumber (this.CcpNumber) ? BvHelper.BuildCcpNumberLine (this.CcpNumber) : "";

			this.Invalidate ();
		}

		/// <summary>
		/// This attribute backs the <see cref="BvWidget.Amount"/> property.
		/// </summary>
		protected string amount;

		/// <summary>
		/// This attribute backs the <see cref="BvWidget.ReferenceClientNumber"/> property.
		/// </summary>
		protected string referenceClientNumber;
		
		/// <summary>
		/// This attributes backs the <see cref="BvWidget.ClearingConstant"/> property.
		/// </summary>
		protected string clearingConstant;
		
		/// <summary>
		/// This attribute backs the <see cref="BvWidget.ClearingBank"/> property.
		/// </summary>
		protected string clearingBank;
		
		/// <summary>
		/// This attribute backs the <see cref="BvWidget.ClearingBankKey"/> property.
		/// </summary>
		protected string clearingBankKey;
		
		/// <summary>
		/// This attribute backs the <see cref="BvWidget.CcpNumber"/> property.
		/// </summary>
		protected string ccpNumber;

		/// <summary>
		/// The <see cref="Color"/> used to draw the pink part of the background of the bv.
		/// </summary>
		protected static readonly Color colorLightPink = Color.FromRgb (0.964, 0.909, 0.882);
		
		/// <summary>
		/// The <see cref="Color"/> used to draw the white part of the background of the bv
		/// and the inner part of the boxes.
		/// </summary>
		protected static readonly Color colorWhite = Color.FromRgb (1, 1, 1);
		
		/// <summary>
		/// The <see cref="Color"/> used to draw black elements on the bv.
		/// </summary>
		protected static readonly Color colorBlack = Color.FromRgb (0, 0, 0);
		
		/// <summary>
		/// The <see cref="Color"/> used to draw red elements on the bv.
		/// </summary>
		protected static readonly Color colorRed = Color.FromRgb (0.788, 0.211, 0.247);

		/// <summary>
		/// The <see cref="Font"/> used to display text on the bv.
		/// </summary>
		protected static readonly Font font = Font.GetFont ("Arial", "Regular");

		/// <summary>
		/// The size of the big texts on the bv.
		/// </summary>
		protected static readonly double fontSizeBig = 0.03;
		
		/// <summary>
		/// The size of the small texts on the bv.
		/// </summary>
		protected static readonly double fontSizeSmall = 0.02;
		
		/// <summary>
		/// The size of the two black dots on the bv and of the layout code.
		/// </summary>
		protected static readonly double fontSizeSpecials = 0.04;

	}

}
