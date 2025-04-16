using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.Containers
{
    internal class Trie
    {
        private class TrieNode
        {
            public Dictionary<char, TrieNode> Children { get; } = new();
            public bool IsEndOfWord { get; set; }
        }

        private readonly TrieNode _root = new();

        public void Insert(string word)
        {
            var node = _root;
            foreach (char c in word)
            {
                if (!node.Children.ContainsKey(c))
                    node.Children[c] = new TrieNode();
                node = node.Children[c];
            }
            node.IsEndOfWord = true;
        }

        public string FindLongestMatch(string text, int startIndex)
        {
            TrieNode node = _root;
            int longestMatch = -1;
            int currentLength = 0;

            for (int i = startIndex; i < text.Length; i++)
            {
                char c = text[i];
                if (!node.Children.TryGetValue(c, out var nextNode))
                    break;

                node = nextNode;
                currentLength++;
                if (node.IsEndOfWord)
                    longestMatch = currentLength;
            }

            return longestMatch > 0 ?
                text.Substring(startIndex, longestMatch) :
                null;
        }
    }
}
