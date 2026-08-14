# base64_url.rb — Ruby версия

require 'base64'
require 'clipboard'
require 'optparse'

options = {}
OptionParser.new do |opts|
  opts.banner = "Usage: ruby base64_url.rb [options]"
  opts.on("--input STRING", "Входная строка") { |v| options[:input] = v }
  opts.on("--file FILE", "Файл для обработки") { |v| options[:file] = v }
  opts.on("--decode", "Декодировать") { |v| options[:decode] = true }
  opts.on("--output FILE", "Сохранить результат в файл") { |v| options[:output] = v }
  opts.on("--no-copy", "Не копировать в буфер обмена") { |v| options[:no_copy] = true }
end.parse!

puts "\e[36m🔐 Base64 URL-safe Encoder (Ruby)\e[0m"

result = ""

if options[:file]
  puts "📂 Обработка файла: #{options[:file]}"
  content = File.read(options[:file], encoding: 'BINARY')
  if options[:decode]
    result = Base64.urlsafe_decode64(content).force_encoding('UTF-8')
  else
    result = Base64.urlsafe_encode64(content, padding: false)
  end
elsif options[:input]
  puts "📝 Входные данные: #{options[:input]}"
  if options[:decode]
    result = Base64.urlsafe_decode64(options[:input])
  else
    result = Base64.urlsafe_encode64(options[:input], padding: false)
  end
else
  puts "📝 Чтение из STDIN (Ctrl+D для окончания)"
  data = STDIN.read
  if data.empty?
    puts "\e[33m⚠️ Пустой ввод.\e[0m"
    exit 1
  end
  if options[:decode]
    result = Base64.urlsafe_decode64(data)
  else
    result = Base64.urlsafe_encode64(data, padding: false)
  end
end

puts "\e[32mРезультат:\e[0m"
puts result
puts

unless options[:no_copy]
  begin
    Clipboard.copy(result)
    puts "\e[32m✅ Результат скопирован в буфер обмена!\e[0m"
  rescue
    puts "\e[33m⚠️ Не удалось скопировать в буфер обмена.\e[0m"
  end
end

if options[:output]
  File.write(options[:output], result)
  puts "\e[32m💾 Сохранено в #{options[:output]}\e[0m"
end
