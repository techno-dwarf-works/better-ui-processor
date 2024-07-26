namespace Better.UIProcessor.Runtime
{
    public static class FragmentPriority
    {
        public const int Critical = 1000;
        public const int Default = 0;
        public const int Fallback = -1000;

        public static int CriticalWithOffset(int offset) => ValueWithOffset(Critical, offset);
        public static int DefaultWithOffset(int offset) => ValueWithOffset(Default, offset);
        public static int FallbackWithOffset(int offset) => ValueWithOffset(Fallback, offset);
        private static int ValueWithOffset(int value, int offset) => value + offset;
    }
}