using ProiectP3_BackendApp.Models;

namespace ProiectP3_BackendApp.Trees
{
    public enum Color
    {
        Red,
        Black
    }
    public class RBTree<T> where T : IComparable<T>
    {
        public Node<T>? Root;  
        private void LeftRotate(Node<T> st)
        {
            Node<T> dr = st.right; 
            st.right = dr.left;
            if (dr.left != null)
            {
                dr.left.parent = st;
            }
            if (dr != null)
            {
                dr.parent = st.parent;
            }
            if (st.parent == null)
            {
                Root = dr;
            }
            if (st == st.parent.left)
            {
                st.parent.left = dr;
            }
            else
            {
                st.parent.right = dr;
            }
            dr.left = st;
            if (st != null)
            {
                st.parent = dr;
            }

        }
        private void RightRotate(Node<T> dr)
        {

            Node<T> st = dr.left;
            dr.left = st.right;
            if (st.right != null)
            {
                st.right.parent = dr;
            }
            if (st != null)
            {
                st.parent = dr.parent;
            }
            if (dr.parent == null)
            {
                Root = st;
            }
            if (dr == dr.parent.right)
            {
                dr.parent.right = st;
            }
            if (dr == dr.parent.left)
            {
                dr.parent.left = st;
            }           
            st.right = dr;
            if (dr != null)
            {
                dr.parent = st;
            }
        }
     
