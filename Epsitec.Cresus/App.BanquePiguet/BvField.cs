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
	/// The <c>BvField</c> class represents a field on a bv, that means a simple line of text.
	/// </summary>
	class BvField
	{

		/// <summary>
		/// Static constructor that registers the OCR-B1 <see cref="Font"/> required by some fields.
		/// </summary>
		static BvField()
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.App.BanquePiguet.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BvField"/> class based on the values contained
		/// in <paramref name="xBvField"/> and <paramref name="bvSize"/>.
		/// </summary>
		/// <param name="xBvField">The <see cref="XElement"/> containing the definition of the <see cref="BvField"/>.</param>
		/// <param name="bvSize">The size of the bv in millimeters.</param>
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
		/// Gets or sets the <see cref="Font"/> of the text.
		/// </summary>
		/// <value>The <see cref="Font"/> of the text.</value>
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
		/// Paints this instance on <paramref name="port"/> within <paramref name="bounds"/>.
		/// </summary>
		/// <remarks>
		/// Override this method if you want to change the display of the <see cref="BvField"/>.
		/// </remarks>
		/// <param name="port">The <see cref="IPaintPort"/> used to paint</param>
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
		/// Computes the absolute position of the text on the x axis given <paramref name="bounds"/>.
		/// </summary>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute position of the text on the x axis.</returns>
		protected double ComputeAbsoluteXPosition(Rectangle bounds)
		{
			return this.XRelativePosition * bounds.Width + bounds.X;
		}

		/// <summary>
		///  Computes the absolute position of the text on the 1 axis given <paramref name="bounds"/>.
		/// </summary>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute position of the text on the y axis.</returns>
		protected double ComputeAbsoluteYPosition(Rectangle bounds)
		{
			return this.YRelativePosition * bounds.Height + bounds.Y;
		}

		/// <summary>
		/// Computes the absolute height of the text, given <paramref name="bounds"/>.
		/// </summary>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute height of the text.</returns>
		protected double ComputeTextAbsoluteHeight(Rectangle bounds)
		{
			return this.TextRelativeHeight * bounds.Height;
		}

		/// <summary>
		/// Builds a new instance of <see cref="BvField"/> or one of its subclass based on the
		/// values contained in <paramref name="xBvField"/> and on <paramref name="bvSize"/>.
		/// </summary>
		/// <param name="xBvField">The <see cref="XElement"/> containing the definition of the <see cref="BvField"/>.</param>
		/// <param name="bvSize">The Size of the bv.</param>
		/// <returns>A new <see cref="BvField"/>.</returns>
		public static BvField GetInstance(XElement xBvField, Size bvSize)
		{
			string type = xBvField.Element ("type").Value;

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
				case "BvFieldMultiLineColumnRight":
					bvField = new BvFieldMultiLineColumnRight (xBvField, bvSize);
					break;
				default:
					throw new System.Exception (string.Format ("Invalid BvField type: {0}.", type));
			}

			return bvField;

		}

	}

}
