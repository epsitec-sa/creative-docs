using System;

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
		
		public abstract void CreateWidgets(Widget parent, Drawing.Point origin);
		
		protected Drawing.Size			size;
	}
}
