using System;
using System.Collections.Generic;
using System.Linq;
using Itinerary.Comparing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnorderedListComparer_Test
    {
        [TestMethod]
        public void TestMethod1()
        {
            var left = new List<string> {"A", "C", "G", "F", "B"};
            var right = new List<string> {"A", "B", "C", "D", "G", "H"};
            var changes = UnorderedListComparer.Compare(left, right, string.Equals).ToList();

            Assert.AreEqual(7, changes.Count);

            Check(changes, 0, ChangeType.Unmodified, "A", "A");
            Check(changes, 1, ChangeType.Unmodified, "C", "C");
            Check(changes, 2, ChangeType.Unmodified, "B", "B");
            Check(changes, 3, ChangeType.Unmodified, "G", "G");
            Check(changes, 4, ChangeType.Added, null, "D");
            Check(changes, 5, ChangeType.Removed, "F", null);
            Check(changes, 6, ChangeType.Added, null, "H");
        }

        private static void Check(List<Change<string>> changes, int index, ChangeType changeType, string left, string right)
        {
            Assert.AreEqual(changeType, changes[index].ChangeType);
            Assert.AreEqual(left, changes[index].LeftItem);
            Assert.AreEqual(right, changes[index].RightItem);
        }
    }
}
