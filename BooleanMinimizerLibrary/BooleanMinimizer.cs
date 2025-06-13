using System;
using System.Collections.Generic;
using System.Linq;

namespace BooleanMinimizerLibrary
{
    public static class BooleanMinimizer
    {
        // Вход: вектор функции (например, "0110" для 2 переменных)
        // Возвращает минимизированную ДНФ (МДНФ)
        public static string MinimizeSDNF(string vector, List<string> variables = null)
        {
            var minterms = GetIndicesByValue(vector, '1');
            if (variables == null)
                variables = GetDefaultVariables(vector.Length);

            if (minterms.Count == 0)
                return "0"; // Функция всегда 0

            if (minterms.Count == vector.Length)
                return "1"; // Функция всегда 1

            var implicants = QuineMcCluskey(minterms, variables.Count);
            var essential = FindEssentialPrimeImplicants(implicants, minterms);

            return BuildExpression(essential, variables, positive: true);
        }

        // Возвращает минимизированную КНФ (МКНФ)
        public static string MinimizeSKNF(string vector, List<string> variables = null)
        {
            var maxterms = GetIndicesByValue(vector, '0');
            if (variables == null)
                variables = GetDefaultVariables(vector.Length);

            if (maxterms.Count == 0)
                return "1"; // Всегда 1

            if (maxterms.Count == vector.Length)
                return "0"; // Всегда 0

            var implicants = QuineMcCluskey(maxterms, variables.Count);
            var essential = FindEssentialPrimeImplicants(implicants, maxterms);

            return BuildExpression(essential, variables, positive: false);
        }

        // Поиск индексов элементов в векторе, равных заданному символу
        public static List<int> GetIndicesByValue(string vector, char value)
        {
            var indices = new List<int>();
            for (int i = 0; i < vector.Length; i++)
                if (vector[i] == value)
                    indices.Add(i);
            return indices;
        }

        // Возвращаем имена переменных по количеству переменных (до 4)
        public static List<string> GetDefaultVariables(int length)
        {
            int n = (int)Math.Log(length, 2);
            var names = new[] { "w", "x", "y", "z" };
            return names.Take(n).ToList();
        }

        // Представление импликанта (группа минтермов), где '-' означает «неважно»
        public class Implicant
        {
            public string Bits; // Например, 1-0-
            public HashSet<int> CoveredMinterms;

            public Implicant(string bits, HashSet<int> covered)
            {
                Bits = bits;
                CoveredMinterms = covered;
            }

            // Проверка, можно ли объединить с другим импликантом
            public bool CanCombine(Implicant other, out Implicant combined)
            {
                int diffCount = 0;
                char[] combinedBits = new char[Bits.Length];
                for (int i = 0; i < Bits.Length; i++)
                {
                    if (Bits[i] == other.Bits[i])
                        combinedBits[i] = Bits[i];
                    else if (Bits[i] != other.Bits[i])
                    {
                        diffCount++;
                        if (diffCount > 1)
                        {
                            combined = null;
                            return false;
                        }
                        combinedBits[i] = '-';
                    }
                }
                combined = new Implicant(new string(combinedBits), new HashSet<int>(CoveredMinterms.Union(other.CoveredMinterms)));
                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj is Implicant other)
                    return Bits == other.Bits;
                return false;
            }

            public override int GetHashCode() => Bits.GetHashCode();
        }

        // Алгоритм Квайна-Мак-Класки для поиска простых импликантов
        public static List<Implicant> QuineMcCluskey(List<int> minterms, int variableCount)
        {
            // Инициализация импликантов с битовыми строками
            var implicants = minterms.Select(m => new Implicant(
                Convert.ToString(m, 2).PadLeft(variableCount, '0'),
                new HashSet<int> { m })).ToList();

            var primeImplicants = new List<Implicant>();
            bool[] combinedFlags;

            do
            {
                combinedFlags = new bool[implicants.Count];
                var newImplicants = new List<Implicant>();

                // Группируем по количеству единиц для оптимизации
                var groups = implicants.GroupBy(i => i.Bits.Count(c => c == '1')).OrderBy(g => g.Key).ToList();

                for (int g = 0; g < groups.Count - 1; g++)
                {
                    foreach (var imp1 in groups[g])
                    {
                        foreach (var imp2 in groups[g + 1])
                        {
                            if (imp1.CanCombine(imp2, out var combined))
                            {
                                if (!newImplicants.Contains(combined))
                                    newImplicants.Add(combined);
                                combinedFlags[implicants.IndexOf(imp1)] = true;
                                combinedFlags[implicants.IndexOf(imp2)] = true;
                            }
                        }
                    }
                }

                for (int i = 0; i < implicants.Count; i++)
                    if (!combinedFlags[i])
                        primeImplicants.Add(implicants[i]);

                implicants = newImplicants;

            } while (implicants.Count > 0);

            // Удаляем дубликаты
            return primeImplicants.Distinct().ToList();
        }

