// base64_url.cs — C# версия

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

class Base64UrlEncoder {
    static void Main(string[] args) {
        string input = "";
        string filePath = "";
        bool decode = false;
        string output = "";
        bool noCopy = false;

        for (int i = 0; i < args.Length; i++) {
            if (args[i] == "--input") {
                input = args[++i];
            } else if (args[i] == "--file") {
                filePath = args[++i];
            } else if (args[i] == "--decode") {
                decode = true;
            } else if (args[i] == "--output") {
                output = args[++i];
            } else if (args[i] == "--no-copy") {
                noCopy = true;
            } else if (!args[i].StartsWith("-")) {
                if (string.IsNullOrEmpty(input) && string.IsNullOrEmpty(filePath)) {
                    input = args[i];
                }
            }
        }

        Console.WriteLine("\u001B[36m🔐 Base64 URL-safe Encoder (C#)\u001B[0m");

        string result = "";

        if (!string.IsNullOrEmpty(filePath)) {
            Console.WriteLine($"📂 Обработка файла: {filePath}");
            if (decode) {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                result = Encoding.UTF8.GetString(Convert.FromBase64String(content.Trim()));
                // URL-safe: стандартный Convert.FromBase64String принимает и URL-safe, так как '-' и '_' не поддерживаются, нужно заменить
                // В C# нет встроенного URL-safe, поэтому заменяем вручную
                string fixedContent = content.Replace('-', '+').Replace('_', '/');
                // Добавляем padding
                int padding = 4 - (fixedContent.Length % 4);
                if (padding != 4) fixedContent += new string('=', padding);
                result = Encoding.UTF8.GetString(Convert.FromBase64String(fixedContent));
            } else {
                byte[] data = File.ReadAllBytes(filePath);
                string base64 = Convert.ToBase64String(data);
                // URL-safe: заменяем + на -, / на _, убираем =
                result = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            }
        } else if (!string.IsNullOrEmpty(input)) {
            Console.WriteLine($"📝 Входные данные: {input}");
            if (decode) {
                string fixedInput = input.Replace('-', '+').Replace('_', '/');
                int padding = 4 - (fixedInput.Length % 4);
                if (padding != 4) fixedInput += new string('=', padding);
                result = Encoding.UTF8.GetString(Convert.FromBase64String(fixedInput));
            } else {
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
                result = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            }
        } else {
            Console.WriteLine("📝 Чтение из STDIN (Ctrl+D для окончания)");
            string line;
            var sb = new StringBuilder();
            while ((line = Console.ReadLine()) != null) {
                sb.AppendLine(line);
            }
            string data = sb.ToString();
            if (string.IsNullOrEmpty(data)) {
                Console.WriteLine("\u001B[33m⚠️ Пустой ввод.\u001B[0m");
                Environment.Exit(1);
            }
            if (decode) {
                string fixedData = data.Trim().Replace('-', '+').Replace('_', '/');
                int padding = 4 - (fixedData.Length % 4);
                if (padding != 4) fixedData += new string('=', padding);
                result = Encoding.UTF8.GetString(Convert.FromBase64String(fixedData));
            } else {
                string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
                result = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            }
        }

        Console.WriteLine($"\u001B[32mРезультат:\u001B[0m");
        Console.WriteLine(result);
        Console.WriteLine();

        if (!noCopy) {
            if (CopyToClipboard(result)) {
                Console.WriteLine("\u001B[32m✅ Результат скопирован в буфер обмена!\u001B[0m");
            } else {
                Console.WriteLine("\u001B[33m⚠️ Не удалось скопировать в буфер обмена.\u001B[0m");
            }
        }

        if (!string.IsNullOrEmpty(output)) {
            File.WriteAllText(output, result, Encoding.UTF8);
            Console.WriteLine($"\u001B[32m💾 Сохранено в {output}\u001B[0m");
        }
    }

    private static bool CopyToClipboard(string text) {
        try {
            Clipboard.SetText(text);
            return true;
        } catch {
            return false;
        }
    }
}
