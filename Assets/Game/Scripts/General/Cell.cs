using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Cell : MonoBehaviour 
{
	public bool isActive = true;
	public Transform shipPosition;
	public PawnController placedShip;
	public bool isEmpty = false;
	[SerializeField] Transform pathHeiglighter;

	private bool _selected = false;
	public bool selected
	{
		get { return _selected; }
	}
	private bool _possiblePath = false;
	public bool possiblePath
	{
		get { return _possiblePath; }
	}

	[SerializeField] private Position _position;
	public Position position
	{
		get { return _position; }
	}
	
	Animator anim;
	int activateHash = Animator.StringToHash("Activate");
	int deactivateHash = Animator.StringToHash("Deactivate");
	int moveHash = Animator.StringToHash("Move");
	int pathEnabledHash = Animator.StringToHash("ActivatePath");
	int pathDisabledHash = Animator.StringToHash("DeactivatePath");


	void Start()
	{
		anim = GetComponent<Animator>();
		isEmpty = placedShip == null ? true : false;

		_position = new Position();
		string name = this.gameObject.name;
		int pos1 = name.IndexOf('_');
		int pos2 = name.LastIndexOf('_');

		_position.x = System.Convert.ToInt32(name.Substring(pos1 + 1, pos2 - pos1 - 1));
		_position.y = System.Convert.ToInt32(name.Substring(pos2 + 1));

//		Debug.Log(_position.x + " " + _position.y);
	}

	void Update()
	{
#if UNITY_EDITOR
		if (isActive != this.gameObject.activeSelf)
		{
			this.gameObject.SetActive(isActive);
			Debug.Log("Change state");
		}
#endif
	}

	void OnMouseDown()
	{
		Board.OnPushCell(this);
	}

	public void ActivateCell(ACTIONS action)
	{
		switch (action)
		{
		case ACTIONS.EMPTY_CELL:
			break;
		case ACTIONS.BEAT:
			anim.SetTrigger(moveHash);
			break;
		case ACTIONS.MOVE:
			anim.SetTrigger(moveHash);
			break;
		case ACTIONS.SELECT:
			anim.SetTrigger(activateHash);
			_selected = true;
			break;
		}
	}

	public void DeactivateCell()
	{
		anim.SetTrigger(deactivateHash);
		_selected = false;
	}

	public bool PlaceObject(PlacedObject placedObject)
	{
		placedObject.transform.position = shipPosition.position;


		if (placedShip == null)
		{
			placedShip = placedObject as PawnController;
			isEmpty = false;
			return true;
		}
		else
		{

			return false;
		}
	}

	public bool DisplaceObject()
	{
		if (placedShip != null)
		{
			placedShip = null;
			isEmpty = true;
			return true;
		}
		else
		{
			return false;
		}
	}

	public void PossibleMovement(CELL_TYPE cellType, Cell selectedCell)
	{
		Color myTeamColor = Color.white;
		Color enemyTeamColor = Color.black;
		switch (selectedCell.placedShip.team)
		{
		case TEAM.WHITE_TEAM:
			myTeamColor = CONST.WHITE_PATH_COLOR;
			enemyTeamColor = CONST.BLACK_PATH_COLOR;
			break;
		case TEAM.BLACK_TEAM:
			myTeamColor = CONST.BLACK_PATH_COLOR;
			enemyTeamColor = CONST.WHITE_PATH_COLOR;
			break;
		}

		switch (cellType)
		{
		case CELL_TYPE.POSSIBLE_PATH:
			iTween.ColorTo(pathHeiglighter.gameObject, myTeamColor, 0.3f);
			break;
		case CELL_TYPE.POSSIBLE_HIT:
			iTween.ColorTo(pathHeiglighter.gameObject, enemyTeamColor, 0.3f);
			break;
		}

		iTween.MoveFrom(pathHeiglighter.gameObject, selectedCell.pathHeiglighter.position, 1.0f);

		anim.SetTrigger(pathEnabledHash);
		_possiblePath = true;

		GameController.OnDisablePath += OnDisablePath;
	}

	void OnDisablePath()
	{
		if (pathHeiglighter != null)
		{
			iTween.ColorTo(pathHeiglighter.gameObject, Color.clear, 0.5f);

			anim.SetTrigger(pathDisabledHash);
			_possiblePath = false;
		}

		GameController.OnDisablePath -= OnDisablePath;
	}

}
