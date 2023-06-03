using Mike;
using System.Collections;
using UnityEngine;

// TODO: Stop using Unity GameObject tags
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Player botPrefab;
    [SerializeField] private float respawnTime = 5;

    [SerializeField] private int playerTeam = 1;
    [SerializeField] private int numberOfPlayersOnTeam1 = 6;
    [SerializeField] private int numberOfPlayersOnTeam2 = 6;

    [SerializeField] private Material team1Material;
    [SerializeField] private Material team2Material;

    Player[] playersOnTeam1 = new Player[0];
    Player[] playersOnTeam2 = new Player[0];

    static SpawnManager _Instance;
    public static SpawnManager Instance
    {
        get
        {
            return _Instance;
        }
    }
    private void Awake()
    {
        _Instance = this;
    }

    private void Start()
    {
        // TODO: Move this to a separate PlayerPrefs manager class
        numberOfPlayersOnTeam1 = PlayerPrefs.GetInt("Players Team1", numberOfPlayersOnTeam1);
        numberOfPlayersOnTeam2 = PlayerPrefs.GetInt("Players Team2", numberOfPlayersOnTeam2);

        if (ValidateSpawnPoints("SpawnPointsTeam1") && ValidateSpawnPoints("SpawnPointsTeam2")) SpawnPlayers();
    }

    void SpawnPlayers()
    {
        SpawnTeam(true, ref playersOnTeam1, numberOfPlayersOnTeam1, GameObject.FindGameObjectWithTag("SpawnPointsTeam1").transform, "Team1", team1Material);
        SpawnTeam(false, ref playersOnTeam2, numberOfPlayersOnTeam2, GameObject.FindGameObjectWithTag("SpawnPointsTeam2").transform, "Team2", team2Material);


        void SpawnTeam(bool humanTeam, ref Player[] teamPlayers, int playersOnTeam, Transform spawnPointParent, string teamTag, Material teamMaterial)
        {
            teamPlayers = new Player[playersOnTeam];
            Player player;

            // both variables are used by GetRandomSpawnPoint()
            Transform spawnPoint;
            int rnd; 

            for (int i = 0; i < playersOnTeam; i++)
            {
                spawnPoint = GetRandomSpawnPoint(100);

                if (i == 0 && humanTeam) // Spawn the player if human team on the first iteration
                {
                    player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
                    player.name = $"Human Player {i} " + teamTag;
                }
                else
                {
                    player = Instantiate(botPrefab, spawnPoint.position, spawnPoint.rotation);
                    player.name = $"Player {i} " + teamTag;
                }

                player.tag = teamTag;
                player.MeshRenderer.sharedMaterial = teamMaterial;
                teamPlayers[i] = player;
            }


            Transform GetRandomSpawnPoint(int tries)
            {
                // TODO: "Anything that can go wrong will go wrong." (There is a (1 in tries * spawnPointParent.childCount - 1) chance that this can go wrong)
                for (int i = 0; i < tries; i++) // failsafe
                {
                    rnd = Random.Range(0, spawnPointParent.childCount);

                    if (ValidatePosition(spawnPointParent.GetChild(rnd).position) || playersOnTeam > spawnPointParent.childCount)
                    {
                        return spawnPointParent.GetChild(rnd);
                    }
                }

                throw new("No valid spawnPoints");
            }
        }
    }

    bool ValidateSpawnPoints(string spawnPointParentTag)
    {
        Transform spawnPointParent = GameObject.FindGameObjectWithTag(spawnPointParentTag).transform;

        foreach (Transform spawnPoint in spawnPointParent)
        {
            if (!ValidatePosition(spawnPoint.position)) { Debug.LogError($"Invalid SpawnPoint: {spawnPoint}"); return false; }
        }

        return true;
    }

    public void Respawn(GameObject player)
    {
        StartCoroutine(RespawnPlayer(player));
    }

    WaitForSeconds _wait;
    IEnumerator RespawnPlayer(GameObject player)
    {
        yield return _wait ??= new WaitForSeconds(respawnTime);

        Transform spawnPoint = transform;
        Transform spawnPointParent = GameObject.FindGameObjectWithTag(player.CompareTag("Team1") ? "SpawnPointsTeam1" : "SpawnPointsTeam2").transform;

        while (spawnPoint == transform)
        {
            for (int i = 0; i < spawnPointParent.childCount; i++)
            {
                int rnd = Random.Range(0, spawnPointParent.childCount);

                if (ValidatePosition(spawnPointParent.GetChild(rnd).position) || numberOfPlayersOnTeam1 > spawnPointParent.childCount)
                {
                    spawnPoint = spawnPointParent.GetChild(rnd);
                    break;
                }
            }

            yield return null;
        }

        player.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);

        player.transform.GetComponent<Health>().Respawn();
    }

    bool ValidatePosition(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, .5f);

        foreach (Collider collider in colliders)
        {
            if (collider.isTrigger) { continue; }
            else { return false; }
        }

        return true;
    }
}
