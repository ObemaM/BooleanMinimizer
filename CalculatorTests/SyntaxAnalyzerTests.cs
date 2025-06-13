using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using BooleanMinimizerLibrary;

namespace CalculatorTests
{
    [TestClass]
    public class SyntaxAnalyzerTests
    {
        private SyntaxAnalyzer analyzer;

        [TestInitialize]
        public void SetUp()
        {
            analyzer = new SyntaxAnalyzer();
        }

        [TestMethod]
        public void Parse_SingleVariable_ReturnsVariableNode()
        {
            var node = analyzer.Parse("x");

            Assert.AreEqual(NodeType.Variable, node.Type);
            Assert.AreEqual("x", node.Value);
        }

        [TestMethod]
        public void Parse_ConstantOne_ReturnsConstantNode()
        {
            var node = analyzer.Parse("1");

            Assert.AreEqual(NodeType.Constant, node.Type);
            Assert.AreEqual("1", node.Value);
        }

        [TestMethod]
        public void Parse_NotOperator_ReturnsNotNode()
        {
            var node = analyzer.Parse("¬x");

            Assert.AreEqual(NodeType.Not, node.Type);
            Assert.AreEqual(NodeType.Variable, node.Right.Type);
            Assert.AreEqual("x", node.Right.Value);
        }

        [TestMethod]
        public void Parse_AndExpression_ReturnsAndNode()
        {
            var node = analyzer.Parse("x∧y");

            Assert.AreEqual(NodeType.And, node.Type);
            Assert.AreEqual("x", node.Left.Value);
            Assert.AreEqual("y", node.Right.Value);
        }

        [TestMethod]
        public void Parse_ExpressionWithParentheses_ReturnsCorrectTree()
        {
            var node = analyzer.Parse("(x∨y)∧z");

            Assert.AreEqual(NodeType.And, node.Type);
            Assert.AreEqual(NodeType.Or, node.Left.Type);
            Assert.AreEqual("z", node.Right.Value);
        }

        [TestMethod]
        public void Parse_ImpliesExpression_ReturnsImpliesNode()
        {
            var node = analyzer.Parse("x→y");

            Assert.AreEqual(NodeType.Implies, node.Type);
            Assert.AreEqual("x", node.Left.Value);
            Assert.AreEqual("y", node.Right.Value);
        }

        [TestMethod]
        public void Parse_EquivalentExpression_ReturnsEquivalentNode()
        {
            var node = analyzer.Parse("x↔y");

            Assert.AreEqual(NodeType.Equivalent, node.Type);
            Assert.AreEqual("x", node.Left.Value);
            Assert.AreEqual("y", node.Right.Value);
        }

        [TestMethod]
        public void Parse_ValidVector_ReturnsVectorNode()
        {
            var node = analyzer.Parse("0101");

            Assert.AreEqual(NodeType.Vector, node.Type);
            Assert.AreEqual("0101", node.Value);
            CollectionAssert.AreEqual(new List<string> { "w", "x" }, node.Variables);
        }

        [TestMethod]
        public void Parse_VectorInsideExpression_ThrowsException()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => analyzer.Parse("x∧0101"));
            StringAssert.Contains(ex.Message, "Вектор не может находиться внутри выражения");
        }

        [TestMethod]
        public void Parse_EmptyInput_ThrowsException()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => analyzer.Parse(""));
            StringAssert.Contains(ex.Message, "Ввод не может быть пустым");
        }

        [TestMethod]
        public void Parse_InvalidSymbol_ThrowsException()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => analyzer.Parse("x$y"));
            StringAssert.Contains(ex.Message, "Неожиданный символ");
        }

        [TestMethod]
        public void Parse_MissingClosingParenthesis_ThrowsException()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => analyzer.Parse("(x∨y"));
            StringAssert.Contains(ex.Message, "Ожидалось ')'");
        }

        [TestMethod]
        public void Parse_VectorLengthNotPowerOfTwo_ThrowsException()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() => analyzer.Parse("011"));
            StringAssert.Contains(ex.Message, "Вектор должен быть длины степени два");
        }
    }
}
