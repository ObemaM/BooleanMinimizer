namespace BooleanMinimizerLibrary
{
    public enum NodeType
    {
        Variable,
        Constant,
        Vector,
        Not,
        And,
        Or,
        Xor,
        Implies,
        Equivalent,
        Nand,
        Nor
    }

    public class Node
    {
        public NodeType Type { get; set; }
        public string Value { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public List<string> Variables { get; set; }

        public Node(NodeType type, string value = null, Node left = null, Node right = null)
        {
            Type = type;
            Value = value;
            Left = left;
            Right = right;
        }

        static public HashSet<string> GetVariablesRecursive(Node node)
        {
            var variables = new HashSet<string>();
            if (node == null) return variables;

            if (node.Type == NodeType.Variable)
            {
                variables.Add(node.Value);
            }
            else
            {
                variables.UnionWith(GetVariablesRecursive(node.Left));
                variables.UnionWith(GetVariablesRecursive(node.Right));
            }
            return variables;
        }
    }
}
