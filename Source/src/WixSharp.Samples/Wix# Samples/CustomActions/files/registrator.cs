using System;
using System.Windows.Forms;

class Script
{
	static public void Main(string[] args)
	{
		if (args.Length != 0)
		{
            if (args[0] == "/u")
				MessageBox.Show("Unregistering...");
			else
				MessageBox.Show("Unknown command\n Must be '/u' or nothing.");
		}
		else
			MessageBox.Show("Registering...");
	}
}

