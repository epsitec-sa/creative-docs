namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VToolBar permet de réaliser des tool bars verticales.
	/// </summary>
	public class VToolBar : AbstractToolBar
	{
		public VToolBar()
		{
			this.direction = Direction.Left;
		}
		
		public VToolBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		static VToolBar()
		{
			Helpers.VisualPropertyMetadata metadataDx = new Helpers.VisualPropertyMetadata (28.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataPadding = new Helpers.VisualPropertyMetadata (new Drawing.Margins (3, 3, 3, 3), Helpers.VisualPropertyMetadataOptions.AffectsChildrenLayout);
			
			Visual.PreferredWidthProperty.OverrideMetadata (typeof (VToolBar), metadataDx);
			Visual.PaddingProperty.OverrideMetadata (typeof (VToolBar), metadataPadding);
		}
		
		public override DockStyle			DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Top;
			}
		}
	}
}
