using System;
using System.Collections.Generic;
using System.Linq;

namespace ShiftingSepulcher
{
    public static class DungeonLayoutBuilder
    {
        public static DungeonLayout LayoutFromTree(DungeonTreeRoom root)
        {
            var layout = TryAddRoom(
                root,
                Vector3i.Zero,
                new DungeonLayout()
            );

            bool theUniverseMakesSense = layout != null;
            if (!theUniverseMakesSense)
                throw new DungeonLayoutException("There is no way to lay out all these rooms without overlap.");

            return layout;

            DungeonLayout TryAddRoom(
                DungeonTreeRoom room,
                Vector3i pos,
                DungeonLayout prevLayout
            )
            {
                // Fail if this spot is already taken
                if (prevLayout.HasRoomAt(pos))
                    return null;

                // Add this room to the layout
                var layoutWithThisRoom = prevLayout.WithRoomAt(pos, room);

                // Fail if we just placed a room with a shortcut in the wrong
                // location
                if (!IsAdjacentToAllPlacedShortcuts(layoutWithThisRoom, room))
                    return null;

                // Try to add all the children
                var layoutWithChildren = TryAddRoomChild(
                    room,
                    pos,
                    0,
                    layoutWithThisRoom
                );

                bool childrenFit = layoutWithChildren != null;
                if (!childrenFit)
                    return null;

                return layoutWithChildren;
            }

            DungeonLayout TryAddRoomChild(
                DungeonTreeRoom parentRoom,
                Vector3i pos,
                int childIndex,
                DungeonLayout prevLayout
            )
            {
                if (childIndex >= parentRoom.ChildDoors.Count)
                    return prevLayout;

                var childRoom = parentRoom.ChildDoors[childIndex].Destination;

                var parentLayoutRoom = prevLayout.GetLayoutRoom(parentRoom);
                var dirsToTry = CardinalDirectionUtils.All()
                    .Where(dir => !parentLayoutRoom.HasDoorAtDirection(dir));

                // Try all the directions in a random order
                // If there's no room on the 0th floor, keep trying other floors
                // until one of them has room
                var rng = new Random(childRoom.RoomSeed);
                var shuffledDirs = rng.Shuffle(dirsToTry);
                for (int floor = 0; floor < int.MaxValue; floor++)
                {
                    foreach (CardinalDirection childDir in shuffledDirs)
                    {
                        var childPos = pos + childDir.ToVector3i();
                        childPos.z = floor;

                        var layoutWithThisChild = TryAddRoom(childRoom, childPos, prevLayout);

                        bool thisChildFits = layoutWithThisChild != null;
                        if (!thisChildFits)
                            continue;

                        // This child and its descendants fit in this direction,
                        // but we're not done.  Does the same apply to all its
                        // _siblings_?
                        var layoutWithSiblings = TryAddRoomChild(
                            parentRoom,
                            pos,
                            childIndex + 1,
                            layoutWithThisChild
                        );

                        bool siblingsFit = layoutWithSiblings != null;
                        if (!siblingsFit)
                        {
                            continue;
                        }

                        // Well I'll be!  This direction worked!
                        return layoutWithSiblings;
                    }
                }

                // We found no directions that could accommodate both this child
                // _and_ its siblings, so go back to the previous child and have
                // him choose a different direction
                return null;
            }
        }

        private static bool IsAdjacentToAllPlacedShortcuts(
            DungeonLayout layout,
            DungeonTreeRoom room
        )
        {
            var coords = layout.CoordsOf(room);

            var shortcuts = room
                .OutgoingShortcuts
                .Concat(room.IncomingShortcuts);

            return shortcuts
                .Where(shortcut => layout.IsPlaced(shortcut))
                .All(shortcut => IsAdjacentIgnoringZ(coords, layout.CoordsOf(shortcut)));
        }

        private static bool IsAdjacentIgnoringZ(Vector3i a, Vector3i b)
        {
            var aFlat = a.FlattenToVector2i();
            var bFlat = b.FlattenToVector2i();

            return aFlat.IsAdjacentTo(bFlat);
        }
    }
}
