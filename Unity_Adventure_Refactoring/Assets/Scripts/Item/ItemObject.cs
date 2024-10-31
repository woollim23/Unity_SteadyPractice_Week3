using UnityEngine;

public interface IInteractable
{
    public string GetInteractPrompt();
    public void OnInteract();

    public bool notGain();
}

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        string str = $"{data.displayName}\n{data.description}";
        return str;
    }

    public bool notGain()
    {
        return data.type == ItemType.Object;
    }

    public void OnInteract()
    {
        CharacterManager.Instance.Player.itemData = data;
        CharacterManager.Instance.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
