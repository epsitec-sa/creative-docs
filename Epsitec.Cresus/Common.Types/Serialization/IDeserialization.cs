//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types.Serialization
{
	/// <summary>
	/// The <c>IDeserialization</c> interface is used to call objects after
	/// a deserialization.
	/// </summary>
	public interface IDeserialization
	{
		bool NotifyDeserializationStarted(Serialization.Context context);
		void NotifyDeserializationCompleted(Serialization.Context context);
	}
}
