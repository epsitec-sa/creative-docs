//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ICaption</c> interface gives access to a caption ID associated
	/// with the object. This can then be mapped to a <see cref="T:Caption"/>
	/// instance through the resource manager, for instance.
	/// </summary>
	public interface ICaption
	{
		/// <summary>
		/// Gets the caption id for the object.
		/// </summary>
		/// <value>The caption id (or -1 if there is none).</value>
		long CaptionId
		{
			get;
		}
	}
}
