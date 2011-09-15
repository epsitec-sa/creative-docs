//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <see cref="EventHandlerExtensions"/> class provides extensions methods for instances of
	/// <see cref="EventHandler{T}"/>.
	/// </summary>
	public static class EventHandlerExtensions
	{
		/// <summary>
		/// Raises an event on <paramref name="eventHandler"/> using the standard C# design
		/// pattern.
		/// </summary>
		/// <remarks>
		/// This extensions methods is to be used on simple auto generated field like event declared
		/// by "public event EventHandler{T} handler".
		/// The field like event ensures that the registration and unregistration are done in a
		/// consistent way (in C# 4).
		/// This methods then ensures that the event is not fired if the event handler is null
		/// because no one has registered to it, or if all listeners have been removed.
		/// However, there is no guarantee on the thread safety while the event is being raised. In
		/// particular, a method can be called by the event handler after it has been unregistered
		/// from that event handler, if event is raised on a thread and the unregistration is done
		/// on another. The reason for that is that the only way to achieve thread safety in this
		/// case would be to hold a lock on the event during its execution which would be a deadlock
		/// prone solution.
		/// 
		/// See the following links for more references
		/// http://blogs.msdn.com/b/cburrows/archive/2010/03/05/events-get-a-little-overhaul-in-c-4-part-i-locks.aspx
		/// http://blogs.msdn.com/b/cburrows/archive/2010/03/08/events-get-a-little-overhaul-in-c-4-part-ii-semantic-changes-and.aspx
		/// http://blogs.msdn.com/b/cburrows/archive/2010/03/18/events-get-a-little-overhaul-in-c-4-part-iii-breaking-changes.aspx
		/// http://blogs.msdn.com/b/cburrows/archive/2010/03/30/events-get-a-little-overhaul-in-c-4-afterward-effective-events.aspx
		/// http://www.codeproject.com/Articles/37474/Threadsafe-Events.aspx
		/// </remarks>
		/// <typeparam name="T">The type of the event data.</typeparam>
		/// <param name="eventHandler">The event handler to fire.</param>
		/// <param name="sender">The object considered as the sender of the event.</param>
		/// <param name="eventArg">The data of the event.</param>
		public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T eventArg)
			where T : System.EventArgs
		{
			if (eventHandler != null)
			{
				eventHandler (sender, eventArg);
			}
		}
		
		public static void Raise(this EventHandler eventHandler, object sender)
		{
			if (eventHandler != null)
			{
				eventHandler (sender);
			}
		}
	}
}
