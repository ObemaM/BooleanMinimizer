using System;
using System.Collections.Generic;
using System.Reflection;

namespace BooleanMinimizerLibrary
{
    public class KarnaughMapBuilder
    {
        public List<KarnaughStep> BuildSteps(Node root, List<string> variables = null)
        {
            if (variables == null)
            {
                if (root.Type == NodeType.Vector && root.Variables != null)
                    variables = root.Variables;
                else
                    variables = GetVariables(root).OrderBy(v => v).ToList();
            }

            int varCount = variables.Count;
            if (varCount < 2 || varCount > 4)
                throw new Exception("Карта Карно поддерживает от 2 до 4 переменных");

            string vector = new FunctionVectorBuilder().BuildVector(root);
            var steps = new List<KarnaughStep>();

            // Строим карту Карно пошагово
            if (varCount == 2)
            {
                steps.AddRange(BuildForTwoVariablesSteps(vector, variables));
            }
            else if (varCount == 3)
            {
                steps.AddRange(BuildForThreeVariablesSteps(vector, variables));
            }
            else // 4 variables
            {
                steps.AddRange(BuildForFourVariablesSteps(vector, variables));
            }

            return steps;
        }

        public List<KarnaughStep> BuildForTwoVariablesSteps(string vector, List<string> variables)
        {
            var steps = new List<KarnaughStep>();

            // Шаг 1: Создаем заголовок таблицы
            var header = new List<List<string>>
            {
                new List<string> { variables[0] + "\\" + variables[1], "0", "1" }
            };
            steps.Add(new KarnaughStep(
                "Создаем заголовок таблицы",
                header
            ));

            // Шаг 2: Добавляем строку для 0
            var step2 = new List<List<string>>(header);
            step2.Add(new List<string> { "0", "-", "-" });
            steps.Add(new KarnaughStep(
                "Добавляем строку для значения переменной " + variables[0] + " = 0",
                step2
            ));

            // Шаг 3: Заполняем первую строку
            var step3 = new List<List<string>>(step2);
            step3[1] = new List<string> { "0", vector[0].ToString(), vector[1].ToString() };
            steps.Add(new KarnaughStep(
                "Заполняем ячейки для комбинаций " + variables[1] + ": 0, 1 при " + variables[0] + "=0",
                step3
            ));

            // Шаг 4: Добавляем строку для 1
            var step4 = new List<List<string>>(step3);
            step4.Add(new List<string> { "1", "-", "-" });
            steps.Add(new KarnaughStep(
                "Добавляем строку для значения переменной " + variables[0] + " = 1",
                step4
            ));

            // Шаг 5: Заполняем вторую строку
            var step5 = new List<List<string>>(step4);
            step5[2] = new List<string> { "1", vector[2].ToString(), vector[3].ToString() };
            steps.Add(new KarnaughStep(
                "Заполняем ячейки для комбинаций " + variables[1] + ": 0, 1 при " + variables[0] + "=1",
                step5
            ));

            return steps;
        }

        public List<KarnaughStep> BuildForThreeVariablesSteps(string vector, List<string> variables)
        {
            var steps = new List<KarnaughStep>();

            // Шаг 1: Создаем заголовок таблицы
            var header = new List<List<string>>
            {
                new List<string> { $"{variables[0]} \\ {variables[1]}{variables[2]}", "00", "01", "11", "10" }
            };
            steps.Add(new KarnaughStep(
                "Создаем заголовок таблицы",
                header
            ));

            // Шаг 2: Добавляем строку для A=0
            var step2 = new List<List<string>>(header);
            step2.Add(new List<string> { "0", "-", "-", "-", "-" });
            steps.Add(new KarnaughStep(
                "Добавляем строку для значения переменной " + variables[0] + " = 0",
                step2
            ));

            // Шаг 3: Заполняем первую строку
            var step3 = new List<List<string>>(step2);
            step3[1] = new List<string> { "0", vector[0].ToString(), vector[1].ToString(), vector[3].ToString(), vector[2].ToString() };
            steps.Add(new KarnaughStep(
                "Заполняем ячейки для комбинаций " + variables[1] + variables[2] + ": 00, 01, 11, 10 при " + variables[0] + "=0",
                step3
            ));

            // Шаг 4: Добавляем строку для A=1
            var step4 = new List<List<string>>(step3);
            step4.Add(new List<string> { "1", "-", "-", "-", "-" });
            steps.Add(new KarnaughStep(
                "Добавляем строку для значения переменной " + variables[0] + " = 1",
                step4
            ));

            // Шаг 5: Заполняем вторую строку
            var step5 = new List<List<string>>(step4);
            step5[2] = new List<string> { "1", vector[4].ToString(), vector[5].ToString(), vector[7].ToString(), vector[6].ToString() };
            steps.Add(new KarnaughStep(
                "Заполняем ячейки для комбинаций " + variables[1] + variables[2] + ": 00, 01, 11, 10 при " + variables[0] + "=1",
                step5
            ));

            return steps;
        }

