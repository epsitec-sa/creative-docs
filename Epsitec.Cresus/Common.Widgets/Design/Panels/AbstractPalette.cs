namespace Epsitec.Common.Widgets.Design.Panels
{
	/// <summary>
	/// La classe AbstractPalette est la base de toutes les classes XyzPalette.
	/// </summary>
	public abstract class AbstractPalette
	{
		public AbstractPalette()
		{
		}
		
		
		public virtual Drawing.Size		Size
		{
			get
			{
				return this.size;
			}
		}
		
		public virtual Widget			Widget
		{
			get
			{
				if (this.widget == null)
				{
					this.widget = this.CreateWidget ();
				}
				
				return this.widget;
			}
		}
		
		
		protected abstract Widget CreateWidget();
		
		protected Drawing.Size			size;
		protected Widget				widget;
	}
}
