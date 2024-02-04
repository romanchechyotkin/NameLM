using System.Diagnostics;

class NameLM
{
    static void Main(string[] args)
    {
        string dataSet = "./names.txt";

        List<string> data = ReadDataSet(dataSet);
        data.ForEach(Console.WriteLine);
        
        data = ReadDataSet("1", "2", "3");
        data.ForEach(Console.WriteLine);
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

    static List<string> ReadDataSet(string set1, string set2, string set3)
    { 
        List<string> data = new List<string>();
        data.Add(set1);
        data.Add(set2); 
        data.Add(set3);
        return data;
    }

}