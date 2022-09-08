using System.Collections.Generic;

namespace RandomDungeons.Services
{
    // Yes, I know global state is bad.
    // No, I don't care.  It's a prototype.
    public static class PlayerInventory
    {
        // Why use a nested class here?  To make it easier to reset, of course!
        // This way, wiping the state clean is as simple as replacing _content
        // with a new instance.  This reduces (but does not eliminate) the risk
        // that comes with global state.
        private static Content _content = new Content();
        private class Content
        {
            public HashSet<int> HeldKeys = new HashSet<int>();
            public int Health = 3;
        }

        public static int Health
        {
            get => _content.Health;
            set => _content.Health = value;
        }

        public static void Reset() => _content = new Content();

        public static bool HasKey(int keyId) => _content.HeldKeys.Contains(keyId);
        public static void CollectKey(int keyId) => _content.HeldKeys.Add(keyId);
        public static void SpendKey(int keyId) => _content.HeldKeys.Remove(keyId);
    }
}
