

### 1. `base64_url.py` (Python)

```python
# base64_url.py — Python версия

import sys
import base64
import argparse
import os
import subprocess
import platform
from pathlib import Path

try:
    import pyperclip
    HAS_CLIPBOARD = True
except ImportError:
    HAS_CLIPBOARD = False

def copy_to_clipboard(text):
    if HAS_CLIPBOARD:
        pyperclip.copy(text)
        return True
    system = platform.system()
    if system == "Windows":
        subprocess.run(["clip"], input=text.encode("utf-8"), check=True, capture_output=True)
        return True
    elif system == "Darwin":
        subprocess.run(["pbcopy"], input=text.encode("utf-8"), check=True)
        return True
    elif system == "Linux":
        try:
            subprocess.run(["xclip", "-selection", "clipboard"], input=text.encode("utf-8"), check=True)
            return True
        except FileNotFoundError:
            try:
                subprocess.run(["xsel", "-b"], input=text.encode("utf-8"), check=True)
                return True
            except FileNotFoundError:
                pass
    return False

def encode_string(data):
    return base64.urlsafe_b64encode(data.encode("utf-8")).decode("ascii").rstrip("=")

def encode_file(filename):
    with open(filename, "rb") as f:
        return base64.urlsafe_b64encode(f.read()).decode("ascii").rstrip("=")

def decode_string(data):
    # Добавляем padding, если нужно
    padding = 4 - (len(data) % 4)
    if padding != 4:
        data += "=" * padding
    return base64.urlsafe_b64decode(data).decode("utf-8")

def decode_file(filename):
    with open(filename, "r") as f:
        content = f.read().strip()
    return decode_string(content)

def main():
    parser = argparse.ArgumentParser(description="Base64 URL-safe Encoder")
    parser.add_argument("input", nargs="?", help="Входная строка или путь к файлу")
    parser.add_argument("--file", "-f", help="Файл для кодирования/декодирования")
    parser.add_argument("--decode", "-d", action="store_true", help="Декодировать")
    parser.add_argument("--output", "-o", help="Сохранить результат в файл")
    parser.add_argument("--no-copy", "-n", action="store_true", help="Не копировать в буфер обмена")
    args = parser.parse_args()

    print("\033[36m🔐 Base64 URL-safe Encoder (Python)\033[0m")

    result = None
    if args.file:
        print(f"📂 Обработка файла: {args.file}")
        if args.decode:
            result = decode_file(args.file)
        else:
            result = encode_file(args.file)
    elif args.input:
        print(f"📝 Входные данные: {args.input}")
        if args.decode:
            result = decode_string(args.input)
        else:
            result = encode_string(args.input)
    else:
        print("📝 Чтение из STDIN (Ctrl+D для окончания)")
        content = sys.stdin.read()
        if not content:
            print("⚠️ Пустой ввод.")
            sys.exit(1)
        if args.decode:
            result = decode_string(content)
        else:
            result = encode_string(content)

    if result is None:
        sys.exit(1)

    print(f"\033[32mРезультат:\033[0m")
    print(result)
    print()

    if not args.no_copy:
        if copy_to_clipboard(result):
            print("\033[32m✅ Результат скопирован в буфер обмена!\033[0m")
        else:
            print("\033[33m⚠️ Не удалось скопировать в буфер обмена.\033[0m")

    if args.output:
        with open(args.output, "w", encoding="utf-8") as f:
            f.write(result)
        print(f"\033[32m💾 Сохранено в {args.output}\033[0m")

if __name__ == "__main__":
    main()
