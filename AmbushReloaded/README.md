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

### World map encounters
- Start in WorldMapScene. Both map scenes are enabled in Build Settings, with the world map first.
- Click the player squad, choose Move, Shoot, or Grenade (M/S/G), then click a target tile. Right-click or Escape deselects.
- Movement accepts an empty orthogonally adjacent tile. Each enemy squad has 2–3 randomly chosen hidden ambush tiles among its empty orthogonal neighbors. At edges or with blocked neighbors, there may be fewer; the initial enemies spawn in the interior to allow 2–3 each.
- Entering an ambush tile loads BattleMapScene with that tile's enemy squads at full health. Overlapping ambush zones include all their enemy squads.
- Shooting an enemy tile loads a battle with 1 opening damage per enemy unit.
- Grenades include enemies on the target tile (3 opening damage per unit) and its four orthogonal neighbors (1 per unit). All hit squads participate, with three battle units per squad. Diagonals do not count.
- Misses leave the player on the world map. World attacks currently have unlimited supplies and range; squads do not yet own an inventory. Grenades on the world map currently apply opening damage to enemies only.
- Configure the scene name and opening damage on WorldMapController. Direct grenade damage is kept greater than splash damage.
- Ambush is an overlay on WorldMapTile linked to enemy squads, rather than a separate terrain type or prefab. Call GenerateAmbushTiles after changing enemy placement to rebuild the zones.
- Map controllers live only in their own scene. BattleEncounter carries coordinates, triggering action, and damage values across the transition; BattleMapController consumes it once. Opening the battle scene directly still spawns the default three healthy enemies.
- Returning to the world map and saving campaign/unit state are future work.

### Squads
- A Squad is the world-map group; a Unit is an individual soldier on the battle map.
- Assign the starting team, optional player/enemy sprites, and unit references on the Squad component. An unassigned team sprite keeps the prefab's existing sprite.
- Use AddUnit and RemoveUnit to change membership. Units can belong to one squad at a time; remove a member before adding it to another squad. Units/UnitCount expose the roster, and Unit.CurrentSquad identifies its owner. Death removes a unit from its squad.
- SetTeam updates the squad's sprite and all members' teams. Membership does not change unit parenting, visibility, or tile placement.
- Squad.MoveToTile moves the player's squad through WorldMapController, preserving adjacent-tile validation, selection, and ambush handling.
- The current Squad prefab has no assigned members. Battle encounters still create fresh units; roster references do not survive the scene transition. Persistent rosters and battle deployment remain future work.
