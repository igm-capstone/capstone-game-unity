using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MinionSpawner : MonoBehaviour {

    public LayerMask ObstacleLayerMask;

    public GameObject UnitToSpawn;

    public bool IsPreviewVisible
    {
        get { return previewContainer.gameObject.activeSelf; }
        set { previewContainer.gameObject.SetActive(value); }
    }

    private Transform previewContainer;
    private GridBehavior grid;
    private Collider2D[] overlapResult;
    private RectTransform minionsPanel;
    private Canvas canvas;
    private bool selectingMinion;
    private Vector3 worldPoint;

    void Start()
    {
        var go = new GameObject("SpawnPreview");
        previewContainer = go.transform;

        grid = FindObjectOfType<GridBehavior>();

        overlapResult = new Collider2D[1];

        minionsPanel = transform.Find("MinionsPanel").GetComponent<RectTransform>();
        canvas = transform.GetComponentInParent<Canvas>();
        foreach (RectTransform minionUI in transform.Find("MinionsPanel"))
        {
            var minionButton = minionUI.GetComponent<Button>();
            minionButton.onClick.AddListener(OnMinionButtonClick);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) )
        {
            var camera = canvas.worldCamera;
            var screenPoint = Input.mousePosition;
            var canvasPoint = Vector2.zero;
            worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
            worldPoint.z = -20;

            var obstaclesOverlaped = Physics2D.OverlapCircleNonAlloc(worldPoint, .75f, overlapResult, ObstacleLayerMask);

            if (!selectingMinion && obstaclesOverlaped == 0)
            {
                selectingMinion = true;
                minionsPanel.gameObject.SetActive(true);
                previewContainer.transform.position = worldPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, screenPoint, camera, out canvasPoint);
                minionsPanel.transform.position = canvas.transform.TransformPoint(canvasPoint);
            }
            else if (selectingMinion)
            {
                selectingMinion = false;
                minionsPanel.gameObject.SetActive(false);
            }
        }
    }


    public void OnMinionButtonClick()
    {
        //Spawn(worldPoint);
        SpawnManager.Instance.CmdSpawn(worldPoint);
    }

    public void Spawn(Vector3 position)
    {
        var spawnedUnit = Instantiate(UnitToSpawn);
        spawnedUnit.transform.position = position;
    }
}
