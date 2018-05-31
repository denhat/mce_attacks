using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace McE_Attack
{
    // Matrices in F2
    class Matrix
    {
        public List<List<int>> data { get; set; }
        private int n1 = 0;
        private int n2 = 0;
        public int[] size { get { return new int[] { n1, n2 }; } set { } }

        public Matrix()
        {
            data = new List<List<int>>();
        }
        public Matrix(int N1, int N2)
        {
            data = new List<List<int>>(N1);
            for (int i = 0; i < N1; i++)
            {
                data.Add(new List<int>(new int[N2]));
            }
            n1 = N1; n2 = N2;
        }
        public Matrix(List<List<int>> data1)
        {
            if (data1 != null)
            {
                data = new List<List<int>>();
                for (int i = 0; i < data1.Count; ++i)
                {
                    data.Add(new List<int>(data1[i])); 
                }
                n1 = data.Count;
                if (data[0] != null)
                    n2 = data[0].Count;
            }
        }
        public Matrix(Vector a, bool col_flag = true)
        {
            if (a != null)
            {
                data = new List<List<int>>();
                if (col_flag)
                {
                    for (int i = 0; i < a.data.Count; ++i)
                    {
                        data.Add(new List<int>(a.data[i]));
                    }
                }
                else
                {
                    data.Add(a.data);
                }
                n1 = data.Count;
                if (n1 != 0)
                    n2 = data[0].Count;              
            }
        }
        public Matrix(Matrix a)
        {
            if (a != null)
            {
                data = new List<List<int>>();
                for (int i = 0; i < a.data.Count; ++i)
                {
                    data.Add(new List<int>(a.data[i]));
                }
                n1 = data.Count;
                if (n1 != 0)
                    n2 = data[0].Count;
            }
        }
        public Matrix(string file_name)
        {
            data = null;
            if (File.Exists(file_name) != false)
            {
                //FileStream fin = File.OpenRead(file_name);
                List<string> rows = File.ReadLines(file_name).ToList();
                data = new List<List<int>>();
                for (int i = 0; i < rows.Count; ++i)
                {
                    data.Add(rows[i].Split(',').Select(a => Int32.Parse(a)).ToList());
                }
                n1 = data.Count;
                if (data[0] != null)
                    n2 = data[0].Count;
            }
        }
        
        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < size[0]; ++i)
            {
                //for (int j = 0; j < size[1]; ++j)
                //{
                //    res += this.data[i][j].ToString();
                //}
                res += String.Join("", this.data[i]) + "\r\n";
            }
            return res;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a == null || b == null) return null;
            if (a.size[0] != b.size[0] || a.size[1] != b.size[1])
                return null;
            Matrix res = new Matrix();
            for (int i = 0; i < a.size[0]; ++i)
            {
                res.data.Add(new List<int>());
                for (int j = 0; j < a.size[1]; ++j)
                {
                    res.data[i].Add((a.data[i][j] + b.data[i][j]) & 1);
                }
            }
            return res;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a == null || b == null) return null;
            if (a.size[1] != b.size[0] || a.size[1] == 0 || b.size[1] == 0)
                return null;
            Matrix res = new Matrix(a.size[0], b.size[1]); int tmp;
            for (int i = 0; i < a.size[0]; ++i)
            {
                for (int j = 0; j < b.size[1]; ++j)
                {
                    tmp = 0;
                    for (int k = 0; k < a.size[1]; ++k)
                    {
                        tmp += a.data[i][k] * b.data[k][j];
                    }
                    res.data[i][j] = tmp & 1;
                }
            }
            return res;
        }

        // Column concatenation
        public static Matrix operator |(Matrix a, Matrix b)
        {
            if (a == null || b == null) return null;
            if (a.size[1] == 0) return new Matrix(b);
            if (b.size[1] == 0) return new Matrix(a);
            if (a.size[0] != b.size[0]) return null;
            Matrix res = new Matrix(a.size[0], a.size[1] + b.size[1]);
            for (int i = 0; i < res.size[0]; ++i)
            {
                for (int j = 0; j < res.size[1]; ++j)
                {
                    if (j < a.size[1])
                        res.data[i][j] = a.data[i][j];
                    else                        
                        res.data[i][j] = b.data[i][j - a.size[1]];
                }
            }
            return res;
        }
        
        // Row concatenation
        public static Matrix operator &(Matrix a, Matrix b)
        {
            if (a == null || b == null) return null;
            if (a.size[0] == 0) return new Matrix(b);
            if (b.size[0] == 0) return new Matrix(a);
            if (a.size[1] != b.size[1]) return null;
            Matrix res = new Matrix(a.size[0] + b.size[0], a.size[1]);
            for (int i = 0; i < res.size[0]; ++i)
            {
                for (int j = 0; j < res.size[1]; ++j)
                {
                    if (i < a.size[0])
                        res.data[i][j] = a.data[i][j];
                    else
                        res.data[i][j] = b.data[i - a.size[0]][j];
                }
            }
            return res;
        }

        // Vector column concatenation
        public static Matrix operator |(Matrix a, Vector b)
        {
            if (a == null || b == null) return null;
            if (a.size[1] == 0) return new Matrix(b);
            if (b.size == 0) return new Matrix(a);
            if (a.size[0] != b.size) return null;
            Matrix res = new Matrix(a.size[0], a.size[1] + 1);
            for (int i = 0; i < res.size[0]; ++i)
            {
                for (int j = 0; j < res.size[1]; ++j)
                {
                    if (j < a.size[1])
                        res.data[i][j] = a.data[i][j];
                    else
                        res.data[i][j] = b.data[i];
                }
            }
            return res;
        }

        // Vector row concatenation
        public static Matrix operator &(Matrix a, Vector b)
        {
            if (a == null || b == null) return null;
            if (a.size[1] == 0) return new Matrix(b, false);
            if (b.size == 0) return new Matrix(a);
            if (a.size[1] != b.size) return null;
            Matrix res = new Matrix(a.size[0] + 1, a.size[1]);
            for (int i = 0; i < a.size[0]; ++i)
            {
                for (int j = 0; j < a.size[1]; ++j)
                {
                    res.data[i][j] = a.data[i][j];
                }                
            }
            for (int j = 0; j < b.size; ++j)
            {
                res.data[a.size[0]][j] = b.data[j];
            }
            return res;
        }

        public static bool operator ==(Matrix a, Matrix b)
        {
            if ((object)a == (object)null && (object)b == (object)null) return true;
            if ((object)a != (object)null && (object)b != (object)null)
            {
                for (int i = 0; i < a.size[0]; ++i)
                    for (int j = 0; j < a.size[1]; ++j)
                        if (a.data[i][j] != b.data[i][j])
                            return false;
                return true;
            }
            return false;
        }

        public static bool operator !=(Matrix a, Matrix b)
        {
            return !(a == b);
        }

        // delete row with index idx
        public static Matrix operator -(Matrix a, int idx)
        {
            a.data.RemoveAt(idx);
            a.n1 -= 1;
            return a;
        }

		// delete column with index idx
		public static Matrix operator ^(Matrix a, int idx)
		{
			a.data.ForEach(x => x.RemoveAt(idx));
			a.n2 -= 1;
			return a;
		}
    }

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
