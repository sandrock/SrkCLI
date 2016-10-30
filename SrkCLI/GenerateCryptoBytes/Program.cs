
namespace GenerateCryptoBytes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            var cout = Console.Out;
            var eout = Console.Error;

            if (args.Length != 2)
            {
                ShowUsage(cout);
                Environment.ExitCode = 1;
                return;
            }

            OutputType type;
            if (!Enum.TryParse<OutputType>(args[0], true, out type))
            {
                ShowError(eout, "Invalid output type.");
                ShowUsage(cout);
                Environment.ExitCode = 2;
                return;
            }

            int size;
            if (!int.TryParse(args[1], out size) || size < 1)
            {
                ShowError(eout, "Invalid size.");
                ShowUsage(cout);
                Environment.ExitCode = 3;
                return;
            }

            var data = new byte[size];
            using (var provider = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                provider.GetBytes(data);
            }

            if (type == OutputType.Binary)
            {
                var file = Path.GetTempFileName();
                try
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Write, FileShare.Read))
                    {
                        stream.Write(data, 0, size);
                    }

                    cout.WriteLine("Wrote binary data to:");
                    cout.WriteLine(file);
                }
                catch (Exception ex)
                {
                    ShowError(eout, ex.Message);
                    Environment.ExitCode = 1001;
                }
            }
            else if (type == OutputType.Hexadecimal)
            {
                var content = BitConverter.ToString(data).Replace("-", "");
                cout.WriteLine(content);
            }
            else if (type == OutputType.Base64)
            {
                var content = Convert.ToBase64String(data, Base64FormattingOptions.None);
                cout.WriteLine(content);
            }
            else
            {
                ShowError(eout, "Not supported format " + type + ".");
                Environment.ExitCode = 4;
            }
        }

        private static void ShowError(TextWriter eout, string message)
        {
            eout.WriteLine(message);
        }

        private static void ShowUsage(TextWriter cout)
        {
            cout.WriteLine("GenerateCryptoBytes");
            cout.WriteLine("Generates crypto random bytes using RNGCryptoServiceProvider.");
            cout.WriteLine("Usage: GenerateCryptoBytes <format> <size>");
            cout.WriteLine("    format:     binary, hexadecimal, base64");
            cout.WriteLine("    size:       the number of desired bytes");
            cout.WriteLine();
        }
    }

    public enum OutputType
    {
        Binary,
        Hexadecimal,
        Base64,
    }
}