        public List<KarnaughStep> BuildForFourVariablesSteps(string vector, List<string> variables)
        {
            var steps = new List<KarnaughStep>();

            // Шаг 1: Создаем заголовок таблицы
            var header = new List<List<string>>
            {
                new List<string> {
                    variables[0] + variables[1] + "\\" + variables[2] + variables[3],
                    "00", "01", "11", "10"
                }
            };
            steps.Add(new KarnaughStep(
                "Создаем заголовок таблицы",
                header
            ));

            // Шаг 2: Добавляем строки для AB=00
            var step2 = new List<List<string>>(header);
            step2.Add(new List<string> { "00", "-", "-", "-", "-" });
            step2.Add(new List<string> { "01", "-", "-", "-", "-" });
            steps.Add(new KarnaughStep(
                "Добавляем строки для комбинаций " + variables[0] + variables[1] + ": 00, 01",
                step2
            ));

            // Шаг 3: Заполняем строки для AB=00
            var step3 = new List<List<string>>(step2);
            step3[1] = new List<string> { "00", vector[0].ToString(), vector[1].ToString(), vector[3].ToString(), vector[2].ToString() };
            step3[2] = new List<string> { "01", vector[4].ToString(), vector[5].ToString(), vector[7].ToString(), vector[6].ToString() };
            steps.Add(new KarnaughStep(
                "Заполняем ячейки для комбинаций CD: 00, 01, 11, 10 при AB=00",
                step3
            ));

            // Шаг 4: Добавляем строки для AB=11
            var step4 = new List<List<string>>(step3);
            step4.Add(new List<string> { "11", "-", "-", "-", "-" });
            step4.Add(new List<string> { "10", "-", "-", "-", "-" });
            steps.Add(new KarnaughStep(
                "Добавляем строки для комбинаций " + variables[0] + variables[1] + ": 11, 10",
                step4
            ));

            // Шаг 5: Заполняем строки для AB=11
            var step5 = new List<List<string>>(step4);
            step5[3] = new List<string> { "11", vector[12].ToString(), vector[13].ToString(), vector[15].ToString(), vector[14].ToString() };
            step5[4] = new List<string> { "10", vector[8].ToString(), vector[9].ToString(), vector[11].ToString(), vector[10].ToString() };
            steps.Add(new KarnaughStep(
                "Заполняем ячейки для комбинаций CD: 00, 01, 11, 10 при AB=11",
                step5
            ));

            return steps;
        }

        public HashSet<string> GetVariables(Node node)
        {
            var variables = new HashSet<string>();
            if (node == null) return variables;
            if (node.Type == NodeType.Variable)
            {
                variables.Add(node.Value);
            }
            else
            {
                variables.UnionWith(GetVariables(node.Left));
                variables.UnionWith(GetVariables(node.Right));
            }
            return variables;
        }

        public List<List<string>> Build(Node root, List<string> variables = null)
        {
            if (variables == null)
            {
                if (root.Type == NodeType.Vector && root.Variables != null)
                    variables = root.Variables;
                else
                    variables = GetVariables(root).OrderBy(v => v).ToList();
            }
            int varCount = variables.Count;
            if (varCount < 2 || varCount > 4)
                throw new Exception("Карта Карно поддерживает от 2 до 4 переменных");
            string vector = new FunctionVectorBuilder().BuildVector(root);
            if (varCount == 2)
                return BuildForTwoVariables(vector, variables);
            else if (varCount == 3)
                return BuildForThreeVariables(vector, variables);
            else // 4 variables
                return BuildForFourVariables(vector, variables);
        }

        public List<List<string>> BuildForTwoVariables(string vector, List<string> variables)
        {
            return new List<List<string>>
            {
                new List<string> { variables[0] + "\\" + variables[1], "0", "1" },
                new List<string> { "0", vector[0].ToString(), vector[1].ToString() },
                new List<string> { "1", vector[2].ToString(), vector[3].ToString() }
            };
        }

