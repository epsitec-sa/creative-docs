//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Cursors
{
	/// <summary>
	/// Summary description for FitterCursor.
	/// </summary>
	public class FitterCursor : Cursors.TempCursor
	{
		public FitterCursor()
		{
		}
		
		
		public static CursorInfo.Filter			Filter
		{
			get
			{
				return new CursorInfo.Filter (FitterCursor.FilterCallback);
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
		
		
		public static CursorInfo.Filter GetFrameFilter(int frame_index)
		{
			FilterFrame filter = new FilterFrame (frame_index);
			return new CursorInfo.Filter (filter.FilterCallback);
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
			
			public double						FrameY
			{
				get
				{
					return this.frame_y;
				}
				set
				{
					this.frame_y = value;
				}
			}
			
			public double						FrameWidth
			{
				get
				{
					return this.frame_width;
				}
				set
				{
					this.frame_width = value;
				}
			}
			
			
			private ushort						length;
			private Layout.StretchProfile		profile;
			private int							frame_index;
			private double						frame_y;
			private double						frame_width;
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
		
		private static bool FilterCallback(ICursor cursor, int position)
		{
			return cursor is Cursors.FitterCursor;
		}
		
		
		private Element[]						elements;
	}
}
