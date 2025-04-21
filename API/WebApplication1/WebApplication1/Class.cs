namespace WebApplication1
{
    public class Class
    {
        /*
        public static int Calculate(string source1, string source2)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Inicializace matice
            for (var i = 0; i <= source1Length; i++)
            {
                matrix[i, 0] = i;
            }
            for (var j = 0; j <= source2Length; j++)
            {
                matrix[0, j] = j;
            }

            // Výpočet vzdálenosti Levenshteinovy matice
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[source1Length, source2Length];
        }

        public static string FindBestMatchFile(string targetString, string directoryPath)
        {
            // Get all CSV files in the directory
            var csvFiles = Directory.GetFiles(directoryPath, "*.csv");
            string bestMatchFile = null;
            int lowestDistance = int.MaxValue;

            foreach (var file in csvFiles)
            {
                var lines = File.ReadAllLines(file);
                var firstLine = lines.FirstOrDefault();

                if (firstLine == null)
                    continue;

                var columns = firstLine.Split(',');

                foreach (var column in columns)
                {
                    int distance = Calculate(targetString, column);

                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        bestMatchFile = file;
                    }
                }
            }

            return bestMatchFile;
        }
        */
    }
}
