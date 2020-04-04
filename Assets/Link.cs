using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Link
{
	public string type;
	private ElementScript firstElement;
	private ElementScript secondElement;
	private LineRenderer line;
	private SpringJoint2D joint;
	private GameObject dummy;

	public Link(string type, ElementScript firstElement, ElementScript secondElement)
	{
		this.type = type;
		this.firstElement = firstElement;
		this.secondElement = secondElement;

		dummy = new GameObject();
		this.line = dummy.AddComponent<LineRenderer>();

		line.positionCount = 2;
		line.SetWidth(0.1f, 0.1f);

		joint = firstElement.gameObject.AddComponent<SpringJoint2D>();
		joint.connectedBody = secondElement.rigidBody;
		joint.distance = 0.4f;
		joint.frequency = 5;
		joint.enableCollision = true;
	}

	public void Update()
	{
		line.SetPosition(0, firstElement.gameObject.transform.position);
		line.SetPosition(1, secondElement.gameObject.transform.position);
	}


	public void Remove()
	{
		firstElement.RemoveComponent(line);
		firstElement.RemoveComponent(joint);
		firstElement.RemoveComponent(dummy);
	}
}
