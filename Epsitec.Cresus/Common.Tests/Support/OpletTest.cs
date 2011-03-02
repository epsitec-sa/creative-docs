using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
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
			
			Assert.IsTrue (queue.CanUndo == true);
			Assert.IsTrue (queue.CanRedo == false);
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			//	Accède aux éléments de l'action en cours :
			
			oplets = queue.LastActionOplets;
			
			Assert.IsNotNull (oplets);
			Assert.AreEqual (2, oplets.Length);
			Assert.AreEqual ("A1", (oplets[0] as NamedOplet).Name);
			Assert.AreEqual ("A2", (oplets[1] as NamedOplet).Name);
			
			//	Défait l'action.
			
			queue.UndoAction ();
			
			System.Diagnostics.Debug.WriteLine ("Action A undone");
			
			Assert.IsTrue (queue.CanUndo == false);
			Assert.IsTrue (queue.CanRedo == true);
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (1, queue.RedoActionCount);
			
			//	Refait l'action.
			
			queue.RedoAction ();
			
			System.Diagnostics.Debug.WriteLine ("Action A redone");
			
			Assert.IsTrue (queue.CanUndo == true);
			Assert.IsTrue (queue.CanRedo == false);
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
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
			
			System.Diagnostics.Debug.WriteLine ("Action B canceled automatically");
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			//	Commence la création d'une action avec abandon explicite :
			
			using (queue.BeginAction ())
			{
				queue.Insert (this.CreateOplet ("B'1"));
				queue.CancelAction ();
			}

			System.Diagnostics.Debug.WriteLine ("Action B' canceled manually");
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
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

			System.Diagnostics.Debug.WriteLine ("Action C inserted / C1 canceled");
			
			Assert.AreEqual (2, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
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
			
			Assert.AreEqual (3, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			//	Accède aux éléments de l'action en cours :
			
			oplets = queue.LastActionOplets;
			
			Assert.IsNotNull (oplets);
			Assert.AreEqual (4, oplets.Length);
			Assert.AreEqual ("D1",   (oplets[0] as NamedOplet).Name);
			Assert.AreEqual ("D1-1", (oplets[1] as NamedOplet).Name);
			Assert.AreEqual ("D1-2", (oplets[2] as NamedOplet).Name);
			Assert.AreEqual ("D2",   (oplets[3] as NamedOplet).Name);
			
			queue.UndoAction ();
			queue.UndoAction ();
			
			System.Diagnostics.Debug.WriteLine ("Undone twice");
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (2, queue.RedoActionCount);
			
			oplets = queue.LastActionOplets;
			
			Assert.IsNotNull (oplets);
			Assert.AreEqual (2, oplets.Length);
			Assert.AreEqual ("A1", (oplets[0] as NamedOplet).Name);
			Assert.AreEqual ("A2", (oplets[1] as NamedOplet).Name);
			
			queue.PurgeRedo ();
			
			System.Diagnostics.Debug.WriteLine ("Purged redo");
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.PurgeUndo ();
			
			System.Diagnostics.Debug.WriteLine ("Purged undo");
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
		}
		
		[Test] public void CheckInsertUndoRedoEmpty()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ("a"))
			{
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			Assert.AreEqual (0, queue.UndoActionNames.Length);
			Assert.AreEqual (0, queue.RedoActionNames.Length);
		}

		[Test] public void CheckPurgeSingleUndo()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ("a"))
			{
				queue.Insert (this.CreateOplet ("A1"));
				queue.Insert (this.CreateOplet ("A2"));
				queue.ValidateAction ();
			}
			
			using (queue.BeginAction ("b"))
			{
				queue.Insert (this.CreateOplet ("B1"));
				queue.Insert (this.CreateOplet ("B2"));
				queue.Insert (this.CreateOplet ("B3"));
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (2, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.PurgeSingleUndo ();
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.PurgeSingleUndo ();
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
		}

		[Test] public void CheckMergeLastActions()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ("a"))
			{
				queue.Insert (this.CreateOplet ("A1"));
				queue.Insert (this.CreateOplet ("A2"));
				queue.ValidateAction ();
			}
			
			using (queue.BeginAction ("b"))
			{
				queue.Insert (this.CreateOplet ("B1"));
				queue.Insert (this.CreateOplet ("B2"));
				queue.Insert (this.CreateOplet ("B3"));
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (2, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.MergeLastActions ();
			
			IOplet[] oplets = queue.LastActionOplets;
			
			Assert.IsNotNull (oplets);
			Assert.AreEqual (5, oplets.Length);
			Assert.AreEqual ("A1", (oplets[0] as NamedOplet).Name);
			Assert.AreEqual ("A2", (oplets[1] as NamedOplet).Name);
			Assert.AreEqual ("B1", (oplets[2] as NamedOplet).Name);
			Assert.AreEqual ("B2", (oplets[3] as NamedOplet).Name);
			Assert.AreEqual ("B3", (oplets[4] as NamedOplet).Name);
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.PurgeSingleUndo ();
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
		}

		[Test] public void CheckHistory()
		{
			OpletQueue queue = new OpletQueue ();
			
			using (queue.BeginAction ("a"))
			{
				queue.Insert (this.CreateOplet ("A1"));
				queue.Insert (this.CreateOplet ("A2"));
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			Assert.AreEqual (1, queue.UndoActionNames.Length);
			Assert.AreEqual (0, queue.RedoActionNames.Length);
			Assert.AreEqual ("a", queue.UndoActionNames[0]);
			
			queue.UndoAction ();
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (1, queue.RedoActionCount);
			Assert.AreEqual (0, queue.UndoActionNames.Length);
			Assert.AreEqual (1, queue.RedoActionNames.Length);
			Assert.AreEqual ("a", queue.RedoActionNames[0]);
			
			queue.RedoAction ();
			
			using (queue.BeginAction ("b"))
			{
				queue.Insert (this.CreateOplet ("B1"));
				queue.Insert (this.CreateOplet ("B2"));
				
				using (queue.BeginAction ("b-a"))
				{
					queue.Insert (this.CreateOplet ("B-A1"));
					queue.Insert (this.CreateOplet ("B-A2"));
					queue.Insert (this.CreateOplet ("B-A3"));
					queue.ValidateAction ();
				}
				
				queue.Insert (this.CreateOplet ("B3"));
				queue.ValidateAction ();
			}
			
			using (queue.BeginAction ("c"))
			{
				queue.Insert (this.CreateOplet ("C1"));
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (3, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			Assert.AreEqual (3, queue.UndoActionNames.Length);
			Assert.AreEqual (0, queue.RedoActionNames.Length);
			Assert.AreEqual ("c", queue.UndoActionNames[0]);
			Assert.AreEqual ("b", queue.UndoActionNames[1]);
			Assert.AreEqual ("a", queue.UndoActionNames[2]);
			
			queue.UndoAction ();
			
			Assert.AreEqual (2, queue.UndoActionCount);
			Assert.AreEqual (1, queue.RedoActionCount);
			Assert.AreEqual (2, queue.UndoActionNames.Length);
			Assert.AreEqual (1, queue.RedoActionNames.Length);
			Assert.AreEqual ("b", queue.UndoActionNames[0]);
			Assert.AreEqual ("a", queue.UndoActionNames[1]);
			Assert.AreEqual ("c", queue.RedoActionNames[0]);
			
			queue.UndoAction ();
			
			Assert.AreEqual (1, queue.UndoActionCount);
			Assert.AreEqual (2, queue.RedoActionCount);
			Assert.AreEqual (1, queue.UndoActionNames.Length);
			Assert.AreEqual (2, queue.RedoActionNames.Length);
			Assert.AreEqual ("a", queue.UndoActionNames[0]);
			Assert.AreEqual ("b", queue.RedoActionNames[0]);
			Assert.AreEqual ("c", queue.RedoActionNames[1]);
			
			queue.UndoAction ();
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (3, queue.RedoActionCount);
			Assert.AreEqual (0, queue.UndoActionNames.Length);
			Assert.AreEqual (3, queue.RedoActionNames.Length);
			Assert.AreEqual ("a", queue.RedoActionNames[0]);
			Assert.AreEqual ("b", queue.RedoActionNames[1]);
			Assert.AreEqual ("c", queue.RedoActionNames[2]);
		}
		
		[Test] public void CheckFailUndo()
		{
			OpletQueue queue = new OpletQueue ();
			Assert.IsTrue (! queue.UndoAction ());
		}
		
		[Test] public void CheckFailRedo()
		{
			OpletQueue queue = new OpletQueue ();
			Assert.IsTrue (! queue.RedoAction ());
		}
		
		[Test] public void CheckDisableUndoRedo()
		{
			OpletQueue queue = new OpletQueue ();
			
			Assert.IsFalse (queue.IsDisabled);
			Assert.IsTrue (queue.IsEnabled);
			
			queue.Disable ();
			
			Assert.IsTrue (queue.IsDisabled);
			Assert.IsFalse (queue.IsEnabled);
			
			using (queue.BeginAction ("a"))
			{
				queue.Insert (this.CreateOplet ("A1"));
				queue.Insert (this.CreateOplet ("A2"));
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.UndoAction ();
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
			
			queue.RedoAction ();
			
			using (queue.BeginAction ("b"))
			{
				queue.Insert (this.CreateOplet ("B1"));
				queue.Insert (this.CreateOplet ("B2"));
				
				using (queue.BeginAction ("b-a"))
				{
					queue.Insert (this.CreateOplet ("B-A1"));
					queue.Insert (this.CreateOplet ("B-A2"));
					queue.Insert (this.CreateOplet ("B-A3"));
					queue.ValidateAction ();
				}
				
				queue.Insert (this.CreateOplet ("B3"));
				queue.ValidateAction ();
			}
			
			using (queue.BeginAction ("c"))
			{
				queue.Insert (this.CreateOplet ("C1"));
				queue.ValidateAction ();
			}
			
			Assert.AreEqual (0, queue.UndoActionCount);
			Assert.AreEqual (0, queue.RedoActionCount);
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
