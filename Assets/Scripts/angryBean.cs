using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine.Assertions.Must;
using UnityEditor.U2D.Path;

public class angryBean : Agent
{
    private float speed = 5.0f;
    private GameObject character;
	new private Rigidbody2D rigidbody;
	public bool trainingMode;
	private Vector3 startingPosition;
	private bool isFrozen = false;

	public override void Initialize()
    {
		character = this.gameObject;
		rigidbody = this.GetComponent<Rigidbody2D>();
		rigidbody.drag = 2f;
		//Debug.Log(character);
		startingPosition = new Vector3(character.transform.position.x, character.transform.position.y, character.transform.position.z);
		// if not training mode, no max steps, play forever
		if (!trainingMode)
		{
			MaxStep = 0;
		}
	}

	public override void OnEpisodeBegin()
    {
		rigidbody.velocity = Vector2.zero;
	}

	public override void OnActionReceived(float[] vectorAction)
	{
		if (isFrozen) return;
		Vector2 moveUpDown = new Vector2(0, vectorAction[1]);
		Vector2 moveLeftRight = new Vector2(vectorAction[0], 0);
		rigidbody.AddForce(moveUpDown * speed);
		rigidbody.AddForce(moveLeftRight * speed);
	}

	void Update()
    {

		//Things to do if the Agent has a child
		if (this.gameObject.transform.childCount > 0)
		{
			//Set the crate position relative to the Agent
			character.transform.GetChild(0).position = character.transform.position + new Vector3(0.8f, 0, 0);
			//Turn off the crate collider so it doesn't push the agent
			character.transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
        }


    }

    public override void Heuristic(float[] actionsOut)
	{
		//placeholders
		Vector3 forward = Vector3.zero;
		Vector3 left = Vector3.zero;

		//forward / backward
		if (Input.GetKey(KeyCode.W)) forward = transform.up;
		else if (Input.GetKey(KeyCode.S)) forward = -transform.up;

		//left / right
		if (Input.GetKey(KeyCode.A)) left = -transform.right;
		else if (Input.GetKey(KeyCode.D)) left = transform.right;

		//combine and normalize
		Vector3 combined = (forward + left).normalized;
		actionsOut[0] = combined.x;
		actionsOut[1] = combined.y;
	}

	//Collision detection for the bean to "grab" the crate
	private void OnCollisionEnter2D(Collision2D collision)
    {
		//only grabs a crate if it doesn't already have one
		if (character.transform.childCount < 1)
		{
			//Checks to see if the collision object is a crate
			if (collision.gameObject.tag == "Crate")
			{
				//pick up the crate and give a reward
				collision.gameObject.transform.parent = character.gameObject.transform;
				AddReward(0.5f);
			}
		}

		if (character.transform.childCount > 0)
		{
			//Checks to see if the collision object is a crate
			if (collision.gameObject.tag == "Crate")
			{
				//if the Agent already has a crate and tries to pick up another, negative reward
				AddReward(-0.1f);
			}
		}

		//If the enemy collides with the wall, give negative reward
		if (collision.gameObject.tag == "Wall")
        {
			AddReward(-0.25f);
            //reset position if training mode
            if (trainingMode)
            {
				character.transform.position = startingPosition;
			}
		}
    }

	/// <summary>
	/// prevent from moving
	/// </summary>
	public void FreezeAgent()
	{
		Debug.Assert(trainingMode == false, "Freeze/unfreeze not supported in training");
		isFrozen = true;
		rigidbody.Sleep();
	}
	/// <summary>
	/// resume movement
	/// </summary>
	public void UnfreezeAgent()
	{
		Debug.Assert(trainingMode == false, "Freeze/unfreeze not supported in training");
		isFrozen = false;
		rigidbody.WakeUp();
	}


	//Dropping off the crate at drop off point
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//collision with drop off point
		if (collision.gameObject.tag == "DropOff")
		{
			//makes sure the enemy has a child
			if (character.transform.childCount > 0)
			{
				//Destroys child and gives points
				//Debug.Log("Hit DropOff");
				Destroy(character.transform.GetChild(0).gameObject);
				AddReward(1f);
				//Reset position if training
				if (trainingMode)
				{
					character.transform.position = startingPosition;
				}
			}
			//If the Agent doesn't have a crate and triggers the drop off, negative reward
			if (character.transform.childCount < 1)
            {
				AddReward(-0.25f);
				//reset position if training
				if (trainingMode)
				{
					character.transform.position = startingPosition;
				}
			}
		}
	}
}
