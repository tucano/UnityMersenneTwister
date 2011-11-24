using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Text;
using System.IO;

public class MersenneDebugWindow : EditorWindow {

	public enum MersenneWindowOptionsType
	{
		INT = 0,
		FLOAT = 1,
		DOUBLE = 2
	}

	private MersenneTwister mrand;
	private int samplig_size = 500;
	private float temperature = 5.0f;
	private ArrayList randomList;
	private int seed = 0;
	private MersenneWindowOptionsType op = MersenneWindowOptionsType.FLOAT;
	private Boolean normalizeToggle = false;
	private String filename = "sampling.txt";
	private String path;
	
	// Add menu named "MersenneDebugWindow" to the Window menu
	[MenuItem ("Window/Mersenne Twister")]
	static void Init () 
	{
		MersenneDebugWindow window = (MersenneDebugWindow)EditorWindow.GetWindowWithRect(typeof (MersenneDebugWindow), new Rect(0, 0, 420, 600));
		window.Show();
	}
	
	void OnGUI() 
	{
		
		GUILayout.BeginArea(new Rect(10, 10, 400, 400));
		GUILayout.Box("RANDOM NUMBER DISTRIBUTION", GUILayout.Width(400), GUILayout.Height(400));
		if (randomList != null && randomList.Count > 0)  MersenneDebugDrawing.DrawPoints(randomList, op, 400, 400);
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(10, 420, 400, 200));
		seed =  EditorGUILayout.IntSlider("Seed:",seed,Int32.MinValue,Int32.MaxValue);
		op = (MersenneWindowOptionsType) EditorGUILayout.EnumPopup("Type:", op);
		samplig_size = EditorGUILayout.IntSlider ("#N", samplig_size, 1, 1000);
		normalizeToggle =  EditorGUILayout.Toggle("Normalize", normalizeToggle);
		
		if (normalizeToggle) {
			temperature = EditorGUILayout.Slider ("Temp", temperature, 0.0f, 10.0f);			
		}
		
		if (GUILayout.Button("Generate Random Numbers")) this.Sample();
		
		if (randomList != null && randomList.Count > 0) {
			filename = EditorGUILayout.TextField("filename:", filename);
			if (GUILayout.Button("Save To File and Open it")) this.Save();	
		}
		
		if (GUILayout.Button("Close Window")) this.Close();
		
		GUILayout.EndArea();
	}
	
	void Sample() 
	{
		Debug.Log("GENERATING RANDOM NUMBERS WITH SEED: " + seed);
		mrand = new MersenneTwister(seed);
		randomList = new ArrayList();
		for (int i = 0; i < samplig_size; i++) {
			
			double myval, rn;
			
			switch (op) 
			{
				case MersenneWindowOptionsType.INT:
					rn = mrand.Next();
				break;
				
				case MersenneWindowOptionsType.FLOAT:
					rn = mrand.NextSingle(true);
				break;
					
				case MersenneWindowOptionsType.DOUBLE:
					rn = mrand.NextDouble(true);
				break;
					
				default:
					rn = mrand.Next();
				break;
			}
			
			if (normalizeToggle) {
				myval = UnityNormalDistribution.toNormalDistribution(rn, temperature);
			} else {
				myval = rn;
			}
			
			randomList.Add(myval);
		}
		randomList.Sort();
		this.Repaint();
	}
	
	void Save()
	{
		path = Application.dataPath + "/" + filename;
		FileStream fs = File.Create(path);
		foreach ( object obj in randomList ) {
			String data = obj + "\n";
			AddText(fs, data);
		}
		UnityEngine.Object myfile = AssetDatabase.LoadMainAssetAtPath("Assets/" + filename);
		AssetDatabase.OpenAsset(myfile);
	}
	
	private static void AddText(FileStream fs, string value)
    {
        byte[] info = new UTF8Encoding(true).GetBytes(value);
        fs.Write(info, 0, info.Length);
    }
}
