using System;
using System.Collections.Generic;
using System.Linq;

namespace Itinerary.Comparing
{
    public static class UnorderedListComparer
    {
        // not proud of this, but it's ok for now...

        public static IEnumerable<Change<T>> Compare<T>(List<T> leftList, List<T> rightList, Func<T, T, bool> areEqualFunc)
        where T : class
        {
            while (leftList.Count > 0 || rightList.Count > 0)
            {
                var left = leftList.FirstOrDefault();
                var right = rightList.FirstOrDefault();
                var areEqual = areEqualFunc.Invoke(left, right);

                if (areEqual)
                {
                    yield return new Change<T>(left, right, ChangeType.Unmodified);
                    leftList.Remove(left);
                    rightList.Remove(right);
                }
                else
                {
                    if (left != null)
                    {
                        leftList.Remove(left);
                        var foundLeftInRightList = false;
                        foreach (var rightSub in rightList.Skip(1).ToList())
                        {
                            if (areEqualFunc.Invoke(left, rightSub))
                            {
                                yield return new Change<T>(left, rightSub, ChangeType.Unmodified);
                                rightList.Remove(rightSub);
                                foundLeftInRightList = true;
                                break;
                            }
                        }
                        if (!foundLeftInRightList)
                        {
                            yield return new Change<T>(left, null, ChangeType.Removed);
                        }
                    }

                    if (right != null)
                    {
                        rightList.Remove(right);
                        var foundRightInLeftList = false;
                        foreach (var leftSub in leftList.ToList()) // no skip 1!! we did leftList.Remove(left) earlier!!
                        {
                            if (areEqualFunc.Invoke(leftSub, right))
                            {
                                yield return new Change<T>(leftSub, right, ChangeType.Unmodified);
                                leftList.Remove(leftSub);
                                foundRightInLeftList = true;
                                break;
                            }
                        }
                        if (!foundRightInLeftList)
                        {
                            yield return new Change<T>(null, right, ChangeType.Added);
                        }
                    }
                }
            }
        }
    }
}
