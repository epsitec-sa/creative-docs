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
		
		
		protected string				prop_name;
		protected object				default_value;
	}
}
