//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 24/11/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
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
		
		public DbEnumValue(params string[] attributes) : this ()
		{
			this.attributes = new DbAttributes (attributes);
		}
		
		
		public string						Id
		{
			get { return this.Attributes[Tags.Id, ResourceLevel.Default]; }
		}
		
		public string						Information
		{
			get { return this.Attributes[Tags.InfoXml, ResourceLevel.Default]; }
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
			get
			{
				if (this.rank == -2)
				{
					//	Si 'rank' n'a jamais été initialisé, faisons-le maintenant en se
					//	basant sur l'attributs 'sort'.
					
					this.rank = -1;
					this.ParseInformation (this.Information);
				}
				
				return this.rank;
			}
		}
		
		
		protected void ParseInformation(string xml)
		{
			if (xml == null)
			{
				return;
			}
			
			//	L'information existe (sous forme XML). On va donc demander à .NET de
			//	parser l'arbre XML.
			
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			
			this.ParseInformation (doc.DocumentElement);
		}
		
		protected virtual void ParseInformation(System.Xml.XmlElement xml)
		{
			if (xml.Name != "enumval")
			{
				throw new System.ArgumentException (string.Format ("Expected root element named <enumval>, but found <{0}>.", xml.Name));
			}
			
			string arg_rank = xml.GetAttribute ("rank");
			
			if (arg_rank != "")
			{
				this.rank = System.Int32.Parse (arg_rank, System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		
		
		#region ICloneable Members
		public virtual object Clone()
		{
			DbEnumValue value = System.Activator.CreateInstance (this.GetType ()) as DbEnumValue;
			
			value.attributes = (this.attributes == null) ? null : this.attributes.Clone () as DbAttributes;
			
			return value;
		}
		#endregion
		
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
		
		public static System.Collections.IComparer	IdComparer
		{
			get
			{
				return new ComparerClass (Tags.Id);
			}
		}
		
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
		
		
		
		protected DbAttributes				attributes;
		protected int						rank = -2;
	}
}