        public List<List<string>> BuildForThreeVariables(string vector, List<string> variables)
        {
            return new List<List<string>>
            {
                new List<string> { $"{variables[0]} \\ {variables[1]}{variables[2]}", "00", "01", "11", "10" },
                new List<string>
                    { "0", vector[0].ToString(), vector[1].ToString(), vector[3].ToString(), vector[2].ToString() },
                new List<string>
                    { "1", vector[4].ToString(), vector[5].ToString(), vector[7].ToString(), vector[6].ToString() }
            };
        }

        public List<List<string>> BuildForFourVariables(string vector, List<string> variables)
        {
            return new List<List<string>>
            {
                new List<string>
                    { variables[0] + variables[1] + "\\" + variables[2] + variables[3], "00", "01", "11", "10" },
                new List<string>
                    { "00", vector[0].ToString(), vector[1].ToString(), vector[3].ToString(), vector[2].ToString() },
                new List<string>
                    { "01", vector[4].ToString(), vector[5].ToString(), vector[7].ToString(), vector[6].ToString() },
                new List<string>
                {
                    "11", vector[12].ToString(), vector[13].ToString(), vector[15].ToString(), vector[14].ToString()
                },
                new List<string>
                    { "10", vector[8].ToString(), vector[9].ToString(), vector[11].ToString(), vector[10].ToString() }
            };
        }
        public List<Area> FindAllMaximalZeroAreas(List<List<string>> map)
        {
            int rowCount = map.Count - 1;       // Без заголовков
            int colCount = map[0].Count - 1;
            var allAreas = new List<Area>();
            var zeros = new HashSet<(int, int)>();

            // Собираем все позиции с "0"
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    if (map[r + 1][c + 1] == "0")
                        zeros.Add((r, c));
                }
            }

            // Все возможные прямоугольные области
            var possibleHeights = GetPowersOfTwoUpTo(rowCount);
            var possibleWidths = GetPowersOfTwoUpTo(colCount);
            possibleHeights.Reverse();
            possibleWidths.Reverse();

            foreach (int height in possibleHeights)
            {
                foreach (int width in possibleWidths)
                {
                    for (int r = 0; r < rowCount; r++)
                    {
                        for (int c = 0; c < colCount; c++)
                        {
                            var area = new Area(r, c, height, width);
                            var covered = area.GetCoveredCells(rowCount, colCount).ToList();
                            if (covered.All(pos => zeros.Contains(pos)))
                            {
                                allAreas.Add(area);
                            }
                        }
                    }
                }
            }

            // Жадно выбираем области, покрывающие максимум новых нулей
            var result = new List<Area>();
            var coveredCells = new HashSet<(int, int)>();

            while (coveredCells.Count < zeros.Count)
            {
                Area bestArea = null;
                int maxNew = 0;

                foreach (var area in allAreas)
                {
                    var cells = area.GetCoveredCells(rowCount, colCount).ToList();
                    int newCount = cells.Count(c => !coveredCells.Contains(c));
                    if (newCount > maxNew)
                    {
                        maxNew = newCount;
                        bestArea = area;
                    }
                }

                if (bestArea == null) break;
                result.Add(bestArea);

                foreach (var cell in bestArea.GetCoveredCells(rowCount, colCount))
                    coveredCells.Add(cell);
            }

