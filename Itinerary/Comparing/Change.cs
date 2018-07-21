namespace Itinerary.Comparing
{
    public struct Change<T>
    where T : class
    {
        public Change(T leftItem, T rightItem, ChangeType changeType)
        {
            LeftItem = leftItem;
            RightItem = rightItem;
            ChangeType = changeType;
        }

        public T LeftItem { get; }
        public T RightItem { get; }
        public T Item => ChangeType == ChangeType.Added ? RightItem : LeftItem;
        public ChangeType ChangeType { get; }
    }
}