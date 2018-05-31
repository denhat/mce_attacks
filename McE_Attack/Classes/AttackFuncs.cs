using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace McE_Attack
{
    class AttackFuncs
    {
        // Additional method to find index sets (for RM-codes)
        private static void Ind_set(Matrix H_trunc, List<int> ind_f, int r, int m, long M, List<List<int>> res)
        {
            res.Clear();
            int w = 1 << r, n = 1 << m, n1 = H_trunc.size[1];
	        Matrix S = new Matrix(n, n); Vector vec_min;
            // Computing of random variable matrix S
	        for (int s = 0; s < M; ++s)
	        {
                vec_min = Code.FindWeight_simple(H_trunc, w);
		        for (int i = 0; i < n1; ++i)
                {
                    for (int j = 0; j < n1; ++j)
                    {
                        if ((i != j) && (vec_min.data[i] == 1) && (vec_min.data[j] == 1))
                        {
                            S.data[ind_f[i]][ind_f[j]] += 1;
                        }
                    }
                }   
	        }

	        // Branch and bounds method to find index sets
	        int ind_i, ind_j, tmp, wmin = (1 << r) - 1, wmin_rm = (1 << (m - r));
	        for (int c = 0; c < wmin; ++c)
	        {
		        tmp = ind_i = ind_j = 0;
		        for (int i = 0; i < n; ++i)
		        {
			        for (int j = 0; j < n; ++j)
				        if (S.data[i][j] > S.data[ind_i][ind_j]) 
                        { 
                            ind_j = j; ind_i = i; 
                        }
		        }
		        res.Add(new List<int>()); res[c].Add(ind_i); res[c].Add(ind_j);
		        for (int i = 0; i < n; ++i)
                {
                    S.data[ind_i][i] = S.data[ind_j][i] = 0;
                }			        
		        while (res[c].Count < wmin_rm)
		        {
			        tmp = 0;
			        for (int i = 0; i < n; ++i)
			        {
				        int sum = 0;
				        for (int jj = 0; jj < res[c].Count; ++jj)
                        {
                            sum += S.data[i][res[c][jj]];
                        }					        
				        if (sum > tmp) { ind_j = i; tmp = sum; }
			        }
			        res[c].Add(ind_j);
			        for (int i = 0; i < n; i++)
				        S.data[ind_j][i] = 0;
		        }
	        }
        }

        // Reduction on r-parameter (for RM-codes)
        public static Code Reduction(Code code_r, int r, int m)
        {
            int k = code_r.G.size[0], n = code_r.G.size[1];
	        Matrix res = new Matrix();

	        int sz = code_r.G.size[0] - Common.C(m, r);
	        Vector x; List<int> ind_f = new List<int>(); List<List<int>> ind_sets = new List<List<int>>();
		    int w = 1 << (m - r), wmin = (1 << r) - 1;
                        
            Matrix H_short;
	        while (res.size[0] < sz)
	        {
                x = Code.FindWeight_simple(code_r.G, w);
		        H_short = new Matrix(n - k, n - w);
                int c = 0; ind_f.Clear();
		        for (int j = 0; j < n; ++j)
                {
                    if (x.data[j] == 0)
                    {
                        for (int i = 0; i < code_r.H.size[0]; ++i)
                        {
                            H_short.data[i][c] = code_r.H.data[i][j];
                        }                            
                        ind_f.Add(j);
                        ++c;
                    }
                }
		        MatrixFuncs.Gauss(H_short); //выделяем максимальную л.н. подсистему

		        Ind_set(H_short, ind_f, r, m, (long)n, ind_sets);
		        List<Vector> vecs = new List<Vector>();
		        for (int i = 0; i < wmin; ++i)
                {
                    vecs.Add(new Vector(n));
                    for (int j = 0; j < w; j++)
                    {
                        vecs[i].data[ind_sets[i][j]] = 1;
                    }
                }
			    for (int i = 0; i < wmin; ++i)
			    {
				    for (int j = 0; j < n; j++)
                    {
                        if (x.data[j] != 0)
                            vecs[i].data[j] = 1;
                    }

				    if (code_r.IsCodeWord(vecs[i]))
				    {
                        if (res.size[0] == 0 || MatrixFuncs.Gauss(res & vecs[i]) == res.size[0] + 1)
                        {
                            res &= vecs[i];
                        }						    
				    }
                    if (res.size[0] == sz) return new Code(res);
			    }
		        ind_f.Clear();
	        }
            return new Code(res);
        }
    }
}
