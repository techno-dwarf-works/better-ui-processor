namespace Better.UIProcessor.Runtime
{
    public static class ModulePriority
    {
        // TODO: Update priorities

        public const int Resolver = 5000;
        public const int Default = 0;
        public const int Fallback = -5000;

        public static int ResolverWithOffset(int offset) => ValueWithOffset(Resolver, offset);
        public static int DefaultWithOffset(int offset) => ValueWithOffset(Default, offset);
        public static int FallbackWithOffset(int offset) => ValueWithOffset(Fallback, offset);
        private static int ValueWithOffset(int value, int offset) => value + offset;
    }
}