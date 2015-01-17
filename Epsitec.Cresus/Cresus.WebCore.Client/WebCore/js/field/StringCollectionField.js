// This class is an edition field that lets the user choose a value from an
// array of string, by using a combo box.

Ext.require([

],
function() {
  Ext.define('Epsitec.cresus.webcore.field.StringCollectionField', {
    extend: 'Ext.form.field.ComboBox',
    alternateClassName: ['Epsitec.StringCollectionField'],
    alias: 'widget.epsitec.stringcollectionfield',

    /* Configuration */
    queryMode: 'local',
    forceSelection: true,
    editable: false,

    /* Constructor */

    constructor: function(options) {
      var store, newOptions;


      newOptions = {
        store: options.value
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    }
  });
});
