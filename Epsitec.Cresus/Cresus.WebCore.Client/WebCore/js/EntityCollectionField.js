Ext.define('Epsitec.cresus.webcore.EntityCollectionField', {
  extend: 'Ext.container.Container',
  alternateClassName: ['Epsitec.EntityCollectionField'],
  alias: 'widget.epsitec.entitycollectionfield',

  /* Config */

  layout: 'column',

  /* Constructor */

  constructor: function(options) {
    var fields, button;

    options.columnWidth = 1;

    fields = Ext.widget('fieldcontainer', options);
    button = Ext.create('Ext.Button', {
      text: '>',
      renderTo: Ext.getBody(),
      margin: '0 0 0 5',
      handler: this.buttonHandler,
      scope: this
    });

    this.items = this.items || [];
    this.items.push(fields);
    this.items.push(button);

    this.callParent(arguments);

    return this;
  },

  buttonHandler: function() {
    var title = 'Cannot edit this enumeration',
        content = 'You cannot directly edit this enumeration. You will need ' +
                  'to save the current changes, click the header menu to ' +
                  'edit the corresponding enumeration, and come back to this ' +
                  'entity to edit it.';
    Ext.Msg.alert(title, content);
  }
});
