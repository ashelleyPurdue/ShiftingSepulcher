using System.Collections.Generic;

namespace RandomDungeons
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
            public int MaxHealth = 6;
            public int Health;

            public HashSet<int> HeldKeys = new HashSet<int>();
            public int Gold;

            public float ClearTime;
        }

        public static int MaxHealth
        {
            get => _content.MaxHealth;
            set => _content.MaxHealth = value;
        }

        public static int Health
        {
            get => _content.Health;
            set => _content.Health = value;
        }

        public static int Gold
        {
            get => _content.Gold;
            set => _content.Gold = value;
        }

        public static float ClearTime
        {
            get => _content.ClearTime;
            set => _content.ClearTime = value;
        }

        public static void Reset()
        {
            _content = new Content();
            FullHeal();
        }

        public static void FullHeal()
        {
            Health = MaxHealth;
        }

        public static bool HasKey(int keyId) => _content.HeldKeys.Contains(keyId);
        public static void CollectKey(int keyId) => _content.HeldKeys.Add(keyId);
        public static void SpendKey(int keyId) => _content.HeldKeys.Remove(keyId);
        public static IEnumerable<int> HeldKeys => _content.HeldKeys;
    }
}
