using System;
using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    public interface IDungeonRoom
    {
        Node2D Node {get;}

        event Action<CardinalDirection> DoorUsed;

        DungeonLayoutRoom LayoutRoom {get;}

        Node2D GetDoorSpawn(CardinalDirection dir);

        void Populate(DungeonLayoutRoom treeRoom);

        void ConnectDoors(
            Dictionary<DungeonTreeRoom, Room2D> treeRoomToRealRoom,
            ShortcutDoorMap shortcutDoorMap
        );
    }

    public class ShortcutDoorMap
    {
        public readonly Dictionary<IncomingShortcutDoor, OneWayDoorClosedSide> IncomingFakeToReal
            = new Dictionary<IncomingShortcutDoor, OneWayDoorClosedSide>();

        public readonly Dictionary<OutgoingShortcutDoor, OneWayDoorOpenSide> OutgoingFakeToReal
            = new Dictionary<OutgoingShortcutDoor, OneWayDoorOpenSide>();
    }
}
