//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 23/04/2004

namespace Epsitec.Cresus.Database
{
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
	/// <summary>
	/// La classe DbType d�crit un type de donn�e pour sp�cifier DbColumn de
	/// mani�re plus pr�cise.
	/// </summary>
	public class DbType : IDbAttributesHost, System.ICloneable, Common.Types.INameCaption
	{
		public DbType()
		{
		}
		
		public DbType(params string[] attributes) : this ()
		{
			this.DefineAttributes (attributes);
		}
		
		public DbType(DbSimpleType type, params string[] attributes) : this (attributes)
		{
			this.Initialise (type);
		}
		
		
		public string							Name
		{
			get { return this.Attributes[Tags.Name, ResourceLevel.Default]; }
		}
		
		public string							Caption
		{
			get { return this.Attributes[Tags.Caption]; }
		}
		
		public string							Description
		{
			get { return this.Attributes[Tags.Description]; }
		}
		
		
		public DbAttributes						Attributes
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
		
		
		public DbSimpleType						SimpleType
		{
			get { return this.simple_type; }
		}
		
		public DbKey							InternalKey
		{
			get { return this.internal_type_key; }
		}
		
		
		internal virtual void SerializeXmlAttributes(System.Text.StringBuilder buffer, bool full)
		{
			if (full)
			{
				DbKey.SerializeToXmlAttributes (buffer, this.internal_type_key);
				this.Attributes.SerializeXmlAttributes (buffer);
			}
		}
		
		internal virtual void SerializeXmlElements(System.Text.StringBuilder buffer, bool full)
		{
		}
		
		internal virtual void DeserializeXmlAttributes(System.Xml.XmlElement xml)
		{
			this.internal_type_key = DbKey.DeserializeFromXmlAttributes (xml);
			this.Attributes.DeserializeXmlAttributes (xml);
		}
		
		internal virtual void DeserializeXmlElements(System.Xml.XmlNodeList nodes, ref int index)
		{
		}
		
		
		internal void DefineInternalKey(DbKey key)
		{
			if (this.internal_type_key == key)
			{
				return;
			}
			
			if (this.internal_type_key != null)
			{
				throw new System.InvalidOperationException (string.Format ("Type '{0}' cannot change its internal key.", this.Name));
			}
			
			this.internal_type_key = key.Clone () as DbKey;
		}
		
		internal void DefineAttributes(string[] attributes)
		{
			if (this.attributes != null)
			{
				this.attributes.SetFromInitialisationList (attributes);
			}
			else
			{
				this.attributes = new DbAttributes (attributes);
			}
		}
		
		internal void DefineName(string name)
		{
			if (this.attributes != null)
			{
				this.attributes.SetAttribute (Epsitec.Common.Support.Tags.Name, name);
			}
			else
			{
				this.attributes = new DbAttributes ();
			}
		}
		
		internal void Initialise(DbSimpleType type)
		{
			this.EnsureTypeIsNotInitialised();
			
			this.simple_type = type;
		}
		
		
		#region ICloneable Members
		public object Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ());
		}
		#endregion
		
		protected virtual object CloneNewObject()
		{
			return new DbType ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			DbType that = o as DbType;
			
			that.attributes  = this.attributes == null ? null : this.attributes.Clone () as DbAttributes;
			that.simple_type = this.simple_type;
			
			return that;
		}
		
		
		#region Equals and GetHashCode support
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'�galit� se base uniquement sur le nom des types, pas sur les
			//	d�tails internes...
			
			DbType that = obj as DbType;
			
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
		
		protected virtual void EnsureTypeIsNotInitialised()
		{
			if (this.simple_type != DbSimpleType.Unsupported)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type.");
			}
		}
		
		
		
		private DbAttributes					attributes;
		private DbSimpleType					simple_type;
		private DbKey							internal_type_key;}
}
