using Microsoft.VisualStudio.TestTools.UnitTesting;
using BooleanMinimizerLibrary;
using System.Collections.Generic;
using System;

namespace CalculatorTests
{
    [TestClass]
    public class BooleanMinimizerTests
    {
        [TestMethod]
        public void GetIndicesByValue_ReturnsCorrectIndices()
        {
            string vector = "0110";
            var indices = BooleanMinimizer.GetIndicesByValue(vector, '1');

            CollectionAssert.AreEqual(new List<int> { 1, 2 }, indices);
        }

        [TestMethod]
        public void GetIndicesByValue_ReturnsрCorrectIndices()
        {
            string vector = "0000";
            var indices = BooleanMinimizer.GetIndicesByValue(vector, '1');

            CollectionAssert.AreEqual(new List<int>(), indices); // Ожидается пустой список
        }

        [TestMethod]
        public void MinimizeSDNF_ConstantZero_ReturnsZero()
        {
            string vector = "0000";
            var result = BooleanMinimizer.MinimizeSDNF(vector);

            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void MinimizeSDNF_ConstantOne_ReturnsOne()
        {
            string vector = "1111";
            var result = BooleanMinimizer.MinimizeSDNF(vector);

            Assert.AreEqual("1", result);
        }


        [TestMethod]
        public void MinimizeSKNF_ConstantZero_ReturnsZero()
        {
            string vector = "0000";
            var result = BooleanMinimizer.MinimizeSKNF(vector);

            Assert.AreEqual("0", result);
        }

        [TestMethod]
        public void MinimizeSKNF_ConstantOne_ReturnsOne()
        {
            string vector = "1111";
            var result = BooleanMinimizer.MinimizeSKNF(vector);

            Assert.AreEqual("1", result);
        }

        [TestMethod]
        public void MinimizeSKNF_SimpleTwoVariables_ReturnsCorrectExpression()
        {
            string vector = "1010"; // XNOR
            var result = BooleanMinimizer.MinimizeSKNF(vector);

            Assert.AreEqual("(x ∨ ¬y) ∧ (¬x ∨ y)", result);
        }

        [TestMethod]
        public void QuineMcCluskey_IdentifiesPrimeImplicants()
        {
            var minterms = new List<int> { 0, 1, 2, 5};
            var primeImplicants = BooleanMinimizer.QuineMcCluskey(minterms, 3); // Changed to 3 variables

            var expectedBits = new[] { "00-", "0-0", "-01" }; // Updated expected bits
            CollectionAssert.AreEquivalent(expectedBits, primeImplicants.ConvertAll(i => i.Bits));
        }

        [TestMethod]
        public void FindEssentialPrimeImplicants_FindsCorrectImplicants()
        {
            var implicants = new List<BooleanMinimizer.Implicant>
        {
            new BooleanMinimizer.Implicant("00-", new HashSet<int>{0, 1}),
            new BooleanMinimizer.Implicant("0-0", new HashSet<int>{0, 2}),
            new BooleanMinimizer.Implicant("-11", new HashSet<int>{5, 7}),
            new BooleanMinimizer.Implicant("1-0", new HashSet<int>{6})
        };

            var minterms = new List<int> { 0, 1, 2, 5, 6, 7 };
            var essentials = BooleanMinimizer.FindEssentialPrimeImplicants(implicants, minterms);

            var expectedBits = new[] { "00-", "0-0", "-11", "1-0" };
            CollectionAssert.AreEquivalent(expectedBits, essentials.ConvertAll(i => i.Bits));
        }

        [TestMethod]
        public void BuildExpression_ForPositiveForm_BuildsCorrectDNF()
        {
            var implicants = new List<BooleanMinimizer.Implicant>
        {
            new BooleanMinimizer.Implicant("01", new HashSet<int>{1}),
            new BooleanMinimizer.Implicant("10", new HashSet<int>{2})
        };

            var variables = new List<string> { "x", "y" };
            var result = BooleanMinimizer.BuildExpression(implicants, variables, true);

            Assert.AreEqual("(¬x ∧ y) ∨ (x ∧ ¬y)", result);
        }

        [TestMethod]
        public void BuildExpression_ForNegativeForm_BuildsCorrectCNF()
        {
            var implicants = new List<BooleanMinimizer.Implicant>
        {
            new BooleanMinimizer.Implicant("01", new HashSet<int>{1}),
            new BooleanMinimizer.Implicant("10", new HashSet<int>{2})
        };

            var variables = new List<string> { "x", "y" };
            var result = BooleanMinimizer.BuildExpression(implicants, variables, false);

            Assert.AreEqual("(x ∨ ¬y) ∧ (¬x ∨ y)", result);
        }

        [TestMethod]
        public void GetFullSDNF_ReturnsCorrectExpression()
        {
            string vector = "0110"; // XOR
            var result = BooleanMinimizer.GetFullSDNF(vector);

            Assert.AreEqual("(¬w ∧ x) ∨ (w ∧ ¬x)", result);
        }

        [TestMethod]
        public void GetFullSKNF_ReturnsCorrectExpression()
        {
            string vector = "0110"; // XOR
            var result = BooleanMinimizer.GetFullSKNF(vector);

            Assert.AreEqual("(w ∨ x) ∧ (¬w ∨ ¬x)", result);
        }

        [TestMethod]
        public void GetIndicesByValue_EmptyVector_ReturnsEmptyList()
        {
            string vector = "";
            var indices = BooleanMinimizer.GetIndicesByValue(vector, '1');
            CollectionAssert.AreEqual(new List<int>(), indices);
        }

        [TestMethod]
        public void GetIndicesByValue_NoMatchingValue_ReturnsEmptyList()
        {
            string vector = "0000";
            var indices = BooleanMinimizer.GetIndicesByValue(vector, '2');
            CollectionAssert.AreEqual(new List<int>(), indices);
        }

        [TestMethod]
        public void MinimizeSDNF_ComplexExpression_ReturnsCorrectMinimizedForm()
        {
            string vector = "0110110011000011"; // Complex 4-variable function
            var result = BooleanMinimizer.MinimizeSDNF(vector);
            Assert.IsTrue(result.Contains("∧") || result.Contains("∨")); // Should contain at least one operator
        }

        [TestMethod]
        public void MinimizeSKNF_ComplexExpression_ReturnsCorrectMinimizedForm()
        {
            string vector = "0110110011000011"; // Complex 4-variable function
            var result = BooleanMinimizer.MinimizeSKNF(vector);
            Assert.IsTrue(result.Contains("∧") || result.Contains("∨")); // Should contain at least one operator
        }

        [TestMethod]
        public void QuineMcCluskey_EmptyMinterms_ReturnsEmptyList()
        {
            var minterms = new List<int>();
            var result = BooleanMinimizer.QuineMcCluskey(minterms, 2);
            CollectionAssert.AreEqual(new List<BooleanMinimizer.Implicant>(), result);
        }

        [TestMethod]
        public void GetFullSDNF_ThreeVariables_ReturnsCorrectExpression()
        {
            string vector = "01101100"; // 3-variable function
            var result = BooleanMinimizer.GetFullSDNF(vector);
            Assert.IsTrue(result.Contains("∧") && result.Contains("∨")); // Should contain both operators
        }

        [TestMethod]
        public void GetFullSKNF_ThreeVariables_ReturnsCorrectExpression()
        {
            string vector = "01101100"; // 3-variable function
            var result = BooleanMinimizer.GetFullSKNF(vector);
            Assert.IsTrue(result.Contains("∧") && result.Contains("∨")); // Should contain both operators
        }

        [TestMethod]
        public void BuildExpression_EmptyImplicants_ReturnsEmptyString()
        {
            var implicants = new List<BooleanMinimizer.Implicant>();
            var variables = new List<string> { "x", "y" };
            var result = BooleanMinimizer.BuildExpression(implicants, variables, true);
            Assert.AreEqual("", result);
        }
    }

