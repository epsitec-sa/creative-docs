//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

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
				return new IdComparerClass ();
			}
		}
		
		public static System.Collections.IComparer	NameComparer
		{
			get
			{
				return new NameComparerClass ();
			}
		}
		
		
		private class IdComparerClass : System.Collections.IComparer
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
				
				return string.Compare (ex.Id, ey.Id);
			}
			#endregion
		}
		
		private class NameComparerClass : System.Collections.IComparer
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
				
				return string.Compare (ex.Name, ey.Name);
			}
			#endregion
		}
		
		
		
		protected DbAttributes				attributes;
	}
}