        // Поиск необходимых простых импликантов для покрытия всех минтермов
        public static List<Implicant> FindEssentialPrimeImplicants(List<Implicant> primeImplicants, List<int> minterms)
        {
            var essentials = new List<Implicant>();
            var uncovered = new HashSet<int>(minterms);

            while (uncovered.Count > 0)
            {
                Implicant essential = null;

                // Ищем импликант, покрывающий минтерм, который покрывает только он один
                foreach (var m in uncovered)
                {
                    var covering = primeImplicants.Where(p => p.CoveredMinterms.Contains(m)).ToList();
                    if (covering.Count == 1)
                    {
                        essential = covering[0];
                        break;
                    }
                }

                // Если не нашли обязательного - берём тот, что покрывает больше всего минтермов
                if (essential == null)
                {
                    essential = primeImplicants.OrderByDescending(p => p.CoveredMinterms.Count(c => uncovered.Contains(c))).First();
                }

                essentials.Add(essential);

                // Убираем покрытые минтермы
                foreach (var m in essential.CoveredMinterms)
                    uncovered.Remove(m);

                primeImplicants.Remove(essential);
            }

            return essentials;
        }

        // Формируем выражение из списка импликантов
        // positive = true для МДНФ (AND в импликанте), false для МКНФ (OR в импликанте)
        public static string BuildExpression(List<Implicant> implicants, List<string> variables, bool positive)
        {
            var terms = new List<string>();

            foreach (var imp in implicants)
            {
                var sb = new List<string>();
                for (int i = 0; i < imp.Bits.Length; i++)
                {
                    char bit = imp.Bits[i];
                    if (bit == '-')
                        continue;

                    if (positive)
                    {
                        sb.Add(bit == '1' ? variables[i] : $"¬{variables[i]}");
                    }
                    else
                    {
                        // Для КНФ инверсия литералов: 0 => variable, 1 => ¬variable
                        sb.Add(bit == '0' ? variables[i] : $"¬{variables[i]}");
                    }
                }

                string term = positive
                    ? "(" + string.Join(" ∧ ", sb) + ")"
                    : "(" + string.Join(" ∨ ", sb) + ")";
                terms.Add(term);
            }

            return positive
                ? string.Join(" ∨ ", terms)
                : string.Join(" ∧ ", terms);
        }
        public static string GetFullSKNF(string vector, List<string> variables = null)
        {
            var maxterms = GetIndicesByValue(vector, '0');
            if (variables == null)
                variables = GetDefaultVariables(vector.Length);

            if (maxterms.Count == 0)
                return "1";
            if (maxterms.Count == vector.Length)
                return "0";

            var terms = new List<string>();
            foreach (int index in maxterms)
            {
                var bits = Convert.ToString(index, 2).PadLeft(variables.Count, '0');
                var literals = new List<string>();

                for (int i = 0; i < variables.Count; i++)
                {
                    literals.Add(bits[i] == '0' ? variables[i] : $"¬{variables[i]}");
                }

                terms.Add("(" + string.Join(" ∨ ", literals) + ")");
            }

            return string.Join(" ∧ ", terms);
        }
        
        public static string GetFullSDNF(string vector, List<string> variables = null)
        {
            var minterms = GetIndicesByValue(vector, '1');
            if (variables == null)
                variables = GetDefaultVariables(vector.Length);

            if (minterms.Count == 0)
                return "0";
            if (minterms.Count == vector.Length)
                return "1";

            var terms = new List<string>();
            foreach (int index in minterms)
            {
                var bits = Convert.ToString(index, 2).PadLeft(variables.Count, '0');
                var literals = new List<string>();

                for (int i = 0; i < variables.Count; i++)
                {
                    literals.Add(bits[i] == '1' ? variables[i] : $"¬{variables[i]}");
                }

                terms.Add("(" + string.Join(" ∧ ", literals) + ")");
            }

            return string.Join(" ∨ ", terms);
        }
    }
}
