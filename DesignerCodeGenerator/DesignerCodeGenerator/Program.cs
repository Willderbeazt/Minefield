using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignerCodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "..\\Output.txt";
            StreamWriter write = new StreamWriter(path);
            
            //Define the 
            for(int i = 1; i <= 400; i++)
            {                
                write.WriteLine("private System.Windows.Forms.Label label" + i.ToString() + ";");
            }

            write.WriteLine();
            write.WriteLine();

            for (int i = 0; i < 400; i++)
            {
                String displayValue = (i + 1).ToString();
                write.WriteLine("//");
                write.WriteLine("// label " + displayValue);
                write.WriteLine("//");                                
                write.WriteLine("this.label" + displayValue + ".BackColor = System.Drawing.Color.Tan;");
                write.WriteLine("this.label" + displayValue + ".Image = Properties.Resources.grass;");
                write.WriteLine("this.label" + displayValue + ".Name = \"label" + displayValue + "\";");
                write.WriteLine("this.label" + displayValue + ".Size = new System.Drawing.Size(20, 20);");
                write.WriteLine("this.label" + displayValue + ".Location = new System.Drawing.Point(" + ((i % 20) * 20).ToString() + ", " + ((i / 20) * 20).ToString() + ");");
                write.WriteLine("this.label" + displayValue + ".TabIndex = 0;");                
                write.WriteLine();
            }            

            write.Close();
        }
    }
}
