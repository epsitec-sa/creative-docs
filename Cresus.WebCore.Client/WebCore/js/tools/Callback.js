// This class represents a callback that might be executed in the future. It is
// basically a function with a context that represent the 'this' in the
// function, in order to make it easier to pass callbacks in functions, without
// having to worry about using a closure to store the 'this' variable that is
// available when the callback is defined.

Ext.define('Epsitec.cresus.webcore.tools.Callback', {
  alternateClassName: ['Epsitec.Callback'],

  /* Properties */

  callback: null,
  context: null,

  /* Constructor */

  constructor: function(callback, context) {
    this.callback = callback;
    this.context = context;
  },

  /* Methods */

  execute: function(callbackArguments) {
    this.callback.apply(this.context, callbackArguments);
  },

  /* Static Methods */

  statics: {
    create: function(func, context) {
      return Ext.create('Epsitec.Callback', func, context);
    }
  }
});
