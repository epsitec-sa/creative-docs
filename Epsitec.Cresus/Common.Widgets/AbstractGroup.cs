namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractGroup sert de base aux autres classes qui
	/// implémentent des groupes de widgets.
	/// </summary>
	public abstract class AbstractGroup : Widget
	{
		public AbstractGroup()
		{
		}
		
		public virtual Drawing.Rectangle	Inside
		{
			get { return new Drawing.Rectangle (0, 0, this.Client.Width, this.Client.Height); }
		}
	}
}
