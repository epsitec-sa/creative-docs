Ext.require([
  'Epsitec.cresus.webcore.EntityCollectionHiddenField',
  'Epsitec.cresus.webcore.EntityFieldList'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EntityCollectionField', {
    extend: 'Ext.form.FieldContainer',
    alternateClassName: ['Epsitec.EntityCollectionField'],
    alias: 'widget.epsitec.entitycollectionfield',

    /* Config */

    layout: 'vbox',

    /* Properties */

    entityFieldList: null,
    hiddenField: null,
    originalValues: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions;

      this.entityFieldList = this.createEntityFieldList(options);
      this.hiddenField = this.createHiddenField(options);

      newOptions = {
        fieldLabel: options.fieldLabel,
        items: [this.entityFieldList, this.hiddenField],
        originalValues: options.values
      };

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEntityFieldList: function(options) {
      var values = options.values,
          readOnly = options.readOnly;

      return Ext.create('Epsitec.EntityFieldList', values, readOnly, {
        width: '100%',
        minHeight: '50',
        maxHeight: '150',
        databaseName: options.databaseName
      });
    },

    createHiddenField: function(options) {
      var me = this;
      return Ext.create('Epsitec.EntityCollectionHiddenField', {
        name: options.name,
        submitValueGetter: function() { return me.getSubmitValue(); },
        onReset: function() { me.resetEntityFieldList(); }
      });
    },

    getSubmitValue: function() {
      return this.entityFieldList
          .getItems()
          .map(function(item) { return item.submitted; })
          .join(';');
    },

    resetEntityFieldList: function() {
      this.entityFieldList.resetContent();
    }
  });
});
