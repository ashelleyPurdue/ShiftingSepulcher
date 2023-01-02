
namespace RandomDungeons
{
    public static class DungeonGraphBuilder
    {
        public static DungeonGraph BuildFromLayout(DungeonLayout layout)
        {
            var graph = new DungeonGraph(layout);

            // Add graph rooms
            foreach (var layoutRoom in layout.AllRooms())
            {
                var graphRoom = graph.CreateRoom(layoutRoom.Position, 0);
                var treeRoom = layoutRoom.TreeRoom;

                graphRoom.ChallengeType = treeRoom.ChallengeType;
                graphRoom.RoomSeed = treeRoom.RoomSeed;
                graphRoom.KeyId = treeRoom.KeyId;
            }

            // Connect adjacent rooms with doors
            foreach (var layoutRoom in layout.AllRooms())
            {
                var coords = layoutRoom.Position;
                var treeRoom = layoutRoom.TreeRoom;

                DungeonGraphRoom graphRoom = graph.GetRoom(coords);

                // Create doors for all the child rooms
                foreach (var childDoor in treeRoom.ChildDoors)
                {
                    Vector2i childCoords = layout.CoordsOf(childDoor.Destination);
                    DungeonGraphRoom childGraphRoom = graph.GetRoom(childCoords);

                    var dir = coords.AdjacentDirection(childCoords);
                    graphRoom.SetChallengeDoor(dir);
                    graphRoom.GetDoor(dir).Destination = childGraphRoom;
                    childGraphRoom.GetDoor(dir.Opposite()).Destination = graphRoom;

                    // Lock the door if it says it's supposed to be
                    if (childDoor is LockedDoor lockedDoor)
                        graphRoom.LockDoor(dir, lockedDoor.KeyId);
                }

                // Create doors for all the shortcuts
                foreach (var shortcutDestination in treeRoom.OutgoingShortcuts)
                {
                    var destCoords = layout.CoordsOf(shortcutDestination);
                    var dir = coords.AdjacentDirection(destCoords);
                    graphRoom.DrillOneWayDoor(dir);
                }
            }

            return graph;
        }
    }
}
