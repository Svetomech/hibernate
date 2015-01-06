using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SmallProjects
{
  class Hibernate
  {
    private const string app_title = "Hibernate";
    private const char cancel_key = 'a';

    public static bool isHibernationEnabled()
    {
      using (RegistryKey reg = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\", false))
      {
        return Convert.ToBoolean(reg.GetValue("HibernateEnabled", 0));
      }
    }

    public static void enableHibernation()
    {
      // Process.Start("cmd", "/C powercfg -h on");
      using (RegistryKey reg = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power\\", true))
      {
        reg.SetValue("HibernateEnabled", 1, RegistryValueKind.DWord);
      }
    }

    public static bool isAppElevated()
    {
      try { using (Registry.LocalMachine.OpenSubKey("Software\\", true)); }
      catch { return false; }
      return true;
    }

    public static void clearConsoleLine()
    {
      int currentLineCursor = Console.CursorTop;
      Console.SetCursorPosition(0, Console.CursorTop);
      Console.Write(new string(' ', Console.WindowWidth));
      Console.SetCursorPosition(0, currentLineCursor);
    }

    private static void doHibernation(string[] args)
    {
      if (0 == args.Length)
      {
        Application.SetSuspendState(PowerState.Hibernate, true, true);
      }
      else
      {
        uint sec;
        bool test = uint.TryParse(args[0], out sec);
        if (false == test)
        {
          Console.WriteLine("Please enter TIME argument.\nUsage: hibernate <seconds>");
        }

        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (o, e) => 
        {
          Console.SetCursorPosition(0, Console.CursorTop - 1);
          clearConsoleLine();
          Console.WriteLine("Windows will hibernate in {0} seconds. Hit '{1}' to abort.", --sec, cancel_key);
          if (0 >= sec)
          {
            timer.Dispose();
            Application.SetSuspendState(PowerState.Hibernate, true, true);
            Environment.Exit(0);
          }
        };

        Console.WriteLine("Windows will hibernate in {0} seconds. Hit '{1}' to abort.", sec, cancel_key);
        timer.Start();

        for (; ; ) { if (Console.ReadKey(true).KeyChar == cancel_key) break; }
      }
    }

    [STAThread]
    public static void Main(string[] args)
    {
      Console.Title = app_title;

      if (isHibernationEnabled())
      {
        doHibernation(args);
      }
      else
      {
        if (isAppElevated())
        {
          enableHibernation();

          doHibernation(args);
        }
        else
        {
          Console.WriteLine("Please run as administrator (one-time action).");
        }
      }
    }
  }
}