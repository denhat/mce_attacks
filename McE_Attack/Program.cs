using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace McE_Attack
{    
    static class Common
    {
        public static int C(int n, int k) //combinations count
        {
	        if (n == k || k == 0)
		        return 1;
	        if (k == 1)
		        return n;
	        return C(n - 1, k - 1) + C(n - 1, k);
        }

        public static IEnumerable<IEnumerable<T>> GetCombs<T>(IEnumerable<T> list, int size) where T : IComparable
        {
            if (size == 1) return list.Select(t => new List<T>(){ t });
            return GetCombs(list, size - 1)
                .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static int wt(List<int> vec)
        {
            int count = 0;
            for (int i = 0; i < vec.Count; ++i)
            {
                if (vec[i] != 0)
                {
                    ++count;
                }
            }
            return count;
        }

        public static int GetRand()
        {
            using (System.Security.Cryptography.RNGCryptoServiceProvider rg = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[5];
                rg.GetBytes(rno);
				return (int)BitConverter.ToUInt32(rno, 0) & 0xFFFFF;
            }
        }

        public static int[] GetIntOfBase(int value, int targetBase, int length)
        {
            int[] result = new int[length]; int count = 0;
            if (targetBase > 1)
            {
                do
                {
                    result[count] = value % targetBase;
                    value = value / targetBase;
                    ++count;
                }
                while (value > 0 && count < length);
            }
            return result;
        }

        // Generate placements with repetitions
        public static bool NextSet(int[] a, int n, int m)
        {
            if (n == 1)
            {
                a = new int[m];
                return true;
            }
            int j = m - 1;
            while (j >= 0 && a[j] == n - 1) j--;
            if (j < 0) return false;
            if (a[j] >= n)
                j--;
            a[j]++;
            if (j == m - 1) return true;
            for (int k = j + 1; k < m; k++)
                a[k] = 0;
            return true;
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            MainForm form = new MainForm();

            Application.Run(form);
        }
    }
}
