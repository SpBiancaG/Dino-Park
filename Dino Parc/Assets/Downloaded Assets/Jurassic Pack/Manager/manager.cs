﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class manager : MonoBehaviour
{
	 const string ManagerHelp=
	"Disable creatures management.\n"+
	"Creatures A.I. still work, player inputs, camera behavior and GUI features are disabled.\n"+
	"Useful if you want to use a third party asset e.g. fps controller. "+
	"However, manager component still to be attached to the MainCam. ";
	[Header("JURASSIC PACK MANAGER")]
	[Tooltip(ManagerHelp)]
	public bool UseManager=true;
	
	[SerializeField] bool ShowFPS=true;



	public AudioClip Wind;
	[Space (10)]
	[Header("CREATURES COLLECTION")]
	[Tooltip("Search automatically the creatures prefabs in the project and add it to the Inspector.")]
  #if UNITY_EDITOR
	[SerializeField] bool ReloadCollection=true;
  #endif
	public List<GameObject> CollectionList;
	[Space (10)]
	[Header("GLOBAL CREATURES SETTINGS")]
	public bool UseIK;
	[Tooltip("Creatures will be active even if they are no longer visible. (performance may be affected).")]
	public bool RealtimeGame;
	const string RaycastHelp=
	"Enabled : allow creatures to walk on all kind of collider with a defined ''walkable'' layer.\n"+
	"Disabled : creatures can only walk on Terrain collider.\n";
	[Tooltip(RaycastHelp)]
	public bool UseRaycast;
	[Tooltip("If ''UseRaycast'' are enabled, select the walkable layer (layer 8 by default), Do not use builtin layers.")]
	public int walkableLayer=8;
	[Tooltip("Unity terrain tree layer, to enable tree finding (trees prefab layer, 9 by default), Do not use builtin layers.")]
	public int treeLayer=9;
	[Tooltip("Weapon exemple to damage creatures")]
	public GameObject MyWeapon;

	[HideInInspector] public List<GameObject> creaturesList, playersList; //list of all creatures/players in game
	[HideInInspector] public int selected, CameraMode=1, message=0; //creature index, camera mode, game messages
	[HideInInspector] public Terrain terrain; //active terrain
	int Health, Food, Water, Sleep; //creature health bar

	Vector2 scroll1=Vector2.zero, scroll2=Vector2.zero; //Scroll position
	
	float timer, frame, fps; //fps counter
	//Rigidbody body;
	Collider playerCollider;
	AudioSource source;

	

//*************************************************************************************************************************************************
// STARTUP
	void Start()
	{
		//Find all JP creatures/players prefab in scene
		GameObject[] creatures= GameObject.FindGameObjectsWithTag("Creature");
		GameObject[] players= GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject element in creatures )
		{ 
			if(!element.name.EndsWith("(Clone)")) creaturesList.Add(element.gameObject); //Add to list
			else Destroy(element.gameObject); //Delete unwanted ghost object in hierarchy
		}
		foreach (GameObject element in players ) {playersList.Add(element.gameObject); }//Add to list

		if(UseManager)
		{
			Cursor.visible = false; Cursor.lockState=CursorLockMode.Locked;
			//body=transform.root.GetComponent<Rigidbody>();
			playerCollider=transform.root.GetComponent<Collider>();
			source=transform.root.GetComponent<AudioSource>();
		}

		//Layers left-shift
		walkableLayer=(1 << walkableLayer); treeLayer=(1 << treeLayer);

		//Get terrain
		if(Terrain.activeTerrain)
		{
			terrain =Terrain.activeTerrain;

			if(UseRaycast && 1<<terrain.gameObject.layer!=walkableLayer)
			{
				UseRaycast=false;
				Debug.LogWarning("Use Raycast disabled : please, add a ''walkable'' layer on your terrain to use Raycast");
			}
		}
		else if(!UseRaycast)
		{
			UseRaycast=true;
			Debug.LogWarning("Terrain Collider not found : Use Raycast enabled, please, add a ''walkable'' layer");
		}
  }
//*************************************************************************************************************************************************
// CAMERA BEHAVIOR
	void Update()
	{
		if(!UseManager) return;

		//Fps counter
		if(ShowFPS) { frame += 1.0f; timer += Time.deltaTime; if(timer>1.0f) { fps = frame; timer = 0.0f; frame = 0.0f; } }

		//Lock/Unlock cursor
	
		
	}

	void FixedUpdate()
	{
		if(!UseManager) return;
		shared creatureScript=null;
		//If creature not found, switch to free camera mode
		if(creaturesList.Count==0) CameraMode=0;
		else if(!creaturesList[selected] | !creaturesList[selected].activeInHierarchy) CameraMode=0;
		else creatureScript=creaturesList[selected].GetComponent<shared>(); //Get creature script

		if(creatureScript)
		{
			//Creature select (Shortcut Key)
			if(Input.GetKeyDown(KeyCode.X)) { if(selected > 0) selected--; else selected=creaturesList.Count-1; }
			else if(Input.GetKeyDown(KeyCode.Y)) { if(selected < creaturesList.Count-1) selected++; else selected=0; }
			
			//Change View (Shortcut Key)
			if(Input.GetKeyDown(KeyCode.C))
			{ if(CameraMode==2) CameraMode=0; else CameraMode++; }
			
			//Use AI (Shortcut Key)
			if(Input.GetKeyDown(KeyCode.I))
			{ if(creatureScript.AI) creatureScript.AI=false; else creatureScript.AI=true; }
		}

		//Prevent camera from going into terrain 
		//if(terrain && (terrain.SampleHeight(transform.root.position)+terrain.GetPosition().y)>transform.root.position.y-1.0f)
		//{
		//	body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
		//	transform.root.position=new Vector3(transform.root.position.x, (terrain.SampleHeight(transform.root.position)+terrain.GetPosition().y)+1.0f, transform.root.position.z);
		//}

	
	}
//*************************************************************************************************************************************************
//CHECK CREATURES COLLECTION
	#if UNITY_EDITOR
	void OnDrawGizmos ()
	{
		if(!UseManager) return;
		if(ReloadCollection) CollectionList.Clear();
		if(CollectionList.Count==0)
		{
			string[] assetPaths = UnityEditor.AssetDatabase.GetAllAssetPaths();
			foreach(string assetPath in assetPaths)
			if(assetPath.Contains("Assets/Jurassic Pack/Creatures/Prefab")&&assetPath.EndsWith(".prefab"))
			CollectionList.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath));
			ReloadCollection=false;
      Debug.Log(CollectionList.Count+" JP creature(s) found in your collection");
		}
	}
	#endif

//*************************************************************************************************************************************************
// DRAW GUI



	void OnGUI ()
	{
		if(!UseManager) return;

		float sw =Screen.width, sh= Screen.height;

	



		

		//Fps
		GUI.color=Color.white;
		if(ShowFPS) GUI.Label(new Rect(sw-60, 1, 55, 20), "Fps : "+ fps);

		//Messages
		
	}

//*************************************************************************************************************************************************
// FIRE WEAPON DAMAGE DINO EXEMPLE


}
