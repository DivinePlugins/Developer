namespace Divine.Plugin.Engine.IO.Source2;

using System;
using System.Collections.Generic;

internal class FieldOpHuffmanTree
{
    public static Node root;
    public static int[][] tree;
    public static FieldOp[] ops = FieldOp.FieldsTable;

    static FieldOpHuffmanTree()
    {
        root = buildTree();
        List<int[]> akku = new();
        buildFixedTreeR(akku, root);
        tree = reverseTree(akku);
    }

    private static Node buildTree()
    {
        var queue = new PriorityQueue<Node, Node>();
        int n = 0;

        foreach (var op in ops)
        {
            var node = new LeafNode(op, n++);
            queue.Enqueue(node, node);
        }

        while (queue.Count > 1)
        {
            var node = new InternalNode(queue.Dequeue(), queue.Dequeue(), n++);
            queue.Enqueue(node, node);
        }
        return queue.Peek();
    }

    private static int buildFixedTreeR(List<int[]> akku, Node n)
    {
        akku.Add(
            new int[]
            {
                n.left is LeafNode ? - n.left.op.Ordinal - 1 : buildFixedTreeR(akku, n.left),
                n.right is LeafNode ? - n.right.op.Ordinal - 1 : buildFixedTreeR(akku, n.right)
            }
        );

        return akku.Count - 1;
    }

    private static int[][] reverseTree(List<int[]> akku)
    {
        int r = akku.Count - 1;
        int[][] reverse = new int[r + 1][];
        for (int i = 0; i <= r; i++)
        {
            reverse[i] = new int[2];
            for (int j = 0; j <= 1; j++)
            {
                int s = akku[r - i][j];
                reverse[i][j] = s < 0 ? s : r - s;
            }
        }
        return reverse;
    }

    private void dump(int i, string prefix)
    {
        for (int s = 0; s < 2; s++)
        {
            if (tree[i][s] < 0)
            {
                Console.WriteLine(ops[-tree[i][s] - 1] + ": " + prefix + s);
            }
            else
            {
                dump(tree[i][s], prefix + s);
            }
        }
    }

    public abstract class Node : IComparable<Node>, IEquatable<Node>
    {
        public int weight;
        public int num;
        public FieldOp op;
        public Node? left;
        public Node? right;

        public Node(int weight, int num)
        {
            this.weight = weight;
            this.num = num;
        }

        public int CompareTo(Node? other)
        {
            int r = weight.CompareTo(other.weight);
            return r != 0 ? r : other.num.CompareTo(num);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Node);
        }

        public bool Equals(Node? other)
        {
            if (other is null)
            {
                return false;
            }

            var r = weight == other.weight;
            return r ? r : num == other.num;
        }

        public override int GetHashCode()
        {
            return weight.GetHashCode() ^ num.GetHashCode();
        }
    }

    public sealed class LeafNode : Node
    {
        public LeafNode(FieldOp op, int num)
            : base(Math.Max(op.Weight, 1), num)
        {
            this.op = op;
        }

        public override string ToString()
        {
            return string.Format("[{0}]", op.ToString());
        }
    }

    public sealed class InternalNode : Node
    {
        public InternalNode(Node left, Node right, int num)
            : base(left.weight + right.weight, num)
        {
            this.left = left;
            this.right = right;
        }

        public override string ToString()
        {
            return string.Format("({0})", op.ToString());
        }
    }
}