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
			
			
			private ushort						length;
			private Layout.StretchProfile		profile;
		}
		
		
		private static bool FilterCallback(ICursor cursor, int position)
		{
			return cursor is Cursors.FitterCursor;
		}
		
		
		private Element[]						elements;
	}
}
