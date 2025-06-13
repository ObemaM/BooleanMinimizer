using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooleanMinimizerLibrary;

namespace BooleanMinimizerTests
{
    [TestClass]
    public class FunctionVectorBuilderTests
    {
        private FunctionVectorBuilder builder;

        [TestInitialize]
        public void Setup()
        {
            builder = new FunctionVectorBuilder();
        }

        [TestMethod]
        public void BuildVector_AndExpression_ReturnsCorrectVector()
        {
            // x ∧ y
            var node = new Node(NodeType.And,
                left: new Node(NodeType.Variable, "x"),
                right: new Node(NodeType.Variable, "y"));

            var vector = builder.BuildVector(node);

            Assert.AreEqual("0001", vector); // x ∧ y
        }

        [TestMethod]
        public void BuildVector_ConstantOne_ReturnsSingleOne()
        {
            var node = new Node(NodeType.Constant, "1");

            var vector = builder.BuildVector(node);

            Assert.AreEqual("1", vector);
        }

        [TestMethod]
        public void BuildVector_VectorWithoutVariables_InferVariables()
        {
            var node = new Node(NodeType.Vector, "1010");

            var vector = builder.BuildVector(node);

            Assert.AreEqual("1010", vector);
            CollectionAssert.AreEqual(new List<string> { "x", "y" }, node.Variables);
        }

        [TestMethod]
        public void BuildTruthTable_OrExpression_IsCorrect()
        {
            // x ∨ y
            var node = new Node(NodeType.Or,
                left: new Node(NodeType.Variable, "x"),
                right: new Node(NodeType.Variable, "y"));

            var table = builder.BuildTruthTable(node);

            Assert.AreEqual(4, table.Count);
            Assert.IsFalse(table[0]["F"]); // 00
            Assert.IsTrue(table[1]["F"]);  // 01
            Assert.IsTrue(table[2]["F"]);  // 10
            Assert.IsTrue(table[3]["F"]);  // 11
        }

        [TestMethod]
        public void BuildTruthTable_VectorNode_CorrectMapping()
        {
            var node = new Node(NodeType.Vector, "1001")
            {
                Variables = new List<string> { "x", "y" } // x - старший бит
            };

            var table = builder.BuildTruthTable(node);

            Assert.AreEqual(4, table.Count);
            Assert.AreEqual(true, table[0]["F"]);  // 00 -> '1'
            Assert.AreEqual(false, table[1]["F"]); // 01 -> '0'
            Assert.AreEqual(false, table[2]["F"]); // 10 -> '0'
            Assert.AreEqual(true, table[3]["F"]);  // 11 -> '1'
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Переменные для вектора не определены")]
        public void EvaluateVector_WithoutVariableList_Throws()
        {
            var node = new Node(NodeType.Vector, "1010");
            // Не указаны переменные
            builder.BuildTruthTable(node); // вызовет EvaluateVector => исключение
        }
    }
}
