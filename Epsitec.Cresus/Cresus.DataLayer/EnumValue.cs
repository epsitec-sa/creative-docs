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
			get { return this.Attributes.GetAttribute (DataAttributes.TagId, ResourceLevel.Default); }
		}
		
		public string						Name
		{
			get { return this.Attributes.GetAttribute (DataAttributes.TagName, ResourceLevel.Default); }
		}
		
		public string						UserLabel
		{
			get { return this.Attributes.GetAttribute (DataAttributes.TagLabel); }
		}
		
		public string						UserDescription
		{
			get { return this.Attributes.GetAttribute (DataAttributes.TagDescription); }
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

		
		protected DataAttributes			attributes;
	}
}
