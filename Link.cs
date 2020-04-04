using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Link
{
	private GameObject first;
	private GameObject second;
	private LineRenderer line;

	public Link(GameObject first, GameObject second)
	{
		this.first = first;
		this.second = second;
		this.line = first.AddComponent<LineRendered>();
	}

}
