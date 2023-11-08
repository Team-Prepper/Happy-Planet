using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone: MonoBehaviour {

    [SerializeField] GameObject zone = null;

    MeshRenderer mesh;

    public static bool zoneError = false;

	[SerializeField] Color set = new Color (0, 0.5f, 0, 1);
	[SerializeField] Color clear = new Color (0.5f, 0.5f, 0.5f, 1);
	[SerializeField] Color overlap = new Color (1, 0, 0, 1);
    
    private void Start()
    {
        mesh = zone.GetComponent<MeshRenderer>();
    }

    public void Set () {
		mesh.material.SetColor ("_EmissionColor", set);
        zoneError = false;

	}

    public void Clear () {

		mesh.material.SetColor ("_EmissionColor", clear);

	}

	public void Overlap () {

        zoneError = true;
		mesh.material.SetColor ("_EmissionColor", overlap);

	}

}
