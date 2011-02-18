//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The <c>BvFieldMultiLineColumnRight</c> represents a <see cref="BvField"/> which can contain
	/// more than one line and whose characters are separated by spaces, and characters are alined
	/// on the right.
	/// </summary>
	class BvFieldMultiLineColumnRight : BvFieldMultiLineColumn
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="BvFieldMultiLineColumnRight"/> class based on
		/// the values contained in <paramref name="xBvField"/> and <paramref name="bvSize"/>.
		/// </summary>
		/// <param name="xBvField">The <see cref="XElement"/> containing the definition of the <see cref="BvFieldMultiLineColumnReverse"/>.</param>
		/// <param name="bvSize">The size of the Bv in millimeters.</param>
		public BvFieldMultiLineColumnRight(XElement xBvField, Size bvSize)
			: base (xBvField, bvSize)
		{
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

			double xPositionBase = this.ComputeAbsoluteXPosition (bounds);
			double yPositionBase = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			string[] lines = this.Text.Split ('\n');

			for (int i = 0; i < lines.Length; i++)
			{
				double yPosition = yPositionBase + this.ComputeAbsoluteVerticalOffset (i, bounds);

				char[] line = lines[i].ToCharArray ();
				System.Array.Reverse (line);

				for (int j = 0; j < line.Length; j++)
				{
					double xPosition = xPositionBase + this.ComputeAbsoluteHorizontalOffset (j, bounds);
					string text = string.Format ("{0}", line[j]);

					port.PaintText (xPosition, yPosition, text, this.TextFont, textHeight);
				}
			}
		}

		/// <summary>
		/// Computes the absolute horizontal offset of the character given by <paramref name="position"/> 
		/// and <paramref name="charNumber"/>.
		/// </summary>
		/// <param name="charNumber">The character number on its line.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute horizontal offset of the given character.</returns>
		/// <remarks>
		/// Character numbers starts at 0 for the first one, 1 for the one on the right, and so on.
		/// </remarks>
		private double ComputeAbsoluteHorizontalOffset(int charNumber, Rectangle bounds)
		{
			return -charNumber * this.RelativeHorizontalSpace * bounds.Width;
		}

	}

}
