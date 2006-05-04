namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HToolBar permet de réaliser des tool bars horizontales.
	/// </summary>
	public class HToolBar : AbstractToolBar
	{
		public HToolBar()
		{
			this.direction = Direction.Up;
		}
		
		public HToolBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		static HToolBar()
		{
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata (28.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Helpers.VisualPropertyMetadata metadataPadding = new Helpers.VisualPropertyMetadata (new Drawing.Margins (3, 3, 3, 3), Helpers.VisualPropertyMetadataOptions.AffectsChildrenLayout);

			Visual.PreferredHeightProperty.OverrideMetadata (typeof (HToolBar), metadataDy);
			Visual.PaddingProperty.OverrideMetadata (typeof (HToolBar), metadataPadding);
		}
		
		public override DockStyle			DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Left;
			}
		}
	}
}
