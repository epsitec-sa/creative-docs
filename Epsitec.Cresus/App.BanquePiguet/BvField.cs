//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The BvField class represents a field on a bv, that means a simple line of text.
	/// </summary>
	class BvField
	{

		/// <summary>
		/// Static constructor that registers the OCR-B1 font required by some fields.
		/// </summary>
		static BvField()
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.App.BanquePiguet.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}

		/// <summary>
		/// Initializes a new instance of the BvField class based on the values contained in xBvField.
		/// </summary>
		/// <param name="xBvField">The XElement containing the definition of the BvField.</param>
		/// <param name="bvSize">The size of the Bv in millimeters.</param>
		public BvField(XElement xBvField, Size bvSize)
		{
			this.Text = "";
			this.TextRelativeHeight = double.Parse (xBvField.Element ("textHeight").Value, CultureInfo.InvariantCulture) / bvSize.Height;
			this.XRelativePosition = double.Parse (xBvField.Element ("xPosition").Value, CultureInfo.InvariantCulture) / bvSize.Width;
			this.YRelativePosition = double.Parse (xBvField.Element ("yPosition").Value, CultureInfo.InvariantCulture) / bvSize.Height;

			switch (xBvField.Element ("font").Value)
			{
				case "OCR-B1":
					this.TextFont = Font.GetFont ("OCR-B Bold", "Regular");
					break;

				case "Arial":
					this.TextFont = Font.GetFont ("Arial", "Regular");
					break;

				default:
					this.TextFont = Font.DefaultFont;
					break;
			}
		}

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the relative (to the bv height) height of the text.
		/// </summary>
		/// <value>The relative height of the text.</value>
		public double TextRelativeHeight
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the font of the text.
		/// </summary>
		/// <value>The font of the text.</value>
		public Font TextFont
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the relative (to the bv height) position of the text on the x axis.
		/// </summary>
		/// <value>The relative position of the text on the x axis.</value>
		public double XRelativePosition
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the relative (to the bv width) position of the text on the y axis.
		/// </summary>
		/// <value>The relative position of the text on the y axis.</value>
		public double YRelativePosition
		{
			get;
			set;
		}

		/// <summary>
		/// Paints this instance on port within bounds.
		/// </summary>
		/// <remarks>
		/// Override this method if you want to change the display of the BvField.
		/// </remarks>
		/// <param name="port">The port used to paint</param>
		/// <param name="bounds">The bounds of the bv.</param>
		public virtual void Paint(IPaintPort port, Rectangle bounds)
		{
			double xPosition = this.ComputeAbsoluteXPosition (bounds);
			double yPosition = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			port.Color = Color.FromRgb (0, 0, 0);
			port.PaintText (xPosition, yPosition, this.Text, this.TextFont, textHeight);
		}

		/// <summary>
		/// Computes the absolute position of the Text on the x axis given the bounds of the bv.
		/// </summary>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute position of the Text on the x axis.</returns>
		protected double ComputeAbsoluteXPosition(Rectangle bounds)
		{
			return this.XRelativePosition * bounds.Width + bounds.X;
		}

		/// <summary>
		///  Computes the absolute position of the Text on the 1 axis given the bounds of the bv.
		/// </summary>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute position of the Text on the y axis.</returns>
		protected double ComputeAbsoluteYPosition(Rectangle bounds)
		{
			return this.YRelativePosition * bounds.Height + bounds.Y;
		}

		/// <summary>
		/// Computes the absolute height of the text, given the bounds of the bv.
		/// </summary>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute height of the text.</returns>
		protected double ComputeTextAbsoluteHeight(Rectangle bounds)
		{
			return this.TextRelativeHeight * bounds.Height;
		}

		/// <summary>
		/// Builds a new instance of BvFIeld or one of its subclass based on the values
		/// contained in xBvField and on bvSize.
		/// </summary>
		/// <param name="xBvField">The XElement containing the definition of the BvField.</param>
		/// <param name="bvSize">The Size of the bv.</param>
		/// <returns>A new BvField.</returns>
		public static BvField GetInstance(XElement xBvField, Size bvSize)
		{
			string type = (string) xBvField.Element ("type");

			BvField bvField;

			switch (type)
			{
				case "BvField":
					bvField = new BvField (xBvField, bvSize);
					break;
				case "BvFieldMultiLine":
					bvField = new BvFieldMultiLine (xBvField, bvSize);
					break;
				case "BvFieldMultiLineColumn":
					bvField = new BvFieldMultiLineColumn (xBvField, bvSize);
					break;
				default:
					throw new System.Exception (string.Format ("Invalid BvField type: {0}.", type));
			}

			return bvField;

		}

	}

}
