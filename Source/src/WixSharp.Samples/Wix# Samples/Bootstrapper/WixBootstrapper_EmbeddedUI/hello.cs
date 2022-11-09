using System.Linq;
using System.Windows;
using System.Windows.Forms;

class Program
{
    static void Main(string[] args)
    {
        MessageBox.Show(args.FirstOrDefault());
    }
}