# Unreleased

## Bug fixes
* Fixed "Tilemancer" enemies leaving their tiles behind if they die before throwing
    them.

## Changes
* Enemies now drop loot after their death animation finishes, instead of
    immediately when their health reaches zero.

# 1.1.2: 2023-05-21
## Bug fixes
* Fixed stone statues becoming invisible after respawning if the player dies
    after killing one.

* Fixed a number of instances where certain objects would be drawn "above"
    other objects when they logically shouldn't.

# 1.1.1: 2023-04-03

## New Content
* Added a new plant-based enemy: the Chompweed.  It lives up to its name.
* Added a new stone-based enemy: the Stone Statue.  It does NOT live up to its name.

## Changes
* Enemies are no longer restricted to spawning in the corners of rooms.
    They can now also spawn in the center, or in any of the four cardinal
    directions. They are still guaranteed to never spawn in front of a doorway,
    so you don't need to worry about walking into them as soon as you enter the
    room.
* All circular shadows in the game now have a consistent appearance.
* The coin collection sound should now hurt your ears less.
* Players no longer experience recoil when rotating a node in the "electric circuit"
    puzzle

## Technical
* Updated to Godot 3.5.2.

# 1.1.0: 2023-03-05

## Changes
* Spruced up the UI with a custom theme.  No more default Godot controls!
* Keys are now found inside chests, which appear when you clear the room they're in.
* A set of stairs now appears when you defeat the boss, instead of a chest.
* You can now throw the weighted stones in the scale puzzle.

## Bug fixes
* Fixed the "electric circuit" puzzle not being properly scrambled.
* Fixed a rare crash upon generating a dungeon, caused by the game trying to
    create a dungeon that can't be laid out without rooms overlapping each other.
    Now the game will place rooms on multiple floors in this scenario.

# 1.0.0: 2023-02-01
Initial release.