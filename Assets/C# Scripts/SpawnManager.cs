using Mike;
using System.Collections;
using UnityEngine;

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
        numberOfPlayersOnTeam1 = PlayerPrefs.GetInt("Players Team1", numberOfPlayersOnTeam1);
        numberOfPlayersOnTeam2 = PlayerPrefs.GetInt("Players Team2", numberOfPlayersOnTeam2);

        if (ValidateSpawnPoints()) SpawnPlayers();
    }

    void SpawnPlayers()
    {
        // BRUH! Wtf is this function? why did I write it like this?!

        //Team1
        while (playersOnTeam1.Length < numberOfPlayersOnTeam1)
        {
            Transform spawnPoint = transform;

            Transform spawnPointParent = GameObject.FindGameObjectWithTag("SpawnPointsTeam1").transform;

            int rnd = Random.Range(0, spawnPointParent.childCount);

            if (ValidatePosition(spawnPointParent.GetChild(rnd).position) || numberOfPlayersOnTeam1 > spawnPointParent.childCount)
            {
                spawnPoint = spawnPointParent.GetChild(rnd);
            }
            else
            {
                continue;
            }

            Player player;

            if (playerTeam == 1 && playersOnTeam1.Length == 0)
            {
                player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                player = Instantiate(botPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            player.tag = "Team1";
            player.MeshRenderer.material = team1Material;
            playersOnTeam1 = MikeArray.Append(playersOnTeam1, player);
        }

        //Team2
        while (playersOnTeam2.Length < numberOfPlayersOnTeam2)
        {
            Transform spawnPoint = transform;

            Transform spawnPointParent = GameObject.FindGameObjectWithTag("SpawnPointsTeam2").transform;

            int rnd = Random.Range(0, spawnPointParent.childCount);

            if (ValidatePosition(spawnPointParent.GetChild(rnd).position) || numberOfPlayersOnTeam1 > spawnPointParent.childCount)
            {
                spawnPoint = spawnPointParent.GetChild(rnd);
            }
            else
            {
                continue;
            }

            Player player;

            if (playerTeam == 2 && playersOnTeam2.Length == 0)
            {
                player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            }
            else
            {
                player = Instantiate(botPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            player.tag = "Team2";
            player.MeshRenderer.material = team2Material;
            playersOnTeam2 = MikeArray.Append(playersOnTeam2, player);
        }
    }

    bool ValidateSpawnPoints()
    {
        Transform spawnPointParent = GameObject.FindGameObjectWithTag("SpawnPointsTeam2").transform;

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

    IEnumerator RespawnPlayer(GameObject player)
    {
        yield return new WaitForSeconds(respawnTime);

        Transform spawnPoint = transform;
        Transform spawnPointParent = GameObject.FindGameObjectWithTag(player.tag == "Team1" ? "SpawnPointsTeam1" : "SpawnPointsTeam2").transform;

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

        player.transform.position = spawnPoint.position;
        player.transform.rotation = spawnPoint.rotation;

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
