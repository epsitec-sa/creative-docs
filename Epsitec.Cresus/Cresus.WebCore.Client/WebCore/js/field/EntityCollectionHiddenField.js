// This class is a hidden field that is used by EntityCollectionField to store
// the values that must be submitted to the server.

Ext.define('Epsitec.cresus.webcore.field.EntityCollectionHiddenField', {
  extend: 'Ext.form.field.Hidden',
  alternateClassName: ['Epsitec.EntityCollectionHiddenField'],

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
