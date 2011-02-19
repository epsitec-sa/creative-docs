//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System.Globalization;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The <c>BvFieldMultiLine</c> represents a <see cref="BvField"/> which can contain more
	/// than one line.
	/// </summary>
	class BvFieldMultiLine : BvField
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="BvFieldMultiLine"/> class based on the
		/// values contained in <paramref name="xBvField"/> and <paramref name="bvSize"/>.
		/// </summary>
		/// <param name="xBvField">The <see cref="XElement"/> containing the definition of the <see cref="BvFieldMultiLine"/>.</param>
		/// <param name="bvSize">The size of the bv in millimeters.</param>
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
		/// Paints this instance on <paramref name="port"/> within <paramref name="bounds"/>.
		/// </summary>
		/// <remarks>
		/// Override this method if you want to change the display of the <see cref="BvField"/>.
		/// </remarks>
		/// <param name="port">The <see cref="IPaintPort"/> used to paint</param>
		/// <param name="bounds">The bounds of the bv.</param>
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
		/// Computes the absolute vertical offset of the line given <paramref name="lineNumber"/> and
		/// <paramref name="bounds"/>.
		/// </summary>
		/// <param name="lineNumber">The number of the current line.</param>
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
