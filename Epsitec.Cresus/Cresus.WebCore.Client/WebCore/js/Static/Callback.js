// This class represents a callback that might be executed in the future. It is
// basically a function with a context that represent the 'this' in the function,
// in order to make it easier to pass callbacks in functions.

Ext.define('Epsitec.Cresus.Core.Static.Callback',
  {
    alias : 'epsitec.callback',
    
    /* Properties */
    callback : null,
    context : null,
    
    /* Constructor */
    constructor : function (callback, context)
    {
      this.callback = callback;
      this.context = context;
    },    
    
    /* Methods */
    execute : function (callbackArguments)
    {
      this.callback.apply(this.context, callbackArguments);
    },
  }
);