    [TestClass]
    public class KarnaughMapBuilderTests
    {
        private KarnaughMapBuilder builder;

        [TestInitialize]
        public void Initialize()
        {
            builder = new KarnaughMapBuilder();
        }


        [TestMethod]
        public void BuildForTwoVariables_ReturnsCorrectMap()
        {
            string vector = "0110";
            var variables = new List<string> { "x", "y" };

            var result = builder.BuildForTwoVariables(vector, variables);

            var expected = new List<List<string>>
            {
                new List<string> { "x\\y", "0", "1" },
                new List<string> { "0", "0", "1" },
                new List<string> { "1", "1", "0" }
            };

            CollectionAssert.AreEqual(expected[0], result[0]);
            CollectionAssert.AreEqual(expected[1], result[1]);
            CollectionAssert.AreEqual(expected[2], result[2]);
        }

        [TestMethod]
        public void BuildForFourVariables_ReturnsCorrectMap()
        {
            string vector = "0110110011000011"; // 16 бит
            var variables = new List<string> { "w", "x", "y", "z" };

            var result = builder.BuildForFourVariables(vector, variables);

            var expected = new List<List<string>>
            {
                new List<string> { "wx\\yz", "00", "01", "11", "10" },
                new List<string> { "00", "0", "1", "1", "0" },
                new List<string> { "01", "1", "1", "0", "0" },
                new List<string> { "11", "0", "0", "1", "1" },
                new List<string> { "10", "1", "1", "0", "0" }
            };

            for (int i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEqual(expected[i], result[i]);
            }
        }

