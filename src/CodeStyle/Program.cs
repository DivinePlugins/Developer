using System.Text;

Console.WriteLine("Start");

//namespace VisibleByEnemyPlus;

//using Divine.Entity;

//public class VisibleByEnemyPlus : Bootstrapper
//{

// вы получитк примерно вот такой стиль

var directory = @"C:\Users\RoccoZero\Documents\GitHub\Divine.Plugins\VisibleByEnemyPlus"; // вот тут пишим путь к проекту

foreach (var file in Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories))
{
    var partDirectory = file[(directory.Length + 1)..];
    if (partDirectory.StartsWith(@"bin\") ||
        partDirectory.StartsWith(@"obj\") ||
        partDirectory.StartsWith(@".git\"))
    {
        continue;
    }

    var lines = File.ReadAllLines(file);

    var sb = new StringBuilder();

    var cancel = false;
    var @namespace = string.Empty;

    for (var i = 0; i < lines.Length; i++)
    {
        var line = lines[i];
        if (line.StartsWith("namespace"))
        {
            if (i == 0 && line.Last() == ';')
            {
                cancel = true;
                break;
            }

            @namespace = $"{line};";
            continue;
        }

        if (line.Length == 0)
        {
            sb.AppendLine(line);
            continue;
        }

        if (line.StartsWith("//") || line.StartsWith("/*"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + partDirectory);
            Console.ResetColor();
            cancel = true;
            break;
        }

        if (line[0] == '{' || line[0] == '}')
        {
            continue;
        }

        if (line[0] == ' ')
        {
            sb.AppendLine(line[4..]);
        }
        else
        {
            sb.AppendLine(line);
        }
    }

    if (cancel)
    {
        continue;
    }

    sb.Insert(0, "\r\n\r\n");
    sb.Insert(0, @namespace);
    sb.Remove(sb.Length - 2, 2);

    File.WriteAllText(file, sb.ToString(), new UTF8Encoding(true));
    Console.WriteLine(partDirectory);
}

Console.WriteLine("End");
Console.ReadLine();