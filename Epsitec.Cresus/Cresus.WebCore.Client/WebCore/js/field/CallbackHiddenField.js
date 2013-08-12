// This class is a hidden field that is used by some user defined field. It does
// not store the value directly, but instead gets its from a callback.

Ext.define('Epsitec.cresus.webcore.field.CallbackHiddenField', {
  extend: 'Ext.form.field.Hidden',
  alternateClassName: ['Epsitec.CallbackHiddenField'],

  /* Properties */

  submitValueGetter: null,
  onReset: null,

  /* Methods */

  getSubmitValue: function() {
    return this.submitValueGetter();
  },

  reset: function() {
    this.callParent(arguments);
    this.onReset();
  }
});
