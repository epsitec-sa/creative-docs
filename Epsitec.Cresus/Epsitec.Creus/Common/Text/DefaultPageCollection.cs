//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for DefaultPageCollection.
	/// </summary>
	public class DefaultPageCollection : IPageCollection
	{
		public DefaultPageCollection()
		{
			this.count = 1;
		}
		
		
		#region IPageCollection Members
		public int GetPageCount()
		{
			return this.count;
		}
		
		public string GetPageLabel(int page)
		{
			return page.ToString (System.Globalization.CultureInfo.CurrentCulture);
		}
		
		public PageFlags GetPageFlags(int page)
		{
			PageFlags flags = PageFlags.None;
			
			Debug.Assert.IsInBounds (page, 0, this.count-1);
			
			//	Comme les numéros de pages "physiques" partent de 0, on convertit
			//	en numéros "logiques" (1..n) pour la suite des opérations :
			
			page += 1;
			
			if (page == 1)
			{
				flags |= PageFlags.First;
			}
			
			if ((page & 1) == 0)
			{
				flags |= PageFlags.Even;
			}
			else
			{
				flags |= PageFlags.Odd;
			}
			
			return flags;
		}
		
		public void SetPageProperty(int page, string key, string value)
		{
			if (this.properties == null)
			{
				this.properties = new System.Collections.Hashtable ();
			}
			
			key = page.ToString (System.Globalization.CultureInfo.InvariantCulture) + "/" + key;
			
			this.properties[key] = value;
		}
		
		public string GetPageProperty(int page, string key)
		{
			if (this.properties == null)
			{
				return null;
			}
			
			key = page.ToString (System.Globalization.CultureInfo.InvariantCulture) + "/" + key;
			
			return this.properties[key] as string;
		}
		
		public void ClearPageProperties(int page)
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			string match = page.ToString (System.Globalization.CultureInfo.InvariantCulture) + "/";
			
			foreach (string key in this.properties.Keys)
			{
				if (key.StartsWith (match))
				{
					list.Add (key);
				}
			}
			
			foreach (string key in list)
			{
				this.properties.Remove (key);
			}
		}
		#endregion
		
		private int								count;
		private System.Collections.Hashtable	properties;
	}
}
