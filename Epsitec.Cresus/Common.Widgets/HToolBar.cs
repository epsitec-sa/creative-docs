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
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataPadding = Visual.PaddingProperty.DefaultMetadata.Clone ();
			
			metadataDy.DefineDefaultValue (28.0);
			metadataPadding.DefineDefaultValue (new Drawing.Margins (3, 3, 3, 3));
			
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
