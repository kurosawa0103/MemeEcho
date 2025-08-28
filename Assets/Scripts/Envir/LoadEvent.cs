using UnityEngine;

public class LoadEvent : MonoBehaviour
{
    private IGameDataHandler gameDataHandler = new GameDataToJson();
    public EventManager eventManager;
    public EventData firstEvent;
    public TimeSystem timeSystem;
    public InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem.LoadInventory();
        inventorySystem.DisplayItems();
        LoadEvents();
    }

    // ���ص�ǰ�¼����ݵķ���
    public void LoadEvents()
    {
        EventData currentEventData = Resources .Load <EventData>(gameDataHandler.GetCurrentEventPath());
        if(currentEventData == null)
        {
            eventManager.currentEvent = firstEvent;
            eventManager.LoadEvent(firstEvent);
        }
        else
        {
            eventManager.currentEvent = currentEventData;
            eventManager.LoadEvent(currentEventData);
        }

    }

   
}
