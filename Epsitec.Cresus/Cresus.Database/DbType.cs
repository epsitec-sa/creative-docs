//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/10/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
	/// <summary>
	/// La classe DbType décrit un type de donnée pour spécifier DbColumn de
	/// manière plus précise.
	/// </summary>
	public class DbType : IDbAttributesHost, System.ICloneable
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
		
		public DbType(System.Xml.XmlElement xml)
		{
		}
		
		
		internal virtual void SerialiseXmlAttributes(System.Text.StringBuilder buffer)
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
		
		
		public string					Name
		{
			get { return this.Attributes[Tags.Name, ResourceLevel.Default]; }
		}
		
		public string					Caption
		{
			get { return this.Attributes[Tags.Caption]; }
		}
		
		public string					Description
		{
			get { return this.Attributes[Tags.Description]; }
		}
		
		
		public DbSimpleType				SimpleType
		{
			get { return this.simple_type; }
		}
		
		public DbKey					InternalKey
		{
			get { return this.internal_type_key; }
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
		
		#region Equals and GetHashCode support
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des types, pas sur les
			//	détails internes...
			
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
		
		
		
		private DbAttributes			attributes;
		private DbSimpleType			simple_type;
		private DbKey					internal_type_key;
		
		internal const string			TagKeyId		= "CR_KeyId";
		internal const string			TagKeyRevision	= "CR_KeyRevision";
		internal const string			TagKeyStatus	= "CR_KeyStatus";
		internal const string			TagName			= "CR_Name";
		internal const string			TagCaption		= "CR_Caption";
		internal const string			TagDescription	= "CR_Description";
		internal const string			TagInfoXml		= "CR_InfoXml";
	}
}
