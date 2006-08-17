//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontSizeInfo</c> class stores glyph size information for a
	/// discrete font size. The information is gathered through calls to
	/// the operating system.
	/// </summary>
	internal class FontSizeInfo
	{
		public FontSizeInfo(int pointSize, Platform.IFontHandle handle)
		{
			System.Diagnostics.Debug.Assert (pointSize > 0);
			System.Diagnostics.Debug.Assert (handle != null);
			
			this.pointSize   = pointSize;
			this.fontHandle  = handle;
			this.glyphWidths = new int[4][];

			Platform.Neutral.FillFontHeights (handle,
				/**/						  out this.height, out this.ascender, out this.descender,
				/**/						  out this.internalLeading, out this.externalLeading);
		}


		public Platform.IFontHandle Handle
		{
			get
			{
				return this.fontHandle;
			}
		}

		public int PointSize
		{
			get
			{
				return this.pointSize;
			}
		}

		public int Ascender
		{
			get
			{
				return this.ascender;
			}
		}

		public int Descender
		{
			get
			{
				return this.descender;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
		}


		public int GetGlyphWidth(int glyph)
		{
			if (glyph >= 0xffff)
			{
				return 0;
			}

			int block = glyph / FontSizeInfo.BlockSize;
			int index = glyph % FontSizeInfo.BlockSize;

			if (block >= this.glyphWidths.Length)
			{
				int[][] old_widths = this.glyphWidths;
				int[][] new_widths = new int[block+1][];

				for (int i = 0; i < old_widths.Length; i++)
				{
					new_widths[i] = old_widths[i];
				}

				this.glyphWidths = new_widths;
			}

			if (this.glyphWidths[block] == null)
			{
				this.glyphWidths[block] = new int[FontSizeInfo.BlockSize];

				Platform.Neutral.FillFontWidths (this.fontHandle, block*FontSizeInfo.BlockSize, FontSizeInfo.BlockSize, this.glyphWidths[block], null, null);
			}

			return this.glyphWidths[block][index];
		}

		private const int BlockSize = 64;

		private int								pointSize;
		private Platform.IFontHandle			fontHandle;
		private int[][]							glyphWidths;
		private int								height;
		private int								ascender;
		private int								descender;
		private int								internalLeading;
		private int								externalLeading;
	}
}
