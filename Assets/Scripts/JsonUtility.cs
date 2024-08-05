using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JsonUtility
{
    private readonly TextAsset modelAsset;
    private readonly TextAsset spaceAsset;

    public JsonUtility(TextAsset modelAsset, TextAsset spaceAsset)
    {
        this.modelAsset = modelAsset;
        this.spaceAsset = spaceAsset;
    }

    public CeramicTest LoadTest() => new()
    {
        Models = JsonConvert.DeserializeObject<Matrix4x4[]>(modelAsset.text),
        Spaces = JsonConvert.DeserializeObject<Matrix4x4[]>(spaceAsset.text)
    };

    public void SaveSolutions(IEnumerable<Matrix4x4> solutions)
    {
        var solutionsJson = JsonConvert.SerializeObject(solutions, new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            ContractResolver = new WhiteListPropertiesResolver(GetMatrixPropNames())
        });
        var path = Path.Combine(Application.dataPath, DateTime.Now.ToString("dd-MM-yy hh-mm-ss") + ".json");
        File.WriteAllText(path, solutionsJson);
        Debug.Log($"Exported: {path}");
    }

    private static IEnumerable<string> GetMatrixPropNames()
    {
        for (var x = 0; x < 4; x++)
        for (var y = 0; y < 4; y++)
            yield return $"m{x}{y}";
    }
}