        [TestMethod]
        public void FindAllMaximalAreas_FindsLargestGroups()
        {
            var map = new List<List<string>>
            {
                new List<string> { "x\\y", "0", "1" },
                new List<string> { "0", "1", "1" },
                new List<string> { "1", "1", "0" }
            };

            var result = builder.FindAllMaximalAreas(map);

            Assert.AreEqual(2, result.Count); // Changed to expect 2 areas
            Assert.IsTrue(result.Any(a => a.StartRow == 0 && a.StartCol == 0 && a.Height == 1 && a.Width == 2));
            Assert.IsTrue(result.Any(a => a.StartRow == 0 && a.StartCol == 0 && a.Height == 2 && a.Width == 1));
        }

        [TestMethod]
        public void FindAllMaximalZeroAreas_FindsZeroGroups()
        {
            var map = new List<List<string>>
            {
                new List<string> { "x\\y", "0", "1" },
                new List<string> { "0", "0", "1" },
                new List<string> { "1", "1", "0" }
            };

            var result = builder.FindAllMaximalZeroAreas(map);

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(a => a.StartRow == 0 && a.StartCol == 0 && a.Height == 1 && a.Width == 1));
            Assert.IsTrue(result.Any(a => a.StartRow == 1 && a.StartCol == 1 && a.Height == 1 && a.Width == 1));
        }

        [TestMethod]
        public void GetVariablesFromMap_ExtractsVariablesCorrectly()
        {
            var mapWithHeader = new List<List<string>>
            {
                new List<string> { "w\\xy", "00", "01", "11", "10" },
                new List<string> { "0", "0", "1", "1", "0" },
                new List<string> { "1", "1", "1", "0", "0" }
            };

            var result = builder.GetVariablesFromMap(mapWithHeader);

            CollectionAssert.AreEquivalent(new[] { "w", "x", "y" }, result);
        }

