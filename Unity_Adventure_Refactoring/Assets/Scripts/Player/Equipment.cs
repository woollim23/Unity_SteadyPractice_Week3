using UnityEngine;
using UnityEngine.InputSystem;

public class Equipment : MonoBehaviour
{
    public Equip curEquip; // ���� ���� ����
    public Transform equipParent; // ��� �޾��� ��ġ(ī�޶� ��ġ)

    private PlayerController controller;
    private PlayerCondition condition;

    void Start()
    {
        controller = CharacterManager.Instance.Player.controller;
        condition = CharacterManager.Instance.Player.condition;
    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook)
        {
            curEquip.OnAttackInput();
        }
    }

    public void EquipNew(ItemData data)
    {
        // ���ο� ������ ���� 
        UnEquip(); // ���� ��� ���� ����
        // ���� ������ ������ �־���
        curEquip = Instantiate(data.equipPrefab, equipParent).GetComponent<Equip>();

        if (curEquip.doesIncrease == true)
            EquipIncrease();

    }

    public void UnEquip()
    {
        if (curEquip != null)
        {
            if (curEquip.doesIncrease == true)
                UnEquipIncrease();
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }

    public void EquipIncrease()
    {
        // ��� ���� ȿ��
        CharacterManager.Instance.Player.controller.moveSpeed += curEquip.increase;
    }

    public void UnEquipIncrease()
    {
        // ��� ���� ȿ�� ����
        CharacterManager.Instance.Player.controller.moveSpeed -= curEquip.increase;
    }
}