//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing.Renderers
{
	public interface ITransformProvider
	{
		event Support.EventHandler			TransformUpdating;
		
		Transform							Transform				{ get; set; }
		Transform							InternalTransform		{ get; }
	}
}
