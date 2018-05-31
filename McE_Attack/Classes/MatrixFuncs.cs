using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McE_Attack
{
    static class MatrixFuncs // Algebraic methods for matrices
    {
        public static void SwapRows(Matrix A, int row1, int row2)
        {
            List<int> tmp = A.data[row1];
            A.data[row1] = A.data[row2];
            A.data[row2] = tmp;
        }
        public static Matrix Transp(Matrix A)
        {
            Matrix A_transp = new Matrix(A.size[1], A.size[0]);
            for (int i = 0; i < A.size[0]; ++i)
            {
                for (int j = 0; j < A.size[1]; ++j)
                {
                    A_transp.data[j][i] = A.data[i][j];
                }
            }
            return A_transp;
        }
        public static Matrix SubMatrix(Matrix A, int beg, int end, bool col_flag = true)
        {
            Matrix A_sub;
            if (col_flag)
            {
                A_sub = new Matrix(A.size[0], end - beg);
                for (int i = 0; i < A.size[0]; ++i)
                {
                    for (int j = beg, jj = 0; j < end; ++j, ++jj)
                    {
                        A_sub.data[i][jj] = A.data[i][j];
                    }
                }
            }
            else
            {
                A_sub = new Matrix(end - beg, A.size[1]);
                for (int i = beg, ii = 0; i < end; ++i, ++ii)
                {
                    for (int j = 0; j < A.size[1]; ++j)
                    {
                        A_sub.data[ii][j] = A.data[i][j];
                    }
                }
            }            
            return A_sub;
        }

        //Gaussian elimination
        public static int Gauss(Matrix A)
        {
            if (A == null || A.size[0] == 0)
            {
                return 0;
            }
            int k = A.size[0], n = A.size[1];
            bool flag = true; int rank = 0;
            for (int i = 0, j = 0; i < k && j < n; ++i, ++j)//c - для инф. окна
            {
                if (i == k - 1)
                    if (A.data[k - 1][j] != 0)
                    {
                        for (int ii = 0; ii < k - 1; ++ii)
                        {
                            if (A.data[ii][j] != 0)
                            {
                                for (int s = 0; s < n; ++s)
                                {
                                    A.data[ii][s] = (A.data[ii][s] + A.data[k - 1][s]) & 1;
                                }
                            }
                        }
                        ++rank;
                        break;
                    }
                    else { i--; }
                else
                {
                    if (A.data[i][j] == 0)
                    {
                        flag = false;
                        for (int s = i + 1; s < k; ++s)
                            if (A.data[s][j] != 0)
                            {
                                flag = true;
                                MatrixFuncs.SwapRows(A, i, s);
                                break;
                            }
                    }
                    if (flag)
                    {
                        for (int s = 0; s < k; ++s)
                            if (s != i && A.data[s][j] != 0)
                                for (int q = 0; q < n; q++)
                                    A.data[s][q] = (A.data[s][q] + A.data[i][q]) & 1;
                        ++rank;
                    }
                    else { i--; flag = true; }
                }
            }
            //избавление от нулевых строк
            for (int i = rank; i < k; ++i)
            {
                A -= rank;
            }
            return rank;
        }

        //Gaussian elimination with information window
        public static int Gauss_E(Matrix A, out List<int> I, out List<int> J)
        {
            I = new List<int>(); J = new List<int>();
            if (A == null || A.size[0] == 0)
            {
                return 0;
            }
            int k = A.size[0], n = A.size[1];
            if (k <= n)
            {
                bool flag = true;
                for (int i = 0, c = 0, j = 0; i < k && j < n; ++i, ++c, ++j)//c - для инф. окна
                {
                    if (i == k - 1)
                        if (A.data[k - 1][j] != 0)
                        {
                            for (int ii = 0; ii < k - 1; ++ii)
                            {
                                if (A.data[ii][j] != 0)
                                {
                                    for (int s = 0; s < n; ++s)
                                    {
                                        A.data[ii][s] = (A.data[ii][s] + A.data[k - 1][s]) & 1;
                                    }
                                }
                            }
                            I.Add(j); ++j;
                            while (j < n) { J.Add(j); ++j; }
                            break;
                        }
                        else { i--; J.Add(j); }
                    else
                    {
                        if (A.data[i][j] == 0)
                        {
                            flag = false;
                            for (int s = i + 1; s < k; ++s)
                                if (A.data[s][j] != 0)
                                {
                                    flag = true;
                                    MatrixFuncs.SwapRows(A, i, s);
                                    break;
                                }
                        }
                        if (flag)
                        {
                            for (int s = 0; s < k; ++s)
                                if (s != i && A.data[s][j] != 0)
                                    for (int q = 0; q < n; q++)
                                        A.data[s][q] = (A.data[s][q] + A.data[i][q]) & 1;
                            I.Add(j);
                        }
                        else { c--; i--; J.Add(j); flag = true; }
                    }
                }
                //избавление от нулевых строк
                for (int i = I.Count; i < A.size[0]; ++i)
                {
                    A -= I.Count;
                }
                return I.Count;
            }
            return -1;
        }
                
        //Gaussian elimination with information window
        public static void Gauss_on_I(Matrix A, List<int> I, List<int> J)
        {
            int k = A.size[0], n = A.size[1], idx = 0, tmp = 0; bool flag = false;
	        if (k <= n)
	        {
		        for (int i = 0, j = 0; i < k && j < k; ++i, ++j)//c - для инф. окна
		        {
			        if (i == k - 1)
                    {
                        if (A.data[i][I[j]] == 0)
                        {
                            idx = Common.GetRand() % J.Count;
                            tmp = I[j];
                            I[j] = J[idx];
                            J[idx] = tmp;
                            --j; --i;
                            continue;
                        }
                        else
                        {
                            for (int ii = 0; ii < k - 1; ++ii)
                            {
                                if (A.data[ii][I[j]] != 0)
                                {
                                    for (int s = 0; s < n; ++s)
                                    {
                                        A.data[ii][s] = (A.data[ii][s] + A.data[k - 1][s]) & 1;
                                    }
                                    break;
                                }
                            }
                        }
                    }				        
			        else
			        {
				        if (A.data[i][I[j]] == 0)
                        {
                            flag = false;
                            for (int s = i + 1; s < k; ++s)
                            {
                                if (A.data[s][I[j]] != 0)
                                {
                                    SwapRows(A, i, s);
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            flag = true;
                        }

                        if (flag == false)
                        {
                            idx = Common.GetRand() % J.Count;
                            tmp = I[j];
                            I[j] = J[idx];
                            J[idx] = tmp;
                            --j; --i;
                            continue;
                        }

				        for (int s = 0; s < k; ++s)
                        {
                            if (s != i && A.data[s][I[j]] != 0)
                            {
                                for (int q = 0; q < n; q++)
                                {
                                    A.data[s][q] = (A.data[s][q] + A.data[i][q]) & 1;
                                }
                            }
                        }
			        }
		        }
	        }
        }
        
        //Solve AX = B using gaussian elimination
        public static Matrix SolveEq(Matrix A, Matrix B)
        {
            Matrix A_copy = new Matrix(A), B_copy = new Matrix(B);
	        int k = A_copy.size[0], n = A_copy.size[1], rank = 0, n1 = B_copy.size[1];
	        bool flag = true;
	        for (int i = 0, j = 0; i < k && j < n; ++i, ++j)
	        {
		        if (i == k - 1)
			        if (A_copy.data[k - 1][j] != 0)
			        {
				        for (int ii = 0; ii < k - 1; ++ii)
					        if (A_copy.data[ii][j] != 0)
					        {
						        for (int s = 0; s < n; ++s)
                                    A_copy.data[ii][s] = (A_copy.data[ii][s] + A_copy.data[k - 1][s]) & 1;
						        for (int s = 0; s < n1; ++s)
                                    B_copy.data[ii][s] = (B_copy.data[ii][s] + B_copy.data[k - 1][s]) & 1;
					        }
                        ++rank; ++j;
				        while (j < n) ++j;
				        break;
			        }
			        else i--;
		        else
		        {
			        if (A_copy.data[i][j] == 0)
			        {
				        flag = false;
				        for (int s = i + 1; s < k; ++s)
					        if (A_copy.data[s][j] != 0)
					        {
						        flag = true;
						        SwapRows(A_copy, i, s); SwapRows(B_copy, i, s);
						        break;
					        }
			        }
			        if (flag)
			        {
				        for (int s = 0; s < k; ++s)
					        if (s != i && A_copy.data[s][j] != 0)
					        {
						        for (int q = 0; q < n; q++)
							        A_copy.data[s][q] = (A_copy.data[s][q] + A_copy.data[i][q]) & 1;
						        for (int q = 0; q < n1; q++)
							        B_copy.data[s][q] = (B_copy.data[s][q] + B_copy.data[i][q]) & 1;
					        }
                        ++rank;
			        }
			        else { --i; flag = true; }
		        }
	        }
            //избавление от нулевых строк
            for (int i = rank; i < k; ++i)
            {
                B_copy -= rank;
            }
	        return B_copy;
        }

        // Find A^{-1}
        public static Matrix Invert(Matrix A)
        {
            Matrix E = new Matrix(A.size[0], A.size[0]);
            for (int i = 0; i < A.size[0]; ++i)
            {
                E.data[i][i] = 1;
            }
            return MatrixFuncs.SolveEq(A, E);
        }
        
        // Generate a random invertible matrix
        public static Matrix GenNoise(int size)
        {
            Matrix res = new Matrix(size, size);
            do
            {
                for (int i = 0; i < size; ++i)
                {
                    for (int j = 0; j < size; ++j)
                    {
                        res.data[i][j] = Common.GetRand() & 1;
                    }
                }
            } while (MatrixFuncs.Gauss(new Matrix(res)) < size);
            return res;
        }
    }
}
