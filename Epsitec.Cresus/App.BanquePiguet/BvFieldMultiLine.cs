//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System.Globalization;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The BvFieldMultiLine represents a BvField which can contain more than one line.
	/// </summary>
	class BvFieldMultiLine : BvField
	{

		/// <summary>
		/// Initializes a new instance of the BvFieldMultiLine class based on the values contained in xBvField.
		/// </summary>
		/// <param name="xBvField">The XElement containing the definition of the BvFieldMultiLine.</param>
		/// <param name="bvSize">The size of the Bv in millimeters.</param>
		public BvFieldMultiLine(XElement xBvField, Size bvSize) : base (xBvField, bvSize)
		{ 
			this.RelativeVerticalSpace = double.Parse (xBvField.Element ("verticalSpace").Value, CultureInfo.InvariantCulture) / bvSize.Height;
		}

		/// <summary>
		/// Gets or sets the relative (to the bv height) vertical space between two adjacent lines.
		/// </summary>
		/// <value>The relative vertical space.</value>
		public double RelativeVerticalSpace
		{
			get;
			set;
		}

		/// <summary>
		/// Paints this instance on port within bounds.
		/// </summary>
		/// <param name="port">The port used to paint</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <remarks>
		/// Override this method if you want to change the display of the BvField.
		/// </remarks>
		public override void Paint(IPaintPort port, Rectangle bounds)
		{
			port.Color = Color.FromRgb (0, 0, 0);

			double xPosition = this.ComputeAbsoluteXPosition (bounds);
			double yPositionBase = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);
			
			string[] lines = this.Text.Split ('\n');

			for (int i = 0; i < lines.Length; i++)
			{
				double yPosition = yPositionBase + this.ComputeAbsoluteVerticalOffset (i, bounds);
				port.PaintText (xPosition, yPosition, lines[i], this.TextFont, textHeight);
			}
		}

		/// <summary>
		/// Computes the absolute vertical offset of the line given by lineNumber and the
		/// bounds of the bv.
		/// </summary>
		/// <param name="lineNumber">The line number.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The vertical offset of the given line.</returns>
		/// <remarks>
		/// Line numbers starts at 0 for the top line, 1 for the one below, and so on.
		/// </remarks>
		protected double ComputeAbsoluteVerticalOffset(int lineNumber, Rectangle bounds)
		{
			return -lineNumber * this.RelativeVerticalSpace * bounds.Height;
		}

	}

}
