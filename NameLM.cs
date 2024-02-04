using System.Diagnostics;

class NameLM
{
    static void Main(string[] args)
    {
        string dataSet = "./names.txt";

        List<string> data = ReadDataSet(dataSet);
    }

    static List<string> ReadDataSet(string path)
    {
        string content = "";
        try
        {
            using StreamReader reader = new StreamReader(path);
            content = reader.ReadToEnd();
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"File not found: {path}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
        }

        List<string> names = new List<string>();
        foreach (string name in content.Split('\n'))
        {
            names.Add(name);
        }
        
        Debug.WriteLine($"data set length {names.Count()}");

        return names;
    }

}