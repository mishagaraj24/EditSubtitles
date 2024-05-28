
using System.Runtime.Serialization;
using System.Text;

namespace EditSubtitles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string number;

            do
            {
                Console.Clear();

                Console.WriteLine("Enter path");
                string dirPath = string.Empty;
                string dirPaths = Console.ReadLine();
                foreach (var item in Directory.GetDirectories(dirPaths))
                {
                    dirPath = item;
                    var files = Directory.GetFiles(dirPath, "*.srt");
                    string backUpSub = dirPath + "\\BackUpSubtitles";
                    Directory.CreateDirectory(backUpSub);
                    foreach (var filePath in files)
                    {
                        string fileName = Path.GetFileName(filePath);
                        string fileP = Path.GetFullPath(backUpSub + $"\\{fileName}");
                        File.SetAttributes(filePath, FileAttributes.Normal);
                        File.Copy(filePath, backUpSub + $"\\{fileName}");
                        File.Delete(filePath);
                        Edit(fileName, dirPath, fileP);

                    }
                }
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Eсли хотите закончить введите 99 если нет тогда enter");
                number = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            } while (number != "99");
        }

        private static void Edit(string fileName, string dirPath, string filePath)
        {
            bool dot = true;
            bool canBeSaved = false;
            var subtitle = new Sub();
            string temp = String.Empty;
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    try
                    {
                        if (line != null)
                        {
                            if (canBeSaved)
                            {
                                using (FileStream writer = new FileStream($"{dirPath}\\{fileName}", FileMode.Append, FileAccess.Write))
                                {
                                    try
                                    {
                                        string result = $"{subtitle.Start} --> {subtitle.End} \n {subtitle.Sentence} \n\n";
                                        byte[] buffer = Encoding.Default.GetBytes(result);
                                        writer.Write(buffer, 0, buffer.Length);
                                        Console.WriteLine(result);
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("Объект успешно записан в файл.");
                                        Console.ForegroundColor = ConsoleColor.White;
                                        canBeSaved = false;
                                        subtitle = new Sub();
                                        subtitle.Sentence = temp + " ";
                                        if (reader.Peek() == -1 && temp != string.Empty || string.IsNullOrWhiteSpace(temp))
                                        {
                                            buffer = Encoding.Default.GetBytes(temp);
                                            writer.Write(buffer, 0, buffer.Length);
                                        }
                                    }
                                    catch (SerializationException ex)
                                    {
                                        Console.WriteLine("Произошла ошибка при сериализации объекта: " + ex.Message);
                                    }
                                }

                            }
                            else
                            {
                                temp = characterIdentification(line, subtitle, ref canBeSaved, ref dot);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }

        public static string characterIdentification(string line, Sub subtitle, ref bool canBeSaved, ref bool dot)
        {
            //2
            //00:00:05,430 --> 00:00:09,410
            //in Visual Studio, I will also show you that based on the given data type, a

            //3
            //00:00:09,410 --> 00:00:15,810
            //default value will be assigned by C#. So let's see how we can use the

            //4
            //00:00:15,810 --> 00:00:21,050
            //arithmetic operators in C#. I'm going to bring in a couple of extra variables
            string separatorString = "-->";
            char[] separator = separatorString.ToCharArray();


            if (int.TryParse(line, out int value))
            {
                if (value >= 0)
                {

                }
            }
            else if (line == "" || line == " ")
            {

            }
            else if (char.IsDigit(line[0]) && line[2] == ':' && dot)
            {
                string[] t = line.Split(separator);
                subtitle.Start = t[0];
                subtitle.End = t[3];

            }
            else if (char.IsDigit(line[0]) && line[2] == ':' && !dot)
            {
                string[] t = line.Split(separator);

                subtitle.End = t[3];

            }
            else
            {
                int index = line.IndexOf('.');
                if (index == line[line.Length - 1])
                {
                    subtitle.Sentence += line;
                    dot = true;
                    canBeSaved = true;

                }
                else if (index != -1)
                {
                    string BeforDot = line.Substring(0, index);

                    subtitle.Sentence += " " + BeforDot + ".";
                    dot = true;
                    canBeSaved = true;
                    return line.Substring(index + 1);
                }

                else
                {
                    subtitle.Sentence += line;
                    dot = false;
                }

            }
            return string.Empty;
        }
    }
}