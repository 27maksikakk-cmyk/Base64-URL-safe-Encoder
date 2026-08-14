// base64_url.java — Java версия

import java.io.*;
import java.nio.file.*;
import java.util.Base64;
import java.awt.*;
import java.awt.datatransfer.*;

public class base64_url {
    public static void main(String[] args) throws Exception {
        String input = "";
        String filePath = "";
        boolean decode = false;
        String output = "";
        boolean noCopy = false;

        for (int i = 0; i < args.length; i++) {
            if (args[i].equals("--input")) {
                input = args[++i];
            } else if (args[i].equals("--file")) {
                filePath = args[++i];
            } else if (args[i].equals("--decode")) {
                decode = true;
            } else if (args[i].equals("--output")) {
                output = args[++i];
            } else if (args[i].equals("--no-copy")) {
                noCopy = true;
            } else if (!args[i].startsWith("-")) {
                if (input.isEmpty() && filePath.isEmpty()) {
                    input = args[i];
                }
            }
        }

        System.out.println("\u001B[36m🔐 Base64 URL-safe Encoder (Java)\u001B[0m");

        String result = "";

        if (!filePath.isEmpty()) {
            System.out.println("📂 Обработка файла: " + filePath);
            if (decode) {
                String content = new String(Files.readAllBytes(Paths.get(filePath)), "UTF-8");
                result = new String(Base64.getUrlDecoder().decode(content.trim()));
            } else {
                byte[] data = Files.readAllBytes(Paths.get(filePath));
                result = Base64.getUrlEncoder().withoutPadding().encodeToString(data);
            }
        } else if (!input.isEmpty()) {
            System.out.println("📝 Входные данные: " + input);
            if (decode) {
                result = new String(Base64.getUrlDecoder().decode(input));
            } else {
                result = Base64.getUrlEncoder().withoutPadding().encodeToString(input.getBytes("UTF-8"));
            }
        } else {
            System.out.println("📝 Чтение из STDIN (Ctrl+D для окончания)");
            StringBuilder sb = new StringBuilder();
            BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
            String line;
            while ((line = reader.readLine()) != null) {
                sb.append(line).append("\n");
            }
            String data = sb.toString();
            if (data.isEmpty()) {
                System.out.println("\u001B[33m⚠️ Пустой ввод.\u001B[0m");
                System.exit(1);
            }
            if (decode) {
                result = new String(Base64.getUrlDecoder().decode(data.trim()));
            } else {
                result = Base64.getUrlEncoder().withoutPadding().encodeToString(data.getBytes("UTF-8"));
            }
        }

        System.out.println("\u001B[32mРезультат:\u001B[0m");
        System.out.println(result);
        System.out.println();

        if (!noCopy) {
            if (copyToClipboard(result)) {
                System.out.println("\u001B[32m✅ Результат скопирован в буфер обмена!\u001B[0m");
            } else {
                System.out.println("\u001B[33m⚠️ Не удалось скопировать в буфер обмена.\u001B[0m");
            }
        }

        if (!output.isEmpty()) {
            Files.write(Paths.get(output), result.getBytes("UTF-8"));
            System.out.println("\u001B[32m💾 Сохранено в " + output + "\u001B[0m");
        }
    }

    private static boolean copyToClipboard(String text) {
        try {
            StringSelection selection = new StringSelection(text);
            Toolkit.getDefaultToolkit().getSystemClipboard().setContents(selection, null);
            return true;
        } catch (Exception e) {
            return false;
        }
    }
}
