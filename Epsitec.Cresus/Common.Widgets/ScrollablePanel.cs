namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ScrollablePanel crée un Panel encapsulé dans une instance
	/// Scrollable, avec un LayoutEngine assigné par défaut au Panel.
	/// </summary>
	public class ScrollablePanel : Scrollable
	{
		public ScrollablePanel()
		{
			this.CreatePanel ();
			this.CreateLayoutEngine ();
		}
		
		public ScrollablePanel(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Helpers.AbstractLayoutEngine		LayoutEngine
		{
			get
			{
				return this.layout_engine;
			}
			set
			{
				if (this.layout_engine != value)
				{
					if (this.layout_engine != null)
					{
						this.layout_engine.Panel = null;
					}
					
					this.layout_engine = value;
					
					if (this.layout_engine != null)
					{
						this.layout_engine.Panel = this.Panel;
					}
				}
			}
		}
		
		
		protected virtual void CreatePanel()
		{
			this.Panel = new Panel (this);
		}
		
		protected virtual void CreateLayoutEngine()
		{
			this.LayoutEngine = new Helpers.AbsPosLayoutEngine ();
		}
		
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if (this.layout_engine != null)
			{
				this.layout_engine.Validate ();
			}
		}
		
		
		protected Helpers.AbstractLayoutEngine	layout_engine;
	}
}
