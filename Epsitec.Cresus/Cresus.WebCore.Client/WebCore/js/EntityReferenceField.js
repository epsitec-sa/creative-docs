Ext.define('Epsitec.cresus.webcore.EntityReferenceField', {
  extend: 'Ext.container.Container',
  alternateClassName: ['Epsitec.EntityReferenceField'],
  alias: 'widget.epsitec.entityreferencefield',

  /* Config */

  layout: 'column',

  /* Constructor */

  constructor: function(options) {
    var combo, button;

    options.columnWidth = 1;

    combo = Ext.create('Epsitec.EntityReferenceComboBox', options);
    button = Ext.create('Ext.Button', {
      text: '>',
      renderTo: Ext.getBody(),
      margin: '19 0 0 5',
      handler: this.buttonHandler,
      scope: this
    });

    this.items = this.items || [];
    this.items.push(combo);
    this.items.push(button);

    this.callParent(arguments);

    return this;
  },

  buttonHandler: function() {
    var title = 'Cannot edit this list',
        content = 'You cannot directly edit this list. You will need to save ' +
                  'the current changes, click the header menu to edit the ' +
                  'corresponding list, and come back to this entity to edit ' +
                  'it.';
    Ext.Msg.alert(title, content);
  }
});
