//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 24/11/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	using Converter = Epsitec.Common.Support.Data.Converter;
	
	/// <summary>
	/// La classe DbEnumValue représente exactement une valeur d'une énumération,
	/// laquelle comprend un nom et les représentations pour l'utilisateur dans
	/// les diverses langues, avec éventuellement une explication.
	/// </summary>
	public class DbEnumValue : IDbAttributesHost, System.ICloneable
	{
		public DbEnumValue()
		{
		}
		
		public DbEnumValue(string name) : this ()
		{
			this.attributes[Tags.Name] = name;
		}
		
		public DbEnumValue(int rank, string name) : this (name)
		{
			this.rank = rank;
		}
		
		public DbEnumValue(System.Xml.XmlElement xml)
		{
			this.ProcessXmlDefinition (xml);
		}
		
		
		public string						Name
		{
			get { return this.Attributes[Tags.Name, ResourceLevel.Default]; }
		}
		
		public string						Caption
		{
			get { return this.Attributes[Tags.Caption]; }
		}
		
		public string						Description
		{
			get { return this.Attributes[Tags.Description]; }
		}
		
		public int							Rank
		{
			get { return this.rank; }
		}
		
		public DbKey						InternalKey
		{
			get { return this.internal_key; }
		}
		
		
		public   void DefineAttributes(params string[] attributes)
		{
			this.attributes.SetFromInitialisationList (attributes);
		}
		
		internal void DefineInternalKey(DbKey key)
		{
			if (this.internal_key == key)
			{
				return;
			}
			
			if (this.internal_key != null)
			{
				throw new System.InvalidOperationException (string.Format ("Enum Value '{0}' cannot change its internal key.", this.Name));
			}
			
			this.internal_key = key.Clone () as DbKey;
		}
		
		
		public static DbEnumValue NewEnumValue(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			return DbEnumValue.NewEnumValue (doc.DocumentElement);
		}
		
		public static DbEnumValue NewEnumValue(System.Xml.XmlElement xml)
		{
			return (xml.Name == "null") ? null : new DbEnumValue (xml);
		}

		
		public static string SerialiseToXml(DbEnumValue value, bool full)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbEnumValue.SerialiseToXml (buffer, value, full);
			return buffer.ToString ();
		}
		
		public static void SerialiseToXml(System.Text.StringBuilder buffer, DbEnumValue value, bool full)
		{
			if (value == null)
			{
				buffer.Append ("<null/>");
			}
			else
			{
				value.SerialiseXmlDefinition (buffer, full);
			}
		}
		
		
		protected void SerialiseXmlDefinition(System.Text.StringBuilder buffer, bool full)
		{
			buffer.Append (@"<enumval");
			
			if (this.rank >= 0)
			{
				buffer.Append (@" rank=""");
				buffer.Append (Converter.ToString (this.rank));
				buffer.Append (@"""");
			}
			
			if (full)
			{
				DbKey.SerialiseToXmlAttributes (buffer, this.internal_key);
				this.Attributes.SerialiseXmlAttributes (buffer);
			}
			
			buffer.Append (@"/>");
		}
		
		protected void ProcessXmlDefinition(System.Xml.XmlElement xml)
		{
			if (xml.Name != "enumval")
			{
				throw new System.FormatException (string.Format ("Expected root element named <enumval>, but found <{0}>.", xml.Name));
			}
			
			Converter.Convert (xml.GetAttribute ("rank"), out this.rank);
			
			this.internal_key = DbKey.DeserialiseFromXmlAttributes (xml);
			this.Attributes.DeserialiseXmlAttributes (xml);
		}
		
		
		#region ICloneable Members
		public object Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ());
		}
		#endregion
		
		protected virtual object CloneNewObject()
		{
			return new DbEnumValue ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			DbEnumValue that = o as DbEnumValue;
			
			that.rank         = this.rank;
			that.internal_key = (this.internal_key == null) ? null : this.internal_key.Clone () as DbKey; 
			that.attributes   = (this.attributes == null) ? null : this.attributes.Clone () as DbAttributes;
			
			return that;
		}
		
		
		#region IDbAttributesHost Members
		public DbAttributes					Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new DbAttributes ();
				}
				
				return this.attributes;
			}
		}
		#endregion
		
		#region Equals and GetHash support
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des valeurs, pas sur les
			//	détails internes...
			
			DbEnumValue that = obj as DbEnumValue;
			
			if (that == null)
			{
				return false;
			}
			
			return (this.Name == that.Name);
		}
		
		public override int GetHashCode()
		{
			string name = this.Name;
			return (name == null) ? 0 : name.GetHashCode ();
		}
		#endregion
		
		public static System.Collections.IComparer	NameComparer
		{
			get
			{
				return new ComparerClass (Tags.Name);
			}
		}
		
		public static System.Collections.IComparer	RankComparer
		{
			get
			{
				return new RankComparerClass ();
			}
		}
		
		
		private class ComparerClass : System.Collections.IComparer
		{
			public ComparerClass(string tag)
			{
				this.tag = tag;
			}
			
			#region IComparer Members
			public int Compare(object x, object y)
			{
				DbEnumValue ex = x as DbEnumValue;
				DbEnumValue ey = y as DbEnumValue;

				if (ex == ey)
				{
					return 0;
				}
				
				if (ex == null)
				{
					return -1;
				}
				if (ey == null)
				{
					return 1;
				}
				
				return string.Compare (ex.Attributes[this.tag, ResourceLevel.Default], ey.Attributes[this.tag, ResourceLevel.Default], false, System.Globalization.CultureInfo.InvariantCulture);
			}
			#endregion
			
			private string					tag;
		}
		
		
		private class RankComparerClass : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				DbEnumValue ex = x as DbEnumValue;
				DbEnumValue ey = y as DbEnumValue;

				if (ex == ey)
				{
					return 0;
				}
				
				if (ex == null)
				{
					return -1;
				}
				if (ey == null)
				{
					return 1;
				}
				
				int rx = ex.Rank;
				int ry = ey.Rank;
				
				return rx - ry;
			}
			#endregion
		}
		
		
		
		protected DbAttributes				attributes = new DbAttributes ();
		protected int						rank = -2;
		protected DbKey						internal_key = null;
	}
}
