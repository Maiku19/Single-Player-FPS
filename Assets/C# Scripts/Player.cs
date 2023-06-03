using UnityEngine;

public class Player : MonoBehaviour
{
    public Stats Stats { get => _stats; }
    [SerializeField] Stats _stats;

    public Bot Bot { get => _bot; }
    [SerializeField] Bot _bot;

    public Health Health { get => _health; }
    [SerializeField] Health _health;

    public bool IsBot { get => Bot != null; }

    public Transform Body { get => _body; }
    [SerializeField] Transform _body;
    
    public MeshRenderer MeshRenderer { get => _meshRenderer; }
    [SerializeField] MeshRenderer _meshRenderer;

    public Rigidbody Rigidbody { get => _rigidbody; }
    [SerializeField] Rigidbody _rigidbody;

    public int Team { get => CompareTag("Team1") ? 1 : 0; }

    private void Update()
    {
        if (IsBot) { return; }
        if (Input.GetKeyDown(KeyCode.Return)) { Time.timeScale = Time.timeScale == 1f ? .1f : 1f; }
    }
}
