using System.IO;

public static class PlayerSettings
{
    public static float mouseSpeed = 2.0f;
    private const string mouseSpeedIdentifier = "mouseSpeed";

    private const char seperator = ':';
    private const string settingsFile = "Assets/BuildResources/settings.txt";

    // settings file content:
    // mouseSpeed:<speed>

    /// <summary>
    /// Save the settings to a file
    /// </summary>
    public static void Save()
    {
        if (!File.Exists(settingsFile))
            File.Create(settingsFile);

        TextReader tr = new StreamReader(settingsFile);
        int lineAmount = TotalLines(settingsFile);
        
        for (int i = 0; i < lineAmount; i++)
        {
            string line = tr.ReadLine();
            if (line.StartsWith(mouseSpeedIdentifier + seperator))
            {
                string value = line.Split(':')[1];
                try
                {
                    float f = float.Parse(value);
                    mouseSpeed = f;
                } catch { }
            }
        }

        tr.Close();
        tr.Dispose();
    }

    /// <summary>
    /// Load the settings from a file
    /// </summary>
    public static void Load()
    {
        if (File.Exists(settingsFile))
        {
            TextWriter tr = new StreamWriter(settingsFile);

            tr.WriteLine(mouseSpeedIdentifier + seperator + mouseSpeed);

            tr.Close();
            tr.Dispose();
        }
    }

    /// <summary>
    /// get the total amount of lines of a file
    /// </summary>
    /// <param name="filePath">Path to the file to get the lines from</param>
    /// <returns>the amount of lines from the given file</returns>
    private static int TotalLines(string filePath)
    {
        using (StreamReader r = new StreamReader(filePath))
        {
            int i = 0;
            while (r.ReadLine() != null) { i++; }
            return i;
        }
    }
}