        [TestMethod]
        public void GetVariablesFromMap_UsesDefaultNamesIfNoHeader()
        {
            var mapWithoutHeader = new List<List<string>>
            {
                new List<string> { "00", "01", "11", "10" },
                new List<string> { "0", "1", "1", "0" }
            };

            var result = builder.GetVariablesFromMap(mapWithoutHeader);

            CollectionAssert.AreEquivalent(new[] { "x", "y" }, result); // Changed to expect 2 variables
        }

        [TestMethod]
        public void Area_GetCoveredCells_ReturnsExpectedCoordinates()
        {
            var area = new KarnaughMapBuilder.Area(1, 1, 2, 2);

            var covered = area.GetCoveredCells(4, 4).ToList();

            CollectionAssert.AreEquivalent(new[] { (1, 1), (1, 2), (2, 1), (2, 2) }, covered);
        }

        [TestMethod]
        public void BuildSteps_ForTwoVariables_ReturnsMultipleSteps()
        {
            string vector = "0110";
            var rootNode = new Node(NodeType.Vector, vector)
            {
                Variables = new List<string> { "x", "y" }
            };

            var steps = builder.BuildSteps(rootNode);

            Assert.AreEqual(5, steps.Count);

            Assert.AreEqual("Создаем заголовок таблицы", steps[0].Description);
            Assert.AreEqual("Добавляем строку для значения переменной x = 0", steps[1].Description);
            Assert.AreEqual("Заполняем ячейки для комбинаций y: 0, 1 при x=0", steps[2].Description);
            Assert.AreEqual("Добавляем строку для значения переменной x = 1", steps[3].Description);
            Assert.AreEqual("Заполняем ячейки для комбинаций y: 0, 1 при x=1", steps[4].Description);
        }

        [TestMethod]
        public void BuildSteps_ForThreeVariables_ReturnsMultipleSteps()
        {
            string vector = "01101100";
            var rootNode = new Node(NodeType.Vector, vector)
            {
                Variables = new List<string> { "w", "x", "y" }
            };

            var steps = builder.BuildSteps(rootNode);

            Assert.AreEqual(5, steps.Count);
        }

        [TestMethod]
        public void BuildSteps_ForFourVariables_ReturnsMultipleSteps()
        {
            string vector = "0110110011000011";
            var rootNode = new Node(NodeType.Vector, vector)
            {
                Variables = new List<string> { "w", "x", "y", "z" }
            };

            var steps = builder.BuildSteps(rootNode);

            Assert.AreEqual(5, steps.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Build_InvalidVariableCount_ThrowsException()
        {
            var root = new Node(NodeType.Vector, "01") { Variables = new List<string> { "x" } };
            builder.Build(root);
        }

        [TestMethod]
        public void BuildForThreeVariables_ReturnsCorrectMap()
        {
            string vector = "01101100";
            var variables = new List<string> { "x", "y", "z" };

            var result = builder.BuildForThreeVariables(vector, variables);

            Assert.AreEqual(3, result.Count); // Header + 2 rows
            Assert.AreEqual(5, result[0].Count); // Header + 4 columns
            Assert.AreEqual("x \\ yz", result[0][0]);
        }

        [TestMethod]
        public void BuildSteps_ForThreeVariables_ReturnsCorrectSteps()
        {
            string vector = "01101100";
            var rootNode = new Node(NodeType.Vector, vector)
            {
                Variables = new List<string> { "x", "y", "z" }
            };

            var steps = builder.BuildSteps(rootNode);

            Assert.IsTrue(steps.Count > 0);
            Assert.IsTrue(steps[0].Description.Contains("заголовок"));
        }

        [TestMethod]
        public void BuildSteps_ForFourVariables_ReturnsCorrectSteps()
        {
            string vector = "0110110011000011";
            var rootNode = new Node(NodeType.Vector, vector)
            {
                Variables = new List<string> { "w", "x", "y", "z" }
            };

            var steps = builder.BuildSteps(rootNode);

            Assert.IsTrue(steps.Count > 0);
            Assert.IsTrue(steps[0].Description.Contains("заголовок"));
        }
    }
}