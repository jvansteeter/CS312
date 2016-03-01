using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void solve_Click(object sender, EventArgs e)
        {
            // retrieve the inputs from the GUI
            long N = Convert.ToInt64(input.Text);
            int k = Convert.ToInt32(ktests.Text);
            Random random = new Random();

            int testsPassed = 0;
            // run the number of tests specified in the input
            // O(c)
            for(int i = 0; i < k; i++)
            {
                //Console.WriteLine("Test" + Convert.ToString(i));
                // generate a random number between 2 and N
                long a = random.Next(2, Convert.ToInt32(N));
                // run the modular exponentiation function a^(N-1) % N
                long result = modular_exp(a, Convert.ToInt64(N - 1), N);
                //Console.WriteLine("Result: " + Convert.ToString(result));
                // if the result of modular_exp is 1 then it passed the test, you may run another test, it may be prime
                if (result == 1)
                {
                    testsPassed++;
                }
                // if the result of modular_exp is not 1 then N is not prime, display "no" and quit testing
                else
                {
                    output.Text = "no";
                    return;
                }
            }
            // if all tests have passed, calculate the percent accuracy as 100 - (100/2^k) and display results
            double percent = 100 - (100 / (Math.Pow(2, testsPassed)));
            output.Text = "yes with probability " + Convert.ToString(percent) + "%";
        }

        private long modular_exp(long value, long exponent, long N)
        {
            // the function for modular exponentiation
            // for every test of solve_Click() this function will fun log2(n) times which will halt after at most n recursive calls, at each call it multiplies n-bit numbers
            // giving us a total run time of O(n^3)
            //Console.WriteLine("Value: " + Convert.ToString(value) + " Exponent: " + Convert.ToString(exponent) + " N: " + Convert.ToString(N));
            if (exponent == 0)
            {
                // base case, when an exponent is 0 the result will always be 1
                //Console.WriteLine("Returning: 1 base");
                return 1;
            }
            // the recursive call
            long z = modular_exp(value, exponent / 2, N);
            if (exponent % 2 == 0)
            {
                // if the exponent is even return z^2 mod N
                //Console.WriteLine("Exponent is even, z= " + Convert.ToString(z) + " value= " + Convert.ToString(value) + " exponent=" + Convert.ToString(exponent));
                long result = ((z * z) % N);
                //Console.WriteLine("Returning: " + Convert.ToString(result));
                return result;
            }
            else
            {
                // if the exponent is odd return x*z^2 mod N
                //Console.WriteLine("Exponent is odd, z= " + Convert.ToString(z) + " value= " + Convert.ToString(value) + " exponent=" + Convert.ToString(exponent));
                //Console.WriteLine("z*z= " + Convert.ToString(z * z) + " value * (z * z)=" + Convert.ToString(value * (z * z)));
                long result = ((value % N) * ((z * z) % N) % N);
                //Console.WriteLine("Returning: " + Convert.ToString(result));
                return result;
            }
        }
    }
}
