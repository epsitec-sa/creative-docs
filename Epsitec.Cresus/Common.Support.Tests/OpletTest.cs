using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class OpletTest
	{
		[Test] public void CheckInsertUndoRedo()
		{
			OpletQueue queue = new OpletQueue ();
			IOplet[] oplets;
			
			//	Insère une action composée de deux oplets :
			
			using (queue.BeginAction ())
			{
				queue.Insert (this.CreateOplet ("A1"));
				queue.Insert (this.CreateOplet ("A2"));
				queue.ValidateAction ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Action A created");
			
			Assertion.Assert (queue.CanUndo == true);
			Assertion.Assert (queue.CanRedo == false);
			Assertion.AssertEquals (1, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			//	Accède aux éléments de l'action en cours :
			
			oplets = queue.LastActionOplets;
			
			Assertion.AssertNotNull (oplets);
			Assertion.AssertEquals (2, oplets.Length);
			Assertion.AssertEquals ("A1", (oplets[0] as NamedOplet).Name);
			Assertion.AssertEquals ("A2", (oplets[1] as NamedOplet).Name);
			
			//	Défait l'action.
			
			queue.UndoAction ();
			
			System.Diagnostics.Debug.WriteLine ("Action A undone");
			
			Assertion.Assert (queue.CanUndo == false);
			Assertion.Assert (queue.CanRedo == true);
			Assertion.AssertEquals (0, queue.UndoActionCount);
			Assertion.AssertEquals (1, queue.RedoActionCount);
			
			//	Refait l'action.
			
			queue.RedoAction ();
			
			System.Diagnostics.Debug.WriteLine ("Action A redone");
			
			Assertion.Assert (queue.CanUndo == true);
			Assertion.Assert (queue.CanRedo == false);
			Assertion.AssertEquals (1, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			//	Commence la création d'une action avec abandon en cours de
			//	route :
			
			try
			{
				using (queue.BeginAction ())
				{
					queue.Insert (this.CreateOplet ("B1"));
					throw new System.Exception ();
				}
			}
			catch
			{
			}
			
			System.Diagnostics.Debug.WriteLine ("Action B cancelled automatically");
			
			Assertion.AssertEquals (1, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			//	Commence la création d'une action avec abandon explicite :
			
			using (queue.BeginAction ())
			{
				queue.Insert (this.CreateOplet ("B'1"));
				queue.CancelAction ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Action B' cancelled manually");
			
			Assertion.AssertEquals (1, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			//	Crée une action avec une sous-action qui est abandonnée :
			
			using (queue.BeginAction ())
			{
				queue.Insert (this.CreateOplet ("C1"));
				using (queue.BeginAction ())
				{
					queue.Insert (this.CreateOplet ("C1-1"));
					queue.Insert (this.CreateOplet ("C1-2"));
				}
				queue.Insert (this.CreateOplet ("C2"));
				queue.ValidateAction ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Action C inserted / C1 cancelled");
			
			Assertion.AssertEquals (2, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			//	Crée une action avec une sous-action qui est validée :
			
			using (queue.BeginAction ())
			{
				queue.Insert (this.CreateOplet ("D1"));
				using (queue.BeginAction ())
				{
					queue.Insert (this.CreateOplet ("D1-1"));
					queue.Insert (this.CreateOplet ("D1-2"));
					queue.ValidateAction ();
				}
				queue.Insert (this.CreateOplet ("D2"));
				queue.ValidateAction ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Action D inserted / D1 validated");
			
			Assertion.AssertEquals (3, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			//	Accède aux éléments de l'action en cours :
			
			oplets = queue.LastActionOplets;
			
			Assertion.AssertNotNull (oplets);
			Assertion.AssertEquals (4, oplets.Length);
			Assertion.AssertEquals ("D1",   (oplets[0] as NamedOplet).Name);
			Assertion.AssertEquals ("D1-1", (oplets[1] as NamedOplet).Name);
			Assertion.AssertEquals ("D1-2", (oplets[2] as NamedOplet).Name);
			Assertion.AssertEquals ("D2",   (oplets[3] as NamedOplet).Name);
			
			queue.UndoAction ();
			queue.UndoAction ();
			
			System.Diagnostics.Debug.WriteLine ("Undone twice");
			
			Assertion.AssertEquals (1, queue.UndoActionCount);
			Assertion.AssertEquals (2, queue.RedoActionCount);
			
			oplets = queue.LastActionOplets;
			
			Assertion.AssertNotNull (oplets);
			Assertion.AssertEquals (2, oplets.Length);
			Assertion.AssertEquals ("A1", (oplets[0] as NamedOplet).Name);
			Assertion.AssertEquals ("A2", (oplets[1] as NamedOplet).Name);
			
			queue.PurgeRedo ();
			
			System.Diagnostics.Debug.WriteLine ("Purged redo");
			
			Assertion.AssertEquals (1, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
			
			queue.PurgeUndo ();
			
			System.Diagnostics.Debug.WriteLine ("Purged undo");
			
			Assertion.AssertEquals (0, queue.UndoActionCount);
			Assertion.AssertEquals (0, queue.RedoActionCount);
		}
		
		
		[Test] public void CheckFailUndo()
		{
			OpletQueue queue = new OpletQueue ();
			Assertion.Assert (! queue.UndoAction ());
		}
		
		[Test] public void CheckFailRedo()
		{
			OpletQueue queue = new OpletQueue ();
			Assertion.Assert (! queue.RedoAction ());
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInsertUndoRedoEx1()
		{
			OpletQueue queue = new OpletQueue ();
			queue.Insert (this.CreateOplet ("test"));
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInsertUndoRedoEx2()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ())
			{
				queue.Insert (new BadOplet (queue));
				queue.ValidateAction ();
			}
			
			queue.UndoAction ();
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInsertUndoRedoEx3()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ())
			{
				queue.PurgeUndo ();
			}
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInsertUndoRedoEx4()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ())
			{
				queue.PurgeRedo ();
			}
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInsertUndoRedoEx5()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ())
			{
				queue.UndoAction ();
			}
		}
		
		[Test] [ExpectedException (typeof (System.InvalidOperationException))] public void CheckInsertUndoRedoEx6()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ())
			{
				queue.RedoAction ();
			}
		}
		
		
		private IOplet CreateOplet(string name)
		{
			return new NamedOplet (name);
		}
		
		
		private class NamedOplet : AbstractOplet
		{
			public NamedOplet(string name)
			{
				this.name = name;
			}
			
			
			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			
			
			public override IOplet Undo()
			{
				System.Diagnostics.Debug.WriteLine ("Undo " + this.name);
				return this;
			}
			
			public override IOplet Redo()
			{
				System.Diagnostics.Debug.WriteLine ("Redo " + this.name);
				return this;
			}
			
			public override void Dispose()
			{
				System.Diagnostics.Debug.WriteLine ("Dispose " + this.name);
			}
			
			
			private string						name;
		}
		
		private class BadOplet : AbstractOplet
		{
			public BadOplet(OpletQueue queue)
			{
				this.queue = queue;
			}
			
			
			public override IOplet Undo()
			{
				//	On n'a pas le droit de faire cela dans un UNDO/REDO :
				
				using (queue.BeginAction ())
				{
					queue.Insert (new NamedOplet ("t"));
					queue.ValidateAction ();
				}
				 
				return this;
			}
			
			public override IOplet Redo()
			{
				return this;
			}
			
			public override void Dispose()
			{
			}
			
			
			private OpletQueue					queue;
		}
	}
}
