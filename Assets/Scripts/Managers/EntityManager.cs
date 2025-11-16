using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance { private set; get; }

    [Header("Data Setting")]
    [SerializeField] private GameObject monsterBase;

    [Header("Spawn Settings")]
    [SerializeField][Min(0.05f)] private float spawnDelay = 3f;
    private Coroutine spawnRoutine;

    [Header("Entities Settings")]
    [SerializeField] private Transform inGame;
    [SerializeField] private Transform towerTrans;
    [SerializeField] private List<Tower> towers = new List<Tower>();
    [SerializeField] private Transform monsterTrans;
    [SerializeField] private List<Monster> monsters = new List<Monster>();

    [Header("Monster Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private Transform[] paths;
    [SerializeField] private float pathMargin = 1f;
    [SerializeField] private int[] pathNum = { 2, 8, 7, 4, 6, 9, 8, 2, 3, 6, 4, 1, 2, 8 };

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (monsterBase == null)
            monsterBase = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Monster.prefab");

        List<Transform> list = new List<Transform>();
        Transform[] all = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t.name.StartsWith("Path ("))
                list.Add(t);
        }

        list.Sort((a, b) => GetPathIndex(a.name).CompareTo(GetPathIndex(b.name)));
        paths = list.ToArray();
    }

    private int GetPathIndex(string _name)
    {
        int start = _name.IndexOf('(');
        int end = _name.IndexOf(')');
        if (start >= 0 && end > start)
        {
            string number = _name.Substring(start + 1, end - start - 1);
            int index;
            if (int.TryParse(number, out index))
                return index;
        }
        return 0;
    }
#endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetEntity();
    }

    #region 타워
    #endregion

    #region 몬스터
    public Monster Spawn(Vector3? _pos = null)
    {
        Vector3 pos = (_pos.HasValue)
            ? _pos.Value
            : transform.position;

        Monster e = Instantiate(monsterBase, pos, Quaternion.identity, monsterTrans)
            .GetComponent<Monster>();

        monsters.Add(e);

        return e;
    }

    public void ToggleSpawn(bool _on)
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        if (_on)
            spawnRoutine = StartCoroutine(SpawnCoroutine());
        else
            spawnRoutine = null;
    }

    public IEnumerator SpawnCoroutine()
    {
        while (true)
        {
            Transform[] monsterPath = new Transform[pathNum.Length];
            for (int i = 0; i < pathNum.Length; i++)
            {
                int index = pathNum[i] - 1;
                monsterPath[i] = paths[index];
            }

            Monster monster = Spawn(monsterPath[0].position);
            monster.SetSpeed(speed);
            monster.SetPath(monsterPath);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void Despawn(Monster _enemy)
    {
        if (_enemy == null) return;

        monsters.Remove(_enemy);
        Destroy(_enemy.gameObject);
    }

    public void DespawnAll()
    {
        for (int i = monsters.Count - 1; i >= 0; i--)
            Despawn(monsters[i]);
    }
    #endregion

    #region SET
    public void SetEntity()
    {
        if (inGame == null) inGame = GameObject.Find("InGame")?.transform;
        if (towerTrans == null) towerTrans = GameObject.Find("InGame/Towers")?.transform;
        if (monsterTrans == null) monsterTrans = GameObject.Find("InGame/Monsters")?.transform;

        Rect r = AutoCamera.WorldRect;

        float x1 = r.xMin + pathMargin;
        float x2 = r.center.x;
        float x3 = r.xMax - pathMargin;

        float y1 = r.yMax - pathMargin;
        float y2 = r.center.y;
        float y3 = r.yMin + pathMargin;

        paths[0].position = new Vector3(x1, y1);
        paths[1].position = new Vector3(x2, y1);
        paths[2].position = new Vector3(x3, y1);
        paths[3].position = new Vector3(x1, y2);
        paths[4].position = new Vector3(x2, y2);
        paths[5].position = new Vector3(x3, y2);
        paths[6].position = new Vector3(x1, y3);
        paths[7].position = new Vector3(x2, y3);
        paths[8].position = new Vector3(x3, y3);
    }
    #endregion

    #region GET
    #endregion
}
