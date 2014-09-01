using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;           // Thread
using System.Threading.Tasks;
//using System.Diagnostics;         // Process
using System.Windows.Forms;       // Application
using Microsoft.Win32;            // Registry

namespace hibernate
{
	class Program
	{
		//private static string ErrorLow = "Please enter TIME argument\nUsage: hibernate <seconds>";
		private static string ErrorMedium = "Please run as administrator (one-time action)";

		static int Main(string[] args)
		{
			if (isHibernationEnabled())
			{
				return doHibernation(args);
			}
			else
			{
				if (isAppElevated())
				{
					enableHibernation();

					return doHibernation(args);
				}
				else
				{
					Console.WriteLine(ErrorMedium);
					return 2;
				}
			}
		}

		private static bool isHibernationEnabled()
		{
			using (RegistryKey _reg = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\", false))
			{
				return Convert.ToBoolean(_reg.GetValue("HibernateEnabled", 0));
			}
		}

		private static void enableHibernation()
		{
			//Process.Start(@"cmd", @"/C powercfg -h on");
			using (RegistryKey _reg = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\", true))
			{
				_reg.SetValue("HibernateEnabled", 1, RegistryValueKind.DWord);
			}
		}

		private static int doHibernation(string[] args) // can be void, w/o returns and ~ (!) args
		{
			if (args.Length == 0)
			{
				Application.SetSuspendState(PowerState.Hibernate, true, true);
				return 0;
			}
			else
			{
				uint sec;
				bool test = uint.TryParse(args[0], out sec);
				if (test == false)
				{
					Console.WriteLine("Please enter TIME argument\nUsage: hibernate <seconds>"); // ErrorLow
                    return 1;
				}

				Thread.Sleep(Convert.ToInt32(sec * 1000));

				Application.SetSuspendState(PowerState.Hibernate, true, true);
				return 0;
			}
		}

		private static bool isAppElevated()
		{
			RegistryKey _reg;
			try { _reg = Registry.LocalMachine.OpenSubKey("Software\\", true); }
			catch { return false; }
			_reg.Close();
			return true;
		}
	}
}