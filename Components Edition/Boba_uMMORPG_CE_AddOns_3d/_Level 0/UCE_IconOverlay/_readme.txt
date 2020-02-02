This is a (work in progress) replacement for the text based quest indicators, it can
be used for monsters, players, pets and npcs as well by using this command to show an icon
only on the client side for a short duration or permanently:

iconOverlay.Show(0);

Where the number is the index of the sprite that is shown, you can edit those sprites
on each IconOverlay (on an entity) individually.

Just add the IconOverlay prefab to the entity, at the position where the quest marker
was before.