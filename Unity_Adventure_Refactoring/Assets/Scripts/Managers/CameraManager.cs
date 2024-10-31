using UnityEngine;

public class CameraManager : Singletone<CameraManager>
{
    private bool isOriginMain = true;
    public Camera originMainCamera; // ���� ī�޶�
    public Camera newMainCamera; // 3��Ī ī�޶�

    public void SwitchMainCamera()
    {
        // ���� ���� ī�޶� ��Ȱ��ȭ
        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(false);
        }

        if(isOriginMain)
        {
            // 3��Ī ī�޶� Ȱ��ȭ�ϰ� ���� ī�޶�� ����
            newMainCamera.gameObject.SetActive(true);
            isOriginMain = false;
        }
        else
        {
            originMainCamera.gameObject.SetActive(true);
            isOriginMain = true;
        }
    }
}
