namespace Epsitec.Common.Widgets.Design.Tools
{
	/// <summary>
	/// Summary description for TextBundleArrayStore.
	/// </summary>
	public class TextBundleArrayStore : Support.Data.ITextArrayStore
	{
		public TextBundleArrayStore()
		{
		}
		
		
		public Support.ResourceBundle			Bundle
		{
			get
			{
				return this.bundle;
			}
			set
			{
				this.bundle = value;
			}
		}
		
		
		#region ITextArrayStore Members
		public void InsertRows(int row, int num)
		{
			if (this.StoreChanged != null)
			{
				this.StoreChanged (this);
			}
		}
		
		public void RemoveRows(int row, int num)
		{
			if (this.StoreChanged != null)
			{
				this.StoreChanged (this);
			}
		}
		
		public string GetCellText(int row, int column)
		{
			Support.ResourceBundle.Field field = this.bundle[row];
			
			switch (column)
			{
				case 0:
					return TextLayout.ConvertToTaggedText (field.Name);
				case 1:
					return field.AsString;
			}
			
			return null;
		}
		
		public void SetCellText(int row, int column, string value)
		{
			Support.ResourceBundle.Field field = this.bundle[row];
			
			switch (column)
			{
				case 0:
					field.SetName (TextLayout.ConvertToSimpleText (value));
					break;
				case 1:
					field.SetStringValue (value);
					break;
			}
		}
		
		public int GetColumnCount()
		{
			return 2;
		}
		
		public int GetRowCount()
		{
			if (this.bundle != null)
			{
				return this.bundle.CountFields;
			}
			
			return 0;
		}
		
		public event Support.EventHandler		StoreChanged;
		#endregion
		
		protected void DumpXml()
		{
			System.Diagnostics.Debug.WriteLine (this.bundle.CreateXmlDocument (false).OuterXml);
		}
		
		protected Support.ResourceBundle		bundle;
	}
}
