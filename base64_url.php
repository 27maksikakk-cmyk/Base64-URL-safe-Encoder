<?php
// base64_url.php — PHP версия

function copyToClipboard($text) {
    if (strtoupper(substr(PHP_OS, 0, 3)) === 'WIN') {
        $cmd = 'echo ' . escapeshellarg($text) . ' | clip';
        exec($cmd, $output, $return);
        return $return === 0;
    } elseif (PHP_OS === 'Darwin') {
        $cmd = 'echo ' . escapeshellarg($text) . ' | pbcopy';
        exec($cmd, $output, $return);
        return $return === 0;
    } else {
        $cmd = 'echo ' . escapeshellarg($text) . ' | xclip -selection clipboard';
        exec($cmd, $output, $return);
        if ($return === 0) return true;
        $cmd = 'echo ' . escapeshellarg($text) . ' | xsel -b';
        exec($cmd, $output, $return);
        return $return === 0;
    }
}

function urlSafeBase64Encode($data) {
    return rtrim(strtr(base64_encode($data), '+/', '-_'), '=');
}

function urlSafeBase64Decode($data) {
    $data = strtr($data, '-_', '+/');
    $padding = 4 - (strlen($data) % 4);
    if ($padding != 4) $data .= str_repeat('=', $padding);
    return base64_decode($data);
}

$input = '';
$filePath = '';
$decode = false;
$output = '';
$noCopy = false;

$args = array_slice($argv, 1);
for ($i = 0; $i < count($args); $i++) {
    if ($args[$i] == '--input') {
        $input = $args[++$i];
    } elseif ($args[$i] == '--file') {
        $filePath = $args[++$i];
    } elseif ($args[$i] == '--decode') {
        $decode = true;
    } elseif ($args[$i] == '--output') {
        $output = $args[++$i];
    } elseif ($args[$i] == '--no-copy') {
        $noCopy = true;
    } elseif (!str_starts_with($args[$i], '-')) {
        if (empty($input) && empty($filePath)) {
            $input = $args[$i];
        }
    }
}

echo "\033[36m🔐 Base64 URL-safe Encoder (PHP)\033[0m\n";

$result = '';

if (!empty($filePath)) {
    echo "📂 Обработка файла: $filePath\n";
    $content = file_get_contents($filePath);
    if ($content === false) {
        echo "\033[31m❌ Ошибка чтения файла.\033[0m\n";
        exit(1);
    }
    if ($decode) {
        $result = urlSafeBase64Decode($content);
    } else {
        $result = urlSafeBase64Encode($content);
    }
} elseif (!empty($input)) {
    echo "📝 Входные данные: $input\n";
    if ($decode) {
        $result = urlSafeBase64Decode($input);
    } else {
        $result = urlSafeBase64Encode($input);
    }
} else {
    echo "📝 Чтение из STDIN (Ctrl+D для окончания)\n";
    $data = file_get_contents('php://stdin');
    if ($data === false || $data === '') {
        echo "\033[33m⚠️ Пустой ввод.\033[0m\n";
        exit(1);
    }
    if ($decode) {
        $result = urlSafeBase64Decode($data);
    } else {
        $result = urlSafeBase64Encode($data);
    }
}

echo "\033[32mРезультат:\033[0m\n";
echo $result . "\n";
echo "\n";

if (!$noCopy) {
    if (copyToClipboard($result)) {
        echo "\033[32m✅ Результат скопирован в буфер обмена!\033[0m\n";
    } else {
        echo "\033[33m⚠️ Не удалось скопировать в буфер обмена.\033[0m\n";
    }
}

if (!empty($output)) {
    file_put_contents($output, $result);
    echo "\033[32m💾 Сохранено в $output\033[0m\n";
}
?>
