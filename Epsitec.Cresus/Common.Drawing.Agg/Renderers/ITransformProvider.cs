//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
