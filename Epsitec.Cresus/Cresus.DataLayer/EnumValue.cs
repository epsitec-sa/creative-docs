//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe EnumValue représente exactement une valeur d'une énumération,
	/// laquelle comprend un nom et les représentations pour l'utilisateur dans
	/// les diverses langues, avec éventuellement une explication.
	/// </summary>
	public class EnumValue : IDataAttributesHost, System.ICloneable
	{
		public EnumValue()
		{
		}
		
		public EnumValue(params string[] attributes)
		{
			this.Attributes.SetFromInitialisationList (attributes);
		}
		
		
		public string						Id
		{
			get { return this.Attributes.GetAttribute (Tags.Id, ResourceLevel.Default); }
		}
		
		public string						Name
		{
			get { return this.Attributes.GetAttribute (Tags.Name, ResourceLevel.Default); }
		}
		
		public string						UserLabel
		{
			get { return this.Attributes.GetAttribute (Tags.Caption); }
		}
		
		public string						UserDescription
		{
			get { return this.Attributes.GetAttribute (Tags.Description); }
		}
		
		
		#region ICloneable Members
		public virtual object Clone()
		{
			EnumValue value = System.Activator.CreateInstance (this.GetType ()) as EnumValue;
			
			value.attributes = this.Attributes.Clone () as DataAttributes;
			
			return value;
		}
		#endregion
		
		#region IDataAttributesHost Members
		public DataAttributes				Attributes
		{
			get
			{
				if (this.attributes == null)
				{
					this.attributes = new DataAttributes ();
				}
				
				return this.attributes;
			}
		}
		#endregion
		
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des valeurs, pas sur les
			//	détails internes...
			
			DataType that = obj as DataType;
			
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
				EnumValue ex = x as EnumValue;
				EnumValue ey = y as EnumValue;

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
				EnumValue ex = x as EnumValue;
				EnumValue ey = y as EnumValue;

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
		
		
		
		protected DataAttributes			attributes;
	}
}
