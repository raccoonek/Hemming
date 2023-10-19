using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hemming
{
    public partial class Form1 : Form
    {
        private Matrix Ht1 = new Matrix(15,4),Ht2 = new Matrix(15, 4), G1 = new Matrix(11, 15), G2 = new Matrix(11, 15);
        
        public Form1()
        {
            InitializeComponent();

            //инициализируем первую транспонированную проверочную матрицу и порождающую
            Ht1[0, 0] = 1; Ht1[1, 0] = 1; Ht1[2,0]=1; Ht1[3, 0]=1; Ht1[11, 0] = 1;
            Ht1[4, 1] = 1; Ht1[5, 1] = 1; Ht1[6, 1] = 1; Ht1[7, 1] = 1; Ht1[12, 1] = 1;
            Ht1[0, 2] = 1; Ht1[1, 2] = 1; Ht1[4, 2] = 1; Ht1[5, 2] = 1; Ht1[8, 2] = 1; Ht1[9, 2] = 1; Ht1[13, 2] = 1;
            Ht1[0, 3] = 1; Ht1[2, 3] = 1; Ht1[4, 3] = 1; Ht1[6, 3] = 1; Ht1[8, 3] = 1; Ht1[10, 3] = 1; Ht1[14, 3] = 1;
            G1 = Matrix.get_G(Ht1);

            //инициализируем вторую транспонированную проверочную матрицу и порождающую
            Ht2[7, 0] = 1; Ht2[8, 0] = 1; Ht2[9, 0] = 1; Ht2[10, 0] = 1; Ht2[11, 0] = 1;
            Ht2[3, 1] = 1; Ht2[4, 1] = 1; Ht2[5, 1] = 1; Ht2[6, 1] = 1; Ht2[12, 1] = 1;
            Ht2[1, 2] = 1; Ht2[2, 2] = 1; Ht2[5, 2] = 1; Ht2[6, 2] = 1; Ht2[9, 2] = 1; Ht2[10, 2] = 1; Ht2[13, 2] = 1;
            Ht2[0, 3] = 1; Ht2[2, 3] = 1; Ht2[4, 3] = 1; Ht2[6, 3] = 1; Ht2[8, 3] = 1; Ht2[10, 3] = 1; Ht2[14, 3] = 1;
            G2 = Matrix.get_G(Ht2);
        }
        


        //обработчик нажатия на кнопку для кодирования
        private void button_run_Click(object sender, EventArgs e)
        {
            for (int i = -3;i<4;i++)
            {
                DecoderRun(1, 2 / 15.0 + 0.03*i, i);
                DecoderRun(2, 2 / 15.0 + 0.03*i, i);
            }
            

        }
        //обработчик нажатия на кнопку для вызова формы статистики
        private void button_get_form_statistic_Click(object sender, EventArgs e)
        {
            form_statistic form_Statistic = new form_statistic();
            form_Statistic.ShowDialog();
        }

        private void DecoderRun(int k, double probability, int kanal)
        {
            
            if (k == 1)
            {
                label_exit_1.Text = "";
                label_sindrom_1.Text = "";
            }
            else
            {
                label_exit_2.Text = "";
                label_sindrom_2.Text = "";
            }
            
            //шифруем
            string message = textBox_message.Text;
            //входная матрица
            Matrix A = new Matrix(1, 11);
            Matrix X = new Matrix(1, 15);//выходная
            Matrix Y = new Matrix(1, 15);//выходная с ошибкой
            Matrix E = new Matrix(1, 15);//вектор ошибки
            Matrix Ok = new Matrix(1, 15);//вектор для исправления
            Matrix S = new Matrix(1, 4);// синдром
            //заполняем
            for (int i = 0; i < 11; i++)
                A[0, i] = Int32.Parse(message[i].ToString());
            //умножаем на порождающую матрицу
            if (k == 1)
                X = A * G1;//выход без ошибки
            else
                X = A * G2;//выход без ошибки

            //задаем Е с определенной вероятностью ошибки на бит
            
            E = getRandomE(probability);
            //проверка вектора ошибки на ее наличие
            Boolean hasE = false;
            for(int i = 0; i < 11; i++)
                if (E[0,i]==1) { hasE = true; break; }

            //выходной вектор с ошибкой
            Y = X + E;

            if (k == 1)
            {
                //вывод результата с ошибкой
                for (int i = 0; i < Y.N; i++)
                    label_exit_1.Text += Y[0, i].ToString();
                label_exit_1.Text += "\n";
                //получение синдрома
                S = Y * Ht1;
                //вывод на форму синдрома
                for (int i = 0; i < S.N; i++)
                    label_sindrom_1.Text += S[0, i].ToString();
            }
            else
            {
                //вывод результата с ошибкой
                for (int i = 0; i < Y.N; i++)
                    label_exit_2.Text += Y[0, i].ToString();
                label_exit_2.Text += "\n";
                //получение синдрома
                S = Y * Ht2;
                //вывод на форму синдрома
                for (int i = 0; i < S.N; i++)
                    label_sindrom_2.Text += S[0, i].ToString();
            }

            
            //считаем где ошибка по синдрому
            int n = 0;
            for (int i = 0; i < S.N; i++)
                if (S[0, i] == 1) n += (int)Math.Pow(2, S.N - 1 - i);

            //проверка на наличие ошибки, исправление ее
            //и проверка, правильно ли она была исправлена
            Boolean discoveredE = false;
            
            Boolean fixedE = false;
            if (n != 0 && n<12)
            {
                discoveredE = true;
                if(k==1) Ok[0, 11 - n] = 1;
                else Ok[0, n - 1] = 1;
                Y += Ok;
                fixedE = true;
                //проверка на совпадение входных и выходных данных
                for (int i = 0; i < A.N; i++)
                    if (A[0, i] != Y[0, i]) fixedE = false;
            }else if(n != 0 && n>11) discoveredE = true;


            //вывод исправленого варианта Готовый вариант вывода 
            if (k == 1)
            {
                for (int i = 0; i < Y.N; i++)
                    label_exit_1.Text += Y[0, i].ToString();
                label_exit_1.Text += "\n";
            }
            else
            {
                for (int i = 0; i < Y.N; i++)
                    label_exit_2.Text += Y[0, i].ToString();
                label_exit_2.Text += "\n";
            }
            

            //добавление в БД
            using (ApplicationContext db = new ApplicationContext())
            {
                    Statistic1 row = new Statistic1
                    {
                        A = GetStringFromMatrix(A),
                        E = GetStringFromMatrix(E),
                        Y = GetStringFromMatrix(Y),
                        S = GetStringFromMatrix(S),
                        HasError = hasE,
                        DiscoveredError = discoveredE,
                        FixedError = fixedE,
                        Coder =k,
                        Kanal = kanal
                    };
                    // добавляем их в бд
                    db.Statistic1.Add(row);
                    db.SaveChanges();
                
            }
        }
        //рандомный вектор ошибки с определенной вероятностью
        private Matrix getRandomE(double probability)
        {
            Matrix E = new Matrix(1, 15);
            Random random = new Random();
            for(int i = 0; i < 11; i++)
            {
                double r = random.Next(1, 100)+ random.NextDouble();//рандомное число типа double
                if (r <= probability * 100) 
                    E[0, i] = 1; 
            }
            return E;
        }
        //перевод матрицы в строку
        private string GetStringFromMatrix(Matrix matrix)
        {
            string message = "";
            for (var i = 0; i < matrix.N; i++)
            {
                message += matrix[0, i].ToString();
            }
            return message;
        }
    }
    //класс матрицы
    public class Matrix
    {
        private double[,] data;

        private int m;
        public int M { get => this.m; }

        private int n;
        public int N { get => this.n; }

        public Matrix(int m, int n)
        {
            this.m = m;
            this.n = n;
            this.data = new double[m, n];
            this.ProcessFunctionOverData((i, j) => this.data[i, j] = 0);
        }
        //метод, позволяющий выполнить какое-либо действие над всеми элементами матрицы
        public void ProcessFunctionOverData(Action<int, int> func)
        {
            for (var i = 0; i < this.M; i++)
            {
                for (var j = 0; j < this.N; j++)
                {
                    func(i, j);
                }
            }
        }
        //дает порождающую из проверочной
        public static Matrix get_G(Matrix H)
        {
            var G = new Matrix(15-H.N, 15); //11,15
            for (var i = 0; i < G.M; i++)
             G[i, i] = 1;

            for (var i = 0; i < G.M; i++)
            {
                for (var j = 0; j < H.N; j++)
                {
                    G[i, j + 11] = H[i, j];
                }
            }
            return G;
        }

        public double this[int x, int y]
        {
            get
            {
                return this.data[x, y];
            }
            set
            {
                this.data[x, y] = value;
            }
        }

        //умножение на число
        public static Matrix operator *(Matrix matrix, double value)
        {
            var result = new Matrix(matrix.M, matrix.N);
            result.ProcessFunctionOverData((i, j) =>
                result[i, j] = matrix[i, j] * value);
            return result;
        }

        //сложение матриц
        public static Matrix operator +(Matrix matrix, Matrix matrix1)
        {
            var result = new Matrix(matrix.M, matrix.N);
            result.ProcessFunctionOverData((i, j) =>
                result[i, j] = (matrix[i, j] + matrix1[i,j])%2);
            return result;
        }

        // метод для умножения матриц
        public static Matrix operator *(Matrix matrix, Matrix matrix2)
        {
            if (matrix.N != matrix2.M)
            {
                throw new ArgumentException("matrixes can not be multiplied");
            }
            var result = new Matrix(matrix.M, matrix2.N);
            result.ProcessFunctionOverData((i, j) => {
                for (var k = 0; k < matrix.N; k++)
                {
                    result[i, j] += matrix[i, k] * matrix2[k, j];
                    result[i, j] = result[i, j] % 2;
                }
            });
            return result;
        }

    }
}
