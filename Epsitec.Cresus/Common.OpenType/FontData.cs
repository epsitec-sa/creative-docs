//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe FontData donne accès aux tables internes d'une fonte OpenType.
	/// </summary>
	public class FontData
	{
		public FontData()
		{
		}
		
		public FontData(byte[] data)
		{
			this.Initialize (new TableDirectory (data, 0));
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
		
		
		public void Initialize(TableDirectory directory)
		{
			this.ot_directory = directory;
		}
		
		
		private TableDirectory					ot_directory;
	}
}
