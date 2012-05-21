//	Copyright © 2005-2009, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

		public bool								NeedsMoreRoom
		{
			get
			{
				if ((this.elements.Length > 0) &&
					(this.elements[this.elements.Length-1].FrameIndex == -1))
				{
					return true;
				}
				else
				{
					return false;
				}
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
				return this.paragraphY;
			}
		}
		
		public double							ParagraphNextY
		{
			get
			{
				return this.paragraphNextY;
			}
		}
		
		public double							SpaceAfterParagraph
		{
			get
			{
				return this.spaceAfterParagraph;
			}
		}
		
		public bool								KeepWithNextParagraph
		{
			get
			{
				return this.keepWithNextParagraph;
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
			this.paragraphY = y;
		}
		
		public void DefineParagraphNextY(double y)
		{
			this.paragraphNextY = y;
		}
		
		public void DefineSpaceAfterParagraph(double y)
		{
			this.spaceAfterParagraph = y;
		}
		
		public void DefineKeepWithNextParagraph(bool keep)
		{
			this.keepWithNextParagraph = keep;
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
					
					Element[] oldElements = this.elements;
					Element[] newElements = new Element[m + n];
					
					System.Array.Copy (oldElements, 0, newElements, 0, m);
					
					for (int i = 0; i < n; i++)
					{
						newElements[m+i] = (Element) values[i];
					}
					
					this.elements = newElements;
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
		
		
		public static CursorInfo.Filter GetFrameFilter(int frameIndex)
		{
			FilterFrame filter = new FilterFrame (frameIndex);
			return new CursorInfo.Filter (filter.FilterCallback);
		}
		
		public static CursorInfo.Filter GetFitterFilter(TextFitter fitter)
		{
			FilterFitter filter = new FilterFitter (fitter);
			return new CursorInfo.Filter (filter.FilterCallback);
		}
		
		
		internal int[] GetLineStartPositions(Internal.TextTable text)
		{
			//	Détermine où commencent des lignes (position exprimée par rapport
			//	au début du texte) en filtrant les éléments qui sont liés à des
			//	marques de tabulation :
			
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
				int[] temp = new int[count];
				System.Array.Copy (pos, 0, temp, 0, count);
				pos = temp;
			}
			
			return pos;
		}
		
		
		internal int GetElementStartPosition(int index)
		{
			if ((index < 0) ||
				(index >= this.elements.Length))
			{
				throw new System.ArgumentOutOfRangeException ("index", index, string.Format ("Index {0} not in 0..{1}", index, this.elements.Length));
			}
			
			int offset = 0;
			
			for (int i = 0; i < index; i++)
			{
				offset += this.elements[i].Length;
			}
			
			return offset;
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
					return this.frameIndex;
				}
				set
				{
					this.frameIndex = value;
				}
			}
			
			
			public double						LineStartX
			{
				get
				{
					return this.lineStartX;
				}
				set
				{
					this.lineStartX = value;
				}
			}
			
			public double						LineOriginX
			{
				get
				{
					return this.lineOriginX;
				}
				set
				{
					this.lineOriginX = value;
				}
			}
			
			public double						LineBaseY
			{
				get
				{
					return this.lineBaseY;
				}
				set
				{
					this.lineBaseY = value;
				}
			}
			
			public double						LineY1
			{
				get
				{
					return this.lineY1;
				}
				set
				{
					this.lineY1 = value;
				}
			}
			
			public double						LineY2
			{
				get
				{
					return this.lineY2;
				}
				set
				{
					this.lineY2 = value;
				}
			}
			
			public double						LineWidth
			{
				get
				{
					return this.lineWidth;
				}
				set
				{
					this.lineWidth = value;
				}
			}
			
			public double						LineAscender
			{
				get
				{
					return this.lineAscender;
				}
				set
				{
					this.lineAscender = value;
				}
			}
			
			public double						LineDescender
			{
				get
				{
					return this.lineDescender;
				}
				set
				{
					this.lineDescender = value;
				}
			}
			
			
			public bool							IsTabulation
			{
				get
				{
					return this.isTabulation;
				}
				set
				{
					this.isTabulation = value;
				}
			}
			
			public bool							IsNewLine
			{
				get
				{
					return this.isNewLine;
				}
				set
				{
					this.isNewLine = value;
				}
			}
			
			
			private ushort						length;
			private Layout.StretchProfile		profile;
			private int							frameIndex;
			private double						lineStartX;
			private double						lineOriginX;
			private double						lineBaseY;
			private double						lineY1;
			private double						lineY2;
			private double						lineWidth;
			private double						lineAscender;
			private double						lineDescender;
			private bool						isTabulation;
			private bool						isNewLine;
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
			public FilterFitter(TextFitter textFitter)
			{
				this.textFitter = textFitter;
			}
			
			
			public bool FilterCallback(ICursor cursor, int position)
			{
				Cursors.FitterCursor fitter = cursor as Cursors.FitterCursor;
				
				if (fitter != null)
				{
					return fitter.TextFitter == this.textFitter;
				}
				
				return false;
			}
			
			
			private TextFitter					textFitter;
		}
		#endregion
		
		private static bool FilterCallback(ICursor cursor, int position)
		{
			return cursor is Cursors.FitterCursor;
		}
		
		
		private Element[]						elements;
		private double							paragraphY;
		private double							paragraphNextY;
		private double							spaceAfterParagraph;
		private bool							keepWithNextParagraph;
		private TextFitter						fitter;
	}
}