            return result;
        }
        public List<Area> FindAllMaximalAreas(List<List<string>> map)
        {
            int rowCount = map.Count - 1;       // Без заголовков
            int colCount = map[0].Count - 1;
            var allAreas = new List<Area>();
            var ones = new HashSet<(int, int)>();

            // Собираем все позиции с "1"
            for (int r = 0; r < rowCount; r++)
            {
                for (int c = 0; c < colCount; c++)
                {
                    if (map[r + 1][c + 1] == "1")
                        ones.Add((r, c));
                }
            }

            // Все возможные прямоугольные области
            var possibleHeights = GetPowersOfTwoUpTo(rowCount);
            var possibleWidths = GetPowersOfTwoUpTo(colCount);
            possibleHeights.Reverse();
            possibleWidths.Reverse();

            foreach (int height in possibleHeights)
            {
                foreach (int width in possibleWidths)
                {
                    for (int r = 0; r < rowCount; r++)
                    {
                        for (int c = 0; c < colCount; c++)
                        {
                            var area = new Area(r, c, height, width);
                            var covered = area.GetCoveredCells(rowCount, colCount).ToList();
                            if (covered.All(pos => ones.Contains(pos)))
                            {
                                allAreas.Add(area);
                            }
                        }
                    }
                }
            }

            // Жадно выбираем области, покрывающие максимум новых единиц
            var result = new List<Area>();
            var coveredCells = new HashSet<(int, int)>();

            while (coveredCells.Count < ones.Count)
            {
                Area bestArea = null;
                int maxNew = 0;

                foreach (var area in allAreas)
                {
                    var cells = area.GetCoveredCells(rowCount, colCount).ToList();
                    int newCount = cells.Count(c => !coveredCells.Contains(c));
                    if (newCount > maxNew)
                    {
                        maxNew = newCount;
                        bestArea = area;
                    }
                }

                if (bestArea == null) break; // Не осталось новых областей

                result.Add(bestArea);
                foreach (var cell in bestArea.GetCoveredCells(rowCount, colCount))
                    coveredCells.Add(cell);
            }

            return result;
        }

        public bool IsValidAreaWithWrap(List<List<string>> map, bool[,] covered, int startRow, int startCol, int height, int width)
        {
            int rowCount = map.Count - 1;
            int colCount = map[0].Count - 1;

            for (int dr = 0; dr < height; dr++)
            {
                int r = (startRow + dr) % rowCount;
                for (int dc = 0; dc < width; dc++)
                {
                    int c = (startCol + dc) % colCount;

                    if (map[r + 1][c + 1] != "1") return false;
                    if (covered[r, c]) return false;
                }
            }

            return true;
        }

        public List<int> GetPowersOfTwoUpTo(int n)
        {
            var result = new List<int>();
            int power = 1;
            while (power <= n)
            {
                result.Add(power);
                power *= 2;
            }
            return result;
        }

        public static bool IsValidArea(List<List<string>> map, bool[,] covered,
            int startRow, int startColMap, int height, int width,
            int rows, int cols)
        {
            for (int dr = 0; dr < height; dr++)
            {
                int r = startRow + dr;
                if (r >= rows) return false;
                for (int dc = 0; dc < width; dc++)
                {
                    int c0 = startColMap - 1 + dc; // индекс в covered
                    int mapCol = c0 + 1; // индекс для map
                    if (mapCol >= map[r + 1].Count) return false; // Проверка границы столбца
                    if (map[r + 1][mapCol] != "1" || covered[r, c0])
                        return false;
                }
            }
            return true;
        }

        public void MarkAreaWithWrap(bool[,] covered, Area area, int rowCount, int colCount)
        {
            for (int dr = 0; dr < area.Height; dr++)
            {
                int r = (area.StartRow + dr) % rowCount;
                for (int dc = 0; dc < area.Width; dc++)
                {
                    int c = (area.StartCol + dc) % colCount;
                    covered[r, c] = true;
                }
            }
        }

        public List<string> GetVariablesFromMap(List<List<string>> map)
        {
            if (map == null || map.Count == 0 || map[0].Count == 0)
                return new List<string>();

            string header = map[0][0]; // Например: "x\yz"

            if (header.Contains("\\"))
            {
                var parts = header.Split('\\');
                var rowVars = parts[0].ToCharArray().Select(c => c.ToString()).ToList();
                var colVars = parts[1].ToCharArray().Select(c => c.ToString()).ToList();

                return rowVars.Concat(colVars).Distinct().ToList();
            }

            // Резервный вариант — стандартные имена
            int varCount = (int)Math.Log(map[0].Count - 1, 2);
            return Enumerable.Range(0, varCount)
                             .Select(i => "xyzw"[i].ToString())
                             .ToList();
        }

        public class Area
        {
            public int StartRow { get; }
            public int StartCol { get; }
            public int Height { get; }
            public int Width { get; }

            public Area(int startRow, int startCol, int height, int width)
            {
                StartRow = startRow;
                StartCol = startCol;
                Height = height;
                Width = width;
            }

            public override string ToString()
            {
                return $"Area: row={StartRow}, col={StartCol}, height={Height}, width={Width}";
            }

            public IEnumerable<(int Row, int Col)> GetCoveredCells(int totalRows, int totalCols)
            {
                for (int dr = 0; dr < Height; dr++)
                {
                    int r = (StartRow + dr) % totalRows;
                    for (int dc = 0; dc < Width; dc++)
                    {
                        int c = (StartCol + dc) % totalCols;
                        yield return (r, c);
                    }
                }
            }

            

        }
    }
}