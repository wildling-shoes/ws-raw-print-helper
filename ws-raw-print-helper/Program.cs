using System.Reflection;
using System.Resources;
using RawPrint;
using Microsoft.Win32;
using ws_raw_print_helper;

class AutoRawPrint
{
    const string TestLabel = @"{^XA
~TA000
~JSN
^LT0
^MNW
^MTD
^PON
^PMN
^LH0,0
^JMA
^PR8,8
~SD15
^JUS
^LRN
^CI27
^PA0,1,1,0
^XZ
^XA
^MMT
^PW871
^LL241
^LS0
^FO3,4^GB371,233,4^FS
^FO370,4^GB465,233,4^FS
^BY2,3,119^FT411,169^BCN,,N,N,N,A
^FH\^FD12345^FS
^FT23,137^A@N,38,38,TT0003M_^FH\^CI28^FDTest^FS^CI27
^FT413,195^A@N,25,25,TT0003M_^FH\^CI28^FD12345^FS^CI27
^PQ1,0,1,Y
^XZ
}
";
    
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (File.Exists(args[0]))
            {
                Console.WriteLine("Wildling RawPrint Helper: Printing ...");
                try
                {
                    PrintFile(args[0]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine($"Wildling RawPrint Helper: Error: {args[0]} seems to be no file!");
            }
        }
        Console.WriteLine("Wildling RawPrint Helper: Press t for a test label.");
        Console.WriteLine("Wildling RawPrint Helper: Press p to select a printer.");
        Console.WriteLine("Wildling RawPrint Helper: Press l to show license information.");
        ConsoleKeyInfo keyPressed = Console.ReadKey();
        if (keyPressed.Key.ToString().ToLower() == "t")
        {
            try
            {
                PrintTestlabel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        if (keyPressed.Key.ToString().ToLower() == "p")
        {
            SelectPrinter();
        }

        if (keyPressed.Key.ToString().ToLower() == "l")
        {
            var rm = Licenses.ResourceManager;
            string licenseText = rm.GetString("License");
            Console.WriteLine("");
            Console.WriteLine(licenseText);
            Console.WriteLine("\r\n... Continue ... ");
            Console.ReadKey();
            Console.WriteLine("\r\nThis binary includes RawPrint, which is distributed under the following license:\r\n");
            Console.WriteLine(rm.GetString("RawPrintLicense"));
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }

    static void PrintTestlabel()
    {
        string printerName = EnsurePrinter();
        if (printerName is null)
        {
            return;
        }
        IPrinter printer = new Printer();
        using (Stream stream = new MemoryStream())
        {
            var writer = new StreamWriter(stream);
            writer.Write(TestLabel);
            writer.Flush();
            stream.Position = 0;
            printer.PrintRawStream(printerName, stream, "test_label", false, 1);
        }
    }

    static void PrintFile(string fileName)
    {
        string printerName = EnsurePrinter();
        if (printerName is null)
        {
            return;
        }
        using FileStream fs = File.OpenRead(fileName);
        IPrinter printer = new Printer();
        printer.PrintRawStream(printerName, fs, "raw_label", false, 1);
    }

    static string? SelectPrinter()
    {
        Console.WriteLine();
        var printerNames = System.Drawing.Printing.PrinterSettings.InstalledPrinters;
        for (int i = 0; i < printerNames.Count; i++)
        {
            Console.WriteLine($"{i}: {printerNames[i]}");
        }
        Console.Write("Type the number, please: ");
        ConsoleKeyInfo keyPressed = Console.ReadKey();
        string value = keyPressed.KeyChar.ToString();
        if (int.TryParse(value, out var chosen) && chosen >= 0 && chosen <= printerNames.Count)
        {
            Console.WriteLine($"\nYour choice is {printerNames[chosen]}");
            SavePrinter(printerNames[chosen]);
            Console.WriteLine("Saved to registry.");
            return printerNames[chosen];
        }
        Console.WriteLine("I don't know that printer. Bye!");
        return null;
    }

    /// <summary>
    /// Get printer, let the user choose if no printer was saved.
    /// </summary>
    /// <returns>printer name or null if the user was not able to choose one.</returns>
    static string EnsurePrinter()
    {
        // Try to load the printer
        string? printer = GetPrinter();
        if (printer is null)
        {
            // Fall back to select
            printer = SelectPrinter();
            if (printer is null)
                throw new Exception("No printer!");
        }
        return printer!;
    }

    /// <summary>
    /// Save the printer name to the registry.
    /// </summary>
    /// <param name="name">Name of the printer</param>
    static void SavePrinter(string name)
    {
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\ws-raw-print-helper");
        key.SetValue("printer", name, RegistryValueKind.String);
        key.Close();
    }

    /// <summary>
    /// Get printer name from the registry.
    /// </summary>
    /// <returns>Printer name or null</returns>
    static string? GetPrinter()
    {
        RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\ws-raw-print-helper");
        string? printer = null;
        if (!(key is null))
        {
            printer = (string?)key.GetValue("printer");
            key.Close();
        }
        return printer;
    }
}