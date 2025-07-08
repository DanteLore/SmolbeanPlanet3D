using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ManaController : MonoBehaviour, IObjectGenerator
{
    public static ManaController Instance;

    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

    public Action<float> OnManaChanged;
    public float startingMana = 1000f;
    public GameObject manaFlyerPrefab;
    public Camera mainCamera;
    public UIDocument uiTarget;
    public string manaElementName = "manaSymbolLabel";
    public float targetZDistance = 2.0f;
    public float flyDurationSeconds = 1.0f;

    private float mana;
    private SoundPlayer soundPlayer;
    private readonly Queue<float> manaQueue = new();

    public float Mana
    {
        get => mana;
        private set
        {
            if (mana != value)
            {
                mana = value;
                soundPlayer.PlayOneShot("Magic1");
                OnManaChanged?.Invoke(mana);
            }
        }
    }

    public void Clear()
    {
        // Nothing to do here!
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        mana = startingMana;
        yield return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        mana = data.mana;
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.mana = mana;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
    }

    public void AddMana(GameObject source, float amount)
    {
        manaQueue.Enqueue(amount);
        StartCoroutine(AwardMana());

        var startPos = source.transform.position;
        var flyer = Instantiate(manaFlyerPrefab, startPos, Quaternion.identity, mainCamera.transform).GetComponent<ManaFlyer>();

        flyer.startPoint = startPos;
        flyer.mainCamera = mainCamera;
        flyer.durationSeconds = flyDurationSeconds;
        flyer.uiElement = uiTarget.rootVisualElement.Q<VisualElement>(manaElementName);
        flyer.targetZDistance = targetZDistance;
    }

    private IEnumerator AwardMana()
    {
        yield return new WaitForSeconds(flyDurationSeconds);
        Mana += manaQueue.Dequeue();
    }
}
