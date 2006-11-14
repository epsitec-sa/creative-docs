//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The <c>IDeserialization</c> interface is used to call objects after
	/// a deserialization.
	/// </summary>
	public interface IDeserialization
	{
		void NotifyDeserializationStarted(Serialization.Context context);
		void NotifyDeserializationCompleted(Serialization.Context context);
	}
}
