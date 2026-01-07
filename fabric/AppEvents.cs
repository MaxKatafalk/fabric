namespace fabric
{
    public static class AppEvents
    {
        public static event System.Action OrderStatusChanged;
        public static void RaiseOrderStatusChanged() => OrderStatusChanged?.Invoke();
    }
}
