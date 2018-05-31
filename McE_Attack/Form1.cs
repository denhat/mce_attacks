using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace McE_Attack
{
    public partial class MainForm : Form
    {
        McElice McE;
        Sidel_mod Sid;
        Wish_mod Wish;
        Matrix N1, P1;
        List<int> I;
        List<Matrix> Nlist;
        Ham ham;
        RM rm;
        int selectedIndex = -1;

        public MainForm()
        {
            McE = new McElice();
            Sid = new Sidel_mod();
            Wish = new Wish_mod();
            InitializeComponent();
            //Ham ham = new Ham(3);
            //Sid.KeyGen(CodeType.Hamming, new[] { 3, 2 });
            //this.txtbox_keys.Text = "r = 3, s = 2 \r\n\r\n Original: \r\n" + Code.DualMatrix(ham.G | ham.G).ToString();
            //this.txtbox_keys.Text += "\r\n Noised: \r\n" + Code.DualMatrix(Sid.N[0] * ham.G | Sid.N[1] * ham.G).ToString();
            //this.txtbox_keys.Text += "\r\n Noised and permuted: \r\n" + Code.DualMatrix(Sid.Gp).ToString();
			//Sid.KeyGen(CodeType.Hamming, new[] { 3, 3 });
			//this.txtbox_keys.Text = "\r\n\r\nr = 3, s = 3 \r\n\r\n Original: \r\n" + Code.DualCode(ham.G | ham.G | ham.G).ToString();
			//this.txtbox_keys.Text += "\r\n Noised: \r\n" + Code.DualCode(Sid.N[0] * ham.G | Sid.N[1] * ham.G | Sid.N[2] * ham.G).ToString();
			//this.txtbox_keys.Text += "\r\n Noised and permuted: \r\n" + Code.DualCode(Sid.Gp).ToString();
            // form parameters
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.AutoSize = false;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btn_keygen_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbox_cslist.SelectedIndex == -1) { MessageBox.Show("Выберите криптосистему в выпадающем списке!", "Ошибка"); return; }
                if (txtbox_cparams.Text == "") { MessageBox.Show("Введите параметры криптосистемы!", "Ошибка"); return; }
                int[] param = txtbox_cparams.Text.Split(',').Select(x => Int32.Parse(x)).ToArray();
				double time = 0;
                selectedIndex = cbox_cslist.SelectedIndex;

                switch (cbox_cslist.SelectedIndex)
                {
                    case 0:
                        {
                            if (param.Length != 1) { MessageBox.Show("Неверное число параметров криптосистемы!", "Ошибка"); return; }
                            if (param[0] < 2) { MessageBox.Show("Некорректные параметры криптосистемы!", "Ошибка"); return; }
                            time = McE.KeyGen(CodeType.Hamming, new[] { param[0] });
                            ham = new Ham(param[0]);
                            txtbox_keys.Text = "N:\r\n" + McE.N.ToString() + "\r\nP:\r\n" + McE.P.ToString() + "\r\nGp:\r\n" + McE.Gp.ToString(); 
                            break;
                        }
                    case 1:
                        {
                            if (param.Length != 2) { MessageBox.Show("Неверное число параметров криптосистемы!", "Ошибка"); return; }
                            if (param[0] < 2 || param[1] < 1) { MessageBox.Show("Некорректные параметры криптосистемы!", "Ошибка"); return; }
                            if (param[1] > 3) { MessageBox.Show("Количество блоков должно быть не больше трёх!", "Ошибка"); return; }
                            time = Sid.KeyGen(CodeType.Hamming, param);
                            ham = new Ham(param[0]);
                            txtbox_keys.Text = "";
                            for (int i = 0; i < Sid.N.Count; ++i)
                            {
                                txtbox_keys.Text += "N" + (i + 1) + ":\r\n" + Sid.N[i].ToString() + "\r\n";
                            }
                            txtbox_keys.Text += "P:\r\n" + Sid.P.ToString() + "\r\nGp:\r\n" + Sid.Gp.ToString();   
                            break;
                        }
                    case 2:
                        {
                            if (param.Length != 2) { MessageBox.Show("Неверное число параметров криптосистемы!", "Ошибка"); return; }
                            if (param[0] < 0 || param[1] < 1) { MessageBox.Show("Некорректные параметры криптосистемы!", "Ошибка"); return; }
							time = McE.KeyGen(CodeType.RM, param);
                            rm = new RM(param[0], param[1]);
                            txtbox_keys.Text = "N:\r\n" + McE.N.ToString() + "\r\nP:\r\n" + McE.P.ToString() + "\r\nGp:\r\n" + McE.Gp.ToString();
                            break;
                        }
                    case 3:
                        {
                            if (param.Length != 3) { MessageBox.Show("Неверное число параметров криптосистемы!", "Ошибка"); return; }
                            if (param[0] < 0 || param[1] < 1 || param[2] < 1 || param[1] < param[0]) { MessageBox.Show("Некорректные параметры криптосистемы!", "Ошибка"); return; }
							time = Sid.KeyGen(CodeType.RM, param);
                            rm = new RM(param[0], param[1]);
                            txtbox_keys.Text = "";
                            for (int i = 0; i < Sid.N.Count; ++i)
                            {
                                txtbox_keys.Text += "N" + (i + 1) + ":\r\n" + Sid.N[i].ToString() + "\r\n";
                            }
                            txtbox_keys.Text += "P:\r\n" + Sid.P.ToString() + "\r\nGp:\r\n" + Sid.Gp.ToString();  
                            break;
                        }
                    case 4:
                        {
                            if (param.Length != 3) { MessageBox.Show("Неверное число параметров криптосистемы!", "Ошибка"); return; }
                            if (param[0] < 0 || param[1] < 1 || param[2] < 1 || param[1] < param[0]) { MessageBox.Show("Некорректные параметры криптосистемы!", "Ошибка"); return; }
							time = Wish.KeyGen(CodeType.RM, param);
                            rm = new RM(param[0], param[1]);
                            txtbox_keys.Text = "N:\r\n" + Wish.N.ToString() + "\r\nP:\r\n" + Wish.P.ToString() + "\r\nR:\r\n" + Wish.R.ToString() + "\r\nGp:\r\n" + Wish.Gp.ToString();
                            break;
                        }
                }
				lbl_time.Text = "Time (sec) = " + time;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void btn_attack_Click(object sender, EventArgs e)
        {
            if (txtbox_keys.Text == "") { MessageBox.Show("Сначала сгенерируйте ключи!", "Ошибка"); return; }
            double time = -1; bool check = false;
            switch (selectedIndex)
            {
                case 0:
                {
                    time = McE.AttackHam(out N1, out P1);
                    Matrix Gp = N1 * ham.G * P1;
                    check = (Gp == McE.Gp);
                    txtbox_unlock.Text = "N':\r\n" + N1.ToString() + "\r\nP':\r\n" + P1.ToString() + "\r\nN' * G * P':\r\n" + Gp.ToString();
                    break;
                }
                case 1:
                {
                    time = Sid.AttackHam(out Nlist, out P1);
                    Matrix NG = new Matrix(), Gp;
                    txtbox_unlock.Text = "";
                    for (int i = 0; i < Nlist.Count; ++i)
                    {
                        txtbox_unlock.Text += "N" + (i + 1) + "':\r\n" + Nlist[i].ToString() + "\r\n";
                        NG |= Nlist[i] * ham.G;
                    }
                    Gp = NG * P1;
                    check = (Gp == Sid.Gp);
                    txtbox_unlock.Text += "\r\nP':\r\n" + P1.ToString() + "\r\n\r\nN' * G * P':\r\n" + Gp.ToString();
                    break;
                }
                case 2:
                {
                    time = McE.AttackRM(out N1, out P1);
                    Matrix Gp = N1 * rm.G * P1;
                    check = (Gp == McE.Gp);
                    txtbox_unlock.Text = "N':\r\n" + N1.ToString() + "\r\nP':\r\n" + P1.ToString() + "\r\nN' * G * P':\r\n" + Gp.ToString();
                    break;
				}
				case 4:
				{
                    time = Wish.AttackRM(out N1, out P1, out I);
                    Matrix Gp = N1 * rm.G * P1, Gp_orig = new Matrix(Wish.Gp);
                    int shift = 0;
                    for (int i = 0; i < I.Count; ++i)
                    {
                        Gp_orig ^= I[i] - shift;
                        ++shift;
                    }
                    check = (Gp == Gp_orig);
                    txtbox_unlock.Text = "N':\r\n" + N1.ToString() + "\r\nP':\r\n" + P1.ToString() + "\r\nIndices: " + String.Join(", ", I);
					break;
				}
            }
            lbl_time.Text = "Time (sec) = " + time + "\r\n\r\n" + "Check = " + check;
        }

        private void cbox_cslist_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_time.Text = "";
            switch (cbox_cslist.SelectedIndex)
            {
                case 0:
                    {
                        lbl_params.Text = "Параметры криптосистемы: r";
                        break;
                    }
                case 1:
                    {
                        lbl_params.Text = "Параметры криптосистемы: r, s";
                        break;
                    }
                case 2:
                    {
                        lbl_params.Text = "Параметры криптосистемы: r, m";
                        break;
                    }
                case 3:
                    {
                        lbl_params.Text = "Параметры криптосистемы: r, m, s";
                        break;
                    }
                case 4:
                    {
                        lbl_params.Text = "Параметры криптосистемы: r, m, l";
                        break;
                    }
            }
        }
    }
}
