using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SolverUI : MonoBehaviour
{
    private MatrixVisualizer matrixVisualizer;
    private JsonUtility jsonUtility;

    private Stage stage = Stage.NotLoaded;
    private CeramicTest test;
    private IEnumerable<Matrix4x4> solutions;

    [Inject]
    public void Construct(MatrixVisualizer matrixVisualizer, JsonUtility jsonUtility)
    {
        Debug.Log(" Construct ");
        this.matrixVisualizer = matrixVisualizer;
        this.jsonUtility = jsonUtility;
    }

    private void OnGUI()
    {
        switch (stage)
        {
            case Stage.NotLoaded:
                if (GUILayout.Button("Load"))
                {
                    test = jsonUtility.LoadTest();
                    matrixVisualizer.Spawn(test.Spaces, 0);
                    stage = Stage.Loaded;
                }

                break;
            case Stage.Loaded:
                if (GUILayout.Button("Solve"))
                {
                    solutions = Solver.Solve(test);
                    stage = Stage.Solved;
                }

                break;
            case Stage.Solved:
                ShowSolutionsGUI();
                if (GUILayout.Button("Export"))
                {
                    jsonUtility.SaveSolutions(solutions);
                    stage = Stage.Exported;
                }

                break;
            case Stage.Exported:
                ShowSolutionsGUI();
                break;
        }
    }

    private void ShowSolutionsGUI()
    {
        var index = 1;
        foreach (var solution in solutions)
        {
            if (GUILayout.Button(index.ToString()))
                ShowSolution(solution);
            index++;
        }
    }

    private void ShowSolution(Matrix4x4 solution)
    {
        matrixVisualizer.Clear(1);
        matrixVisualizer.Spawn(test.Models.Select(m => solution * m), 1);
    }
}

enum Stage
{
    NotLoaded,
    Loaded,
    Solved,
    Exported
}