        public void DisplayTree()
        {
            if (Root == null)
            {
                Console.WriteLine("Nothing in the tree!");
                return;
            }
            if (Root != null)
            {
                InOrderDisplay(Root);
            }
        }
        public Node<T> Find(T key)
        {
            bool isFound = false;
            Node<T> aux = Root;
            Node<T> nod = null;
            while (!isFound)
            {
                if (aux == null)
                {
                    break;
                }
                if (key.CompareTo(aux.val)<0)
                {
                    aux = aux.left;
                }
                if (key.CompareTo(aux.val)>0)
                {
                    aux = aux.right;
                }
                if (key.CompareTo(aux.val) == 0)
                {
                    isFound = true;
                    nod = aux;
                }
            }
            if (isFound)
            {
                return aux;
            }
            else
            {
                return null;
            }
        }
        public void Insert(T item)
        {
            Node<T> newItem = new Node<T>(item);
            if (Root == null)
            {
                Root = newItem;
                Root.colour = Color.Black;
                return;
            }
            Node<T> dr = null;
            Node<T> st = Root;
            while (st != null)
            {
                dr = st;
                if (newItem.val.CompareTo(st.val)<0)
                {
                    st = st.left;
                }
                else
                {
                    st = st.right;
                }
            }
            newItem.parent = dr;
            if (dr == null)
            {
                Root = newItem;
            }
            else if (newItem.val.CompareTo(dr.val) < 0)
            {
                dr.left = newItem;
            }
            else
            {
                dr.right = newItem;
            }
            newItem.left = null;
            newItem.right = null;
            newItem.colour = Color.Red;
            InsertFixUp(newItem);
        }
        private void InOrderDisplay(Node<T> current)
        {
            if (current != null)
            {
                InOrderDisplay(current.left);                
                InOrderDisplay(current.right);
            }
        }
        private void InsertFixUp(Node<T> item)
        {    
            while (item != Root && item.parent.colour == Color.Red)
            { 
                if (item.parent == item.parent.parent.left)
                {
                    Node<T> dr = item.parent.parent.right;
                    if (dr != null && dr.colour == Color.Red)
                    {
                        item.parent.colour = Color.Black;
                        dr.colour = Color.Black;
                        item.parent.parent.colour = Color.Red;
                        item = item.parent.parent;
                    }
                    else 
                    {
                        if (item == item.parent.right)
                        {
                            item = item.parent;
                            LeftRotate(item);
                        }                        
                        item.parent.colour = Color.Black;
                        item.parent.parent.colour = Color.Red;
                        RightRotate(item.parent.parent);
                    }

                }
                else
                {

                    Node<T> st = null;

                    st = item.parent.parent.left;
                    if (st != null && st.colour == Color.Black)
                    {
                        item.parent.colour = Color.Red;
                        st.colour = Color.Red;
                        item.parent.parent.colour = Color.Black;
                        item = item.parent.parent;
                    }
                    else
                    {
                        if (item == item.parent.left)
                        {
                            item = item.parent;
                            RightRotate(item);
                        }
                        item.parent.colour = Color.Black;
                        item.parent.parent.colour = Color.Red;
                        LeftRotate(item.parent.parent);

                    }

                }
                Root.colour = Color.Black;
            }
        }
        public void Delete(T key)
        {
            Node<T> item = Find(key);
            if (item == null)
            {
                return;
            }
            Node<T> dr;
            if (item.left == null || item.right == null)
            {
                dr = item;
            }
            else
            {
                dr = TreeSuccessor(item);
            }
            Node<T> st;
            if (dr.left != null)
            {
                st = dr.left;
            }
            else
            {
                st = dr.right;
            }
            if (st != null)
            {
                st.parent = dr;
            }
            if (dr.parent == null)
            {
                Root = st;
            }
            else if (dr == dr.parent.left)
            {
                dr.parent.left = st;
            }
            else
            {
                dr.parent.left = st;
            }
            if (dr != item)
            {
                item.val = dr.val;
            }
            if (dr.colour == Color.Black)
            {
                DeleteFixUp(st);
            }

        }
        private void DeleteFixUp(Node<T> st)
        {

            while (st != null && st != Root && st.colour == Color.Black)
            {
                if (st == st.parent.left)
                {
                    Node<T> pdr = st.parent.right;
                    if (pdr.colour == Color.Red)
                    {
                        pdr.colour = Color.Black; 
                        st.parent.colour = Color.Red; 
                        LeftRotate(st.parent); 
                        pdr = st.parent.right;
                    }
                    if (pdr.left.colour == Color.Black && pdr.right.colour == Color.Black)
                    {
                        pdr.colour = Color.Red;
                        st = st.parent;
                    }
                    else if (pdr.right.colour == Color.Black)
                    {
                        pdr.left.colour = Color.Black;
                        pdr.colour = Color.Red;
                        RightRotate(pdr);
                        pdr = st.parent.right;
                    }
                    pdr.colour = st.parent.colour;
                    st.parent.colour = Color.Black;
                    pdr.right.colour = Color.Black; 
                    LeftRotate(st.parent);
                    st = Root;
                }
                else
                {
                    Node<T> pst = st.parent.left;
                    if (pst.colour == Color.Red)
                    {
                        pst.colour = Color.Black;
                        st.parent.colour = Color.Red;
                        RightRotate(st.parent);
                        pst = st.parent.left;
                    }
                    if (pst.right.colour == Color.Black && pst.left.colour == Color.Black)
                    {
                        pst.colour = Color.Black;
                        st = st.parent;
                    }
                    else if (pst.left.colour == Color.Black)
                    {
                        pst.right.colour = Color.Black;
                        pst.colour = Color.Red;
                        LeftRotate(pst);
                        pst = st.parent.left;
                    }
                    pst.colour = st.parent.colour;
                    st.parent.colour = Color.Black;
                    pst.left.colour = Color.Black;
                    RightRotate(st.parent);
                    st = Root;
                }
            }
            if (st != null)
                st.colour = Color.Black;
        }
        public Node<T> Minimum(Node<T> st)
        {
            if (st.left != null)
            {
                while (st.left.left != null)
                {
                    st = st.left;
                }
                if (st.left.right != null)
                {
                    st = st.left.right;
                }
            }
            return st;
        }
        private Node<T> TreeSuccessor(Node<T> st)
        {
            if (st.left != null)
            {
                return Minimum(st);
            }
            else
            {
                Node<T> dr = st.parent;
                while (dr != null && st == dr.right)
                {
                    st = dr;
                    dr = dr.parent;
                }
                return dr;
            }
        }
    }

    public class Node<T>
    {
        public Color colour;

        public Node<T> left;
        public Node<T> right;
        public Node<T> parent;

        public T val;

        public Node(T data) { this.val = data; }
        public Node(Color colour) { this.colour = colour; }
        public Node(T data, Color colour) { this.val = data; this.colour = colour; }
    }
}
