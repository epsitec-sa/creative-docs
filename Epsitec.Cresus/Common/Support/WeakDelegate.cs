//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

//	TODO: implement fully or kill

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe générique WeakDelegate permet de réaliser un lien avec WeakReference
	/// entre un événement et une cible, pour autant que la cilbe implémente l'interface
	/// IWeakDelegateTarget.
	/// </summary>
	/// <typeparam name="TEventArgs"></typeparam>
	public class WeakDelegate<TEventArgs>
		where TEventArgs : EventArgs
	{
		public WeakDelegate()
		{
			this.weak = null;
		}
		public WeakDelegate(EventHandler<TEventArgs> target)
		{
			this.weak = new System.WeakReference (new Trampoline (target));
		}

		public static WeakDelegate<TEventArgs> operator+(WeakDelegate<TEventArgs> w, EventHandler<TEventArgs> h)
		{
			w.Event += h;
			return w;
		}
		public static WeakDelegate<TEventArgs> operator-(WeakDelegate<TEventArgs> w, EventHandler<TEventArgs> h)
		{
			w.Event -= h;
			return w;
		}
		
		public event EventHandler<TEventArgs> Event
		{
			add
			{
				Trampoline trampoline;

				if (this.weak == null)
				{
					trampoline = new Trampoline ();
					this.weak = new System.WeakReference (trampoline);
				}
				else
				{
					trampoline = this.weak.Target as Trampoline;

					if (trampoline == null)
					{
						trampoline = new Trampoline ();
						this.weak.Target = trampoline;
					}
				}

				trampoline.Add (value);
			}
			remove
			{
				if (this.weak != null)
				{
					Trampoline trampoline = this.weak.Target as Trampoline;

					if (trampoline != null)
					{
						trampoline.Remove (value);
					}
				}
			}
		}

		public void Invoke(object sender, TEventArgs args)
		{
			if (this.weak != null)
			{
				Trampoline trampoline = this.weak.Target as Trampoline;

				if (trampoline == null)
				{
					this.weak = null;
				}
				else
				{
					trampoline.Invoke (sender, args);
				}
			}
		}
		
		private class Trampoline
		{
			public Trampoline()
			{
			}
			public Trampoline(EventHandler<TEventArgs> target)
			{
				this.Target += target;
			}

			~Trampoline()
			{
				System.Diagnostics.Debug.WriteLine ("Trampoline collected by GC");
			}

			public void Invoke(object sender, TEventArgs args)
			{
				if (this.Target != null)
				{
					this.Target (sender, args);
				}
			}
			public void Add(EventHandler<TEventArgs> h)
			{
				IWeakDelegateTarget iwdt = h.Target as IWeakDelegateTarget;
				
				if (iwdt == null)
				{
					throw new System.InvalidOperationException ("Target must implement IWeakDelegateTarget");
				}

				iwdt.AddTrampoline (this);
				this.Target += h;
			}
			public void Remove(EventHandler<TEventArgs> h)
			{
				IWeakDelegateTarget iwdt = h.Target as IWeakDelegateTarget;

				if (iwdt == null)
				{
					throw new System.InvalidOperationException ("Target must implement IWeakDelegateTarget");
				}
				
				iwdt.RemoveTrampoline (this);
				this.Target -= h;
			}

			private event EventHandler<TEventArgs> Target;
		}
		
		private System.WeakReference	weak;
	}
	
	public interface IWeakDelegateTarget
	{
		void AddTrampoline(object t);
		void RemoveTrampoline(object t);
	}
}
