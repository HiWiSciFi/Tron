using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Linq;

public class AppendFilesAfterBuild
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject)
    {

        string path = pathToBuildProject.TrimEnd(new char[] { 'e', 'x', 'e', '.', 'n', 'o', 'r', 'T' });
        string AssetsPath = "Assets/BuildResources/";

        string[] fs = Directory.GetFiles(AssetsPath);
        string[] fte = Directory.GetFiles(AssetsPath, "*.meta");
        string[] files = fs.Except(fte).ToArray();


        string destinationPath = path + "/" + AssetsPath;

        Directory.CreateDirectory(destinationPath);
        for(int i = 0; i < files.Length; i++)
        {
            File.Copy(files[i], path + files[i]);
        }
    }
}