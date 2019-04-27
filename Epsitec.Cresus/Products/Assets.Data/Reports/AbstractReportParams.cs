//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public abstract class AbstractReportParams : IGuid, System.IEquatable<AbstractReportParams>
	{
		public AbstractReportParams()
		{
			this.customTitle = null;
			this.guid        = Guid.NewGuid ();
		}

		public AbstractReportParams(string customTitle)
		{
			this.customTitle = customTitle;
			this.guid        = Guid.NewGuid ();
		}

		public AbstractReportParams(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == X.Guid)
					{
						var s = reader.ReadElementContentAsString ();
						this.guid = s.ParseGuid ();
					}
					else if (reader.Name == X.CustomTitle)
					{
						this.customTitle = reader.ReadElementContentAsString ();

						break;  // fin de la lecture de la classe abstraite -> on passe à la classe dérivée
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}


		#region IGuid Members
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}
		#endregion


		public string							CustomTitle
		{
			get
			{
				return this.customTitle;
			}
		}

		public abstract string					Title
		{
			get;
		}

		public virtual bool						HasParams
		{
			get
			{
				return true;
			}
		}


		#region IEquatable<AbstractReportParams> Members
		public virtual bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			return !object.ReferenceEquals (other, null)
				&& this.CustomTitle == other.CustomTitle;
		}
		#endregion

		public override sealed bool Equals(object obj)
		{
			return this.Equals (obj as AbstractReportParams);
		}

		public override int GetHashCode()
		{
			return this.CustomTitle.GetHashCode ();
		}


		public static bool operator ==(AbstractReportParams a, AbstractReportParams b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(AbstractReportParams a, AbstractReportParams b)
		{
			return !(a == b);
		}


		public virtual AbstractReportParams ChangePeriod(int direction)
		{
			return null;
		}

		public virtual AbstractReportParams ChangeCustomTitle(string customTitle)
		{
			return null;
		}


		public virtual void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString (X.Guid,        this.guid.ToStringIO ());
			writer.WriteElementString (X.CustomTitle, this.customTitle);
		}


		private readonly Guid					guid;
		private readonly string					customTitle;
	}
}
