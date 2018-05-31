using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace McE_Attack
{
    // Vectors in F2
    class Vector
    {
        public List<int> data { get; set; }
        private int _size = -1;
        public int size { get { return _size; } set { } }

        public Vector()
        {
            data = new List<int>();
            _size = 0;
        }
        public Vector(int len)
        {
            data = new List<int>(new int[len]);
            _size = len;
        }
        public Vector(List<int> data1)
        {
            if (data1 != null)
            {
                data = new List<int>(data1);
                _size = data1.Count;
            }
        }
        public Vector(Vector a)
        {
            data = new List<int>(a.data);
            if (a != null && a.data != null)
            {
                data = new List<int>(a.data);
                _size = a.data.Count;
            }
        }
        public Vector(string file_name)
        {
            if (File.Exists(file_name) != false)
            {
                //FileStream fin = File.OpenRead(file_name);
                string row = File.ReadLines(file_name).ToList()[0];
                data = row.Split(',').Select(a => Int32.Parse(a)).ToList();
                _size = data.Count;
            }
        }

        public static Vector operator +(Vector a, Vector b)
        {
            if (a == null || b == null) return null;
            if (a.size != b.size)
                return null;
            Vector res = new Vector();
            for (int i = 0; i < a.size; ++i)
            {
                res.data.Add(a.data[i] + b.data[i]);
            }
            return res;
        }

        // component-wise vector multiplying
        public static Vector operator *(Vector a, Vector b)
        {
            if (a == null || b == null) return null;
            if (a.size != b.size || a.size == 0 || b.size == 0)
                return null;
            Vector res = new Vector();
            for (int i = 0; i < a.size; ++i)
            {
                res.data.Add(a.data[i] * b.data[i]);
            }
            return res;
        }

        public static Vector operator *(Vector x, Matrix A)
        {
            if (x == null || A == null) return null;
            if (x.size != A.size[0] || x.size == 0 || A.size[1] == 0)
                return null;
            Vector res = new Vector();
            int tmp;
            for (int i = 0; i < A.size[1]; ++i)
            {
                tmp = 0;
                for (int j = 0; j < x.size; ++j)
                {
                    tmp += x.data[j] * A.data[j][i];
                }
                res.data.Add(tmp & 1);
            }
            res._size = res.data.Count;
            return res;
        }

        public static Vector operator *(Matrix A, Vector x)
        {
            if (x == null || A == null) return null;
            if (x.size != A.size[1] || x.size == 0 || A.size[0] == 0)
                return null;
            Vector res = new Vector();
            int tmp;
            for (int i = 0; i < A.size[0]; ++i)
            {
                tmp = 0;
                for (int j = 0; j < x.size; ++j)
                {
                    tmp += x.data[j] * A.data[i][j];
                }
                res.data.Add(tmp & 1);
                res._size = res.data.Count;
            }
            return res;
        }

        public static bool operator ==(Vector a, Vector b)
        {
            if ((object)a == (object)null && (object)b == (object)null) return true;
            if ((object)a != (object)null && (object)b != (object)null)
            {
                for (int i = 0; i < a.size; ++i)
                    if (a.data[i] != b.data[i])
                        return false;
                return true;
            }
            return false;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }
    }
}
