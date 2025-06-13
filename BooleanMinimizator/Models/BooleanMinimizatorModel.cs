using BooleanMinimizerLibrary;
using static BooleanMinimizerLibrary.KarnaughMapBuilder;

namespace BooleanMinimizator.Models
{
    public class BooleanMinimizatorModel
    {
        public string InputFunction { get; set; }  // сюда пользователь вводит функцию

        public string ResultMessage { get; set; }  // сюда выводится результат: успех или текст ошибки

        public string? PolizOutput { get; set; }   // сюда выводится ПОЛИЗ

        public bool IsSolved { get; set; }

        public string? VectorOutput { get; set; }  // сюда выводится вектор функции

        public List<Dictionary<string, bool>> TruthTable { get; set; } // Таблица истинности

        public string FunctionExpression { get; set; } // Новое поле для хранения функции

        public string SKNFOutput { get; set; }      // Совершенная конъюнктивная нормальная форма

        public string SDNFOutput { get; set; }      // Совершенная дизъюнктивная нормальная форма

        public string MKNFOutput { get; set; }      // Минимальная конъюнктивная нормальная форма

        public string MDNFOutput { get; set; }      // Минимальная дизъюнктивная нормальная форма

        public List<List<string>> KarnaughMap { get; set; }

        public List<KarnaughStep> KarnaughSteps { get; set; }

        public List<KarnaughMapBuilder.Area> Areas { get; set; }

        public List<Area> ZeroAreas { get; set; }

        public List<string> Variables { get; set; }
        public string GetExpressionForArea(Area area, List<string> variables)
        {
            if (area == null || variables == null || variables.Count == 0 || KarnaughMap == null || KarnaughMap.Count == 0)
                return "";

            // Разбор заголовка карты
            string header = KarnaughMap[0][0];
            string[] separator = new string[] { "\\\\", "\\" };
            string[] parts = header.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return "";

            string rowHeader = parts[0].Trim();
            string colHeader = parts[1].Trim();

            List<string> rowVars = rowHeader.Select(c => c.ToString()).ToList();
            List<string> colVars = colHeader.Select(c => c.ToString()).ToList();

            // Словарь для информации о переменных
            Dictionary<string, (string type, int index)> varInfo = new Dictionary<string, (string, int)>();
            for (int i = 0; i < rowVars.Count; i++)
                varInfo[rowVars[i]] = ("row", i);
            for (int i = 0; i < colVars.Count; i++)
                varInfo[colVars[i]] = ("col", i);

            // Собираем значения переменных в области
            Dictionary<string, HashSet<int>> variableValues = new Dictionary<string, HashSet<int>>();
            foreach (var v in variables)
            {
                if (varInfo.ContainsKey(v))
                    variableValues[v] = new HashSet<int>();
            }

            int numRows = KarnaughMap.Count - 1;
            int numCols = KarnaughMap[0].Count - 1;

            // Анализируем все ячейки области
            for (int dr = 0; dr < area.Height; dr++)
            {
                for (int dc = 0; dc < area.Width; dc++)
                {
                    int r = (area.StartRow + dr) % numRows;
                    int c = (area.StartCol + dc) % numCols;

                    // Получаем битовые комбинации
                    List<int> rowBits = GetGrayCode(r, rowVars.Count);
                    List<int> colBits = GetGrayCode(c, colVars.Count);

                    // Собираем значения переменных
                    foreach (var v in variableValues.Keys)
                    {
                        var (type, index) = varInfo[v];
                        int value = type == "row" ? rowBits[index] : colBits[index];
                        variableValues[v].Add(value);
                    }
                }
            }

            // Формируем литералы для МДНФ
            List<string> literals = new List<string>();
            foreach (var v in variableValues.Keys)
            {
                if (variableValues[v].Count == 1)
                {
                    int value = variableValues[v].First();
                    literals.Add(value == 1 ? v : $"¬{v}");
                }
            }

            return literals.Count > 0
                ? $"({string.Join(" ∧ ", literals)})"
                : "1"; // Константа 1 если нет переменных
        }

        public string GetExpressionForZeroArea(Area area, List<string> variables)
        {
            if (area == null || variables == null || variables.Count == 0 || KarnaughMap == null || KarnaughMap.Count == 0)
                return "";

            // Разбор заголовка карты
            string header = KarnaughMap[0][0];
            string[] separator = new string[] { "\\\\", "\\" };
            string[] parts = header.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                return "";

            string rowHeader = parts[0].Trim();
            string colHeader = parts[1].Trim();

            List<string> rowVars = rowHeader.Select(c => c.ToString()).ToList();
            List<string> colVars = colHeader.Select(c => c.ToString()).ToList();

            // Словарь для информации о переменных
            Dictionary<string, (string type, int index)> varInfo = new Dictionary<string, (string, int)>();
            for (int i = 0; i < rowVars.Count; i++)
                varInfo[rowVars[i]] = ("row", i);
            for (int i = 0; i < colVars.Count; i++)
                varInfo[colVars[i]] = ("col", i);

            // Собираем значения переменных в области
            Dictionary<string, HashSet<int>> variableValues = new Dictionary<string, HashSet<int>>();
            foreach (var v in variables)
            {
                if (varInfo.ContainsKey(v))
                    variableValues[v] = new HashSet<int>();
            }

            int numRows = KarnaughMap.Count - 1;
            int numCols = KarnaughMap[0].Count - 1;

            // Анализируем все ячейки области
            for (int dr = 0; dr < area.Height; dr++)
            {
                for (int dc = 0; dc < area.Width; dc++)
                {
                    int r = (area.StartRow + dr) % numRows;
                    int c = (area.StartCol + dc) % numCols;

                    // Получаем битовые комбинации
                    List<int> rowBits = GetGrayCode(r, rowVars.Count);
                    List<int> colBits = GetGrayCode(c, colVars.Count);

                    // Собираем значения переменных
                    foreach (var v in variableValues.Keys)
                    {
                        var (type, index) = varInfo[v];
                        int value = type == "row" ? rowBits[index] : colBits[index];
                        variableValues[v].Add(value);
                    }
                }
            }

            // Формируем литералы для МКНФ
            List<string> literals = new List<string>();
            foreach (var v in variableValues.Keys)
            {
                if (variableValues[v].Count == 1)
                {
                    int value = variableValues[v].First();
                    literals.Add(value == 0 ? v : $"¬{v}");
                }
            }

            return literals.Count > 0
                ? $"({string.Join(" ∨ ", literals)})"
                : "0"; // Константа 0 если нет переменных
        }

        // Вспомогательный метод для получения кода Грея
        private List<int> GetGrayCode(int index, int numBits)
        {
            if (numBits == 0)
                return new List<int>();

            int gray = index ^ (index >> 1);
            List<int> bits = new List<int>();
            for (int i = numBits - 1; i >= 0; i--)
            {
                bits.Add((gray >> i) & 1);
            }
            return bits;
        }
    }
}