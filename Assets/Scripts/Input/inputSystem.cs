﻿using UnityEngine;
using System.Collections;

public class inputSystem : MonoBehaviour {

    ManagerSystem manager;
    PlayerAssistanceSystem assist;
	GameObject player;
	Cell zelle;
	DijkstraSystem dijSys;
    AttributeComponent attr;
	CameraRotationScript rotationScript;

	bool figurGewaehlt;
    bool spielerAmZug;

	bool angriffAusgewaehlt;
	MovementSystem moveSys;
	bool smokeAusgewaehlt;
	bool molotovAusgewaehlt;
	// Use this for initialization
	void Start () {

        dijSys = (DijkstraSystem) FindObjectOfType (typeof(DijkstraSystem));
        manager = GameObject.Find("Manager").GetComponent<ManagerSystem>();
		rotationScript = (CameraRotationScript)FindObjectOfType (typeof(CameraRotationScript));
        assist = (PlayerAssistanceSystem)GameObject.Find("Manager").GetComponent(typeof(PlayerAssistanceSystem));
	}
	

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) 
		{
            spielerAmZug = manager.getPlayerTurn();  //True = Spieler Eins, False = Spieler zwei
            Ray clicked = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

            Physics.Raycast(clicked, out hit, Mathf.Infinity);
			if(hit.collider != null)
			{                
				if (((hit.collider.gameObject.tag == "FigurSpieler1" && spielerAmZug) 
                    || (hit.collider.gameObject.tag == "FigurSpieler2" && !spielerAmZug)) 
                    && !angriffAusgewaehlt) 
				{
					if(player != hit.collider.gameObject)
					{
                        manager.setSelectedFigurine(hit.collider.gameObject);
						player = hit.collider.gameObject;
						figurGewaehlt = true;
						attr = (AttributeComponent) player.GetComponent(typeof(AttributeComponent));
						Cell currentCell = (Cell) attr.getCurrentCell().GetComponent(typeof(Cell));
						dijSys.executeDijsktra(currentCell, attr.actMovRange, attr.attackRange);
						rotationScript.setNewTarget(player);
					}
				}
				if (angriffAusgewaehlt)
				{
					if((hit.collider.gameObject.tag == "FigurSpieler2" && spielerAmZug)
                        || hit.collider.gameObject.tag == "FigurSpieler1" && !spielerAmZug)
					{
                        manager.shoot(player, hit.collider.gameObject);
					}
                    else
                    {
                        Debug.Log("Kann nicht angegriffen werden");
                    }
                }
				if (smokeAusgewaehlt)
				{
					if(hit.collider.gameObject.tag == "Cell")
					{
						AbilitySystem playerAbilSys = (AbilitySystem) player.GetComponent(typeof(AbilitySystem));
						zelle = (Cell)hit.collider.gameObject.GetComponent(typeof(Cell));
						playerAbilSys.throwSmoke(zelle);
						smokeAusgewaehlt = false;
					}
				}
				if (molotovAusgewaehlt)
				{
					if(hit.collider.gameObject.tag == "Cell")
					{
						AbilitySystem playerAbilSys = (AbilitySystem) player.GetComponent(typeof(AbilitySystem));
						zelle = (Cell)hit.collider.gameObject.GetComponent(typeof(Cell));
						playerAbilSys.throwMolotov(zelle);
						molotovAusgewaehlt = false;
					}
				}
			}
			else
			{
				figurGewaehlt = false;
			}
		}
        if (Input.GetMouseButton(1))
        {
            Ray clicked = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(clicked, out hit);
            if (hit.collider != null)
            {
                if (figurGewaehlt && hit.collider.gameObject.tag == "Cell")
                {
                    zelle = (Cell)hit.collider.gameObject.GetComponent(typeof(Cell));
                    ArrayList path = dijSys.getPath(attr.getCurrentCell(), zelle);
                    assist.PaintWalkPath(path);
                }
            }
        }

        if (Input.GetMouseButtonUp (1)) {
			Ray clicked = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			Physics.Raycast (clicked, out hit);
			if(hit.collider != null)
			{
				if(figurGewaehlt && hit.collider.gameObject.tag == "Cell")
				{
					zelle = (Cell)hit.collider.gameObject.GetComponent(typeof(Cell));
					MovementSystem moveSys = (MovementSystem) player.GetComponent(typeof(MovementSystem));
					moveSys.MoveTo(zelle);
				}
			}
			else
			{
				figurGewaehlt = false;
			}
		}
        if(Input.GetKeyDown("a") && player != null)
        {
            angriffAusgewaehlt = !angriffAusgewaehlt;
            if (angriffAusgewaehlt)
                Debug.Log("Angriff ausgewählt");
            else
                Debug.Log("Kein Angriff");
        }
		if (Input.GetKey ("r")) {
			rotationScript.setStartRotation ();
		}
		if (!Input.GetKey ("r")) {
			rotationScript.setStopRotation ();  
		}

		if (Input.GetKeyDown ("s")) {
			smokeAusgewaehlt = true;
		}
		if (Input.GetKeyDown ("f")) {
			molotovAusgewaehlt = true;
		}
        if(Input.GetKeyDown("space"))
        {
			rotationScript.backToTarget();
        }
	}
}

