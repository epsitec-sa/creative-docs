//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/10/2003

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe BundleAttribute d�finit un attribut [Bundle] qui
	/// peut �tre utilis� pour sp�cifier des champs qui sont stock�s
	/// dans un bundle.
	/// </summary>
	public class BundleAttribute : System.Attribute
	{
		public BundleAttribute()
		{
			this.prop_name = null;
		}
		
		public BundleAttribute(string name)
		{
			this.prop_name = name;
		}
		
		public object					DefaultValue
		{
			get { return this.default_value; }
			set { this.default_value = value; }
		}
		
		public string					PropertyName
		{
			get { return this.prop_name; }
			set { this.prop_name = value; }
		}
		
		
		protected string				prop_name;
		protected object				default_value;
	}
}
