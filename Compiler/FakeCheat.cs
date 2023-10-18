using System;
using System.Threading;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // No editar esto
        foreach (Process process in Process.GetProcessesByName("taskmgr"))
        {
            process.Kill();
        }
        Console.WriteLine("Practice Autoclick");
        Console.WriteLine("20 cps | Esto es un autoclick no funcional");
        Console.WriteLine("hecho para probar ss");
        Thread.Sleep(1500);
    }
}
