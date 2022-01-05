using System;
using System.Diagnostics;
using System.Linq;

namespace ProiectP3_BackendApp.Trees
{
    public class BTree<TK, TP> where TK : IComparable<TK>
    {
        public BTree(int degree)
        {
            if (degree < 2)
            {
                throw new ArgumentException("Gradul trebuie sa fie cel putin 2", "eroare_grad");
            }

            this.Root = new Node<TK, TP>(degree);
            this.Degree = degree;
            this.Height = 1;
        }

        public Node<TK, TP> Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }

        public Entry<TK, TP> Search(TK key)
        {
            return this.SearchInternal(this.Root, key);
        }

        public void Insert(TK newKey)
        {   

            if (!this.Root.HasReachedMaxEntries)
            {
                this.InsertNonFull(this.Root, newKey);
                return;
            }

            Node<TK, TP> oldRoot = this.Root;
            this.Root = new Node<TK, TP>(this.Degree);
            this.Root.Children.Add(oldRoot);
            this.SplitChild(this.Root, 0, oldRoot);
            this.InsertNonFull(this.Root, newKey);

            this.Height++;
        }

        public void Delete(TK keyToDelete)
        {
            this.DeleteInternal(this.Root, keyToDelete);

            if (this.Root.Entries.Count == 0 && !this.Root.IsLeaf)
            {
                this.Root = this.Root.Children.Single();
                this.Height--;
            }
        }

        private void DeleteInternal(Node<TK, TP> node, TK keyToDelete)
        {
            int i = node.Entries.TakeWhile(entry => keyToDelete.CompareTo(entry.Key) > 0).Count();

            if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(keyToDelete) == 0)
            {
                this.DeleteKeyFromNode(node, keyToDelete, i);
                return;
            }

