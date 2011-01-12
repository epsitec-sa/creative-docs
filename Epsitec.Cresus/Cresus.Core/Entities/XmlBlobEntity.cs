//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Cresus.Core.Business.Accounting;

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
				string content = XmlBlobEntity.ByteArrayToString (this.Data);

				try
				{
					return XElement.Parse (content);
				}
				catch
				{
					return null;
				}
			}
			set
			{
				string content = value.ToString ();
				this.Data = XmlBlobEntity.StringToByteArray (content);
			}
		}

		private static byte[] StringToByteArray(string str)
		{
			System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding ();
			return encoding.GetBytes (str);
		}

		private static string ByteArrayToString(byte[] data)
		{
			System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
			return encoding.GetString (data);
		}
	}
}
