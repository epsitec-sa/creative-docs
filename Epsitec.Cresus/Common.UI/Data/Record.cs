//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Record décrit un ensemble de champs utilisés pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public class Record : Types.AbstractDataCollection, Types.IDataFolder
	{
		public Record()
		{
		}
		
		
		#region IDataItem Members
		public Types.DataItemClasses			Classes
		{
			get
			{
				return Types.DataItemClasses.Folder;
			}
		}
		#endregion
		
		#region INameCaption Members
		public string							Description
		{
			get
			{
				return null;
			}
		}
		
		public string							Caption
		{
			get
			{
				return null;
			}
		}
		
		public string							Name
		{
			get
			{
				return null;
			}
		}
		#endregion
		
		protected override Types.IDataItem[] GetCachedItemArray()
		{
			return this.fields;
		}
		
		protected override void ClearCachedItemArray()
		{
			base.ClearCachedItemArray ();
			
			this.fields = null;
		}
		
		protected override void UpdateCachedItemArray()
		{
			base.UpdateCachedItemArray ();
			
			this.fields = new Field[this.list.Count];
			this.list.CopyTo (this.fields);
		}

		
		private Field[]							fields;
	}
}
