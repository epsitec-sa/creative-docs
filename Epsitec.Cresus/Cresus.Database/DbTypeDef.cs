//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTypeDef</c> class represents a type definition, as stored in
	/// the database.
	/// </summary>
	public class DbTypeDef : ICaption, IName
	{
		public DbTypeDef()
		{
		}
		
		public DbSimpleType SimpleType
		{
			get
			{
				return this.simpleType;
			}
		}

		public DbRawType RawType
		{
			get
			{
				return this.rawType;
			}
		}

		public DbNumDef NumDef
		{
			get
			{
				return this.numDef;
			}
		}

		public INamedType Type
		{
			get
			{
				return this.type;
			}
		}

		public DbKey Key
		{
			get
			{
				return this.key;
			}
		}

		#region ICaption Members

		public Druid CaptionId
		{
			get
			{
				return this.type == null ? Druid.Empty : this.type.CaptionId;
			}
		}

		#endregion

		#region IName Members

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					return this.type == null ? null : this.type.Name;
				}
				else
				{
					return this.name;
				}
			}
		}

		#endregion
		
		internal void DefineInternalKey(DbKey key)
		{
			if (this.key == key)
			{
				return;
			}

			if (this.key != null)
			{
				throw new System.InvalidOperationException (string.Format ("Type '{0}' cannot change its internal key.", this.Name));
			}

			this.key = key.Clone () as DbKey;
		}


		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("type");

			DbTools.WriteAttribute (xmlWriter, "name", this.Name);
			DbTools.WriteAttribute (xmlWriter, "id", DbTools.TypeToString (this.type));
			DbTools.WriteAttribute (xmlWriter, "raw", DbTools.RawTypeToString (this.rawType));
			DbTools.WriteAttribute (xmlWriter, "simple", DbTools.SimpleTypeToString (this.simpleType));
			DbTools.WriteAttribute (xmlWriter, "length", DbTools.IntToString (this.length));
			DbTools.WriteAttribute (xmlWriter, "fixed", this.isFixedSize ? "Y" : null);
			DbTools.WriteAttribute (xmlWriter, "multi", this.isMultilingual ? "Y" : null);

			if (this.numDef != null)
			{
				this.numDef.SerializeAttributes (xmlWriter, "n.");
			}
			
			xmlWriter.WriteEndElement ();
		}

		private static readonly Caption nullCaption = new Caption ();

		private string name;
		private INamedType type;
		private DbRawType rawType;
		private DbSimpleType simpleType;
		private DbNumDef numDef;
		private int length;
		private bool isFixedSize;
		private bool isMultilingual;
		private DbKey key;
	}
}
