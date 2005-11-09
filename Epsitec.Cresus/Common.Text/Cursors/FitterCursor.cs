//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Cursors
{
	/// <summary>
	/// Summary description for FitterCursor.
	/// </summary>
	public class FitterCursor : Cursors.TempCursor
	{
		public FitterCursor(TextFitter fitter)
		{
			this.fitter = fitter;
		}
		
		
		public int								LineCount
		{
			get
			{
				return this.elements.Length;
			}
		}
		
		public FitterCursor.Element[]			Elements
		{
			get
			{
				return this.elements;
			}
		}
		
		public int								ParagraphLength
		{
			get
			{
				int length = 0;
				
				for (int i = 0; i < this.elements.Length; i++)
				{
					length += this.elements[i].Length;
				}
				
				return length;
			}
		}
		
		public double							ParagraphY
		{
			get
			{
				return this.paragraph_y;
			}
		}
		
		public double							ParagraphNextY
		{
			get
			{
				return this.paragraph_next_y;
			}
		}
		
		public double							SpaceAfterParagraph
		{
			get
			{
				return this.space_after_paragraph;
			}
		}
		
		public bool								KeepWithNextParagraph
		{
			get
			{
				return this.keep_with_next_paragraph;
			}
		}
		
		public int[]							TextFrameIndexes
		{
			get
			{
				int index = -1;
				int count = 0;
				int n = this.elements.Length;
				
				for (int i = 0; i < n; i++)
				{
					if (this.elements[i].FrameIndex != index)
					{
						index  = this.elements[i].FrameIndex;
						count += 1;
					}
				}
				
				int[] result = new int[count];
				
				index = -1;
				count = 0;
				
				for (int i = 0; i < n; i++)
				{
					if (this.elements[i].FrameIndex != index)
					{
						index  = this.elements[i].FrameIndex;
						result[count] = index;
						count += 1;
					}
				}
				
				return result;
			}
		}
		
		public TextFitter						TextFitter
		{
			get
			{
				return this.fitter;
			}
		}
		
		
		public static CursorInfo.Filter			Filter
		{
			get
			{
				return new CursorInfo.Filter (FitterCursor.FilterCallback);
			}
		}
		
		
		public override void Clear()
		{
			//	Efface les informations liées au curseur, mais se souvient du
			//	fitter attaché...
			
			base.Clear ();
			this.elements = null;
		}

		
		public void DefineParagraphY(double y)
		{
			this.paragraph_y = y;
		}
		
		public void DefineParagraphNextY(double y)
		{
			this.paragraph_next_y = y;
		}
		
		public void DefineSpaceAfterParagraph(double y)
		{
			this.space_after_paragraph = y;
		}
		
		public void DefineKeepWithNextParagraph(bool keep)
		{
			this.keep_with_next_paragraph = keep;
		}
		
		
		public void AddRange(System.Collections.IList values)
		{
			int n = values.Count;
			
			if (n > 0)
			{
				if (this.elements == null)
				{
					this.elements = new Element[n];
					
					for (int i = 0; i < n; i++)
					{
						this.elements[i] = (Element) values[i];
					}
				}
				else
				{
					int m = this.elements.Length;
					
					Element[] old_elements = this.elements;
					Element[] new_elements = new Element[m + n];
					
					System.Array.Copy (old_elements, 0, new_elements, 0, m);
					
					for (int i = 0; i < n; i++)
					{
						new_elements[m+i] = (Element) values[i];
					}
					
					this.elements = new_elements;
				}
			}
		}
		
		
		public bool ContainsFrameIndex(int index)
		{
			for (int i = 0; i < this.elements.Length; i++)
			{
				if (this.elements[i].FrameIndex == index)
				{
					return true;
				}
			}
			
			return false;
		}
		
		public bool IsLastLine(int index)
		{
			return FitterCursor.IsLastLine (this.elements, index);
		}
		
		
		public static CursorInfo.Filter GetFrameFilter(int frame_index)
		{
			FilterFrame filter = new FilterFrame (frame_index);
			return new CursorInfo.Filter (filter.FilterCallback);
		}
		
		public static CursorInfo.Filter GetFitterFilter(TextFitter fitter)
		{
			FilterFitter filter = new FilterFitter (fitter);
			return new CursorInfo.Filter (filter.FilterCallback);
		}
		
		
		internal int[] GetLineStartPositions(Internal.TextTable text)
		{
			int   count = this.elements.Length + 1;
			int[] pos   = new int[count];
			int   offset;
			
			pos[0] = text.GetCursorPosition (this.CursorId);
			offset = pos[0];
			count  = 1;
			
			for (int i = 0, j = 1; i < this.elements.Length; i++)
			{
				offset += this.elements[i].Length;
				
				if (this.elements[i].IsNewLine)
				{
					pos[j++] = offset;
					count++;
				}
			}
			
			if (count < pos.Length)
			{
				int[] copy = new int[count];
				
				for (int i = 0; i < count; i++)
				{
					copy[i] = pos[i];
				}
				
				pos = copy;
			}
			
			return pos;
		}
		
		
		internal static bool IsLastLine(Cursors.FitterCursor.Element[] elements, int i)
		{
			for (int j = i; j < elements.Length - 1; j++)
			{
				if (elements[j].IsNewLine)
				{
					return false;
				}
			}
			
			return true;
		}
		
		
		#region Element Structure
		public struct Element
		{
			public int							Length
			{
				get
				{
					return this.length;
				}
				set
				{
					this.length = (ushort) value;
				}
			}
			
			public Layout.StretchProfile		Profile
			{
				get
				{
					return this.profile;
				}
				set
				{
					this.profile = value;
				}
			}
			
			
			public int							FrameIndex
			{
				get
				{
					return this.frame_index;
				}
				set
				{
					this.frame_index = value;
				}
			}
			
			
			public double						LineStartX
			{
				get
				{
					return this.line_start_x;
				}
				set
				{
					this.line_start_x = value;
				}
			}
			
			public double						LineOriginX
			{
				get
				{
					return this.line_origin_x;
				}
				set
				{
					this.line_origin_x = value;
				}
			}
			
			public double						LineBaseY
			{
				get
				{
					return this.line_base_y;
				}
				set
				{
					this.line_base_y = value;
				}
			}
			
			public double						LineY1
			{
				get
				{
					return this.line_y1;
				}
				set
				{
					this.line_y1 = value;
				}
			}
			
			public double						LineY2
			{
				get
				{
					return this.line_y2;
				}
				set
				{
					this.line_y2 = value;
				}
			}
			
			public double						LineWidth
			{
				get
				{
					return this.line_width;
				}
				set
				{
					this.line_width = value;
				}
			}
			
			public double						LineAscender
			{
				get
				{
					return this.line_ascender;
				}
				set
				{
					this.line_ascender = value;
				}
			}
			
			public double						LineDescender
			{
				get
				{
					return this.line_descender;
				}
				set
				{
					this.line_descender = value;
				}
			}
			
			
			public bool							IsTabulation
			{
				get
				{
					return this.is_tabulation;
				}
				set
				{
					this.is_tabulation = value;
				}
			}
			
			public bool							IsNewLine
			{
				get
				{
					return this.is_new_line;
				}
				set
				{
					this.is_new_line = value;
				}
			}
			
			
			private ushort						length;
			private Layout.StretchProfile		profile;
			private int							frame_index;
			private double						line_start_x;
			private double						line_origin_x;
			private double						line_base_y;
			private double						line_y1;
			private double						line_y2;
			private double						line_width;
			private double						line_ascender;
			private double						line_descender;
			private bool						is_tabulation;
			private bool						is_new_line;
		}
		#endregion
		
		#region FilterFrame Class
		private class FilterFrame
		{
			public FilterFrame(int index)
			{
				this.index = index;
			}
			
			
			public bool FilterCallback(ICursor cursor, int position)
			{
				Cursors.FitterCursor fitter = cursor as Cursors.FitterCursor;
				
				if (fitter != null)
				{
					foreach (Element elem in fitter.elements)
					{
						if (elem.FrameIndex == this.index)
						{
							return true;
						}
					}
				}
				
				return false;
			}
			
			
			private int							index;
		}
		#endregion
		
		#region FilterFitter Class
		private class FilterFitter
		{
			public FilterFitter(TextFitter text_fitter)
			{
				this.text_fitter = text_fitter;
			}
			
			
			public bool FilterCallback(ICursor cursor, int position)
			{
				Cursors.FitterCursor fitter = cursor as Cursors.FitterCursor;
				
				if (fitter != null)
				{
					return fitter.TextFitter == this.text_fitter;
				}
				
				return false;
			}
			
			
			private TextFitter					text_fitter;
		}
		#endregion
		
		private static bool FilterCallback(ICursor cursor, int position)
		{
			return cursor is Cursors.FitterCursor;
		}
		
		
		private Element[]						elements;
		private double							paragraph_y;
		private double							paragraph_next_y;
		private double							space_after_paragraph;
		private bool							keep_with_next_paragraph;
		private TextFitter						fitter;
	}
}
