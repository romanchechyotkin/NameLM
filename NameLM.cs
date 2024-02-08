using System.Diagnostics;

class NameLM
{
    static void Main(string[] args)
    {
        string dataSet = "./names.txt";

        List<string> data = ReadDataSet(dataSet);
        
        Dictionary<Tuple<string, string>, int> bigrams = GetBigrams(data);

        Dictionary<string, int> stoi = GetStoi(data);

        Dictionary<int, string> itos = GetItos(stoi);

        float[,] matrix = BuildMatrix(data, stoi, itos);
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

        foreach (var bigram in bigrams)
        {
            Debug.WriteLine($"{bigram.Key}, {bigram.Value}");
        }

        Debug.WriteLine($"bigrams all amount {bigrams.Count}");

        var maxBigram = bigrams.Aggregate((x, y) => x.Value > y.Value ? x : y);
        Debug.WriteLine($"max amout of bigram {maxBigram.Key} {maxBigram.Value}");

        return bigrams;
    }

    static Dictionary<string, int> GetStoi(List<string> data)
    {
        string concatenatedString = string.Join("", data.Select(d => d.Trim()));
        char[] characters = concatenatedString.ToCharArray();

        List<char> sortedList = characters.ToList();
        sortedList.Sort();

        IEnumerable<char> uniqueCharacters = sortedList.Distinct();
        HashSet<char> characterSet = new HashSet<char>(uniqueCharacters);


        Dictionary<string, int> stoi = new Dictionary<string, int>();

        int index = 0;
        stoi.Add('.'.ToString(), index);

        foreach (var character in characterSet)
        {
            {
                stoi.Add(character.ToString(), index + 1);
                index++;    
            }
        }

        DebugDictionary("stoi", stoi);

        return stoi;
    }

    static Dictionary<int, string> GetItos(Dictionary<string, int> stoi)
    {
        Dictionary<int, string> itos = new Dictionary<int, string>();

        foreach (var kvp in stoi)
        {
            itos.Add(kvp.Value, kvp.Key);
        }

        DebugDictionary("itos", itos);

        return itos;
    }
        
    static float[,] BuildMatrix(List<string> data, Dictionary<string, int> stoi, Dictionary<int, string> itos)
    {
        float[,] matrix = new float[27, 27];

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
                int idx1 = stoi[bigram.Ch1];
                int idx2 = stoi[bigram.Ch2];
                matrix[idx1, idx2] += 1;
            }
        }

        DebugMatrix(matrix);

        return matrix;
    }

    static void DebugDictionary<TKey, TValue>(string name, Dictionary<TKey, TValue> dictionary)
    {
        Debug.WriteLine($"{name}");
        foreach (var kvp in dictionary)
        {
            Debug.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }
    }

    static void DebugMatrix(float[,] matrix)
    {

        for (int row = 0; row < 27; row++)
        {
            for (int col = 0; col < 27; col++)
            {
                Console.Write($"{matrix[row, col]} ");
            }
            Console.WriteLine();
        }

    }

}