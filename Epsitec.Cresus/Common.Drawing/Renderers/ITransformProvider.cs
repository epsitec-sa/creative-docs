namespace Epsitec.Common.Drawing.Renderers
{
	public interface ITransformProvider
	{
		event Support.EventHandler			TransformUpdating;
		
		Transform							Transform				{ get; set; }
		Transform							InternalTransform		{ get; }
	}
}
