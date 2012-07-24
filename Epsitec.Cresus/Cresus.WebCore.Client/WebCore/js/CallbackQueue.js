// This class represents an immutable queue of callbacks. The callbacks can be
// enqueued one after another and they will be called in the order in that they
// where added with the execute() method.
// Two queues can be merged, in which case with the statement q1.merge(q2). In
// this cases, the elements of q1 will be executed before those of q2.
// Note that all the callbacks will be called synchronously. That means if part
// of a callback executes asynchronously, all the callback in the queue will be
// executed before the asynchronous operation terminates. Keep that in mind when
// dealing with this class.

Ext.define('Epsitec.cresus.webcore.CallbackQueue', {
  alternateClassName: ['Epsitec.CallbackQueue'],

  /* Properties */

  head: null,
  tail: null,

  /* Constructor */

  constructor: function(head, tail) {
    this.head = head;
    this.tail = tail;
  },

  /* Methods */

  isEmpty: function() {
    return this.tail === null;
  },

  enqueue: function(callback) {
    return Ext.create(
        'Epsitec.CallbackQueue', callback, this
    );
  },

  enqueueCallback: function(func, context) {
    return this.enqueue(Epsitec.Callback.create(func, context));
  },

  merge: function(queue) {
    if (queue.isEmpty()) {
      return this;
    }

    return this.merge(queue.tail).enqueue(queue.head);
  },

  execute: function(callbackArguments) {
    if (!this.isEmpty())
    {
      this.tail.execute(callbackArguments);
      this.head.execute(callbackArguments);
    }
  },

  statics: {
    empty: function() {
      return Ext.create('Epsitec.CallbackQueue', null, null);
    },

    create: function(func, context) {
      return this.empty().enqueueCallback(func, context);
    }
  }
});
