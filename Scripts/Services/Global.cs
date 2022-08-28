using System.Collections.Generic;

namespace RandomDungeons.Services
{
    // Yes, I know global state is bad.
    // No, I don't care.  It's a prototype.
    public static class PlayerInventory
    {
        private static HashSet<int> _heldKeys = new HashSet<int>();

        public static bool HasKey(int keyId) => _heldKeys.Contains(keyId);
        public static void CollectKey(int keyId) => _heldKeys.Add(keyId);
        public static void SpendKey(int keyId) => _heldKeys.Remove(keyId);
    }
}
