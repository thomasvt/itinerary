using System;
using System.Collections.Generic;

namespace Itinerary.Comparing
{
    public static class OrderedListComparer
    {
        public static IEnumerable<Change<T>> Compare<T>(List<T> leftList, List<T> rightList, Func<T, T, int> compareFunc)
        where T : class
        {
            var leftIndex = 0;
            var rightIndex = 0;
            while (leftIndex < leftList.Count || rightIndex < rightList.Count)
            {
                var left = leftIndex >= leftList.Count ? null : leftList[leftIndex];
                var right = rightIndex >= rightList.Count ? null : rightList[rightIndex];
                var compareResult = compareFunc.Invoke(left, right);

                if (compareResult == 0)
                {
                    yield return new Change<T>(left, right, ChangeType.Unmodified);
                    leftIndex++;
                    rightIndex++;
                }
                else if (compareResult == -1 && left != null || right == null)
                {
                    yield return new Change<T>(left, right, ChangeType.Removed);
                    leftIndex++;
                }
                else
                {
                    yield return new Change<T>(right, right, ChangeType.Added);
                    rightIndex++;
                }
            }
        }
    }
}
