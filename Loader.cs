using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    public GameObject gameManager;

	// If a game manager doesn't exist yet, create one.
	void Awake () {
		if (GameManager.instance == null) {
            Instantiate(gameManager);
        }
	}
	
}
