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
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataPadding = Visual.PaddingProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (28.0);
			metadataPadding.DefineDefaultValue (new Drawing.Margins (3, 3, 3, 3));
			
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
