//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
				
				TableTtcf ttcf = new TableTtcf (data, 0);
				
				System.Diagnostics.Debug.Assert (ttcIndex >= 0);
				System.Diagnostics.Debug.Assert (ttcIndex < ttcf.NumFonts);
				
				//	The table directory is located further in the file :
				
				int fontOffset = ttcf.GetFontOffset (ttcIndex);
				
				this.Initialize (new TableDirectory (data, fontOffset));
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
				if (this.otDirectory == null)
				{
					return null;
				}
				else
				{
					return this.otDirectory.FindTable (name);
				}
			}
		}
		
		public TableDirectory					Directory
		{
			get
			{
				return this.otDirectory;
			}
		}
		
		public System.ArraySegment<byte>		Data
		{
			get
			{
				return new System.ArraySegment<byte> (this.otDirectory.BaseData, this.otDirectory.BaseOffset, this.otDirectory.BaseLength);
			}
		}
		
		
		public TableTtcf						TrueTypeCollectionTable
		{
			get
			{
				byte[] data = this.otDirectory.BaseData;

				if ((data[0] == 't') &&
					(data[1] == 't') &&
					(data[2] == 'c') &&
					(data[3] == 'f'))
				{
					return new TableTtcf (data, 0);
				}
				else
				{
					return null;
				}
			}
		}
		
		
		private void Initialize(TableDirectory directory)
		{
			System.Diagnostics.Debug.Assert (this.otDirectory == null);
			System.Diagnostics.Debug.Assert (directory != null);
			
			this.otDirectory = directory;
		}
		
		
		private TableDirectory					otDirectory;
	}
}
