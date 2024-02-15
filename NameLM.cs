using System.Diagnostics;

public interface LanguageModel
{
    Dictionary<int, string> GetItos(Dictionary<string, int> stoi);
    void DebugMatrix(float[,] matrix);
    void DebugDictionary<TKey, TValue>(string name, Dictionary<TKey, TValue> dictionary);
}

class NameLM: LanguageModel
{
    public List<string> ReadDataSet(string path)
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

    public Dictionary<Tuple<string, string>, int> GetBigrams(List<string> data)
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

    public Dictionary<string, int> GetStoi(List<string> data)
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

    public Dictionary<int, string> GetItos(Dictionary<string, int> stoi)
    {
        Dictionary<int, string> itos = new Dictionary<int, string>();

        foreach (var kvp in stoi)
        {
            itos.Add(kvp.Value, kvp.Key);
        }

        DebugDictionary("itos", itos);

        return itos;
    }

    public float[,] BuildMatrix(List<string> data, Dictionary<string, int> stoi, Dictionary<int, string> itos)
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

    public void DebugDictionary<TKey, TValue>(string name, Dictionary<TKey, TValue> dictionary)
    {
        Debug.WriteLine($"{name}");
        foreach (var kvp in dictionary)
        {
            Debug.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }
    }

    public void DebugMatrix(float[,] matrix)
    {

        for (int row = 0; row < 27; row++)
        {
            for (int col = 0; col < 27; col++)
            {
                Console.Write($"{matrix[row, col]}\t");
            }
            Console.WriteLine();
        }

    }

    public int CalculateTotalSum(float[] matrix)
    {
        int sum = 0;
        foreach (int element in matrix)
        {
            sum += element;
        }
        return sum;
    }
    public float[] GetRow(float[,] matrix, int rowIndex)
    {
        int cols = matrix.GetLength(1);
        float[] row = new float[cols];
        for (int j = 0; j < cols; j++)
        {
            row[j] = matrix[rowIndex, j];
        }
        return row;
    }
}

class Program
{
    static void Main(string[] args)
    {

        NameLM lm = new NameLM();

        string dataSet = "./names.txt";

        List<string> data = lm.ReadDataSet(dataSet);
        
        Dictionary<Tuple<string, string>, int> bigrams = lm.GetBigrams(data);

        Dictionary<string, int> stoi = lm.GetStoi(data);

        Dictionary<int, string> itos = lm.GetItos(stoi);

        float[,] matrix = lm.BuildMatrix(data, stoi, itos);

        Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");

        for (int i = 0; i < 27; i++)
        {
            int totalSum = lm.CalculateTotalSum(lm.GetRow(matrix, i));
            for (int j = 0; j < 27; j++)
            {
                matrix[i, j] = (float)Math.Round(matrix[i, j] / totalSum, 4);
            }
        }

        lm.DebugMatrix(matrix);
    }

}