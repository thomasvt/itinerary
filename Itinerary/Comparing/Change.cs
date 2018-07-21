namespace Itinerary.Comparing
{
    public struct Change<T>
    where T : class
    {
        public Change(T item, ChangeType changeType)
        {
            Item = item;
            ChangeType = changeType;
        }

        public T Item { get; }
        public ChangeType ChangeType { get; }
    }
}