// base64_url.go — Go версия

package main

import (
	"bufio"
	"encoding/base64"
	"flag"
	"fmt"
	"io"
	"os"
	"os/exec"
	"runtime"
	"strings"
)

func copyToClipboard(text string) bool {
	var cmd *exec.Cmd
	switch runtime.GOOS {
	case "windows":
		cmd = exec.Command("clip")
	case "darwin":
		cmd = exec.Command("pbcopy")
	case "linux":
		if _, err := exec.LookPath("xclip"); err == nil {
			cmd = exec.Command("xclip", "-selection", "clipboard")
		} else if _, err := exec.LookPath("xsel"); err == nil {
			cmd = exec.Command("xsel", "-b")
		} else {
			return false
		}
	default:
		return false
	}
	stdin, err := cmd.StdinPipe()
	if err != nil {
		return false
	}
	if err := cmd.Start(); err != nil {
		return false
	}
	stdin.Write([]byte(text))
	stdin.Close()
	return cmd.Wait() == nil
}

func encodeString(data string) string {
	return base64.URLEncoding.EncodeToString([]byte(data))
}

func encodeFile(filename string) (string, error) {
	content, err := os.ReadFile(filename)
	if err != nil {
		return "", err
	}
	return base64.URLEncoding.EncodeToString(content), nil
}

func decodeString(data string) (string, error) {
	// URLEncoding автоматически игнорирует padding
	decoded, err := base64.URLEncoding.DecodeString(data)
	if err != nil {
		return "", err
	}
	return string(decoded), nil
}

func main() {
	var input string
	var filePath string
	var decode bool
	var output string
	var noCopy bool

	flag.StringVar(&input, "input", "", "Входная строка")
	flag.StringVar(&filePath, "file", "", "Файл для обработки")
	flag.BoolVar(&decode, "decode", false, "Декодировать")
	flag.StringVar(&output, "output", "", "Сохранить результат в файл")
	flag.BoolVar(&noCopy, "no-copy", false, "Не копировать в буфер обмена")
	flag.Parse()

	fmt.Println("\x1b[36m🔐 Base64 URL-safe Encoder (Go)\x1b[0m")

	var result string
	var err error

	if filePath != "" {
		fmt.Printf("📂 Обработка файла: %s\n", filePath)
		if decode {
			content, err := os.ReadFile(filePath)
			if err != nil {
				fmt.Printf("\x1b[31m❌ Ошибка: %v\x1b[0m\n", err)
				os.Exit(1)
			}
			result, err = decodeString(strings.TrimSpace(string(content)))
		} else {
			result, err = encodeFile(filePath)
		}
	} else if input != "" {
		fmt.Printf("📝 Входные данные: %s\n", input)
		if decode {
			result, err = decodeString(input)
		} else {
			result = encodeString(input)
		}
	} else {
		fmt.Println("📝 Чтение из STDIN (Ctrl+D для окончания)")
		reader := bufio.NewReader(os.Stdin)
		content, err := io.ReadAll(reader)
		if err != nil {
			fmt.Printf("\x1b[31m❌ Ошибка чтения: %v\x1b[0m\n", err)
			os.Exit(1)
		}
		if len(content) == 0 {
			fmt.Println("\x1b[33m⚠️ Пустой ввод.\x1b[0m")
			os.Exit(1)
		}
		if decode {
			result, err = decodeString(strings.TrimSpace(string(content)))
		} else {
			result = encodeString(string(content))
		}
	}

	if err != nil {
		fmt.Printf("\x1b[31m❌ Ошибка: %v\x1b[0m\n", err)
		os.Exit(1)
	}

	fmt.Printf("\x1b[32mРезультат:\x1b[0m\n")
	fmt.Println(result)
	fmt.Println()

	if !noCopy {
		if copyToClipboard(result) {
			fmt.Println("\x1b[32m✅ Результат скопирован в буфер обмена!\x1b[0m")
		} else {
			fmt.Println("\x1b[33m⚠️ Не удалось скопировать в буфер обмена.\x1b[0m")
		}
	}

	if output != "" {
		err = os.WriteFile(output, []byte(result), 0644)
		if err != nil {
			fmt.Printf("\x1b[31m❌ Ошибка сохранения: %v\x1b[0m\n", err)
		} else {
			fmt.Printf("\x1b[32m💾 Сохранено в %s\x1b[0m\n", output)
		}
	}
}
