using System;
using System.Collections.Generic;
using Godot;

namespace ShiftingSepulcher
{
    /// <summary>
    /// Allows more than one object to attempt to pause the scene tree at the
    /// same time, without stepping on each others' toes.
    ///
    /// An object pauses the tree by calling
    /// <see cref="ShiftingSepulcher.PauseManager.RequestPause"/>, which adds it
    /// to the "pause list".
    ///
    /// An object attempts to unpause the tree by calling
    /// <see cref="ShiftingSepulcher.PauseManager.AllowUnpause"/>, which removes
    /// it from the "pause list".
    ///
    /// If there is at least one object in the "pause list", then the scene
    /// tree is paused.  It will become unpaused again when the "pause list" is
    /// empty.
    ///
    /// If an object is deleted before calling
    /// <see cref="ShiftingSepulcher.PauseManager.AllowUnpause"/>, then it will
    /// be automatically deleted from the pause list.  This is to prevent the
    /// game from becoming permanently paused if the player, for example, returns
    /// to the title screen during a room transition.
    /// </summary>
    public class PauseManager : Node
    {
        private static HashSet<Godot.Object> _pausers = new HashSet<Godot.Object>();
        private static HashSet<Godot.Object> _pausersSwap = new HashSet<Godot.Object>();

        public static void RequestPause(Godot.Object pauser)
        {
            _pausers.Add(pauser);
        }

        public static void AllowUnpause(Godot.Object pauser)
        {
            _pausers.Remove(pauser);
        }

        public override void _Process(float delta)
        {
            CleanUpDeletedPausers();
            GetTree().Paused = _pausers.Count > 0;
        }

        private static void CleanUpDeletedPausers()
        {
            // Swapping between two "double-buffered" sets, rather than creating
            // a temporary list or using LINQ, allows us to delete things from
            // the set every frame without creating work for the garbage
            // collector.
            _pausersSwap.Clear();

            foreach (var pauser in _pausers)
            {
                if (IsInstanceValid(pauser) && !pauser.IsQueuedForDeletion())
                    _pausersSwap.Add(pauser);
            }

            var holder = _pausers;
            _pausers = _pausersSwap;
            _pausersSwap = holder;
        }
    }
}
