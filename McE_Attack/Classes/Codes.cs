using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace McE_Attack
{
    class Code // Linear code
    {
        public Matrix G; // generator
        public Matrix H; // parity check
		public int n { get { return G.size[1]; } set { } }
		public int k { get { return G.size[0]; } set { } } 

        public Vector Encode(Vector x)
        {
            if (x == null || G == null) return null;
            return x * G;
        }

        public Code()
        {
            G = new Matrix();
            H = new Matrix();
        }

        public Code(Matrix Generator, bool H_flag = true)
        {
            G = new Matrix(Generator);
            if (H_flag)
                H = DualMatrix(Generator);
        }

        public static Matrix DualMatrix(Matrix G1)
        {
            Matrix G1_copy = new Matrix(G1);
            List<int> I, J; int k = G1_copy.size[0], n = G1_copy.size[1];
            Matrix H1 = new Matrix(n - k, n);
            MatrixFuncs.Gauss_E(G1_copy, out I, out J);
            for (int s = 0; s < k; ++s)
                for (int i = 0; i < n - k; ++i)
                {
                    H1.data[i][I[s]] = G1_copy.data[s][J[i]];
                }
            for (int i = 0; i < n - k; ++i)
            {
                H1.data[i][J[i]] = 1;
            }
            return H1;
        }

        public bool IsCodeWord(Vector a)
        {
            return H * a == new Vector(a.size);
        }

		// Find vector by weight in a code
        public static Vector FindWeight_simple(Matrix G1, int w)
        {
	        int tmp, k = G1.size[0], n = G1.size[1];
            List<int> I = new List<int>(), J = new List<int>(); int[] check;
            Random rnd = new Random();
	        while (true)
	        {
                check = new int[n];
                while (I.Count < k)
                {
                    tmp = Math.Abs(Common.GetRand()) % n; // rnd.Next(0, n);
                    if (check[tmp] == 0)
                    {
                        I.Add(tmp);
                        check[tmp] = 1;
                    }
                }
                for (int i = 0; i < check.Length; ++i)
                {
                    if (check[i] == 0)
                        J.Add(i);
                }
                MatrixFuncs.Gauss_on_I(G1, I, J);	        
		        for (int i = 0; i < G1.size[0]; ++i)
                {
                    if (Common.wt(G1.data[i]) <= w) return new Vector(G1.data[i]);
                }
                I.Clear(); J.Clear();
	        }
        }

        public static Matrix operator *(Code C1, Code C2)
		{
			if (C1.n != C2.n) return null;
			List<List<int>> new_basis = new List<List<int>>(); 
			List<int> item = new List<int>();
			for (int i = 0; i < C1.k; ++i)
			{
				for (int j = 0; j < C2.k; ++j)
				{
					item = new List<int>();
					for (int s = 0; s < C2.n; ++s)
					{
						item.Add(C1.G.data[i][s] * C2.G.data[j][s]);
					}
					new_basis.Add(item);
				}
			}
			Matrix res = new Matrix(new_basis);
            MatrixFuncs.Gauss(res);
            return res;
		}

        public static Matrix Square(Code C)
		{
			return C * C;
		}
    }

    class Ham : Code // Hamming code
    {
        private int _r;
        public int r { get { return _r; } set { } }

        public Ham(int r)
        {
            _r = r;
            int k = (1 << r) - r - 1, n = (1 << r) - 1;
            H = new Matrix(r, n);
            for (int j = 1; j <= n; ++j)
            {
                for (int i = r - 1; i >= 0; --i)
                {
                    H.data[r - i - 1][j - 1] = (j >> i) & 1;
                }
            }
            G = Code.DualMatrix(H);
        }
    }

    class RM : Code // Hamming code
    {
        private int _r, _m;
        public int r { get { return _r; } set { } }
        public int m { get { return _m; } set { } }    
        
        public RM(int r, int m)
        {
            _r = r; _m = m;
	        int n = 1 << m, k = 0;
	        for (int i = 0; i <= r; ++i)
            {
		        k += Common.C(m, i);
            }

            G = new Matrix(k, n);
            List<int> e = Enumerable.Repeat(1, n).ToList();
	        if (r >= 0) // G0
            {
		        G.data[0] = e;
            }
	        else return;

	        int tmp;
	        if (r >= 1) // G1
            {
		        for (int i = 1; i <= m; ++i)
		        {
		            tmp = 0;
		            for (int j = 0; j < n; ++j)
		            {
			            if (j % (1 << (m - i)) == 0)
                        {
                            tmp = (tmp + 1) & 1;
                        }
			            G.data[i][n - j - 1] = tmp;
		            }
		        }
            }
	        else { H = Code.DualMatrix(G); return; }
            	        
	        int r1 = 2, count = m + 1; tmp = 0;
	        List<List<int>> comb_ind;
            IEnumerable<int> indices = Enumerable.Range(1, m);
	        while (r1 <= r) // G2, G3, ...
	        {
                comb_ind = Common.GetCombs(indices, r1).Select(c => c.ToList()).ToList();
		        for (int i = 0; i < comb_ind.Count; i++)
		        {
			        List<int> v = new List<int>(e);
                    for (int j = 0; j < comb_ind[0].Count; j++)
				        for (int s = 0; s < n; s++)
					        v[s] = v[s] * G.data[comb_ind[i][j]][s];
			        G.data[count] = v;
                    ++count;
		        }
		        ++r1;
	        }
            H = Code.DualMatrix(G);            
        }
    }
}
