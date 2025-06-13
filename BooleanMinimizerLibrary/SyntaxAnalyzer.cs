using System;
using System.Collections.Generic;

namespace BooleanMinimizerLibrary
{
    public class SyntaxAnalyzer
    {
        private string expr;
        private char currentChar;
        private int pos;
        private bool parsingPureVector = false;

        public Node Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Ошибка: Ввод не может быть пустым.");

            input = input.Replace(" ", "");

            // Проверяем, является ли ввод вектором
            bool isVector = true;
            foreach (char ch in input)
            {
                if (ch != '0' && ch != '1')
                {
                    isVector = false;
                    break;
                }
            }

            if (isVector)
            {
                if (input.Length == 1)
                    throw new ArgumentException("Ошибка: Вектор не может состоять из одного символа. Введите вектор длиной 2, 4, 8, 16 или 32 символа.");
                
                if (!IsPowerOfTwo(input.Length))
                    throw new ArgumentException($"Ошибка: Вектор имеет недопустимую длину {input.Length}. Допустимые длины: 2, 4, 8, 16, 32 символа.");

                parsingPureVector = true;
                var variables = new List<string>();
                int numVars = (int)Math.Log(input.Length, 2);
                for (int i = 0; i < numVars; i++)
                {
                    variables.Add(i switch
                    {
                        0 => "w",
                        1 => "x",
                        2 => "y",
                        3 => "z",
                        _ => throw new Exception("Вектор не должен быть длиннее 32 символов")
                    });
                }

                return new Node(NodeType.Vector, input) { Variables = variables };
            }

            parsingPureVector = false;
            expr = input;
            pos = 0;
            NextChar();
            Node tree = Expression();
            if (currentChar != '\0')
                ThrowError("Лишние символы в конце выражения");

            return tree;
        }

        private void NextChar()
        {
            while (pos < expr.Length && char.IsWhiteSpace(expr[pos]))
                pos++;

            currentChar = (pos < expr.Length) ? expr[pos++] : '\0';
        }

        private void ThrowError(string message)
        {
            throw new ArgumentException($"Ошибка на символе №{pos}: {message}");
        }

        private Node Expression()
        {
            Node node = Implies();
            while (currentChar == '↔')
            {
                NextChar();
                node = new Node(NodeType.Equivalent, null, node, Implies());
            }
            return node;
        }

        private Node Implies()
        {
            Node node = OrXor();
            while (currentChar == '→')
            {
                NextChar();
                node = new Node(NodeType.Implies, null, node, OrXor());
            }
            return node;
        }

        private Node OrXor()
        {
            Node node = AndNandNor();
            while (currentChar == '∨' || currentChar == '⊕')
            {
                char op = currentChar;
                NextChar();
                NodeType type = op == '∨' ? NodeType.Or : NodeType.Xor;
                node = new Node(type, null, node, AndNandNor());
            }
            return node;
        }

        private Node AndNandNor()
        {
            Node node = Not();
            while (currentChar == '∧' || currentChar == '|' || currentChar == '↓' || currentChar == '↑')
            {
                char op = currentChar;
                NextChar();
                NodeType type = op switch
                {
                    '∧' => NodeType.And,
                    '|' => NodeType.Nand,
                    '↑' => NodeType.Nand,
                    '↓' => NodeType.Nor,
                    _ => throw new Exception("Неизвестный оператор")
                };
                node = new Node(type, null, node, Not());
            }
            return node;
        }

        private Node Not()
        {
            if (currentChar == '¬')
            {
                NextChar();
                Node operand = Primary();
                if (operand == null)
                    ThrowError("Ожидается переменная или константа после ¬");
                return new Node(NodeType.Not, null, null, operand);
            }
            return Primary();
        }

        private Node Primary()
        {
            if (IsVariable(currentChar))
            {
                var varName = currentChar.ToString();
                NextChar();
                return new Node(NodeType.Variable, varName);
            }
            else if (currentChar == '0' || currentChar == '1')
            {
                if (IsVector())
                {
                    if (!parsingPureVector)
                        ThrowError("Ошибка: вектор не может быть совмещен с функцией. Введите вектор отдельно.");

                    string vector = ReadVector();

                    if (!IsPowerOfTwo(vector.Length))
                        ThrowError("Ошибка: вектор должен иметь длину степени два (2, 4, 8, 16, 32)");

                    int numVars = (int)Math.Log(vector.Length, 2);
                    var variables = new List<string>();
                    for (int i = 0; i < numVars; i++)
                    {
                        variables.Add(i switch
                        {
                            0 => "w",
                            1 => "x",
                            2 => "y",
                            3 => "z",
                            _ => throw new Exception("Максимум 4 переменные")
                        });
                    }

                    return new Node(NodeType.Vector, vector) { Variables = variables };
                }
                else
                {
                    ThrowError("Ошибка: нельзя использовать одиночные константы 0 или 1. Используйте переменные или введите вектор.");
                    return null;
                }
            }
            else if (currentChar == '(')
            {
                NextChar();
                Node node = Expression();
                if (currentChar != ')')
                    ThrowError("Ожидалось ')'");
                NextChar();
                return node;
            }
            else
            {
                ThrowError("Неожиданный символ");
                return null;
            }
        }

        private bool IsVariable(char ch) => ch == 'w' || ch == 'x' || ch == 'y' || ch == 'z';

        private bool IsVector()
        {
            int tempPos = pos;
            int length = 1;

            while (tempPos < expr.Length && (expr[tempPos] == '0' || expr[tempPos] == '1'))
            {
                tempPos++;
                length++;
            }
            return length >= 2;
        }

        private string ReadVector()
        {
            string result = currentChar.ToString();
            NextChar();

            while (currentChar == '0' || currentChar == '1')
            {
                result += currentChar;
                NextChar();
            }

            if (!IsPowerOfTwo(result.Length))
                ThrowError("Вектор должен быть длины степени два (1, 2, 4, 8, 16, 32)");

            return result;
        }

        private bool IsPowerOfTwo(int n) => n > 0 && (n & (n - 1)) == 0;

        private bool IsPureVectorInput(string input)
        {
            foreach (char ch in input)
            {
                if (ch != '0' && ch != '1')
                    return false;
            }
            
            if (input.Length == 1)
                throw new ArgumentException("Ошибка: Вектор не может состоять из одного символа. Введите вектор длиной 2, 4, 8, 16 или 32 символа.");
            
            if (!IsPowerOfTwo(input.Length))
                throw new ArgumentException($"Ошибка: Вектор имеет недопустимую длину {input.Length}. Допустимые длины: 2, 4, 8, 16, 32 символа.");
                
            return true;
        }
    }
}
