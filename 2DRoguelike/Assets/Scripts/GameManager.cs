using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{	
	public static GameManager instance = null;	// Make this class a singleton
	public BoardManager boardScript;
	private int level = 3;	
	public int playerFoodPoints = 100;
	[HideInInspector]
	public bool playersTurn = true;
	
	// Use this for initialization
	private void Awake()
	{
		if(instance == null)
		{
			instance = this;			
		}
		else if (instance != this)
		{						
			Destroy(gameObject);
		}
		
		DontDestroyOnLoad(gameObject);
		boardScript = GetComponent<BoardManager>();
		InitGame();
	}
	
	private void InitGame()
	{
		boardScript.SetupScene(level);
	}

	public void GameOver()
	{
		this.enabled = false;
	}
	
	// Update is called once per frame
	private void Update()
	{
		
	}
}
