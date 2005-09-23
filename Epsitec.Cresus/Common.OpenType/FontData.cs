//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe FontData donne accès aux tables internes d'une fonte OpenType.
	/// </summary>
	internal sealed class FontData
	{
		public FontData()
		{
		}
		
		public FontData(byte[] data, int ttc_index)
		{
			if ((data[0] == 't') &&
				(data[1] == 't') &&
				(data[2] == 'c') &&
				(data[3] == 'f'))
			{
				//	Ce n'est pas une fonte unique, mais une collection de fontes
				//	groupées dans une TrueType Collection.
				
				Table_ttcf ttcf = new Table_ttcf (data, 0);
				
				System.Diagnostics.Debug.Assert (ttc_index >= 0);
				System.Diagnostics.Debug.Assert (ttc_index < ttcf.NumFonts);
				
				//	Détermine l'offset dans le fichier TTC pour accéder aux
				//	véritables informations de la fonte :
				
				int font_offset = ttcf.GetFontOffset (ttc_index);
				
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
		
		public byte[]							Data
		{
			get
			{
				return this.ot_directory.BaseData;
			}
		}
		
		
		public Table_ttcf						TrueTypeCollection
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
				
				return null;
			}
		}
		
		
		private void Initialize(TableDirectory directory)
		{
			this.ot_directory = directory;
		}
		
		
		private TableDirectory					ot_directory;
	}
}
