//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class XmlBlobEntity
	{
		//	Donne accès au champ 'Data' sous sa forme effective d'un XElement.
		public XElement XmlData
		{
			get
			{
				if ((this.xmlCache == null) &&
					(this.Data != null) && (this.Data.Length > 0))
				{
					string content = XmlBlobEntity.ByteArrayToString (this.Data);

					try
					{
						this.xmlSourceCache = content;
						this.xmlCache = XElement.Parse (content);
					}
					catch
					{
						this.xmlSourceCache = null;
						this.xmlCache = null;
					}
				}
				
				return this.xmlCache;
			}
			set
			{
				string content = value.ToString ();

				if (content != this.xmlSourceCache)
				{
					this.Data = XmlBlobEntity.StringToByteArray (content);
					this.xmlSourceCache = content;
				}
			}
		}

		partial void OnDataChanged(byte[] oldValue, byte[] newValue)
		{
			this.xmlCache       = null;
			this.xmlSourceCache = null;
		}

		private static byte[] StringToByteArray(string str)
		{
			var encoding = new System.Text.UTF8Encoding ();
			return encoding.GetBytes (str);
		}

		private static string ByteArrayToString(byte[] data)
		{
			var encoding = new System.Text.UTF8Encoding ();
			return encoding.GetString (data);
		}

		private XElement xmlCache;
		private string xmlSourceCache;
	}
}
