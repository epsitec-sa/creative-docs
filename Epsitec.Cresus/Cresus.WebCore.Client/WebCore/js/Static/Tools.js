Ext.define('Epsitec.Cresus.Core.Static.Tools', {
  alias: 'epsitec.tools',

  statics: {
    isUndefined: function(item) {
      return typeof item === 'undefined';
    },

    getValueOrDefault: function(value, defaultValue) {
      return this.isUndefined(value) ?
          defaultValue : value;
    },

    getValueOrNull: function(value)  {
      return this.getValueOrDefault(value, null);
    },

    doCallback: function(callback, context, callbackArguments) {
      if (this.isUndefined(callback) || callback === null) {
        return;
      }

      callback.apply(context, callbackArguments);
    }
  }
});
