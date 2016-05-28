using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {

	private int score;
	private bool gameOver = true;

	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
	}

	public void GameOver ()
	{
		gameOver = true;
	}

	// Use this for initialization
	void Start ()
	{
	}

	// Update is called once per frame
	void Update ()
	{
		if (gameOver) {
			if (Input.GetKeyDown (KeyCode.R)) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
			}
		}
	}

}
