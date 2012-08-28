//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Epsitec.Cresus.Core.Library;

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
				if (value == null)
				{
					this.Data = null;
				}
				else
				{
					string content = value.ToString ();

					if (content != this.xmlSourceCache)
					{
						this.Data = XmlBlobEntity.StringToByteArray (content);
						this.xmlSourceCache = content;
					}
				}
			}
		}

		public System.Guid GuidCode
		{
			get
			{
				string code = this.Code;

				if (string.IsNullOrEmpty (code))
				{
					return System.Guid.Empty;
				}
				else
				{
					return ItemCodeGenerator.ToGuid (code);
				}
			}
			set
			{
				if (System.Guid.Empty == value)
				{
					this.Code = null;
				}
				else
				{
					this.Code = ItemCodeGenerator.FromGuid (value);
				}
			}
		}


		public SettingsCollection GetSettingsCollection()
		{
			XElement xml = this.XmlData;

			if ((xml == null) ||
				(xml.Name != Xml.Settings))
			{
				return new SettingsCollection ();
			}
			else
			{
				var settings = new SettingsCollection ();

				settings.Restore (xml);

				return settings;
			}
		}

		public void SetSettingsCollection(SettingsCollection settings)
		{
			if ((settings == null) ||
				(settings.Count == 0))
			{
				this.XmlData = null;
			}
			else
			{
				this.XmlData = settings.Save (Xml.Settings);
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

		private static class Xml
		{
			public const string Settings = "settings";
		}

		private XElement xmlCache;
		private string xmlSourceCache;
	}
}
