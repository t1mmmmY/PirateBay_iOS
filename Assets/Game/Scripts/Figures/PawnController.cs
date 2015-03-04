using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class PawnController : PlacedObject 
{
	public Animator animator;
	public PAWN_TYPE pawnType;

	BoatAnimation boatAnimation;
	NavMeshAgent navAgent;
	Vector3 startRotation;
	List<PawnController> _opponents;
	Vector3 endPosition;

	public PawnController opponent
	{
		get 
		{ 
			if (_opponents.Count > 0)
			{
				return _opponents[_opponents.Count - 1]; 
			}
			else
			{
				return null;
			}
		}
	}

	ACTIONS action;

	void Start()
	{
		startRotation = this.transform.rotation.eulerAngles;
		navAgent = this.GetComponent<NavMeshAgent>();
		boatAnimation = this.GetComponentInChildren<BoatAnimation>();
		_opponents = new List<PawnController>();

		action = ACTIONS.NOTHING;
	}

	void FixedUpdate()
	{
		if (/*(action == ACTIONS.MOVE || action == ACTIONS.BEAT || action == ACTIONS.DIE) && */navAgent.remainingDistance == 0.0f) //May be the situation when ship can not rich the destination
		{
			iTween.RotateTo(this.gameObject, startRotation, 1.0f);

			switch (action)
			{
			case ACTIONS.MOVE:
				action = ACTIONS.NOTHING;
//				Debug.Log("NextTurn");
				GameController.NextTurn();
				break;
			case ACTIONS.BEAT:
				action = ACTIONS.NOTHING;
				Beat();
				break;
			case ACTIONS.DIE:
				action = ACTIONS.NOTHING;
				break;
			case ACTIONS.REVERT_MOVE:
				action = ACTIONS.NOTHING;
				break;
			case ACTIONS.REVERT_BEAT:
				action = ACTIONS.NOTHING;
//				RevertBeat();
				break;
			}


		}
		if (action == ACTIONS.MOVE)
		{
//			Debug.Log(navAgent.remainingDistance.ToString());
		}
	}

	public void MoveHere(Vector3 newPosition)
	{
		navAgent.SetDestination(newPosition);
		action = ACTIONS.MOVE;
	}

	public void RevertMoveHere(Vector3 newPosition, OneTurn revert)
	{
		if (navAgent.enabled)
		{
			navAgent.SetDestination(newPosition);
		}
		else
		{
			endPosition = newPosition;
		}
		action = ACTIONS.REVERT_MOVE;
	}

	public void BeatOppenent(PawnController opponent, Vector3 endPosition)
	{
		this._opponents.Add(opponent);
		this.endPosition = endPosition;
		Vector3 newPosition = opponent.transform.position - opponent.transform.right * 5.0f;
		navAgent.SetDestination(newPosition);
		action = ACTIONS.BEAT;
		opponent.PrepareToDie();

	}

	public void RevertBeatOppenent(/*PawnController opponent, */Vector3 endPosition, OneTurn revert)
	{
		navAgent.SetDestination(endPosition);
		action = ACTIONS.REVERT_MOVE;

		opponent.Resurrect();

//		_opponents.RemoveAt(_opponents.Count-1);
	}

	public void RemoveLastOpponent()
	{
		_opponents.RemoveAt(_opponents.Count-1);
	}

	public void PrepareToDie()
	{
		Vector3 newPosition = this.transform.position + this.transform.right * 5.0f;
		navAgent.SetDestination(newPosition);
		action = ACTIONS.DIE;
	}

	private void Beat()
	{
		//There should be fire animation
		opponent.Die();
		MoveHere(endPosition);
	}

//	private void RevertBeat()
//	{
//		//There resurrection be fire animation
//		opponent.Resurrect();
////		MoveHere(endPosition);
//	}

	public void Die()
	{
		animator.SetTrigger("Die");
		navAgent.enabled = false;
		this.enabled = false;
		boatAnimation.OnResurrect += OnResurrect;
	}

	public void Resurrect()
	{
		animator.SetTrigger("Resurrect");

	}

	private void OnResurrect()
	{
//		Debug.LogWarning("OnResurrect");
		navAgent.enabled = true;
		this.enabled = true;
//		PlaceToTheCell();
		boatAnimation.OnResurrect -= OnResurrect;

		if (action == ACTIONS.REVERT_MOVE)
		{
			navAgent.SetDestination(endPosition);
		}
	}

	public void Select()
	{
	}
	
	public void Unselect()
	{
	}
}
