Ext.define('Epsitec.cresus.webcore.EntityCollectionHiddenField', {
  extend: 'Ext.form.field.Hidden',
  alternateClassName: ['Epsitec.EntityCollectionHiddenField'],

  /* Properties */

  submitValueGetter: null,
  onReset: null,

  /* Additional methods */

  getSubmitValue: function() {
    return this.submitValueGetter();
  },

  reset: function() {
    this.callParent(arguments);
    this.onReset();
  }
});
