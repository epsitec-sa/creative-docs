namespace Epsitec.Common.Drawing.Renderers
{
	public interface ITransformProvider
	{
		event System.EventHandler			TransformUpdating;
		
		Transform							Transform				{ get; set; }
		Transform							InternalTransform		{ get; }
	}
}
