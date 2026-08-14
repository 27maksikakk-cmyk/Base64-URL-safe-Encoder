// base64_url.js — JavaScript версия

const fs = require('fs');
const readline = require('readline');
const clipboardy = require('clipboardy');

function encodeString(data) {
    return Buffer.from(data, 'utf8').toString('base64url');
}

function encodeFile(filename) {
    const data = fs.readFileSync(filename);
    return data.toString('base64url');
}

function decodeString(data) {
    return Buffer.from(data, 'base64url').toString('utf8');
}

function decodeFile(filename) {
    const data = fs.readFileSync(filename, 'utf8');
    return Buffer.from(data, 'base64url').toString('utf8');
}

async function main() {
    const args = process.argv.slice(2);
    let input = '';
    let filePath = '';
    let decode = false;
    let output = '';
    let noCopy = false;

    for (let i = 0; i < args.length; i++) {
        if (args[i] === '--input') input = args[++i];
        else if (args[i] === '--file') filePath = args[++i];
        else if (args[i] === '--decode') decode = true;
        else if (args[i] === '--output') output = args[++i];
        else if (args[i] === '--no-copy') noCopy = true;
        else if (!args[i].startsWith('-')) {
            if (!input && !filePath) input = args[i];
        }
    }

    console.log('\x1b[36m🔐 Base64 URL-safe Encoder (JavaScript)\x1b[0m');

    let result = '';

    if (filePath) {
        console.log(`📂 Обработка файла: ${filePath}`);
        if (decode) {
            result = decodeFile(filePath);
        } else {
            result = encodeFile(filePath);
        }
    } else if (input) {
        console.log(`📝 Входные данные: ${input}`);
        if (decode) {
            result = decodeString(input);
        } else {
            result = encodeString(input);
        }
    } else {
        console.log('📝 Чтение из STDIN (Ctrl+D для окончания)');
        const rl = readline.createInterface({
            input: process.stdin,
            output: process.stdout,
            terminal: false
        });
        let chunks = [];
        for await (const line of rl) {
            chunks.push(line);
        }
        const data = chunks.join('\n');
        if (!data) {
            console.log('\x1b[33m⚠️ Пустой ввод.\x1b[0m');
            process.exit(1);
        }
        if (decode) {
            result = decodeString(data);
        } else {
            result = encodeString(data);
        }
    }

    console.log(`\x1b[32mРезультат:\x1b[0m`);
    console.log(result);
    console.log();

    if (!noCopy) {
        try {
            await clipboardy.write(result);
            console.log('\x1b[32m✅ Результат скопирован в буфер обмена!\x1b[0m');
        } catch (err) {
            console.log(`\x1b[33m⚠️ Не удалось скопировать: ${err.message}\x1b[0m`);
        }
    }

    if (output) {
        fs.writeFileSync(output, result);
        console.log(`\x1b[32m💾 Сохранено в ${output}\x1b[0m`);
    }
}

main().catch(console.error);
