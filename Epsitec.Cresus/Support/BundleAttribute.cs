namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe BundleAttribute définit un attribut [Bundle] qui
	/// peut être utilisé pour spécifier des champs qui sont stockés
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
		
		
		protected string				prop_name;
		protected object				default_value;
	}
}
