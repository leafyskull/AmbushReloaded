# Ambush: Reloaded
### At least this is the name I am using for now


### MVP Specs:
- World map
- Battle map
- Friendly squad & unit control
- Enemy squad & unit implementation / AI

### Battle controls
- Left-click a player unit to select it; its tile turns yellow.
- Choose Move, Shoot, or Grenade from the action panel, or press M, S, or G, then left-click a target tile.
- Move accepts an empty, walkable tile one step away horizontally or vertically.
- Shooting uses one round, including when targeting an empty tile. Without a weapon assigned, shots deal 1 damage; an assigned weapon supplies damage and hit chance.
- Grenades use one grenade and deal 3 damage on the target tile and its four adjacent tiles, including friendly units. Damage and splash radius can be changed on the Unit prefab.
- Click another player unit to switch selection. Right-click, press Escape, or click outside the map to deselect.
