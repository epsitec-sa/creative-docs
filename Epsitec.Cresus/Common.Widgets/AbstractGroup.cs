namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractGroup sert de base aux autres classes qui
	/// implémentent des groupes de widgets.
	/// </summary>
	[Support.SuppressBundleSupport]
	public abstract class AbstractGroup : Widget
	{
		public AbstractGroup()
		{
		}
		
		public virtual Drawing.Rectangle	Inside
		{
			get { return this.Client.Bounds; }
		}
	}
}
