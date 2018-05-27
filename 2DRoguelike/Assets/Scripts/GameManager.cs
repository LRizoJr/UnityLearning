using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{	
	public float levelStartDelay = 2f;
	public float turnDelay = 0.1f;
	public static GameManager instance = null;	// Make this class a singleton
	public BoardManager boardScript;
	
	private Text levelText;
	private GameObject levelImage;
	private int level = 0;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;

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
	}
	
	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		level++;
		InitGame();	// init the level
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}
	private void InitGame()
	{
		doingSetup = true;

		levelImage = GameObject.Find("LevelImage");
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		levelText.text = "Day " + level.ToString();
		levelImage.SetActive(true);		
		Invoke("HideLevelImage", levelStartDelay);

		enemies.Clear();
		boardScript.SetupScene(level);
	}

	private void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

	public void GameOver()
	{
		levelText.text = "After " + level.ToString() + " days, you starved.";
		levelImage.SetActive(true);
		this.enabled = false;
	}
	
	// Update is called once per frame
	private void Update()
	{
		if(playersTurn || enemiesMoving || doingSetup)
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
