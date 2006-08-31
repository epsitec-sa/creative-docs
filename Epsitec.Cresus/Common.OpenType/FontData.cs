//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontData</c> class is a wrapper for the internal OpenType font
	/// tables. It keeps a reference to the raw font data.
	/// </summary>
	public sealed class FontData
	{
		internal FontData()
		{
		}

		internal FontData(byte[] data, int ttcIndex)
		{
			if ((data[0] == 't') &&
				(data[1] == 't') &&
				(data[2] == 'c') &&
				(data[3] == 'f'))
			{
				//	This is not a unique font, but a TrueType font Collection.
				
				Table_ttcf ttcf = new Table_ttcf (data, 0);
				
				System.Diagnostics.Debug.Assert (ttcIndex >= 0);
				System.Diagnostics.Debug.Assert (ttcIndex < ttcf.NumFonts);
				
				//	The table directory is located further in the file :
				
				int font_offset = ttcf.GetFontOffset (ttcIndex);
				
				this.Initialize (new TableDirectory (data, font_offset));
			}
			else
			{
				this.Initialize (new TableDirectory (data, 0));
			}
		}
		
		
		public TableEntry						this[string name]
		{
			get
			{
				if (this.ot_directory == null)
				{
					return null;
				}
				else
				{
					return this.ot_directory.FindTable (name);
				}
			}
		}
		
		public TableDirectory					Directory
		{
			get
			{
				return this.ot_directory;
			}
		}
		
		public System.ArraySegment<byte>		Data
		{
			get
			{
				return new System.ArraySegment<byte> (this.ot_directory.BaseData, this.ot_directory.BaseOffset, this.ot_directory.BaseLength);
			}
		}
		
		
		public Table_ttcf						TrueTypeCollectionTable
		{
			get
			{
				byte[] data = this.ot_directory.BaseData;

				if ((data[0] == 't') &&
					(data[1] == 't') &&
					(data[2] == 'c') &&
					(data[3] == 'f'))
				{
					return new Table_ttcf (data, 0);
				}
				else
				{
					return null;
				}
			}
		}
		
		
		private void Initialize(TableDirectory directory)
		{
			System.Diagnostics.Debug.Assert (this.ot_directory == null);
			System.Diagnostics.Debug.Assert (directory != null);
			
			this.ot_directory = directory;
		}
		
		
		private TableDirectory					ot_directory;
	}
}
