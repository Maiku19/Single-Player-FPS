using UnityEngine;

public class MovementPoints : MonoBehaviour
{
	static MovementPoints _instance;
	public static MovementPoints Instance { get => _instance; }

	private void Awake()
	{
		_instance = this;
	}

	public Transform[] RegisteredMovementPoints { get => _registeredMovementPoints; }
	[SerializeField] Transform[] _registeredMovementPoints = new Transform[0];

	public Transform GetRandomMovementPoint()
	{
		return RegisteredMovementPoints[Random.Range(0, RegisteredMovementPoints.Length)];
	}
}
