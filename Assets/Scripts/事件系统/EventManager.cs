using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;
    public TimeSystem timeSystem;
    public Sailor sailor;
    public InventorySystem inventorySystem;
    //public CharacterManager characterManager;
    public Transform eventParent;

    [LabelText("当前事件"),ReadOnly]
    public EventData currentEvent;

    [Header("实例化的事件预制体")]
    public List<GameObject> eventPrefabs = new List<GameObject>(); // 存储实例化的事件预制体

    private ISailorHandler sailorHandler = new SailorToJson();
    private IGameDataHandler gameDataHandler = new GameDataToJson();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SyncEventPrefabs();
    }

    /*
    private void ExecuteEventActions(EventData eventData)
    {
        foreach (var action in eventData.eventActions)
        {
            switch (action.eventActionType)
            {
                case EventData.EventActionType.ModifyCharacterStats:
                    characterManager.ModifyCharacterAttribute(action.characterAttribute, action.attributeValue);
                    break;
                case EventData.EventActionType.unlockSwitchButton:
                    GameObject.Find("切换按钮").transform.GetComponent<MouseEnter>().SetDisabled(false);
                    break;
            }
        }
    }
    */
    public void CreatEvent(EventData eventData)
    {
        if (eventData.eventPrefab != null)
        {
            GameObject prefabIsland = Instantiate(eventData.eventPrefab, eventParent);
            if (prefabIsland != null)
            {
                eventPrefabs.Add(prefabIsland);
            }
        }
    }

    private void DestroyEventPrefabs()
    {
        foreach (var prefab in eventPrefabs)
        {
            Destroy(prefab);
        }

        eventPrefabs.Clear();
    }

    public void LoadEvent(EventData eventData)
    {
        DestroyEventPrefabs();

        currentEvent = eventData;

        //储存
        gameDataHandler.SetCurrentEventPath(currentEvent.eventAddress);
        inventorySystem.SaveInventory();

        CreatEvent(currentEvent);
        //同步
        SyncEventPrefabs();
    }
    private void SyncEventPrefabs()
    {
        foreach (Transform child in eventParent)
        {
            if (!eventPrefabs.Contains(child.gameObject) && child.gameObject.activeInHierarchy)
            {
                eventPrefabs.Add(child.gameObject);
            }
        }
    }

}