            if (!node.IsLeaf)
            {
                this.DeleteKeyFromSubtree(node, keyToDelete, i);
            }
        }

        private void DeleteKeyFromSubtree(Node<TK, TP> parentNode, TK keyToDelete, int subtreeIndexInNode)
        {
            Node<TK, TP> childNode = parentNode.Children[subtreeIndexInNode];

            if (childNode.HasReachedMinEntries)
            {
                int leftIndex = subtreeIndexInNode - 1;
                Node<TK, TP> leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInNode + 1;
                Node<TK, TP> rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
                                                ? parentNode.Children[rightIndex]
                                                : null;

                if (leftSibling != null && leftSibling.Entries.Count > this.Degree - 1)
                {
                    childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = leftSibling.Entries.Last();
                    leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Entries.Count > this.Degree - 1)
                {
                    childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = rightSibling.Entries.First();
                    rightSibling.Entries.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    if (leftSibling != null)
                    {
                        childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode]);
                        var oldEntries = childNode.Entries;
                        childNode.Entries = leftSibling.Entries;
                        childNode.Entries.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf)
                        {
                            var oldChildren = childNode.Children;
                            childNode.Children = leftSibling.Children;
                            childNode.Children.AddRange(oldChildren);
                        }

                        parentNode.Children.RemoveAt(leftIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                    else
                    {
                        Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                        childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                        childNode.Entries.AddRange(rightSibling.Entries);
                        if (!rightSibling.IsLeaf)
                        {
                            childNode.Children.AddRange(rightSibling.Children);
                        }

                        parentNode.Children.RemoveAt(rightIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                }
            }
            this.DeleteInternal(childNode, keyToDelete);
        }

        private void DeleteKeyFromNode(Node<TK, TP> node, TK keyToDelete, int keyIndexInNode)
        {
            if (node.IsLeaf)
            {
                node.Entries.RemoveAt(keyIndexInNode);
                return;
            }

            Node<TK, TP> predecessorChild = node.Children[keyIndexInNode];
            if (predecessorChild.Entries.Count >= this.Degree)
            {
                Entry<TK, TP> predecessor = this.DeletePredecessor(predecessorChild);
                node.Entries[keyIndexInNode] = predecessor;
            }
            else
            {
                Node<TK, TP> successorChild = node.Children[keyIndexInNode + 1];
                if (successorChild.Entries.Count >= this.Degree)
                {
                    Entry<TK, TP> successor = this.DeleteSuccessor(predecessorChild);
                    node.Entries[keyIndexInNode] = successor;
                }
                else
                {
                    predecessorChild.Entries.Add(node.Entries[keyIndexInNode]);
                    predecessorChild.Entries.AddRange(successorChild.Entries);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    node.Entries.RemoveAt(keyIndexInNode);
                    node.Children.RemoveAt(keyIndexInNode + 1);

                    this.DeleteInternal(predecessorChild, keyToDelete);
                }
            }
        }
        private Entry<TK, TP> DeletePredecessor(Node<TK, TP> node)
        {
            if (node.IsLeaf)
            {
                var result = node.Entries[node.Entries.Count - 1];
                node.Entries.RemoveAt(node.Entries.Count - 1);
                return result;
            }

            return this.DeletePredecessor(node.Children.Last());
        }
        private Entry<TK, TP> DeleteSuccessor(Node<TK, TP> node)
        {
            if (node.IsLeaf)
            {
                var result = node.Entries[0];
                node.Entries.RemoveAt(0);
                return result;
            }

            return this.DeletePredecessor(node.Children.First());
        }
        private Entry<TK, TP> SearchInternal(Node<TK, TP> node, TK key)
        {
            int i = node.Entries.TakeWhile(entry => key.CompareTo(entry.Key) > 0).Count();

            if (i < node.Entries.Count && node.Entries[i].Key.CompareTo(key) == 0)
            {
                return node.Entries[i];
            }

            return node.IsLeaf ? null : this.SearchInternal(node.Children[i], key);
        }
        private void SplitChild(Node<TK, TP> parentNode, int nodeToBeSplitIndex, Node<TK, TP> nodeToBeSplit)
        {
            var newNode = new Node<TK, TP>(this.Degree);

            parentNode.Entries.Insert(nodeToBeSplitIndex, nodeToBeSplit.Entries[this.Degree - 1]);
            parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            newNode.Entries.AddRange(nodeToBeSplit.Entries.GetRange(this.Degree, this.Degree - 1));

            nodeToBeSplit.Entries.RemoveRange(this.Degree - 1, this.Degree);

            if (!nodeToBeSplit.IsLeaf)
            {
                newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(this.Degree, this.Degree));
                nodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
            }
        }

        private void InsertNonFull(Node<TK, TP> node, TK newKey)
        {
            int positionToInsert = node.Entries.TakeWhile(entry => newKey.CompareTo(entry.Key) >= 0).Count();
            if (node.IsLeaf)
            {
                node.Entries.Insert(positionToInsert, new Entry<TK, TP>() { Key = newKey});
                return;
            }

            Node<TK, TP> child = node.Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                this.SplitChild(node, positionToInsert, child);
                if (newKey.CompareTo(node.Entries[positionToInsert].Key) > 0)
                {
                    positionToInsert++;
                }
            }

            this.InsertNonFull(node.Children[positionToInsert], newKey);
        }
    }
    public class Node<TK, TP>
    {
        private int degree;

        public Node(int degree)
        {
            this.degree = degree;
            this.Children = new List<Node<TK, TP>>(degree);
            this.Entries = new List<Entry<TK, TP>>(degree);
        }

        public List<Node<TK, TP>> Children { get; set; }

        public List<Entry<TK, TP>> Entries { get; set; }

        public bool IsLeaf
        {
            get
            {
                return this.Children.Count == 0;
            }
        }

        public bool HasReachedMaxEntries
        {
            get
            {
                return this.Entries.Count == (2 * this.degree) - 1;
            }
        }

        public bool HasReachedMinEntries
        {
            get
            {
                return this.Entries.Count == this.degree - 1;
            }
        }
    }
    public class Entry<TK, TP> : IEquatable<Entry<TK, TP>>
    {
        public TK Key { get; set; }

        public TP Pointer { get; set; }

        public bool Equals(Entry<TK, TP> other)
        {
            return this.Key.Equals(other.Key) && this.Pointer.Equals(other.Pointer);
        }
    }
}
