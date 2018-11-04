using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorControl : MonoBehaviour {
	private List<string> listTextTutorial;
	private int currentText;
	public static TutorControl Instance { get; private set; }
	private void Awake () {
		Instance = this;
	}
	private void Start () {
		listTextTutorial = new List<string> ();

		listTextTutorial.Add ("Hi! Wellcome in.");
		listTextTutorial.Add ("A block is falling.");
		listTextTutorial.Add ("And it can attach to your center block.");
		listTextTutorial.Add ("Now, drag your finger to move your block 3 steps to the right.");
		listTextTutorial.Add ("Very good!");
		listTextTutorial.Add ("Now, drag your finger to the left to attach the block!");
		listTextTutorial.Add ("Excellent!");
		listTextTutorial.Add ("Tap any where on screen to rotate your block.");
		listTextTutorial.Add ("You've just completed the tutorial. Welldone!");
	}
	public void OnTutor () {
		currentText = 0;
	}
	public string GetNextText () {
		if (currentText < listTextTutorial.Count)
			return listTextTutorial[currentText++];
		return "";
	}
}