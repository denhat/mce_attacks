using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace McE_Attack
{
    enum CodeType
    {
        Hamming = 0,
        RM = 1
    }

    class McElice
    {
        public Matrix Gp; // public key
        public Matrix N; // secret key
        public Matrix P; // secret key

        public double KeyGen(CodeType codetype, int[] param)
        {
            Code code = new Code();
            int k = 0, n = 0, r, m;
            switch (codetype)
            {
                case CodeType.Hamming:
                    r = param[0];
                    code = new Ham(r);
                    break;
                case CodeType.RM:
                    r = param[0]; m = param[1];
                    code = new RM(r, m);
                    break;
            }

            k = code.G.size[0]; n = code.G.size[1];
			Stopwatch t = new Stopwatch(); t.Start();

            //Matrix N
            N = MatrixFuncs.GenNoise(k);

            //Matrix P
            P = new Matrix(n, n);
            int[] check = new int[n]; int ind = 0;
            for (int i = 0; i < n; ++i)
            {
                do
                {
                    ind = Common.GetRand() % n;
                } while (check[ind] == 1);
                P.data[ind][i] = 1;
                check[ind] = 1;
            }
            Gp = N * code.G * P;
			t.Stop();
			return t.ElapsedMilliseconds / 1000.00;
        }

        public double AttackHam(out Matrix N1, out Matrix P1)
        {
            int k = Gp.size[0], n = Gp.size[1]; 
            N1 = new Matrix(k, k); P1 = new Matrix(n, n);
            Code code = new Code(Gp);
            // compute r
            int r = 0;
            while ((1 << r) - 1 < n)
            {
                ++r;
            }
            Stopwatch t = new Stopwatch(); t.Start();

            // Matrix P
            Ham ham = new Ham(r);
            IEnumerable<int> tmp1, tmp2; 
            Matrix P1_inv = new Matrix(n, n);
            for (int j = 0; j < n; ++j)
            {
                tmp1 = code.H.data.Select(x => x[j]);
                for (int jj = 0; jj < n; ++jj)
                {
                    tmp2 = ham.H.data.Select(x => x[jj]);
                    if (tmp1.SequenceEqual(tmp2))
                    {
                        P1.data[jj][j] = 1;
                        P1_inv.data[j][jj] = 1;
                        break;
                    }
                }
            }
            // Matrix N
            code.G = code.G * P1_inv;
            N1 = MatrixFuncs.Transp(MatrixFuncs.SolveEq(MatrixFuncs.Transp(ham.G), MatrixFuncs.Transp(code.G)));
            t.Stop();
            return t.ElapsedMilliseconds / 1000.00;
        }

        public double AttackRM(out Matrix N1, out Matrix P1)
        {
            int k = Gp.size[0], n = Gp.size[1];
            N1 = new Matrix(k, k); P1 = new Matrix(n, n);
            Code code = new Code(Gp); Code Gp_reduced;
            // Compute r, m
            int r = -1, m = (int)Math.Log(n, 2), k1 = 0;
            while (k1 < k)
            {
                ++r;
                k1 += Common.C(m, r);
            }

            RM rm = new RM(r, m);
            Stopwatch t = new Stopwatch(); t.Start();

            // Reduction
            //2 * r >= m => reduction on dual code
            if (2 * r < m || m - r == 1)
            {
                Gp_reduced = new Code(code.G);
            }
            else 
            {
                Gp_reduced = new Code(code.H); 
                r = m - r - 1; 
            }
            while (r > 1)
            {
                Gp_reduced = AttackFuncs.Reduction(Gp_reduced, r, m);
                --r;
            }

            // Matrix P
            Matrix Tmp; int idx = -1;
            Vector e = new Vector(Enumerable.Repeat(1, n).ToList());
            do
            {
                ++idx;
                Tmp = (new Matrix(Gp_reduced.G) - idx) & e;
            }
            while (MatrixFuncs.Gauss(Tmp) < m + 1);
            Tmp = Gp_reduced.G; Tmp -= idx; Matrix RM_1 = (new RM(1, m)).G -= 0;
            IEnumerable<int> tmp1, tmp2;
            Matrix P1_inv = new Matrix(n, n);
            for (int j = 0; j < n; ++j)
            {
                tmp1 = Tmp.data.Select(x => x[j]);
                for (int jj = 0; jj < n; ++jj)
                {
                    tmp2 = RM_1.data.Select(x => x[jj]);
                    if (tmp1.SequenceEqual(tmp2))
                    {
                        P1.data[jj][j] = 1;
                        P1_inv.data[j][jj] = 1;
                        break;
                    }
                }
            }
            // Matrix N
            code.G = code.G * P1_inv;
            N1 = MatrixFuncs.Transp(MatrixFuncs.SolveEq(MatrixFuncs.Transp(rm.G), MatrixFuncs.Transp(code.G)));

            t.Stop();
            return t.ElapsedMilliseconds / 1000.00;
        }
    }

    class Sidel_mod
    {
        public Matrix Gp; // public key
        public List<Matrix> N; // secret key
        public Matrix P; // secret key

        public double KeyGen(CodeType codetype, int[] param)
        {
            Code code = new Code();
            int k, n, r = 0, s = 0, m, nn;
            switch (codetype)
            {
                case CodeType.Hamming:
                    r = param[0];
                    s = param[1];
                    code = new Ham(r);
                    break;
                case CodeType.RM:
                    r = param[0]; m = param[1]; s = param[2];
                    code = new RM(r, m);                 
                    break;
            }

			k = code.G.size[0]; n = code.G.size[1]; nn = n * s;
			Stopwatch t = new Stopwatch(); t.Start();

            //Matrix list N
            N = new List<Matrix>(); 
            Matrix N_i, NG = new Matrix();
            for (int ii = 0; ii < s; ++ii)
            {
                N_i = MatrixFuncs.GenNoise(k);
                N.Add(N_i);
                NG |= N_i * code.G;
            }

            //Matrix P
            P = new Matrix(nn, nn);
            int[] check = new int[nn]; int ind = 0;
            for (int i = 0; i < nn; ++i)
            {
                do
				{
					ind = Common.GetRand() % nn;
                } while (check[ind] == 1);
                P.data[ind][i] = 1;
                check[ind] = 1;
            }

            Gp = NG * P;
			t.Stop();
			return t.ElapsedMilliseconds / 1000.00;
        }

        public Matrix GetBasisOfWeight(Matrix B, int weight, int num)
        {
            Matrix res = new Matrix(), Bchange, B_copy = new Matrix(B);
            while (res.size[0] < num)
            {
                Bchange = MatrixFuncs.GenNoise(B.size[0]);
                B_copy = Bchange * B_copy;
                for (int i = 0; i < B.size[0]; ++i)
                {
                    if (Common.wt(B_copy.data[i]) == weight)
                    {
                        if (MatrixFuncs.Gauss(res & new Vector(B_copy.data[i])) == res.size[0] + 1);
                            res &= new Vector(B_copy.data[i]);
                    }
                    if (res.size[0] == num) break;
                }
            }
            return res;
        }

        public List<int>[] GetHamIndices(Code code, Ham ham, int r, int s, int n, int k)
        {
            int hweight = 1 << (r - 1);
            Matrix Bchange = MatrixFuncs.GenNoise(n * s - k), Hsub = GetBasisOfWeight(code.H, s * hweight, n - k);
            IEnumerable<int> hcol1, hcol2;
            List<int> check = Enumerable.Range(0, n * s).ToList();
            List<int>[] res_idx = new List<int>[n];

            for (int j = 0; j < n; ++j)
            {
                res_idx[j] = new List<int>();
                hcol1 = ham.H.data.Select(x => x[j]);
                for (int jj = 0; jj < check.Count; ++jj)
                {
                    hcol2 = Hsub.data.Select(x => x[check[jj]]);
                    if (hcol2.SequenceEqual(hcol1))
                    {
                        res_idx[j].Add(check[jj]);
                        check.RemoveAt(jj);
                        --jj;
                    }
                    if (res_idx[j].Count > s) break;
                }
                if (res_idx[j].Count != s)
                {
                    Hsub = GetBasisOfWeight(code.H, s * hweight, n - k);
                    res_idx = new List<int>[n];
                    check = Enumerable.Range(0, n * s).ToList();
                    j = -1;
                }
            }
            return res_idx;
        }

        bool checkP(Matrix Gp, Matrix P1_inv, Ham ham, Matrix Zero, int s, int n)
        {
            bool result = true;
            for (int i = 0; i < s - 1; ++i)
            {
                result &= MatrixFuncs.SubMatrix(Gp * P1_inv, i * n, (i + 1) * n) * MatrixFuncs.Transp(ham.H) == Zero;
            }
            return result;
        }

        public double AttackHam(out List<Matrix> Nlist, out Matrix P1)
        {
            int k = Gp.size[0], nn = Gp.size[1];
            Nlist = new List<Matrix>(); P1 = new Matrix(nn, nn);
            int r = (int)Math.Log((nn + 2) / 2, 2);
            int tmp = nn % ((1 << r) - 1), s;
            while (tmp != 0)
            {
                --r;
                tmp = nn % ((1 << r) - 1);
            }
            s = nn / ((1 << r) - 1); int n = nn / s;
            Matrix Ni = new Matrix(k, k); P1 = new Matrix(nn, nn); Matrix P1_inv = new Matrix(nn, nn);
            Code code = new Code(Gp);
            Ham ham = new Ham(r);

            Stopwatch t = new Stopwatch(); t.Start();

            // Matrix P1
            List<int>[] ham_idx = GetHamIndices(code, ham, r, s, n, k);            
            Matrix Zero = new Matrix(k, n - k);
            BigInteger iter_count = 0, max_iter_num = 1, ind_count = 0;
            List<int>[] idx_copy = new List<int>[n]; int[][] indices = new int[s][];
            for (int i = 0; i < s; ++i)
            {
                indices[i] = new int[n];
                max_iter_num = BigInteger.Multiply(max_iter_num, BigInteger.Pow(s - i, n));
            }
            do
            {
                ++iter_count; ++ind_count;
                if (iter_count > max_iter_num)
                {
                    ham_idx = GetHamIndices(code, ham, r, s, n, k);
                    for (int i = 0; i < s; ++i)
                    {
                        indices[i] = new int[n];
                    }
                    iter_count = 1; ind_count = 1;
                }
                idx_copy = ham_idx.Select(x => new List<int>(x)).ToArray();
                P1 = new Matrix(nn, nn); P1_inv = new Matrix(nn, nn);
                for (int i = 0; i < s; i++)
                {               
                    for (int j = 0; j < n; ++j)
                    {
                        P1.data[i * n + j][ idx_copy[j][ indices[i][j] ] ] = 1;
                        P1_inv.data[idx_copy[j][indices[i][j]]][i * n + j] = 1;
                        idx_copy[j].RemoveAt(indices[i][j]);
                    }
                }
                Common.NextSet(indices[0], s, n);
                if (ind_count == BigInteger.Pow(s, n))
                {
                    indices[0] = new int[n];
                    Common.NextSet(indices[1], s - 1, n);
                    ind_count = 0;
                }
            } while (checkP(Gp, P1_inv, ham, Zero, s, n) == false);

            // Matrices N
            Matrix NG = Gp * P1_inv;
            for (int i = 0; i < s; ++i)
            {
                Ni = MatrixFuncs.SolveEq(MatrixFuncs.Transp(ham.G), MatrixFuncs.Transp(MatrixFuncs.SubMatrix(NG, i*n, (i + 1)*n)));
                var ttt = MatrixFuncs.Transp(Ni) * ham.G == MatrixFuncs.SubMatrix(NG, i * n, (i + 1) * n);
                Nlist.Add(MatrixFuncs.Transp(Ni));
            }
            NG = new Matrix();
            for (int i = 0; i < Nlist.Count; ++i)
            {
                NG |= Nlist[i] * ham.G;
            }
            t.Stop();
            return t.ElapsedMilliseconds / 1000.00;
        }
    }

    class Wish_mod 
    {
        public Matrix Gp; // public key
        public Matrix N; // secret key
        public Matrix P; // secret key
        public Matrix R; // secret key

        public double KeyGen(CodeType codetype, int[] param)
        {
            Code code = new Code();
			int k = 0, n = 0, r, m, l = param[2];
            switch (codetype)
            {
                case CodeType.Hamming:
                    r = param[0]; 
                    code = new Ham(r);
                    break;
                case CodeType.RM:
                    r = param[0]; m = param[1];
                    code = new RM(r, m);
                    break;
            }
			k = code.G.size[0]; n = code.G.size[1];
			Stopwatch t = new Stopwatch(); t.Start();

            //Matrix N
            N = MatrixFuncs.GenNoise(k);

            //Matrix P
            int nn = n + l; P = new Matrix(nn, nn);
            int[] check = new int[nn]; int ind = 0;
            for (int i = 0; i < nn; ++i)
            {
                do
				{
					ind = Common.GetRand() % nn;
                } while (check[ind] == 1);
                P.data[ind][i] = 1;
                check[ind] = 1;
            }

            //Matrix R
            R = new Matrix(k, l);
            for (int i = 0; i < k; ++i)
            {
                for (int j = 0; j < l; ++j)
                {
					R.data[i][j] = Common.GetRand() & 1;
                }
            }

            Gp = N * (code.G | R) * P;
			t.Stop();
			return t.ElapsedMilliseconds / 1000.00;
        }

		public double AttackRM(out Matrix N1, out Matrix P1, out List<int> I)
        {
            int k = Gp.size[0], nn = Gp.size[1];            
            Code code = new Code(Gp);
            int r = -1, m = 1, count = 0;
            while ((1 << (m + 1)) < nn)
            {
                ++m;
            }
            while (count < k)
            {
                ++r;
                count += Common.C(m, r);
            }
            Code code_attack;
            //2 * r >= m => consider the dual code
            if (2 * r < m)
            {
                code_attack = new Code(code.G);
            }
            else
            {
                code_attack = new Code(code.H);
                r = m - r - 1;
            }
            int n = (1 << m), l = nn - n;
            N1 = new Matrix(k, k); P1 = new Matrix(n, n);

			Stopwatch t = new Stopwatch(); t.Start();

            I = new List<int>(); Code Li;
            int dim_square = Code.Square(code_attack).size[0], idx;
            List<int> check = Enumerable.Range(0, nn).ToList();
            while (I.Count != l)
            {
                idx = Common.GetRand() % check.Count;
                Li = new Code(new Matrix(code_attack.G) ^ check[idx], false);
                if (Code.Square(Li).size[0] == dim_square - 1)
                {
                    I.Add(check[idx]);
                }
                check.RemoveAt(idx);
            }

            //I = new List<int>(); Code Li;
            //int dim_square = Code.Square(code).size[0];
            //for (int i = 0; i < nn; ++i)
            //{
            //    Li = new Code(new Matrix(code.G) ^ i, false);
            //    if (Code.Square(Li).size[0] == dim_square - 1)
            //    {
            //        I.Add(i);
            //    }
            //    if (I.Count == l) break;
            //}

            t.Stop();

            Matrix Tmp = new Matrix(code.G); int shift = 0;
            I.Sort();
			for (int i = 0; i < l; ++i)
			{
				Tmp ^= I[i] - shift;
                ++shift;
            }
			McElice McE = new McElice();
			McE.Gp = Tmp;

			McE.AttackRM(out N1, out P1);
            
            return t.ElapsedMilliseconds / 1000.00;
        }
    }
}
