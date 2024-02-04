using System.Diagnostics;
using System.Linq;

class NameLM
{
    static void Main(string[] args)
    {
        string dataSet = "./names.txt";

        List<string> data = ReadDataSet(dataSet);
        
        Dictionary<Tuple<string, string>, int> bigrams = GetBigrams(data);
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

    static Dictionary<Tuple<string, string>, int> GetBigrams(List<string> data)
    {
        Dictionary<Tuple<string, string>, int> bigrams = new Dictionary<Tuple<string, string>, int>();

        for (int i = 0; i < data.Count; i++)
        {
            data[i] = data[i].Trim();

            string[] chars = { "." };
            string[] words_seq = data[i].Select(c => c.ToString()).ToArray();
            chars = chars.Concat(words_seq).ToArray();
            chars = chars.Concat(new string[] { "." }).ToArray();

            var zipped = chars.Zip(chars.Skip(1), (ch1, ch2) => new { Ch1 = ch1, Ch2 = ch2 });

            foreach (var bigram in zipped)
            {
                bigrams[Tuple.Create(bigram.Ch1, bigram.Ch2)] = bigrams.GetValueOrDefault(Tuple.Create(bigram.Ch1, bigram.Ch2), 0) + 1;
            }
        }

        Debug.WriteLine($"bigrams all amount {bigrams.Count}");

        var maxBigram = bigrams.Aggregate((x, y) => x.Value > y.Value ? x : y);
        Debug.WriteLine($"max amout of bigram {maxBigram.Key} {maxBigram.Value}");

        return bigrams;
    }

}