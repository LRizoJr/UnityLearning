﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{	
	public float turnDelay = 0.1f;
	public static GameManager instance = null;	// Make this class a singleton
	public BoardManager boardScript;
	
	private int level = 3;	
	private List<Enemy> enemies;
	private bool enemiesMoving;

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
		enemies = new List<Enemy>();
		boardScript = GetComponent<BoardManager>();
		InitGame();
	}
	
	private void InitGame()
	{
		enemies.Clear();
		boardScript.SetupScene(level);
	}

	public void GameOver()
	{
		this.enabled = false;
	}
	
	// Update is called once per frame
	private void Update()
	{
		if(playersTurn || enemiesMoving)
			return;
		
		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	private IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);
		if(enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}

		for(int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(turnDelay);
		}
		playersTurn = true;
		enemiesMoving = false;
	}
}
