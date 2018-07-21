using System;
using System.Collections.Generic;
using System.Linq;
using Itinerary.DiffTreeNodeExpanders.CSharp;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class CSharpFileExtender_Test
    {
        [TestMethod]
        public void TestMethod1()
        {
            var results = CSharpFileExpander.BuildCSharpTree(new List<CodeNode>
                {
                    new CodeNode("ExpressionStatement", SyntaxKind.ExpressionStatement, "Console.Read();")
                },
                new List<CodeNode>()).ToList();
        }
    }
}
