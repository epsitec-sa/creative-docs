namespace Epsitec.Common.Drawing.Renderer
{
	public interface ITransformProvider
	{
		event System.EventHandler			TransformUpdating;
		
		Transform							Transform				{ get; set; }
		Transform							InternalTransform		{ get; }
	}
}
